using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cinta transportadora que mueve objetos en una dirección.
/// Similar a las cintas en fábricas o niveles de plataformas.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Cube largo y plano
/// 2. Adjunta este script
/// 3. Configura la velocidad y dirección
/// 4. (Opcional) Añade TextureTiling animado para efecto visual
/// </summary>
public class ConveyorBelt : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de la cinta (unidades por segundo)")]
    public float speed = 3f;
    
    [Tooltip("Dirección del movimiento (se normalizará)")]
    public Vector3 moveDirection = Vector3.forward;
    
    [Tooltip("¿Usar la dirección forward del objeto?")]
    public bool useObjectForward = true;
    
    [Tooltip("¿La cinta se puede invertir?")]
    public bool canReverse = false;
    
    [Tooltip("Tecla para invertir dirección")]
    public KeyCode reverseKey = KeyCode.R;

    [Header("Física")]
    [Tooltip("Fuerza aplicada a objetos en la cinta")]
    public float force = 10f;
    
    [Tooltip("¿Afectar solo objetos que están quietos?")]
    public bool onlyAffectStationary = false;
    
    [Tooltip("Umbral de velocidad para considerar 'quieto'")]
    public float stationaryThreshold = 0.5f;

    [Header("Efectos Visuales")]
    [Tooltip("¿Animar la textura de la cinta?")]
    public bool animateTexture = true;
    
    [Tooltip("Velocidad de animación de textura")]
    public float textureSpeed = 1f;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido del motor de la cinta (loop)")]
    public AudioClip motorSound;
    
    [Tooltip("Volumen del sonido")]
    [Range(0f, 1f)]
    public float soundVolume = 0.5f;

    private List<Rigidbody> objectsOnBelt = new List<Rigidbody>();
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private int currentDirection = 1; // 1 = normal, -1 = invertido
    private float textureOffset = 0f;

    void Start()
    {
        if (!useObjectForward)
        {
            moveDirection = moveDirection.normalized;
        }

        meshRenderer = GetComponent<MeshRenderer>();
        
        if (motorSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = motorSound;
            audioSource.loop = true;
            audioSource.volume = soundVolume;
            audioSource.Play();
        }
    }

    void Update()
    {
        // Invertir dirección si está habilitado
        if (canReverse && Input.GetKeyDown(reverseKey))
        {
            currentDirection *= -1;
            Debug.Log($"Cinta transportadora invertida. Dirección: {currentDirection}");
        }

        // Animar textura
        if (animateTexture && meshRenderer != null && meshRenderer.material != null)
        {
            textureOffset += speed * textureSpeed * currentDirection * Time.deltaTime;
            meshRenderer.material.mainTextureOffset = new Vector2(textureOffset, 0f);
        }
    }

    void FixedUpdate()
    {
        // Aplicar fuerza a todos los objetos en la cinta
        Vector3 finalDirection = useObjectForward ? transform.forward : moveDirection;
        finalDirection = finalDirection.normalized * currentDirection;

        foreach (Rigidbody rb in objectsOnBelt)
        {
            if (rb == null) continue;

            // Verificar si solo debe afectar objetos quietos
            if (onlyAffectStationary)
            {
                if (rb.linearVelocity.magnitude > stationaryThreshold)
                {
                    continue;
                }
            }

            Vector3 movement = finalDirection * force * Time.fixedDeltaTime;
            rb.AddForce(movement, ForceMode.Force);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null && !objectsOnBelt.Contains(rb))
        {
            objectsOnBelt.Add(rb);
            Debug.Log($"{collision.gameObject.name} entró en la cinta transportadora");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null && objectsOnBelt.Contains(rb))
        {
            objectsOnBelt.Remove(rb);
            Debug.Log($"{collision.gameObject.name} salió de la cinta transportadora");
        }
    }

    void OnDrawGizmos()
    {
        Vector3 direction = useObjectForward ? transform.forward : moveDirection.normalized;
        direction *= currentDirection;
        
        // Dibujar la cinta
        Gizmos.color = new Color(0.8f, 0.6f, 0.2f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        // Dibujar flechas de dirección
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        
        Vector3 center = transform.position;
        int arrowCount = 5;
        float spacing = transform.localScale.z / arrowCount;
        
        for (int i = 0; i < arrowCount; i++)
        {
            Vector3 arrowPos = center + direction * (i - arrowCount/2f) * spacing;
            Vector3 arrowEnd = arrowPos + direction * spacing * 0.8f;
            
            Gizmos.DrawLine(arrowPos, arrowEnd);
            
            // Punta de flecha
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized * 0.3f;
            Gizmos.DrawLine(arrowEnd, arrowEnd - direction * 0.3f + perpendicular);
            Gizmos.DrawLine(arrowEnd, arrowEnd - direction * 0.3f - perpendicular);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Información adicional cuando está seleccionado
        #if UNITY_EDITOR
        Vector3 labelPos = transform.position + Vector3.up * 2f;
        UnityEditor.Handles.Label(labelPos, $"Velocidad: {speed} u/s\nDirección: {(currentDirection > 0 ? "→" : "←")}");
        #endif
    }
}
