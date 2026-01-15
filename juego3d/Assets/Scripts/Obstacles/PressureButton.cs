using UnityEngine;

/// <summary>
/// Plataforma que se activa/desactiva con un botón de presión.
/// Útil para puzzles y mecánicas de plataformas.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject pequeño para el botón (ej: Cylinder aplanado)
/// 2. Adjunta PressureButton.cs al botón
/// 3. Crea el objeto que quieres controlar (pared, plataforma, etc.)
/// 4. Adjunta este script ButtonActivatedObject.cs al objeto controlado
/// 5. En el botón, arrastra el objeto controlado al campo "activatedObject"
/// </summary>
public class PressureButton : MonoBehaviour
{
    [Header("Configuración del Botón")]
    [Tooltip("Objeto que se activa/desactiva con este botón")]
    public GameObject activatedObject;
    
    [Tooltip("¿El botón se queda presionado o es temporal?")]
    public bool stayPressed = false;
    
    [Tooltip("Que tag debe tener el objeto para activar el botón")]
    public string activatorTag = "Player";

    [Header("Movimiento del Botón")]
    [Tooltip("Distancia que baja el botón cuando se presiona")]
    public float pressDepth = 0.1f;
    
    [Tooltip("Velocidad de animación del botón")]
    public float animationSpeed = 10f;

    [Header("Efectos")]
    [Tooltip("Color cuando está desactivado")]
    public Color inactiveColor = Color.gray;
    
    [Tooltip("Color cuando está activado")]
    public Color activeColor = Color.green;
    
    [Tooltip("Sonido al presionar")]
    public AudioClip pressSound;
    
    [Tooltip("Sonido al despresionar")]
    public AudioClip releaseSound;

    private bool isPressed = false;
    private int objectsOnButton = 0;
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition - new Vector3(0f, pressDepth, 0f);

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = inactiveColor;
        }

        if (pressSound != null || releaseSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (activatedObject != null)
        {
            activatedObject.SetActive(false);
        }
    }

    void Update()
    {
        // Animar posición del botón
        Vector3 targetPosition = isPressed ? pressedPosition : originalPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * animationSpeed);

        // Si no se queda presionado y no hay objetos encima, despresionar
        if (!stayPressed && objectsOnButton == 0 && isPressed)
        {
            SetPressed(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            objectsOnButton++;
            
            if (!isPressed)
            {
                SetPressed(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            objectsOnButton--;
            objectsOnButton = Mathf.Max(0, objectsOnButton);
        }
    }

    void SetPressed(bool pressed)
    {
        if (isPressed == pressed) return;

        isPressed = pressed;

        // Activar/desactivar objeto
        if (activatedObject != null)
        {
            activatedObject.SetActive(pressed);
        }

        // Cambiar color
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = pressed ? activeColor : inactiveColor;
        }

        // Reproducir sonido
        if (audioSource != null)
        {
            AudioClip soundToPlay = pressed ? pressSound : releaseSound;
            if (soundToPlay != null)
            {
                audioSource.PlayOneShot(soundToPlay);
            }
        }

        Debug.Log($"Botón {gameObject.name} ahora está: {(pressed ? "PRESIONADO" : "LIBERADO")}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isPressed ? activeColor : inactiveColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
        
        // Dibujar conexión al objeto activado
        if (activatedObject != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, activatedObject.transform.position);
        }
    }
}
