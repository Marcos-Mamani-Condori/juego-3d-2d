using UnityEngine;

/// <summary>
/// Plataforma que impulsa la pelota hacia arriba cuando aterriza sobre ella.
/// Similar a los trampolines en juegos de plataformas.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Cylinder o Cube
/// 2. Adjunta este script
/// 3. Asegúrate de que tenga un Collider
/// 4. Configura la fuerza del salto en el Inspector
/// 5. (Opcional) Añade un efecto de partículas cuando se active
/// </summary>
public class JumpPad : MonoBehaviour
{
    [Header("Configuración de Impulso")]
    [Tooltip("Fuerza vertical del salto")]
    public float jumpForce = 20f;
    
    [Tooltip("Fuerza horizontal adicional (en la dirección actual de la pelota)")]
    public float horizontalBoost = 0f;
    
    [Tooltip("¿Aplicar impulso o velocidad directa?")]
    public bool useImpulse = true;
    
    [Tooltip("Tiempo de recarga entre activaciones (segundos)")]
    public float cooldownTime = 0.5f;

    [Header("Dirección del Impulso")]
    [Tooltip("¿Usar dirección personalizada en vez de arriba?")]
    public bool useCustomDirection = false;
    
    [Tooltip("Dirección personalizada (se normalizará)")]
    public Vector3 customDirection = Vector3.up;

    [Header("Efectos Visuales")]
    [Tooltip("Efecto de partículas al activarse")]
    public ParticleSystem jumpEffect;
    
    [Tooltip("Color del JumpPad")]
    public Color padColor = Color.green;
    
    [Tooltip("¿Animar el JumpPad cuando se activa?")]
    public bool animateOnJump = true;
    
    [Tooltip("Escala de animación")]
    public float animationScale = 0.8f;
    
    [Tooltip("Velocidad de animación")]
    public float animationSpeed = 10f;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido al activarse")]
    public AudioClip jumpSound;
    
    [Tooltip("Volumen del sonido")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    private bool canJump = true;
    private float cooldownTimer = 0f;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = padColor;
        }

        if (jumpSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }

        // Normalizar dirección personalizada
        if (useCustomDirection)
        {
            customDirection = customDirection.normalized;
        }
    }

    void Update()
    {
        // Manejar cooldown
        if (!canJump)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canJump = true;
            }
        }

        // Animar el JumpPad
        if (animateOnJump)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canJump) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // --- MODO CAÑÓN (PRECISIÓN TOTAL) ---
            // 1. Calculamos la dirección exacta hacia donde mira la plataforma (Flecha Azul)
            Vector3 forwardDirection = transform.forward;
            
            // 2. Calculamos la velocidad final
            // Horizontal: Hacia adelante con la fuerza configurada
            Vector3 finalVelocity = forwardDirection * horizontalBoost;
            
            // Vertical: Hacia arriba con la fuerza de salto
            finalVelocity.y = jumpForce;

            // 3. APLICAMOS LA VELOCIDAD DIRECTAMENTE
            // Sobrescribimos la velocidad actual para evitar desviaciones.
            // No importa cómo entres, siempre saldrás perfecto hacia donde apunta la plataforma.
            rb.linearVelocity = finalVelocity;
            
            // Resetear rotación angular para que no salga rodando loco
            rb.angularVelocity = Vector3.zero;

            Debug.Log($"JumpPad activado: Fuerza {jumpForce} aplicada a {collision.gameObject.name}");

            // Activar efectos
            ActivateEffects();

            // Iniciar cooldown
            canJump = false;
            cooldownTimer = cooldownTime;
        }
    }

    void ActivateEffects()
    {
        // Reproducir sonido
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }

        // Reproducir partículas
        if (jumpEffect != null)
        {
            jumpEffect.Play();
        }

        // Animar escala
        if (animateOnJump)
        {
            targetScale = originalScale * animationScale;
            StartCoroutine(ResetScaleAfterDelay());
        }
    }

    System.Collections.IEnumerator ResetScaleAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        targetScale = originalScale;
    }

    void OnDrawGizmos()
    {
        // Dibujar la dirección del salto
        Gizmos.color = canJump ? Color.green : Color.gray;
        
        Vector3 direction = useCustomDirection ? transform.TransformDirection(customDirection) : Vector3.up;
        Vector3 start = transform.position;
        Vector3 end = start + direction * (jumpForce * 0.2f);
        
        // Dibujar flecha
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.3f);
        
        // Dibujar base del JumpPad
        Gizmos.color = new Color(padColor.r, padColor.g, padColor.b, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
