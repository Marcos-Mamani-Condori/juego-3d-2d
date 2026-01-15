using UnityEngine;

/// <summary>
/// Plataforma deslizante resbaladiza (hielo).
/// Reduce la fricción haciendo que la pelota se deslice.
/// 
/// INSTRUCCIONES DE USO:
/// 1. Crea un GameObject Cube/Plane
/// 2. Adjunta este script
/// 3. Crea un Physic Material con baja fricción
/// 4. Asígnalo en el script
/// </summary>
public class IcePlatform : MonoBehaviour
{
    [Header("Configuración de Deslizamiento")]
    [Tooltip("Multiplica la velocidad horizontal al estar sobre hielo")]
    public float slipperiness = 1.2f;
    
    [Tooltip("Fricción cuando la pelota está sobre el hielo (0-1)")]
    [Range(0f, 1f)]
    public float iceFriction = 0.05f;

    [Header("Material Físico")]
    [Tooltip("Physic Material del hielo (créalo en: Assets > Create > Physic Material)")]
    public PhysicsMaterial iceMaterial;

    [Header("Efectos Visuales")]
    [Tooltip("Color del hielo")]
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.8f);
    
    [Tooltip("Brillo del hielo")]
    [Range(0f, 1f)]
    public float iceGloss = 0.9f;

    private MeshRenderer meshRenderer;
    private Collider platformCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        platformCollider = GetComponent<Collider>();

        // Aplicar material visual
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = iceColor;
            meshRenderer.material.SetFloat("_Glossiness", iceGloss);
        }

        // Crear o aplicar material físico
        if (iceMaterial == null)
        {
            // Crear material de física si no existe
            iceMaterial = new PhysicsMaterial("Ice");
            iceMaterial.dynamicFriction = iceFriction;
            iceMaterial.staticFriction = iceFriction;
            iceMaterial.bounciness = 0.1f;
            iceMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            iceMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }

        if (platformCollider != null)
        {
            platformCollider.material = iceMaterial;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Aplicar fuerza adicional en la dirección del movimiento para simular deslizamiento
            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0f;
            
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Vector3 slideForce = horizontalVelocity.normalized * slipperiness * Time.deltaTime;
                rb.AddForce(slideForce, ForceMode.VelocityChange);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(iceColor.r, iceColor.g, iceColor.b, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
