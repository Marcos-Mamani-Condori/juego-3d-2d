using UnityEngine;

/// <summary>
/// Trampa de pinchos que sale del suelo periódicamente.
/// Resetea la pelota si la toca.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Cube para los pinchos
/// 2. Adjunta este script
/// 3. Configura tiempos de activación
/// 4. Ajusta la altura de los pinchos
/// </summary>
public class SpikeTrap : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    [Tooltip("Tiempo que los pinchos están abajo (segundos)")]
    public float retractedTime = 2f;
    
    [Tooltip("Tiempo que los pinchos están arriba (segundos)")]
    public float extendedTime = 2f;
    
    [Tooltip("Tiempo de advertencia antes de salir (parpadeo)")]
    public float warningTime = 0.5f;
    
    [Tooltip("Velocidad de salida/entrada")]
    public float movementSpeed = 5f;

    [Header("Movimiento")]
    [Tooltip("Altura máxima de los pinchos")]
    public float maxHeight = 2f;
    
    [Tooltip("¿Comenzar extendidos?")]
    public bool startExtended = false;

    [Header("Efectos")]
    [Tooltip("Color normal")]
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f);
    
    [Tooltip("Color de advertencia")]
    public Color warningColor = Color.red;
    
    [Tooltip("Sonido al extender")]
    public AudioClip extendSound;
    
    [Tooltip("Sonido de advertencia")]
    public AudioClip warningSound;

    private bool isExtended = false;
    private float timer;
    private Vector3 retractedPosition;
    private Vector3 extendedPosition;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private bool isInWarning = false;

    void Start()
    {
        retractedPosition = transform.localPosition;
        extendedPosition = retractedPosition + new Vector3(0f, maxHeight, 0f);

        meshRenderer = GetComponent<MeshRenderer>();
        
        if (extendSound != null || warningSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        isExtended = startExtended;
        timer = isExtended ? extendedTime : retractedTime;

        if (isExtended)
        {
            transform.localPosition = extendedPosition;
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // Advertencia antes de extender
        if (!isExtended && timer <= warningTime && timer > 0f)
        {
            if (!isInWarning)
            {
                isInWarning = true;
                if (audioSource != null && warningSound != null)
                {
                    audioSource.PlayOneShot(warningSound);
                }
            }

            // Parpadeo de advertencia
            if (meshRenderer != null && meshRenderer.material != null)
            {
                float t = Mathf.PingPong(Time.time * 10f, 1f);
                meshRenderer.material.color = Color.Lerp(normalColor, warningColor, t);
            }
        }
        else if (isInWarning)
        {
            isInWarning = false;
            if (meshRenderer != null && meshRenderer.material != null)
            {
                meshRenderer.material.color = normalColor;
            }
        }

        // Cambiar estado
        if (timer <= 0f)
        {
            isExtended = !isExtended;
            timer = isExtended ? extendedTime : retractedTime;

            if (isExtended && audioSource != null && extendSound != null)
            {
                audioSource.PlayOneShot(extendSound);
            }
        }

        // Mover pinchos
        Vector3 targetPosition = isExtended ? extendedPosition : retractedPosition;
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            movementSpeed * Time.deltaTime
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isExtended && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("¡Pelota golpeó los pinchos! Reseteando...");
            
            if (CheckpointSystem.Instance != null)
            {
                CheckpointSystem.Instance.ResetToCheckpoint();
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 basePos = Application.isPlaying ? retractedPosition : transform.localPosition;
        Vector3 topPos = basePos + new Vector3(0f, maxHeight, 0f);

        Gizmos.color = isExtended ? Color.red : Color.gray;
        Gizmos.DrawLine(
            transform.parent != null ? transform.parent.TransformPoint(basePos) : basePos,
            transform.parent != null ? transform.parent.TransformPoint(topPos) : topPos
        );
    }
}
