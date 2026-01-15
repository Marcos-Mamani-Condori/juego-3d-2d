using UnityEngine;

/// <summary>
/// Péndulo oscilante que puede golpear la pelota o bloquear el camino.
/// Se balancea de lado a lado como un péndulo real.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject vacío como pivote superior y nómbralo "PendulumPivot"
/// 2. Crea un hijo Cylinder largo que será el brazo del péndulo
/// 3. Adjunta este script al GameObject pivote
/// 4. Asigna el brazo en el Inspector
/// 5. Ajusta el ángulo máximo y la velocidad
/// </summary>
public class Pendulum : MonoBehaviour
{
    [Header("Configuración del Péndulo")]
    [Tooltip("Objeto que se balanceará (el brazo del péndulo)")]
    public Transform pendulumArm;
    
    [Tooltip("Ángulo máximo de balanceo (grados)")]
    public float maxAngle = 45f;
    
    [Tooltip("Velocidad del balanceo")]
    public float swingSpeed = 1f;
    
    [Tooltip("Eje de rotación")]
    public PendulumAxis axis = PendulumAxis.Z;
    
    [Tooltip("¿Comenzar desde el ángulo máximo?")]
    public bool startAtMaxAngle = true;

    [Header("Física")]
    [Tooltip("¿El péndulo puede golpear objetos?")]
    public bool canHitObjects = true;
    
    [Tooltip("Fuerza de golpe al contacto")]
    public float hitForce = 10f;
    
    [Tooltip("¿Usar física realista (gravedad)?")]
    public bool useRealisticPhysics = false;

    [Header("Efectos")]
    [Tooltip("Sonido al pasar por el centro")]
    public AudioClip swingSound;
    
    [Tooltip("Sonido al golpear algo")]
    public AudioClip hitSound;

    public enum PendulumAxis
    {
        X,
        Y,
        Z
    }

    private float currentAngle = 0f;
    private float angularVelocity = 0f;
    private AudioSource audioSource;
    private bool wasPositive = false;

    void Start()
    {
        if (pendulumArm == null)
        {
            Debug.LogError("¡Falta asignar el brazo del péndulo!");
            enabled = false;
            return;
        }

        if (swingSound != null || hitSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (startAtMaxAngle)
        {
            currentAngle = maxAngle;
        }
    }

    void Update()
    {
        if (useRealisticPhysics)
        {
            UpdateRealisticPendulum();
        }
        else
        {
            UpdateSimplePendulum();
        }

        ApplyRotation();
    }

    void UpdateSimplePendulum()
    {
        // Balanceo simple usando seno
        currentAngle = Mathf.Sin(Time.time * swingSpeed) * maxAngle;
        
        // Detectar cuando pasa por el centro
        bool isPositive = currentAngle >= 0;
        if (isPositive != wasPositive)
        {
            if (audioSource != null && swingSound != null)
            {
                audioSource.PlayOneShot(swingSound, 0.5f);
            }
        }
        wasPositive = isPositive;
    }

    void UpdateRealisticPendulum()
    {
        // Física de péndulo más realista
        float gravity = 9.81f;
        float length = pendulumArm != null ? pendulumArm.localScale.y : 1f;
        
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        float acceleration = (-gravity / length) * Mathf.Sin(angleInRadians);
        
        angularVelocity += acceleration * Time.deltaTime * swingSpeed;
        currentAngle += angularVelocity * Time.deltaTime * Mathf.Rad2Deg;
        
        // Aplicar fricción/damping
        angularVelocity *= 0.999f;
        
        // Limitar el ángulo
        if (Mathf.Abs(currentAngle) > maxAngle)
        {
            currentAngle = Mathf.Sign(currentAngle) * maxAngle;
            angularVelocity *= -0.8f; // Rebote con pérdida de energía
        }
    }

    void ApplyRotation()
    {
        if (pendulumArm == null) return;

        Vector3 rotationAxis = Vector3.zero;
        switch (axis)
        {
            case PendulumAxis.X:
                rotationAxis = Vector3.right;
                break;
            case PendulumAxis.Y:
                rotationAxis = Vector3.up;
                break;
            case PendulumAxis.Z:
                rotationAxis = Vector3.forward;
                break;
        }

        pendulumArm.localRotation = Quaternion.AngleAxis(currentAngle, rotationAxis);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canHitObjects) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null && pendulumArm != null)
        {
            // Calcular dirección del golpe
            Vector3 hitDirection = collision.transform.position - pendulumArm.position;
            hitDirection.y = 0; // Mantener golpe horizontal
            hitDirection.Normalize();

            // Aplicar fuerza basada en la velocidad angular del péndulo
            float velocityFactor = Mathf.Abs(angularVelocity);
            if (!useRealisticPhysics)
            {
                velocityFactor = Mathf.Abs(Mathf.Cos(Time.time * swingSpeed));
            }

            Vector3 force = hitDirection * hitForce * (1f + velocityFactor);
            rb.AddForce(force, ForceMode.Impulse);

            Debug.Log($"Péndulo golpeó a {collision.gameObject.name} con fuerza {force.magnitude:F2}");

            // Reproducir sonido de golpe
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (pendulumArm == null) return;

        // Dibujar el punto de pivote
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        // Dibujar el rango de movimiento
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        
        Vector3 rotationAxis = Vector3.zero;
        switch (axis)
        {
            case PendulumAxis.X:
                rotationAxis = Vector3.right;
                break;
            case PendulumAxis.Y:
                rotationAxis = Vector3.up;
                break;
            case PendulumAxis.Z:
                rotationAxis = Vector3.forward;
                break;
        }

        // Dibujar posiciones extremas
        float armLength = pendulumArm.localScale.y;
        
        Quaternion maxRotation = Quaternion.AngleAxis(maxAngle, rotationAxis);
        Quaternion minRotation = Quaternion.AngleAxis(-maxAngle, rotationAxis);
        
        Vector3 down = -Vector3.up;
        Vector3 maxPos = transform.position + maxRotation * (down * armLength);
        Vector3 minPos = transform.position + minRotation * (down * armLength);
        
        Gizmos.DrawLine(transform.position, maxPos);
        Gizmos.DrawLine(transform.position, minPos);
        Gizmos.DrawWireSphere(maxPos, 0.3f);
        Gizmos.DrawWireSphere(minPos, 0.3f);
    }
}
