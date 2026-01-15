using UnityEngine;

/// <summary>
/// Zona de viento que afecta la trayectoria de la pelota de golf.
/// Crea corrientes de aire que empujan la pelota en una dirección específica.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject vacío y nómbralo "WindZone"
/// 2. Adjunta este script
/// 3. Añade un BoxCollider y márcalo como "Is Trigger"
/// 4. Ajusta el tamaño del collider para definir el área del viento
/// 5. Configura la dirección y fuerza del viento en el Inspector
/// 6. (Opcional) Añade partículas para visualizar el viento
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WindZone : MonoBehaviour
{
    [Header("Configuración del Viento")]
    [Tooltip("Dirección del viento (se normalizará automáticamente)")]
    public Vector3 windDirection = Vector3.right;
    
    [Tooltip("Fuerza base del viento")]
    public float windForce = 5f;
    
    [Tooltip("¿El viento cambia de intensidad aleatoriamente?")]
    public bool turbulence = true;
    
    [Tooltip("Variación aleatoria de la fuerza (0-1)")]
    [Range(0f, 1f)]
    public float turbulenceAmount = 0.3f;
    
    [Tooltip("Velocidad de cambio de turbulencia")]
    public float turbulenceSpeed = 2f;

    [Header("Efectos Visuales")]
    [Tooltip("Sistema de partículas para visualizar el viento (opcional)")]
    public ParticleSystem windParticles;
    
    [Tooltip("Color de los gizmos en el editor")]
    public Color gizmoColor = new Color(0.5f, 0.8f, 1f, 0.3f);

    [Header("Configuración de Sonido")]
    [Tooltip("Sonido del viento (opcional)")]
    public AudioClip windSound;
    
    [Tooltip("Volumen del sonido")]
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    private AudioSource audioSource;
    private float currentTurbulence = 0f;
    private BoxCollider windCollider;

    void Start()
    {
        // Normalizar la dirección del viento
        windDirection = windDirection.normalized;

        // Configurar el collider
        windCollider = GetComponent<BoxCollider>();
        if (windCollider != null)
        {
            windCollider.isTrigger = true;
        }

        // Configurar el sonido
        if (windSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = windSound;
            audioSource.loop = true;
            audioSource.volume = soundVolume;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.Play();
        }

        // Configurar partículas si existen
        if (windParticles != null)
        {
            var main = windParticles.main;
            main.startSpeed = windForce;
        }
    }

    void Update()
    {
        // Actualizar la turbulencia
        if (turbulence)
        {
            currentTurbulence = Mathf.PerlinNoise(Time.time * turbulenceSpeed, 0f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Solo afectar a objetos con Rigidbody (como la pelota de golf)
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcular la fuerza del viento con turbulencia
            float currentForce = windForce;
            if (turbulence)
            {
                float variation = currentTurbulence * turbulenceAmount * windForce;
                currentForce = windForce + variation;
            }

            // Aplicar la fuerza del viento
            Vector3 windPush = windDirection * currentForce * Time.deltaTime;
            rb.AddForce(windPush, ForceMode.Force);

            Debug.Log($"Viento afectando a {other.name} con fuerza {currentForce:F2}");
        }
    }

    void OnDrawGizmos()
    {
        // Dibujar el área del viento
        Gizmos.color = gizmoColor;
        
        BoxCollider col = GetComponent<BoxCollider>();
        if (col != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(col.center, col.size);
        }

        // Dibujar flechas indicando la dirección del viento
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position;
        Vector3 direction = transform.TransformDirection(windDirection);
        
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3 offset = new Vector3(i * 2f, j * 2f, 0f);
                Vector3 start = center + offset;
                Vector3 end = start + direction * 3f;
                
                Gizmos.DrawLine(start, end);
                
                // Dibujar punta de flecha
                Vector3 arrowTip = end;
                Vector3 arrowBase = end - direction * 0.5f;
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * 0.3f;
                
                Gizmos.DrawLine(arrowTip, arrowBase + perpendicular);
                Gizmos.DrawLine(arrowTip, arrowBase - perpendicular);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Mostrar información adicional cuando está seleccionado
        Gizmos.color = Color.yellow;
        Vector3 labelPos = transform.position + Vector3.up * 3f;
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, $"Viento: {windForce:F1} unidades\nDirección: {windDirection}");
        #endif
    }
}
