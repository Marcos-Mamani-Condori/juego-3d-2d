using UnityEngine;

/// <summary>
/// Zona de gravedad que modifica la gravedad dentro de su área.
/// Puede invertir la gravedad, reducirla o aumentarla.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject vacío
/// 2. Añade un Box Collider marcado como "Is Trigger"
/// 3. Adjunta este script
/// 4. Configura el multiplicador de gravedad
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class GravityZone : MonoBehaviour
{
    [Header("Configuración de Gravedad")]
    [Tooltip("Multiplicador de gravedad (1 = normal, -1 = invertida, 0 = sin gravedad)")]
    public float gravityMultiplier = -1f;
    
    [Tooltip("¿Usar dirección de gravedad personalizada?")]
    public bool useCustomGravityDirection = false;
    
    [Tooltip("Dirección personalizada de gravedad")]
    public Vector3 customGravityDirection = Vector3.down;

    [Header("Efectos Visuales")]
    [Tooltip("Color de la zona")]
    public Color zoneColor = new Color(0.5f, 0f, 1f, 0.3f);
    
    [Tooltip("¿Mostrar partículas?")]
    public bool showParticles = true;
    
    [Tooltip("Sistema de partículas (opcional)")]
    public ParticleSystem gravityParticles;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido ambiental de la zona")]
    public AudioClip zoneAmbience;

    private AudioSource audioSource;

    void Start()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (zoneAmbience != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = zoneAmbience;
            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.spatialBlend = 1f;
            audioSource.Play();
        }
    }

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            // Cancelar la gravedad normal
            rb.useGravity = false;

            // Aplicar gravedad personalizada
            Vector3 gravityDirection;
            if (useCustomGravityDirection)
            {
                gravityDirection = customGravityDirection.normalized;
            }
            else
            {
                gravityDirection = Vector3.down;
            }

            Vector3 customGravity = gravityDirection * (Physics.gravity.magnitude * gravityMultiplier);
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Restaurar gravedad normal
            rb.useGravity = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = zoneColor;
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(col.center, col.size);
            Gizmos.DrawWireCube(col.center, col.size);
        }

        // Dibujar flechas de dirección de gravedad
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.magenta;
        Vector3 direction = useCustomGravityDirection ? customGravityDirection.normalized : Vector3.down;
        direction *= gravityMultiplier;
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 offset = new Vector3(i * 2f, 0f, j * 2f);
                Vector3 start = transform.position + offset;
                Vector3 end = start + direction * 2f;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
