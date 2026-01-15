using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    private Tilemap destructibleTilemap;
    private Tilemap indestructibleTilemap;
    private GameObject explosionPrefab;
    private Collider2D bombCollider;

    [Header("Bomb Settings")]
    public float delay = 2f;
    public float explosionDuration = 1f;
    public int explosionRange = 1;

    [Header("PowerUp Settings")]
    [Range(0f, 1f)]
    public float powerUpChance = 0.2f;             // Probabilidad de generar power-up
    public GameObject bombRangePowerUpPrefab;      // Prefab que aumenta rango
    public GameObject extraBombPowerUpPrefab;      // Prefab que aumenta número de bombas

    [Header("Audio")]
    public AudioClip explosionSound;               // Asigna el clip en el Inspector
    private AudioSource audioSource;

    private bool readyToExplode = false;
    private int playersOnTop = 0;

    // 🔹 Referencia al jugador que colocó la bomba
    private PlayerController owner;

    public void Initialize(Tilemap destructible, Tilemap indestructible, GameObject explosion)
    {
        destructibleTilemap = destructible;
        indestructibleTilemap = indestructible;
        explosionPrefab = explosion;

        bombCollider = GetComponent<Collider2D>();
        if (bombCollider != null)
            bombCollider.isTrigger = true;

        // AudioSource para sonido de explosión
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        readyToExplode = true;
        Invoke(nameof(Explode), delay);
    }

    // 🔹 Asignar dueño de la bomba
    public void SetOwner(PlayerController player)
    {
        owner = player;
    }

    // 🔹 Detonar la bomba inmediatamente (cadena de explosiones)
    public void Detonate()
    {
        if (!readyToExplode) return;
        CancelInvoke(nameof(Explode));
        Explode();
    }

    private void Explode()
    {
        if (!readyToExplode) return;
        readyToExplode = false;

        // Reproducir sonido de explosión
        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        // Explosión en cuatro direcciones
        SpawnExplosion(Vector2.up);
        SpawnExplosion(Vector2.down);
        SpawnExplosion(Vector2.left);
        SpawnExplosion(Vector2.right);

        // Explosión central
        if (explosionPrefab != null)
        {
            GameObject centerFire = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(centerFire, explosionDuration);
        }

        // Notificar al jugador dueño
        if (owner != null)
        {
            owner.BombExploded();
        }

        Destroy(gameObject, 0.1f); // Pequeño delay para permitir que se reproduzca el audio
    }

    private void SpawnExplosion(Vector2 direction)
    {
        if (destructibleTilemap == null) return;

        for (int i = 1; i <= explosionRange; i++)
        {
            Vector3Int tilePos = destructibleTilemap.WorldToCell(transform.position + (Vector3)(direction * i));

            // Bloque indestructible
            if (indestructibleTilemap != null && indestructibleTilemap.HasTile(tilePos))
                break;

            // Bloque destructible
            if (destructibleTilemap.HasTile(tilePos))
            {
                destructibleTilemap.SetTile(tilePos, null);

                // Generar power-up con probabilidad
                if (Random.value <= powerUpChance)
                {
                    Vector3 spawnPos = destructibleTilemap.CellToWorld(tilePos) + destructibleTilemap.cellSize / 2f;
                    float choice = Random.value;

                    if (choice < 0.5f && bombRangePowerUpPrefab != null)
                        Instantiate(bombRangePowerUpPrefab, spawnPos, Quaternion.identity);
                    else if (choice >= 0.5f && extraBombPowerUpPrefab != null)
                        Instantiate(extraBombPowerUpPrefab, spawnPos, Quaternion.identity);
                }

                break; // Detener explosión al destruir
            }

            // Efecto de fuego
            if (explosionPrefab != null)
            {
                Vector3 spawnPos = destructibleTilemap.CellToWorld(tilePos) + destructibleTilemap.cellSize / 2f;
                GameObject fire = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
                Destroy(fire, explosionDuration);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnTop++;
        }
        else if (other.CompareTag("Explosion"))
        {
            Detonate();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersOnTop--;
            if (playersOnTop <= 0 && bombCollider != null)
                bombCollider.isTrigger = false;
        }
    }
}
