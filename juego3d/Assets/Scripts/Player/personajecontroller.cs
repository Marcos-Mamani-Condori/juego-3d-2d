using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Necesario para Image y GameObject UI

public class PersonajeControlador : MonoBehaviour
{
    // --- ESTADOS DEL JUEGO (Modos) ---
    public enum GameMode { GolfMode, ShooterMode }

    [Header("Control General del Juego")]
    public GameMode currentMode = GameMode.GolfMode; 
    public KeyCode toggleModeKey = KeyCode.F; 

    [Header("Referencias a Otros Elementos (¡Arrastrar desde Unity!)")]
    public Transform pelota; 
    public GolfBallController golfBallScript; 

    // La cámara y su script de seguimiento (ambos se refieren al mismo GameObject Main Camera)
    public Camera mainCamera; 
    public CameraFollow cameraFollowScript; 

    // Solo necesitamos el panel de la UI del Shooter (la mirilla).
    // La UI del Golf será gestionada directamente por GolfBallController cuando se active/desactive.
    public GameObject shooterUI; 

    // Componentes del robot: Para el disparo.
    public GameObject weaponFlashPoint; 
    public Animator robotAnimator;      
    private AudioSource robotAudioSource; 

    [Header("Configuración del Disparo")]
    public KeyCode shootKey = KeyCode.Tab; 
    public float maxShootDistance = 100f; 
    public float robotRotationSpeed = 5f; 
    public string shootAnimationTrigger = "Disparar"; 
    public AudioClip shootSound; 
    [Tooltip("Retraso antes de aplicar el disparo para sincronizar con la animación.")]
    public float shootDelayBeforeFire = 1f;
    [Tooltip("Tiempo total mínimo entre disparos (incluye el retraso).")]
    public float shootCooldown = 1.2f;

    public GameObject shootFlashPrefab;   
    public GameObject impactEffectPrefab; 
    public LayerMask enemyLayer;          

    [Header("Acciones Especiales del Shooter")]
    public KeyCode destroyShipKey = KeyCode.Space;
    [Tooltip("Tag opcional para identificar la nave enemiga a destruir.")]
    public string enemyShipTag = "EnemyShip";

    // --- Configuración Original del Robot (para el Golf) ---
    [Header("Configuración del Golpe de Golf")]
    public float distanciaDelGolpe = 0.5f; 
    public LayerMask capaDelSuelo;          
    public float alturaDelPivote = 0.42f;   
    [Header("Ajuste de Altura del Shooter")]
    public float shooterHeightOffset = 0.42f;
    public float shooterGroundRayLength = 3f;
    
    // (Private) Variables internas que no necesitas ver ni modificar en el Inspector.
    private Animator animator; 
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private bool lockShooterHeight;
    private float shooterBaseHeight;
    private bool isShooting;
    private Coroutine shootCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        robotAudioSource = GetComponent<AudioSource>(); 

        if (robotAnimator == null) robotAnimator = animator; 
        
