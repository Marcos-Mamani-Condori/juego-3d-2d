using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 1; 
    [Tooltip("Controlador que gestiona cuándo desactivar el escudo/muro asociado.")]
    public ShieldBarrierController shieldController;
    [Tooltip("Compatibilidad: referencia directa al módulo de escudo si no se usa ShieldBarrierController.")]
    public ShieldModuleScript linkedShield; 

    private bool isDead;

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        health = Mathf.Max(health, 0);
        Debug.Log(gameObject.name + " recibió daño. Vida restante: " + health);

        if (health <= 0)
        {
            isDead = true;

            if (shieldController != null)
            {
                shieldController.NotifyEnemyDestroyed(this);
            }
            else if (linkedShield != null)
            {
                linkedShield.DeactivateShield(); 
            }

            Destroy(gameObject); 
        }
    }
}