using UnityEngine;

/// <summary>
/// Pared que se mueve entre dos puntos. 
/// Puede bloquear el camino de la pelota de golf o del jugador en modo shooter.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject 3D (Cube) en Unity y nómbralo "MovingWall"
/// 2. Ajusta su escala (ej: X=5, Y=3, Z=0.5)
/// 3. Adjunta este script al GameObject
/// 4. Configura los puntos A y B en el Inspector (posiciones relativas)
/// 5. Ajusta la velocidad y el tiempo de pausa
/// </summary>
public class MovingWall : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Primera posición (relativa a la posición inicial del objeto)")]
    public Vector3 pointA = Vector3.zero;
    
    [Tooltip("Segunda posición (relativa a la posición inicial del objeto)")]
    public Vector3 pointB = new Vector3(10f, 0f, 0f);
    
    [Tooltip("Velocidad de movimiento en unidades por segundo")]
    public float moveSpeed = 3f;
    
    [Tooltip("Tiempo de pausa en cada extremo (en segundos)")]
    public float pauseTime = 1f;
    
    [Header("Opciones Visuales")]
    [Tooltip("Mostrar gizmos en el editor para visualizar el recorrido")]
    public bool showGizmos = true;
    
    [Tooltip("Color del gizmo")]
    public Color gizmoColor = Color.yellow;

    [Header("Opciones de Interacción")]
    [Tooltip("¿La pared puede empujar la pelota de golf?")]
    public bool canPushBall = true;
    
    [Tooltip("¿La pared puede dañar al jugador en modo shooter?")]
    public bool canDamagePlayer = false;
    
    [Tooltip("Daño por contacto (si canDamagePlayer está activado)")]
    public int damageAmount = 10;

    // Variables privadas
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingToB = true;
    private float pauseTimer = 0f;
    private bool isPaused = false;

    void Start()
    {
        // Guardar la posición inicial
        startPosition = transform.position;
        
        // La primera meta es ir hacia el punto B
        targetPosition = startPosition + pointB;
    }

    void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                // Cambiar la dirección
                if (movingToB)
                {
                    targetPosition = startPosition + pointA;
                }
                else
                {
                    targetPosition = startPosition + pointB;
                }
                movingToB = !movingToB;
            }
            return;
        }

        // Mover hacia el objetivo
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Verificar si llegamos al objetivo
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isPaused = true;
            pauseTimer = pauseTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si la pelota choca con la pared en movimiento
        if (canPushBall && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar un pequeño impulso para evitar que la pared "aplaste" la pelota
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                rb.AddForce(pushDirection * 2f, ForceMode.Impulse);
            }
        }

        // Daño al jugador (si está implementado en tu juego)
        if (canDamagePlayer)
        {
            // Puedes expandir esto si tienes un sistema de vida para el robot
            Debug.Log($"¡Pared móvil golpeó a {collision.gameObject.name}!");
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 posA = start + pointA;
        Vector3 posB = start + pointB;

        // Dibujar las posiciones
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(posA, 0.3f);
        Gizmos.DrawWireSphere(posB, 0.3f);
        
        // Dibujar la línea de recorrido
        Gizmos.DrawLine(posA, posB);
        
        // Dibujar la posición actual
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
