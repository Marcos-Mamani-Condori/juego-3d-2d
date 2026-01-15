using UnityEngine;

public class EnemyShipTarget : MonoBehaviour
{
    [Tooltip("Efecto opcional al destruir la nave.")]
    public GameObject destructionEffect;

    public void DestroyShip()
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
