using UnityEngine;

/// <summary>
/// Plataforma que desaparece y reaparece periódicamente.
/// La pelota puede caer a través de ella si desaparece en el momento equivocado.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject 3D (Cube o Plane)
/// 2. Adjunta este script
/// 3. Configura los tiempos de visible/invisible
/// 4. (Opcional) Añade materiales para estados visible/invisible
/// </summary>
public class DisappearingPlatform : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo que la plataforma está sólida (segundos)")]
    public float visibleTime = 3f;
    
    [Tooltip("Tiempo que la plataforma está invisible (segundos)")]
    public float invisibleTime = 2f;
    
    [Tooltip("¿Comenzar visible?")]
    public bool startVisible = true;
    
    [Tooltip("Tiempo de advertencia antes de desaparecer (parpadeo)")]
    public float warningTime = 1f;

    [Header("Efectos Visuales")]
    [Tooltip("Material cuando está visible")]
    public Material visibleMaterial;
    
    [Tooltip("Material cuando está invisible")]
    public Material invisibleMaterial;
    
    [Tooltip("Velocidad de parpadeo durante advertencia")]
    public float blinkSpeed = 5f;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido al desaparecer")]
    public AudioClip disappearSound;
    
    [Tooltip("Sonido al aparecer")]
    public AudioClip appearSound;
    
    [Tooltip("Sonido de advertencia (loop)")]
    public AudioClip warningSound;

    [Header("Física")]
    [Tooltip("¿Empujar objetos hacia arriba antes de desaparecer?")]
    public bool pushObjectsBeforeDisappear = true;
    
    [Tooltip("Fuerza del empuje hacia arriba")]
    public float pushForce = 5f;

    private bool isVisible;
    private float timer;
    private MeshRenderer meshRenderer;
    private Collider platformCollider;
    private AudioSource audioSource;
    private bool isInWarning = false;
    private float blinkTimer = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();
        
        if (platformCollider == null)
        {
            Debug.LogError("¡La plataforma necesita un Collider!");
        }

        if (disappearSound != null || appearSound != null || warningSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        isVisible = startVisible;
        timer = isVisible ? visibleTime : invisibleTime;
        
        UpdatePlatformState(true);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Advertencia antes de desaparecer
        if (isVisible && timer <= warningTime && timer > 0f)
        {
            if (!isInWarning)
            {
                isInWarning = true;
                if (audioSource != null && warningSound != null)
                {
                    audioSource.clip = warningSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }

            // Efecto de parpadeo
            blinkTimer += Time.deltaTime * blinkSpeed;
            bool shouldShow = Mathf.Sin(blinkTimer) > 0f;
            
            if (meshRenderer != null)
            {
                meshRenderer.enabled = shouldShow;
            }
        }
        else if (isInWarning)
        {
            isInWarning = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // Cambiar estado cuando el timer llega a 0
        if (timer <= 0f)
        {
            isVisible = !isVisible;
            timer = isVisible ? visibleTime : invisibleTime;
            blinkTimer = 0f;
            
            UpdatePlatformState(false);
        }
    }

    void UpdatePlatformState(bool instant)
    {
        if (platformCollider != null)
        {
            platformCollider.enabled = isVisible;
        }

        if (meshRenderer != null)
        {
            meshRenderer.enabled = isVisible;
            
            if (isVisible && visibleMaterial != null)
            {
                meshRenderer.material = visibleMaterial;
            }
            else if (!isVisible && invisibleMaterial != null)
            {
                meshRenderer.material = invisibleMaterial;
            }
        }

        // Reproducir sonido
        if (!instant && audioSource != null)
        {
            AudioClip soundToPlay = isVisible ? appearSound : disappearSound;
            if (soundToPlay != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(soundToPlay);
            }
        }

        Debug.Log($"Plataforma ahora está: {(isVisible ? "VISIBLE" : "INVISIBLE")}");
    }

    void OnCollisionStay(Collision collision)
    {
        // Si la plataforma está a punto de desaparecer, empujar objetos hacia arriba
        if (pushObjectsBeforeDisappear && isVisible && timer <= 0.2f)
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * pushForce, ForceMode.Impulse);
                Debug.Log($"Empujando {collision.gameObject.name} hacia arriba antes de desaparecer");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isVisible ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        
        // Dibujar borde
        Gizmos.color = isVisible ? Color.green : Color.red;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
