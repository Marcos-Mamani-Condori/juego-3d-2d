using UnityEngine;

/// <summary>
/// Portal que teletransporta la pelota a otro portal vinculado.
/// Similar a los portales de Portal o Mario.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea dos GameObjects con forma de anillo o esfera
/// 2. Adjunta este script a AMBOS
/// 3. En el Inspector de cada uno, asigna el OTRO portal como "linkedPortal"
/// 4. Marca los colliders como "Is Trigger"
/// 5. Ajusta el impulso de salida si quieres velocidad adicional
/// </summary>
[RequireComponent(typeof(Collider))]
public class TeleportPortal : MonoBehaviour
{
    [Header("Configuración del Portal")]
    [Tooltip("El portal de destino al que teletransportar")]
    public TeleportPortal linkedPortal;
    
    [Tooltip("Tiempo de cooldown para evitar bucles infinitos (segundos)")]
    public float cooldownTime = 1f;
    
    [Tooltip("¿Mantener la velocidad al teletransportar?")]
    public bool preserveVelocity = true;
    
    [Tooltip("Multiplicador de velocidad al salir (1 = normal)")]
    public float velocityMultiplier = 1f;
    
    [Tooltip("Impulso adicional hacia adelante al salir")]
    public float exitBoost = 0f;

    [Header("Dirección de Salida")]
    [Tooltip("¿Usar dirección personalizada para la salida?")]
    public bool useCustomExitDirection = false;
    
    [Header("Configuración Unidireccional")]
    [Tooltip("Si es verdadero, este portal puede teletransportar. Si es falso, solo es un destino de salida.")]
    public bool isEntrance = true;

    [Tooltip("Dirección de salida (relativa al portal de destino)")]
    public Vector3 customExitDirection = Vector3.forward;

    [Header("Efectos Visuales")]
    [Tooltip("Efecto de partículas al entrar")]
    public ParticleSystem enterEffect;
    
    [Tooltip("Efecto de partículas al salir")]
    public ParticleSystem exitEffect;
    
    [Tooltip("Color del portal")]
    public Color portalColor = Color.cyan;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido al teletransportar")]
    public AudioClip teleportSound;
    
    [Tooltip("Sonido ambiental del portal (loop)")]
    public AudioClip portalAmbience;

    private bool canTeleport = true;
    private float cooldownTimer = 0f;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = portalColor;
            // Hacer el portal emisivo
            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", portalColor * 2f);
        }

        if (teleportSound != null || portalAmbience != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            
            if (portalAmbience != null)
            {
                audioSource.clip = portalAmbience;
                audioSource.loop = true;
                audioSource.volume = 0.5f;
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        if (!canTeleport)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                canTeleport = true;
            }
        }

        // Rotar el portal para efecto visual
        transform.Rotate(Vector3.forward, 30f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isEntrance) return;
        if (!canTeleport || linkedPortal == null) return;

        // Verificar que el portal destino también puede recibir
        if (!linkedPortal.canTeleport) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            TeleportObject(other.gameObject, rb);
        }
    }

    void TeleportObject(GameObject obj, Rigidbody rb)
    {
        // Guardar velocidad actual
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentAngularVelocity = rb.angularVelocity;

        // Calcular nueva posición
        Vector3 offset = obj.transform.position - transform.position;
        Vector3 newPosition = linkedPortal.transform.position + offset;

        // Teletransportar
        obj.transform.position = newPosition;

        // Calcular nueva velocidad
        if (preserveVelocity)
        {
            Vector3 newVelocity = currentVelocity * velocityMultiplier;
            
            if (useCustomExitDirection)
            {
                // Usar dirección personalizada
                Vector3 exitDir = linkedPortal.transform.TransformDirection(customExitDirection);
                newVelocity = exitDir * currentVelocity.magnitude * velocityMultiplier;
            }

            // Añadir impulso adicional
            if (exitBoost > 0f)
            {
                Vector3 boostDirection = useCustomExitDirection 
                    ? linkedPortal.transform.TransformDirection(customExitDirection)
                    : linkedPortal.transform.forward;
                
                newVelocity += boostDirection * exitBoost;
            }

            rb.linearVelocity = newVelocity;
            
            if (preserveVelocity)
            {
                rb.angularVelocity = currentAngularVelocity;
            }
        }

        // Activar efectos
        if (enterEffect != null) enterEffect.Play();
        if (linkedPortal.exitEffect != null) linkedPortal.exitEffect.Play();

        // Reproducir sonido
        if (audioSource != null && teleportSound != null)
        {
            audioSource.PlayOneShot(teleportSound);
        }

        // Activar cooldown en AMBOS portales
        canTeleport = false;
        cooldownTimer = cooldownTime;
        linkedPortal.canTeleport = false;
        linkedPortal.cooldownTimer = cooldownTime;

        Debug.Log($"[PORTAL] {obj.name} teletransportado de {gameObject.name} a {linkedPortal.gameObject.name}");
    }

    void OnDrawGizmos()
    {
        // Dibujar el portal
        Gizmos.color = new Color(portalColor.r, portalColor.g, portalColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, 1f);
        
        // Dibujar borde
        Gizmos.color = portalColor;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // Dibujar conexión al portal vinculado
        if (linkedPortal != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, linkedPortal.transform.position);
        }

        // Dibujar dirección de salida
        if (linkedPortal != null && useCustomExitDirection)
        {
            Gizmos.color = Color.green;
            Vector3 exitDir = linkedPortal.transform.TransformDirection(customExitDirection);
            Gizmos.DrawRay(linkedPortal.transform.position, exitDir * 2f);
        }
    }
}
