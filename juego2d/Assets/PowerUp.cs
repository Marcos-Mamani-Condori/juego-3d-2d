using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { BombRange, ExtraBomb }
    public PowerUpType type;
    public int value = 1; // Incremento

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                switch (type)
                {
                    case PowerUpType.BombRange:
                        player.IncreaseBombRange(value);
                        break;
                    case PowerUpType.ExtraBomb:
                        player.IncreaseMaxBombs(value);
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
