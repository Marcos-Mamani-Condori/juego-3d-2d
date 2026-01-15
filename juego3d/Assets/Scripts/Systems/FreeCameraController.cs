using UnityEngine;

/// <summary>
/// Controlador de cámara libre para explorar el nivel sin restricciones.
/// Presiona una tecla para activar/desactivar el modo vista libre.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Adjunta este script a la Main Camera
/// 2. Configura la tecla de activación (por defecto: V)
/// 3. Presiona la tecla durante el juego para activar modo libre
/// 4. Usa WASD para mover, Mouse para rotar, Q/E para subir/bajar
/// </summary>
public class FreeCameraController : MonoBehaviour
{
    [Header("Activación")]
    [Tooltip("Tecla para activar/desactivar modo cámara libre")]
    public KeyCode toggleKey = KeyCode.V;
    
    [Tooltip("¿Comenzar en modo libre?")]
    public bool startInFreeMode = false;

    [Header("Velocidad de Movimiento")]
    [Tooltip("Velocidad normal de movimiento")]
    public float normalSpeed = 10f;
    
    [Tooltip("Velocidad rápida (con Shift)")]
    public float fastSpeed = 50f;
    
    [Tooltip("Velocidad lenta (con Ctrl)")]
    public float slowSpeed = 2f;

    [Header("Sensibilidad de Rotación")]
    [Tooltip("Sensibilidad del mouse")]
    public float mouseSensitivity = 3f;
    
    [Tooltip("Límite de rotación vertical (grados)")]
    public float verticalRotationLimit = 90f;

    [Header("Suavizado")]
    [Tooltip("Suavizado del movimiento (0 = sin suavizado, mayor = más suave)")]
    [Range(0f, 0.9f)]
    public float movementSmoothing = 0.1f;
    
    [Tooltip("Suavizado de la rotación")]
    [Range(0f, 0.9f)]
    public float rotationSmoothing = 0.1f;

    [Header("Referencias")]
    [Tooltip("Script de cámara original (se desactivará en modo libre)")]
    public CameraFollow cameraFollowScript;

    [Header("Herramientas de Demo")]
    [Tooltip("Permitir teletransporte con tecla T (Útil para demos)")]
    public bool enableTeleport = true;

    private bool isFreeModeActive = false;
    private float currentVerticalRotation = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    // Guardar estado original
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    void Start()
    {
        if (cameraFollowScript == null)
        {
            cameraFollowScript = GetComponent<CameraFollow>();
        }

        // Guardar estado original
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        targetPosition = transform.position;
        targetRotation = transform.rotation;

        if (startInFreeMode)
        {
            ActivateFreeMode();
        }
    }

    void Update()
    {
        // Toggle del modo libre
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFreeMode();
        }

        if (isFreeModeActive)
        {
            HandleFreeCameraMovement();
            HandleFreeCameraRotation();
        }
    }

    void ToggleFreeMode()
    {
        if (isFreeModeActive)
        {
            DeactivateFreeMode();
        }
        else
        {
            ActivateFreeMode();
        }
    }

    void ActivateFreeMode()
    {
        isFreeModeActive = true;
        
        // Desactivar script de cámara original
        if (cameraFollowScript != null)
        {
            cameraFollowScript.enabled = false;
        }

        // Desemparentar la cámara
        transform.parent = null;

        // Bloquear y ocultar el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar rotación vertical
        Vector3 currentRotation = transform.eulerAngles;
        currentVerticalRotation = currentRotation.x;
        if (currentVerticalRotation > 180f)
        {
            currentVerticalRotation -= 360f;
        }

        Debug.Log("🎥 MODO CÁMARA LIBRE ACTIVADO - Usa WASD para mover, Mouse para rotar, Q/E para subir/bajar");
    }

    void DeactivateFreeMode()
    {
        isFreeModeActive = false;

        // Reactivar script de cámara original
        if (cameraFollowScript != null)
        {
            cameraFollowScript.enabled = true;
        }

        // Restaurar estado del cursor (depende del modo del juego)
        // Por defecto dejamos el cursor visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("🎥 Modo cámara libre DESACTIVADO");
    }

    void HandleFreeCameraMovement()
    {
        // Determinar velocidad actual
        float currentSpeed = normalSpeed;
        
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = fastSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            currentSpeed = slowSpeed;
        }

        // Capturar input de movimiento
        Vector3 moveDirection = Vector3.zero;

        // WASD para movimiento horizontal
        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;

        // Q/E para subir/bajar
        if (Input.GetKey(KeyCode.E)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) moveDirection -= Vector3.up;

        // Normalizar para que no se mueva más rápido en diagonal
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Calcular nueva posición
        targetPosition += moveDirection * currentSpeed * Time.deltaTime;

        // Aplicar con suavizado
        if (movementSmoothing > 0f)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref currentVelocity, 
                movementSmoothing
            );
        }
        else
        {
            transform.position = targetPosition;
        }

        // --- TELETRANSPORTE ---
        if (enableTeleport && Input.GetKeyDown(KeyCode.T))
        {
            TeleportPlayerHere();
        }
    }

    void TeleportPlayerHere()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Pelota");
        if (player != null)
        {
            // Mover jugador
            player.transform.position = transform.position + transform.forward * 2f; // 2 metros enfrente de la cámara
            
            // Resetear físicas si tiene Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log($"✨ Teletransportado {player.name} a la posición de la cámara");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró ningún objeto con el tag 'Player' para teletransportar.");
        }
    }

    void HandleFreeCameraRotation()
    {
        // Capturar input del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal (Y axis)
        float horizontalRotation = transform.eulerAngles.y + mouseX;

        // Rotación vertical (X axis) con límites
        currentVerticalRotation -= mouseY;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, -verticalRotationLimit, verticalRotationLimit);

        // Aplicar rotación
        targetRotation = Quaternion.Euler(currentVerticalRotation, horizontalRotation, 0f);

        if (rotationSmoothing > 0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - rotationSmoothing);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    // OnGUI eliminado para usar GameUIController centralizado

    void OnDisable()
    {
        // Restaurar el cursor al desactivar el script
        if (isFreeModeActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
