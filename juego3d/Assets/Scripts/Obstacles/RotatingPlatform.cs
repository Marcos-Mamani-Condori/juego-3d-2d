using UnityEngine;

/// <summary>
/// Plataforma que rota continuamente alrededor de un eje.
/// Útil para crear obstáculos dinámicos que requieren timing preciso.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject 3D (Cube o Cylinder) y nómbralo "RotatingPlatform"
/// 2. Adjunta este script al GameObject
/// 3. Selecciona el eje de rotación (X, Y, o Z)
/// 4. Ajusta la velocidad de rotación
/// 5. (Opcional) Añade objetos hijos que rotarán con la plataforma
/// </summary>
public class RotatingPlatform : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = 45f;
    
    [Tooltip("Eje de rotación")]
    public RotationAxis axis = RotationAxis.Y;
    
    [Tooltip("¿Rotar en sentido contrario?")]
    public bool reverseDirection = false;
    
    [Tooltip("¿La rotación cambia de velocidad periódicamente?")]
    public bool varySpeed = false;
    
    [Tooltip("Velocidad mínima (si varySpeed está activo)")]
    public float minSpeed = 20f;
    
    [Tooltip("Velocidad máxima (si varySpeed está activo)")]
    public float maxSpeed = 80f;
    
    [Tooltip("Velocidad de cambio de velocidad")]
    public float speedChangeRate = 1f;

    [Header("Opciones de Física")]
    [Tooltip("¿Los objetos se mueven con la plataforma?")]
    public bool moveObjectsWithPlatform = true;

    [Header("Efectos Visuales")]
    [Tooltip("Mostrar dirección de rotación en el editor")]
    public bool showRotationDirection = true;

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    private float currentSpeed;

    void Start()
    {
        currentSpeed = rotationSpeed;
    }

    void Update()
    {
        // Variar la velocidad si está habilitado
        if (varySpeed)
        {
            float t = Mathf.PingPong(Time.time * speedChangeRate, 1f);
            currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
        }
        else
        {
            currentSpeed = rotationSpeed;
        }

        // Aplicar la dirección
        float finalSpeed = reverseDirection ? -currentSpeed : currentSpeed;

        // Rotar según el eje seleccionado
        Vector3 rotationVector = Vector3.zero;
        
        switch (axis)
        {
            case RotationAxis.X:
                rotationVector = Vector3.right;
                break;
            case RotationAxis.Y:
                rotationVector = Vector3.up;
                break;
            case RotationAxis.Z:
                rotationVector = Vector3.forward;
                break;
        }

        transform.Rotate(rotationVector, finalSpeed * Time.deltaTime, Space.World);
    }

    void OnCollisionStay(Collision collision)
    {
        if (moveObjectsWithPlatform)
        {
            // Si un objeto está sobre la plataforma, moverlo con ella
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // La rotación de la plataforma naturalmente moverá el objeto
                // pero podemos añadir un pequeño impulso para mantenerlo adherido
                foreach (ContactPoint contact in collision.contacts)
                {
                    // Si el contacto es desde arriba
                    if (Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
                    {
                        // Aplicar una pequeña fuerza hacia abajo para mantener contacto
                        rb.AddForce(Vector3.down * 2f, ForceMode.Force);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showRotationDirection) return;

        // Dibujar el eje de rotación
        Gizmos.color = Color.green;
        Vector3 axisVector = Vector3.zero;
        
        switch (axis)
        {
            case RotationAxis.X:
                axisVector = transform.right;
                break;
            case RotationAxis.Y:
                axisVector = transform.up;
                break;
            case RotationAxis.Z:
                axisVector = transform.forward;
                break;
        }

        Vector3 center = transform.position;
        float axisLength = 3f;
        
        Gizmos.DrawLine(center - axisVector * axisLength, center + axisVector * axisLength);
        Gizmos.DrawWireSphere(center, 0.2f);

        // Dibujar círculo de rotación
        Gizmos.color = Color.yellow;
        DrawRotationCircle(center, axisVector, 2f, 32);
    }

    void DrawRotationCircle(Vector3 center, Vector3 axis, float radius, int segments)
    {
        Vector3 perpendicular = Vector3.Cross(axis, Vector3.up);
        if (perpendicular.magnitude < 0.1f)
        {
            perpendicular = Vector3.Cross(axis, Vector3.right);
        }
        perpendicular = perpendicular.normalized * radius;

        Vector3 previousPoint = center + perpendicular;
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = (float)i / segments * 360f;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 currentPoint = center + rotation * perpendicular;
            
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        // Dibujar flecha de dirección
        float arrowAngle = reverseDirection ? -45f : 45f;
        Quaternion arrowRotation = Quaternion.AngleAxis(arrowAngle, axis);
        Vector3 arrowPoint = center + arrowRotation * perpendicular;
        
        Gizmos.color = reverseDirection ? Color.red : Color.cyan;
        Gizmos.DrawLine(center, arrowPoint);
        Gizmos.DrawWireSphere(arrowPoint, 0.3f);
    }
}