        OnEnterGolfMode(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleModeKey)) 
        {
            ToggleMode(); 
        }

        if (currentMode == GameMode.ShooterMode) 
        {
            HandleShooterMode(); 
        }
    }

    void LateUpdate()
    {
        if (currentMode == GameMode.ShooterMode && lockShooterHeight)
        {
            MaintainShooterHeight();
        }
    }

    public void ToggleMode()
    {
        if (currentMode == GameMode.GolfMode) 
        {
            OnEnterShooterMode(); 
        }
        else 
        {
            OnEnterGolfMode(); 
        }
    }

    void OnEnterGolfMode()
    {
        currentMode = GameMode.GolfMode; 
        lockShooterHeight = false;
        isShooting = false;
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        // Activamos el script de la bola. Su propio script activará/desactivará su UI.
        if (golfBallScript != null) golfBallScript.enabled = true; 
        
        // Ocultamos la UI del shooter.
        if (shooterUI != null) shooterUI.SetActive(false);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetMode(CameraFollow.CameraMode.Golf);
            cameraFollowScript.target = pelota; 
        }

        if (robotAnimator != null)
        {
            robotAnimator.applyRootMotion = true;
        }

        Debug.Log("Modo Golf Activado");
    }

    void OnEnterShooterMode()
    {
        currentMode = GameMode.ShooterMode; 
        lockShooterHeight = true;
        shooterBaseHeight = transform.position.y;

        // Desactivamos el script de la bola. Su propio script activará/desactivará su UI.
        if (golfBallScript != null) golfBallScript.enabled = false; 
        
        // Mostramos la UI del shooter (la mirilla).
        if (shooterUI != null) shooterUI.SetActive(true); 
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 

        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetMode(CameraFollow.CameraMode.Shooter);
            cameraFollowScript.target = transform; 
        }

        if (robotAnimator != null)
        {
            robotAnimator.applyRootMotion = false;
        }

        MaintainShooterHeight();

        Debug.Log("Modo Shooter Activado");
    }

    void HandleShooterMode()
    {
        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward; 
            cameraForward.y = 0; 
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward); 
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * robotRotationSpeed);
        }

        if (Input.GetKeyDown(shootKey)) 
        {
            StartShootSequence(); 
        }

        if (!isShooting && Input.GetKeyDown(destroyShipKey))
        {
            TryDestroyShip();
        }
    }

    private void MaintainShooterHeight()
    {
        if (!lockShooterHeight) return;

        Vector3 pos = transform.position;
        Vector3 origin = pos + Vector3.up * Mathf.Max(0.1f, shooterGroundRayLength * 0.5f);
        float rayLength = Mathf.Max(0.5f, shooterGroundRayLength);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, capaDelSuelo))
        {
            shooterBaseHeight = hit.point.y + shooterHeightOffset;
            pos.y = shooterBaseHeight;
        }
        else
        {
            pos.y = shooterBaseHeight;
        }

        transform.position = pos;
    }

    void TryDestroyShip()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, enemyLayer)) return;

        EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            int remainingHealth = Mathf.Max(1, enemy.health);
            enemy.TakeDamage(remainingHealth);
            return;
        }

        EnemyShipTarget ship = hit.collider.GetComponent<EnemyShipTarget>();
        if (ship != null)
        {
            ship.DestroyShip();
            return;
        }

        if (!string.IsNullOrEmpty(enemyShipTag) && hit.collider.CompareTag(enemyShipTag))
        {
            Destroy(hit.collider.gameObject);
        }
    }

    void StartShootSequence()
    {
        if (isShooting) return;

        shootCoroutine = StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        isShooting = true;

        if (robotAnimator != null)
        {
            robotAnimator.SetTrigger(shootAnimationTrigger); 
        }

        float delay = Mathf.Max(0f, shootDelayBeforeFire);
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        ExecuteShot();

        float cooldown = Mathf.Max(shootCooldown, delay);
        float remaining = cooldown - delay;
        if (remaining > 0f)
        {
            yield return new WaitForSeconds(remaining);
        }

        isShooting = false;
        shootCoroutine = null;
    }

    void ExecuteShot()
    {
        if (weaponFlashPoint != null && shootFlashPrefab != null)
        {
            Instantiate(shootFlashPrefab, weaponFlashPoint.transform.position, weaponFlashPoint.transform.rotation);
        }

        if (robotAudioSource != null && shootSound != null)
        {
            robotAudioSource.PlayOneShot(shootSound);
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit, maxShootDistance, enemyLayer)) 
        {
            Debug.Log("Impacto en: " + hit.collider.name + " en el punto: " + hit.point);

            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>(); 
            if (enemy != null)
            {
                // Verificar si es un enemigo especial de desafío de patrones
                PatternChallengeEnemy patternEnemy = hit.collider.GetComponent<PatternChallengeEnemy>();
                if (patternEnemy != null)
                {
                    // Activar el desafío en lugar de hacer daño
                    Debug.Log($"[PersonajeControlador] ✅ PatternChallengeEnemy detectado en: {hit.collider.name}");
                    patternEnemy.OnShotHit();
                    Debug.Log("¡Enemigo especial disparado! Activando desafío de patrones.");
                }
                else
                {
                    // Enemigo normal: hacer daño
                    Debug.Log($"[PersonajeControlador] Enemigo normal detectado: {hit.collider.name}");
                    enemy.TakeDamage(1);
                }
            }
            else 
            {
                ShieldModuleScript shieldModule = hit.collider.GetComponent<ShieldModuleScript>(); 
                if (shieldModule != null)
                {
                    shieldModule.DeactivateShield(); 
                }
            }
        }
        else
        {
            Debug.Log("Disparo al aire."); 
        }

        MaintainShooterHeight();
    }

    // --- FUNCIONES ORIGINALES DEL ROBOT PARA EL GOLF (SIN CAMBIOS) ---
    public void IniciarSecuenciaDeGolpe()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
        AlinearParaGolpear();
        if (animator != null) animator.SetTrigger("Golpear");
    }

    private void AlinearParaGolpear()
    {
        if (pelota == null || golfBallScript == null || golfBallScript.aimPivot == null) return;
        Vector3 aimLeft = -golfBallScript.aimPivot.right;
        aimLeft.y = 0;
        Vector3 posicionHorizontal = pelota.position + (aimLeft.normalized * distanciaDelGolpe);
        Vector3 posicionFinal = posicionHorizontal;
        RaycastHit hit;
        Vector3 puntoDeInicioRaycast = posicionHorizontal + (Vector3.up * 100f);
        if (Physics.Raycast(puntoDeInicioRaycast, Vector3.down, out hit, 200f, capaDelSuelo))
        {
            posicionFinal = hit.point + (Vector3.up * alturaDelPivote);
        }
        else
        {
            posicionFinal.y = pelota.position.y + alturaDelPivote;
        }
        transform.position = posicionFinal;
        Vector3 puntoDeMira = new Vector3(pelota.position.x, transform.position.y, pelota.position.z);
        transform.LookAt(puntoDeMira);
    }

    public void Evento_Impacto()
    {
        if (golfBallScript != null)
        {
            golfBallScript.ExecuteHitFromAnimation();
        }
    }

    public void Evento_RegresarAPosicion()
    {
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;
    }

    public void MoverseHaciaLaPelota()
    {
        if (pelota == null || golfBallScript == null || golfBallScript.aimPivot == null) return;
        Vector3 aimLeft = -golfBallScript.aimPivot.right;
        aimLeft.y = 0;
        Vector3 posicionHorizontal = pelota.position + (aimLeft.normalized * distanciaDelGolpe);
        Vector3 posicionFinal = posicionHorizontal;
        RaycastHit hit;
        Vector3 puntoDeInicioRaycast = posicionHorizontal + (Vector3.up * 100f);
        if (Physics.Raycast(puntoDeInicioRaycast, Vector3.down, out hit, 200f, capaDelSuelo))
        {
            posicionFinal = hit.point + (Vector3.up * alturaDelPivote);
        }
        else
        {
            posicionFinal.y = pelota.position.y + alturaDelPivote;
        }
        transform.position = posicionFinal;
        Vector3 puntoDeMira = new Vector3(pelota.position.x, transform.position.y, pelota.position.z);
        transform.LookAt(puntoDeMira);
    }
}