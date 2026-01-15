using UnityEngine;

/// <summary>
/// Barrera láser que se activa y desactiva periódicamente.
/// Bloquea el paso y puede destruir la pelota o dañar al jugador.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Cube delgado (ej: X=5, Y=3, Z=0.1)
/// 2. Adjunta este script
/// 3. Crea un material emisivo rojo y asígnalo al objeto
/// 4. Configura el patrón de activación en el Inspector
/// 5. (Opcional) Añade LineRenderer para efecto láser más realista
/// </summary>
public class LaserBarrier : MonoBehaviour
{
    [Header("Configuración de Activación")]
    [Tooltip("Tiempo que el láser está activo (segundos)")]
    public float activeTime = 2f;
    
    [Tooltip("Tiempo que el láser está inactivo (segundos)")]
    public float inactiveTime = 2f;
    
    [Tooltip("¿Comenzar activo?")]
    public bool startActive = true;
    
    [Tooltip("Tiempo de transición (fade in/out)")]
    public float transitionTime = 0.3f;

    [Header("Efectos")]
    [Tooltip("¿Destruir la pelota al contacto?")]
    public bool destroyBallOnContact = false;
    
    [Tooltip("¿Devolver la pelota a su posición inicial?")]
    public bool resetBallOnContact = true;
    
    [Tooltip("Sonido de activación")]
    public AudioClip activationSound;
    
    [Tooltip("Sonido de desactivación")]
    public AudioClip deactivationSound;
    
    [Tooltip("Sonido de contacto/destrucción")]
    public AudioClip contactSound;

    [Header("Referencias Visuales")]
    [Tooltip("Material cuando está activo (rojo emisivo)")]
    public Material activeMaterial;
    
    [Tooltip("Material cuando está inactivo (transparente)")]
    public Material inactiveMaterial;
    
    [Tooltip("LineRenderer para efecto láser (opcional)")]
    public LineRenderer laserLine;

    private bool isActive;
    private float timer;
    private MeshRenderer meshRenderer;
    private BoxCollider laserCollider;
    private AudioSource audioSource;
    private Color currentEmissionColor;
    private float currentAlpha = 1f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        laserCollider = GetComponent<BoxCollider>();
        
        if (laserCollider == null)
        {
            laserCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (activationSound != null || deactivationSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        isActive = startActive;
        timer = isActive ? activeTime : inactiveTime;
        
        UpdateLaserState(true); // Actualizar estado inicial sin transición
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            // Cambiar estado
            isActive = !isActive;
            timer = isActive ? activeTime : inactiveTime;
            
            UpdateLaserState(false);
            
            // Reproducir sonido
            if (audioSource != null)
            {
                AudioClip soundToPlay = isActive ? activationSound : deactivationSound;
                if (soundToPlay != null)
                {
                    audioSource.PlayOneShot(soundToPlay);
                }
            }
        }
    }

    void UpdateLaserState(bool instant)
    {
        laserCollider.enabled = isActive;
        
        if (meshRenderer != null)
        {
            if (isActive && activeMaterial != null)
            {
                meshRenderer.material = activeMaterial;
            }
            else if (!isActive && inactiveMaterial != null)
            {
                meshRenderer.material = inactiveMaterial;
            }
        }

        if (laserLine != null)
        {
            laserLine.enabled = isActive;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        // Verificar si es la pelota de golf
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Pelota tocó el láser!");
            
            if (audioSource != null && contactSound != null)
            {
                audioSource.PlayOneShot(contactSound);
            }

            if (destroyBallOnContact)
            {
                // Destruir la pelota (cuidado, esto podría romper el juego)
                Destroy(other.gameObject);
            }
            else if (resetBallOnContact)
            {
                // Reiniciar la posición de la pelota
                GolfBallController ballController = other.GetComponent<GolfBallController>();
                if (ballController != null)
                {
                    // Detener la pelota
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                    
                    Debug.Log("Pelota reiniciada por láser");
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isActive ? new Color(1f, 0f, 0f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Gizmos.matrix = transform.localToWorldMatrix;
        
        BoxCollider col = GetComponent<BoxCollider>();
        Vector3 size = col != null ? col.size : Vector3.one;
        
        Gizmos.DrawCube(Vector3.zero, size);
        
        // Dibujar borde
        Gizmos.color = isActive ? Color.red : Color.gray;
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
