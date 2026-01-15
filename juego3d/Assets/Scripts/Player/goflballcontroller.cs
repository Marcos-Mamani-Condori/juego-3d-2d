using System.Collections;
using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    // --- Configuración de Golpe ---
    [Header("Configuración de Golpe")]
    public float maxHitForce = 50f;
    public float loftAngle = 0.04f;
    public float powerChargeRate = 1.0f;
    public float aimRotationSpeed = 80f;

    // --- Estabilidad y Animación ---
    [Header("Ajuste de Estabilidad")]
    [Tooltip("Tiempo mínimo que debe estar 'en el aire' antes de chequear si se ha detenido.")]
    public float minMovementTime = 0.5f;
    
    [Header("Referencia al Personaje")]
    public PersonajeControlador miPersonajeControlador;

    // --- Control de Potencia Cíclica ---
    [Header("Control de Potencia Cíclica")]
    public float swingSpeed = 1f;
    public float maxIndicatorScale = 10f;

    // --- Referencias de Pivote y Visuales ---
    [Header("Referencias de Pivote")]
    public Transform aimPivot;
    [Header("Referencias Visuales")]
    public GameObject aimIndicator;
    public GameObject directionPointer;

    // --- Variables Internas ---
    private Rigidbody rb;
    private bool isCharging = false;
    private float currentPower = 0f;
    private float timeSinceChargeStart = 0f;
    private bool hasBeenHit = false; // Indica si la bola ha sido golpeada y está en movimiento
    private float timeSinceHit = 0f;
    private Vector3 originalIndicatorScale;
    private Vector3 originalIndicatorPosition;
    private float storedPower = 0f;
    private Coroutine captureRoutine;

    // --- UMBRALES ---
    private const float PERFECT_HIT_THRESHOLD = 0.95f;
    private const float STOPPING_THRESHOLD_SQRD = 0.0001f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Error FATAL: El componente Rigidbody es necesario.");
            enabled = false;
            return;
        }

        rb.Sleep();
        hasBeenHit = false;
        timeSinceHit = 0f;

        if (aimIndicator != null)
        {
            originalIndicatorScale = aimIndicator.transform.localScale;
            originalIndicatorPosition = aimIndicator.transform.localPosition;
            aimIndicator.SetActive(false);
        }
        // El estado inicial del directionPointer se maneja en OnEnable/OnDisable
    }

    // Se llama cuando el GameObject se activa (enabled = true)
    void OnEnable()
    {
        if (rb != null) rb.Sleep(); // Asegúrate de que la bola esté dormida al activar
        hasBeenHit = false;
        timeSinceHit = 0f;

        if (directionPointer != null)
        {
            directionPointer.SetActive(true); // Mostrar el puntero de dirección
        }
        if (aimIndicator != null)
        {
            aimIndicator.SetActive(false); // Ocultar el indicador de carga
        }
    }

    // Se llama cuando el GameObject se desactiva (enabled = false)
    void OnDisable()
    {
        if (captureRoutine != null)
        {
            StopCoroutine(captureRoutine);
            captureRoutine = null;
        }

        if (aimIndicator != null) aimIndicator.SetActive(false);
        if (directionPointer != null) directionPointer.SetActive(false);
        isCharging = false; // Reiniciar estado de carga por si acaso
    }

    void Update()
    {
        // Si el script está deshabilitado por PersonajeControlador, este Update no se ejecuta.
        // Si la bola ha sido golpeada y está en movimiento, solo gestiona su estado.
        if (hasBeenHit)
        {
            timeSinceHit += Time.deltaTime;

            if (timeSinceHit < minMovementTime)
            {
                return; // Esperar un tiempo mínimo antes de considerar que se detuvo
            }

            if (!IsMoving())
            {
                // La bola se ha detenido
                if (!rb.IsSleeping())
                {
                    Debug.Log($"[DETENIDO] Fuerza aplicada: {maxHitForce * storedPower:F2}N. Velocidad sqr: {rb.linearVelocity.sqrMagnitude:F6}. TURNO FINALIZADO.");
                    rb.Sleep();
                }

                hasBeenHit = false;
                timeSinceHit = 0f;
                
                // Mover el personaje a la posición de la bola
                if (miPersonajeControlador != null)
                {
                    miPersonajeControlador.MoverseHaciaLaPelota();
                    // Una vez que el personaje se mueve, reactivamos el puntero de dirección
                    if (directionPointer != null) directionPointer.SetActive(true);
                }
            }
        }
        else // La bola NO ha sido golpeada, y está lista para el próximo golpe
        {
            // Actualizar la posición del pivote de puntería
            if (aimPivot != null)
            {
                aimPivot.position = transform.position;
            }

            HandleAiming();
            HandlePowerCharge();
        }
    }

    public void ExecuteHitFromAnimation()
    {
        if (hasBeenHit) return; // Ya está en movimiento, no golpear de nuevo

        ApplyHit(storedPower);
        
        hasBeenHit = true;
        timeSinceHit = 0f;

        // Ocultar indicadores cuando la bola ha sido golpeada
        if (directionPointer != null) directionPointer.SetActive(false);
        if (aimIndicator != null) aimIndicator.SetActive(false);
    }

    private void HandleAiming()
    {
        // Solo permitir apuntar si no estamos cargando y la bola no ha sido golpeada
        if (isCharging || hasBeenHit) return;

        float rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.Q)) rotationInput = -1f;
        else if (Input.GetKey(KeyCode.E)) rotationInput = 1f;
        
        if (rotationInput != 0f)
        {
            if (aimPivot != null)
            {
                aimPivot.Rotate(Vector3.up, rotationInput * aimRotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandlePowerCharge()
    {
        // Solo permitir carga si la bola no ha sido golpeada
        if (hasBeenHit) return;

        if (Input.GetKey(KeyCode.Space))
        {
            if (!isCharging)
            {
                // Iniciar la carga
                if (directionPointer != null) directionPointer.SetActive(false);
                
                if (aimIndicator != null)
                {
                    aimIndicator.SetActive(true);
                    aimIndicator.transform.localScale = originalIndicatorScale;
                    aimIndicator.transform.localPosition = originalIndicatorPosition;
                }

                currentPower = 0f;
                timeSinceChargeStart = 0f;
                isCharging = true;
            }

            // Actualizar la carga
            timeSinceChargeStart += Time.deltaTime;
            currentPower = Mathf.PingPong(timeSinceChargeStart * swingSpeed, 1.0f);

            // Actualizar la visual del indicador
            if (aimIndicator != null)
            {
                float scaleFactorY = Mathf.Lerp(1.0f, maxIndicatorScale, currentPower);
                float maxSideScale = 1.5f;
                float sideFactor = Mathf.Lerp(1.0f, maxSideScale, currentPower);

                Vector3 newScale = new Vector3(
                    originalIndicatorScale.x * sideFactor,
                    originalIndicatorScale.y * scaleFactorY,
                    originalIndicatorScale.z * sideFactor
                );
                aimIndicator.transform.localScale = newScale;

                float scaleDifference = newScale.y - originalIndicatorScale.y;
                Vector3 newPosition = originalIndicatorPosition;
                newPosition.y += scaleDifference / 2f;
                
                aimIndicator.transform.localPosition = newPosition;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Finalizar la carga y aplicar el golpe
            if (isCharging && !hasBeenHit)
            {
                storedPower = currentPower;
                
                if (currentPower >= PERFECT_HIT_THRESHOLD)
                {
                    storedPower = 1.0f;
                }

                if (miPersonajeControlador != null)
                {
                    miPersonajeControlador.IniciarSecuenciaDeGolpe();
                }
                else
                {
                    ExecuteHitFromAnimation();
                }

                isCharging = false;
                currentPower = 0f;
                timeSinceChargeStart = 0f;

                if (aimIndicator != null)
                {
                    aimIndicator.SetActive(false);
                }
            }
        }
    }

    private void ApplyHit(float power)
    {
        rb.WakeUp();
        
        Vector3 flatDirection;
        
        if (aimPivot != null)
        {
            Vector3 rawDirection = aimPivot.forward;
            rawDirection.y = 0;
            flatDirection = rawDirection.normalized;
        }
        else
        {
            Vector3 rawDirection = transform.forward;
            rawDirection.y = 0;
            flatDirection = rawDirection.normalized;
        }
        
        float finalForce = maxHitForce * power;
        
        Debug.Log($"[GOLPE APLICADO] Fuerza TOTAL (N): {finalForce:F2}");

        HitBall(flatDirection, finalForce);
        
        if (loftAngle > 0.0f)
        {
            float liftAmount = finalForce * loftAngle;
            Vector3 liftImpulse = Vector3.up * liftAmount;
            rb.AddForce(liftImpulse, ForceMode.Impulse);
        }
    }

    private void HitBall(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    
    private bool IsMoving()
    {
        if (rb.IsSleeping()) return false;
        
        if (rb.linearVelocity.sqrMagnitude < STOPPING_THRESHOLD_SQRD)
        {
            return false;
        }
        
        return true;
    }

    public void CaptureInCup(Vector3 cupPosition, float settleDuration = 0.35f)
    {
        if (captureRoutine != null)
        {
            StopCoroutine(captureRoutine);
        }

        captureRoutine = StartCoroutine(CaptureRoutine(cupPosition, settleDuration));
    }

    private IEnumerator CaptureRoutine(Vector3 cupPosition, float duration)
    {
        hasBeenHit = false;
        timeSinceHit = 0f;
        storedPower = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPosition, cupPosition, t);
            yield return null;
        }

        transform.position = cupPosition;
        rb.isKinematic = false;
        rb.Sleep();

        if (directionPointer != null) directionPointer.SetActive(false);
        if (aimIndicator != null) aimIndicator.SetActive(false);

        captureRoutine = null;
    }
}