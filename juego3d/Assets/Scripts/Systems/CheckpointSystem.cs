using UnityEngine;

/// <summary>
/// Sistema de checkpoints y reset para el juego.
/// Permite guardar posiciones y resetear la pelota al último checkpoint.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject vacío y nómbralo "GameManager"
/// 2. Adjunta este script al GameManager
/// 3. Asigna la pelota y el robot en el Inspector
/// 4. Configura la tecla de reset (por defecto: R)
/// 5. Crea checkpoints usando el script Checkpoint.cs
/// </summary>
public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;

    [Header("Referencias de Objetos")]
    [Tooltip("La pelota de golf")]
    public GameObject golfBall;
    
    [Tooltip("El robot jugador")]
    public GameObject robot;
    
    [Tooltip("Cámara principal")]
    public Camera mainCamera;

    [Header("Configuración de Reset")]
    [Tooltip("Tecla para resetear al último checkpoint")]
    public KeyCode resetKey = KeyCode.R;
    
    [Tooltip("Tecla para resetear al inicio absoluto")]
    public KeyCode hardResetKey = KeyCode.Backspace;

    [Header("Posiciones Iniciales")]
    [Tooltip("¿Guardar posición inicial automáticamente?")]
    public bool saveInitialPosition = true;

    private Vector3 initialBallPosition;
    private Quaternion initialBallRotation;
    private Vector3 initialRobotPosition;
    private Quaternion initialRobotRotation;

    private Vector3 currentCheckpointPosition;
    private Quaternion currentCheckpointRotation;
    private bool hasCheckpoint = false;

    [Header("Efectos")]
    [Tooltip("Efecto al resetear")]
    public ParticleSystem resetEffect;
    
    [Tooltip("Sonido al resetear")]
    public AudioClip resetSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (golfBall == null)
        {
            golfBall = GameObject.FindGameObjectWithTag("Player");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (resetSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Guardar posiciones iniciales
        if (saveInitialPosition && golfBall != null)
        {
            initialBallPosition = golfBall.transform.position;
            initialBallRotation = golfBall.transform.rotation;

            if (robot != null)
            {
                initialRobotPosition = robot.transform.position;
                initialRobotRotation = robot.transform.rotation;
            }

            // El checkpoint inicial es la posición de inicio
            currentCheckpointPosition = initialBallPosition;
            currentCheckpointRotation = initialBallRotation;
            hasCheckpoint = true;
        }
    }

    void Update()
    {
        // Reset al último checkpoint
        if (Input.GetKeyDown(resetKey))
        {
            ResetToCheckpoint();
        }

        // Hard reset (volver al inicio absoluto)
        if (Input.GetKeyDown(hardResetKey))
        {
            HardReset();
        }
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        currentCheckpointPosition = position;
        currentCheckpointRotation = rotation;
        hasCheckpoint = true;
        Debug.Log($"Checkpoint guardado en: {position}");
    }

    public void ResetToCheckpoint()
    {
        if (!hasCheckpoint || golfBall == null)
        {
            Debug.LogWarning("No hay checkpoint guardado o la pelota no está asignada.");
            return;
        }

        // Detener la pelota
        Rigidbody rb = golfBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        // Mover al checkpoint
        golfBall.transform.position = currentCheckpointPosition;
        golfBall.transform.rotation = currentCheckpointRotation;

        // Mover el robot cerca de la pelota
        if (robot != null)
        {
            PersonajeControlador personaje = robot.GetComponent<PersonajeControlador>();
            if (personaje != null)
            {
                personaje.MoverseHaciaLaPelota();
            }
        }

        // Efectos
        if (resetEffect != null)
        {
            resetEffect.transform.position = currentCheckpointPosition;
            resetEffect.Play();
        }

        if (audioSource != null && resetSound != null)
        {
            audioSource.PlayOneShot(resetSound);
        }

        Debug.Log("Pelota reseteada al último checkpoint");
    }

    public void HardReset()
    {
        if (golfBall == null) return;

        // Detener la pelota
        Rigidbody rb = golfBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        // Volver a la posición inicial absoluta
        golfBall.transform.position = initialBallPosition;
        golfBall.transform.rotation = initialBallRotation;

        if (robot != null)
        {
            robot.transform.position = initialRobotPosition;
            robot.transform.rotation = initialRobotRotation;
        }

        // Resetear el checkpoint actual al inicial
        currentCheckpointPosition = initialBallPosition;
        currentCheckpointRotation = initialBallRotation;

        // Efectos
        if (resetEffect != null)
        {
            resetEffect.transform.position = initialBallPosition;
            resetEffect.Play();
        }

        if (audioSource != null && resetSound != null)
        {
            audioSource.PlayOneShot(resetSound);
        }

        Debug.Log("¡HARD RESET! Vuelto al inicio absoluto");
    }
}
