using UnityEngine;

/// <summary>
/// Agujero negro que atrae objetos hacia su centro.
/// Puede destruir o teletransportar la pelota.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Sphere
/// 2. Adjunta este script
/// 3. Configura la fuerza de atracción
/// 4. Crea un efecto de partículas espiral (opcional)
/// </summary>
public class BlackHole : MonoBehaviour
{
    [Header("Configuración de Atracción")]
    [Tooltip("Fuerza de atracción hacia el centro")]
    public float pullForce = 20f;
    
    [Tooltip("Radio de detección (usa el trigger collider)")]
    public float detectionRadius = 10f;
    
    [Tooltip("Radio del núcleo (destruye al entrar)")]
    public float coreRadius = 1f;

    [Header("Comportamiento")]
    [Tooltip("¿Qué hacer al llegar al centro?")]
    public BlackHoleAction action = BlackHoleAction.Reset;
    
    [Tooltip("Portal de salida (si usas Teleport)")]
    public Transform exitPortal;

    public enum BlackHoleAction
    {
        Reset,      // Resetea al checkpoint
        Teleport,   // Teletransporta a otro lugar
        Destroy     // Destruye la pelota (game over)
    }

    [Header("Efectos Visuales")]
    [Tooltip("Velocidad de rotación del agujero negro")]
    public float rotationSpeed = 100f;
    
    [Tooltip("Sistema de partículas espiral")]
    public ParticleSystem spiralEffect;
    
    [Tooltip("Color del agujero negro")]
    public Color holeColor = new Color(0.1f, 0f, 0.3f);

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido de succión (loop)")]
    public AudioClip suctionSound;
    
    [Tooltip("Sonido al ser absorbido")]
    public AudioClip absorbSound;

    private SphereCollider detectionCollider;
    private SphereCollider coreCollider;
    private AudioSource audioSource;

    void Start()
    {
        // Configurar collider de detección
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.radius = detectionRadius;
        detectionCollider.isTrigger = true;

        // Configurar collider del núcleo
        GameObject core = new GameObject("BlackHole_Core");
        core.transform.parent = transform;
        core.transform.localPosition = Vector3.zero;
        coreCollider = core.AddComponent<SphereCollider>();
        coreCollider.radius = coreRadius;
        coreCollider.isTrigger = true;
        core.tag = "BlackHoleCore";

        // Configurar visual
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = holeColor;
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", holeColor * 2f);
        }

        // Configurar sonido
        if (suctionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = suctionSound;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.spatialBlend = 1f;
            audioSource.Play();
        }
    }

    void Update()
    {
        // Rotar el agujero negro
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerStay(Collider other)
    {
        // Atraer objetos hacia el centro
        if (other.CompareTag("BlackHoleCore")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, other.transform.position);
            
            // Fuerza aumenta cerca del centro
            float distanceFactor = Mathf.Clamp01(detectionRadius / (distance + 0.1f));
            Vector3 force = direction * pullForce * distanceFactor;
            
            rb.AddForce(force, ForceMode.Force);

            // Verificar si llegó al núcleo
            if (distance <= coreRadius)
            {
                AbsorbObject(other.gameObject, rb);
            }
        }
    }

    void AbsorbObject(GameObject obj, Rigidbody rb)
    {
        if (!obj.CompareTag("Player")) return;

        Debug.Log($"¡Agujero negro absorbió a {obj.name}!");

        // Reproducir sonido
        if (audioSource != null && absorbSound != null)
        {
            audioSource.PlayOneShot(absorbSound);
        }

        // Ejecutar acción
        switch (action)
        {
            case BlackHoleAction.Reset:
                if (CheckpointSystem.Instance != null)
                {
                    CheckpointSystem.Instance.ResetToCheckpoint();
                }
                break;

            case BlackHoleAction.Teleport:
                if (exitPortal != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    obj.transform.position = exitPortal.position;
                }
                break;

            case BlackHoleAction.Destroy:
                Destroy(obj);
                break;
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar radio de detección
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Dibujar núcleo
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, coreRadius);

        // Línea al portal de salida
        if (action == BlackHoleAction.Teleport && exitPortal != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, exitPortal.position);
        }
    }
}
