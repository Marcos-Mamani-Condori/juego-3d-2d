using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // --- NUEVO ENUMERADOR DE MODOS DE CÁMARA ---
    public enum CameraMode { Golf, Shooter }
    public CameraMode currentCameraMode = CameraMode.Golf;

    // --- Configuración (Ajustar en el Inspector) ---
    [Header("Referencias y Distancia")]
    public Transform target; // Objeto que la cámara sigue (pelota o personaje)
    
    [Header("Configuración de Modo Golf")]
    public Vector3 golfLocalOffset = new Vector3(0f, 0f, -2f); // Posición relativa a la pelota en modo Golf
    public float golfSmoothSpeed = 0.125f; // Velocidad de suavizado del seguimiento en Golf
    public float golfMouseSensitivity = 5f; 
    public float golfMaxVerticalAngle = 80f; 
    public float golfMinVerticalAngle = -10f; 

    [Header("Configuración de Modo Shooter")]
    public Vector3 shooterLocalOffset = new Vector3(1f, 1.5f, -2f); // Posición relativa al personaje en modo Shooter (vista desde el hombro)
    public float shooterSmoothSpeed = 0.08f; // Velocidad de suavizado en Shooter (más rápido)
    public float shooterMouseSensitivity = 3f; // Menor sensibilidad para apuntar
    public float shooterMaxVerticalAngle = 80f; 
    public float shooterMinVerticalAngle = -30f; 

    // --- Variables de Estado ---
    private Rigidbody targetRb;
    private const float stoppingThreshold = 0.001f; 
    
    private float yaw;   
    private float pitch; 

    void Start()
    {
        // Inicializar rotación de la cámara
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        // El target será asignado por PersonajeControlador al inicio
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        // Actualizar Rigidbody del target si cambia (entre pelota y personaje)
        if (targetRb == null || targetRb.gameObject != target.gameObject)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }

        if (currentCameraMode == CameraMode.Golf)
        {
            HandleGolfCamera();
        }
        else // CameraMode.Shooter
        {
            HandleShooterCamera();
        }
        
        ApplyFinalPosition();
    }

    // --- Métodos Públicos para que PersonajeControlador cambie el modo ---
    public void SetMode(CameraMode newMode)
    {
        currentCameraMode = newMode;
        // Reiniciar la rotación para evitar saltos bruscos al cambiar de modo
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    // --- Lógica de la Cámara en Modo Golf ---
    private void HandleGolfCamera()
    {
        bool isMoving = (targetRb != null) ? targetRb.linearVelocity.sqrMagnitude > stoppingThreshold : false; 
        
        if (isMoving)
        {
            FollowMovingBall();
        }
        else
        {
            HandleFreeLook(golfMouseSensitivity, golfMaxVerticalAngle, golfMinVerticalAngle);
        }
    }

    private void FollowMovingBall()
    {
        transform.LookAt(target.position);
        
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void HandleFreeLook(float sensitivity, float maxAngle, float minAngle)
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        
        yaw += mouseX;
        pitch -= mouseY; 

        pitch = Mathf.Clamp(pitch, minAngle, maxAngle);

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);

        transform.rotation = targetRotation;
    }
    
    // --- Lógica de la Cámara en Modo Shooter ---
    private void HandleShooterCamera()
    {
        // En modo shooter, la cámara siempre se controla libremente para apuntar
        HandleFreeLook(shooterMouseSensitivity, shooterMaxVerticalAngle, shooterMinVerticalAngle);
    }
    
    // --- Lógica de Posicionamiento Final ---
    private void ApplyFinalPosition()
    {
        Vector3 currentOffset = (currentCameraMode == CameraMode.Golf) ? golfLocalOffset : shooterLocalOffset;
        float currentSmoothSpeed = (currentCameraMode == CameraMode.Golf) ? golfSmoothSpeed : shooterSmoothSpeed;

        Vector3 desiredPosition = target.position + transform.rotation * currentOffset;
        
        float speedMultiplier = currentSmoothSpeed * 50f; 
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, speedMultiplier * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}