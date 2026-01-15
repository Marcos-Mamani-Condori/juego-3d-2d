using UnityEngine;

public class ShieldModuleScript : MonoBehaviour
{
    public GameObject energyShieldVisual; // Arrastra tu modelo 3D del escudo aquí

    void Start()
    {
        // Asegurarse de que el escudo esté visible al inicio
        ActivateShield();
    }

    public void ActivateShield()
    {
        if (energyShieldVisual != null)
        {
            energyShieldVisual.SetActive(true);
        }
        Debug.Log("Escudo Activado.");
    }

    public void DeactivateShield()
    {
        if (energyShieldVisual != null)
        {
            energyShieldVisual.SetActive(false);
        }
        Debug.Log("¡Escudo Desactivado!");
        // Opcional: Destruir el módulo después de desactivar el escudo.
        // Destroy(gameObject); 
    }
}