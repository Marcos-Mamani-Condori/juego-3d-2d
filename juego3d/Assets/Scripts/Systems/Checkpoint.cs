using UnityEngine;

/// <summary>
/// Checkpoint individual que guarda la posición cuando la pelota pasa por él.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject (Cylinder o Cube)
/// 2. Hazlo alto y delgado como una columna
/// 3. Adjunta este script
/// 4. Añade un collider marcado como "Is Trigger"
/// 5. Ajusta el color y efectos
/// </summary>
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("¿Este checkpoint está activo?")]
    public bool isActive = true;
    
    [Tooltip("¿Solo puede activarse una vez?")]
    public bool oneTimeUse = false;

    [Header("Efectos Visuales")]
    [Tooltip("Color cuando está inactivo")]
    public Color inactiveColor = Color.gray;
    
    [Tooltip("Color cuando está activo")]
    public Color activeColor = Color.yellow;
    
    [Tooltip("Color cuando ha sido alcanzado")]
    public Color reachedColor = Color.green;
    
    [Tooltip("Efecto de partículas al alcanzar")]
    public ParticleSystem reachEffect;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido al alcanzar el checkpoint")]
    public AudioClip checkpointSound;

    private bool hasBeenReached = false;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();

        if (checkpointSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (hasBeenReached && oneTimeUse) return;

        if (other.CompareTag("Player"))
        {
            ActivateCheckpoint(other.gameObject);
        }
    }

    void ActivateCheckpoint(GameObject ball)
    {
        hasBeenReached = true;

        // Guardar en el sistema de checkpoints
        if (CheckpointSystem.Instance != null)
        {
            CheckpointSystem.Instance.SetCheckpoint(ball.transform.position, ball.transform.rotation);
        }

        // Efectos visuales
        UpdateVisuals();

        if (reachEffect != null)
        {
            reachEffect.Play();
        }

        // Sonido
        if (audioSource != null && checkpointSound != null)
        {
            audioSource.PlayOneShot(checkpointSound);
        }

        Debug.Log($"¡Checkpoint alcanzado: {gameObject.name}!");
    }

    void UpdateVisuals()
    {
        if (meshRenderer == null || meshRenderer.material == null) return;

        Color targetColor;
        if (!isActive)
        {
            targetColor = inactiveColor;
        }
        else if (hasBeenReached)
        {
            targetColor = reachedColor;
        }
        else
        {
            targetColor = activeColor;
        }

        meshRenderer.material.color = targetColor;

        // Hacer emisivo si está activo
        if (isActive)
        {
            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", targetColor);
        }
    }

    void Update()
    {
        // Rotación visual para que sea más visible
        if (isActive && !hasBeenReached)
        {
            transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Color gizmoColor = hasBeenReached ? reachedColor : (isActive ? activeColor : inactiveColor);
        gizmoColor.a = 0.5f;
        
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 1f);
        
        // Dibujar borde
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
