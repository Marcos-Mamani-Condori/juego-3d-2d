using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float velocity = 5f;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 lastDirection = Vector2.down;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int score = 0;
    public TextMeshProUGUI scoreText;
    [Header("Audio")]
    public AudioClip coinSound;    
    private AudioSource audioSource;

    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public GameObject explosionPrefab;
    public Tilemap mapTilemap;
    public Tilemap destructibleTilemap;
    public Tilemap indestructibleTilemap;
    public int bombRange = 1; 

    [Header("Bomb Count")]
    public int maxBombs = 1;      
    private int currentBombs = 0; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        UpdateScoreText();
        audioSource = GetComponent<AudioSource>();
if (audioSource == null)
{
    audioSource = gameObject.AddComponent<AudioSource>();
}
audioSource.playOnAwake = false;
    }

    void Update()
    {
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 inputDirection = new Vector2(moveX, moveY);

        if (inputDirection.sqrMagnitude > 0)
        {
            lastDirection = inputDirection;
            direction = inputDirection.normalized;
        }
        else
        {
            direction = Vector2.zero;
        }

        bool isWalking = direction.sqrMagnitude > 0;
        if (animator != null) animator.SetBool("isWalking", isWalking);

        Vector2 animDirection = isWalking ? direction : lastDirection;
        if (animator != null)
        {
            animator.SetFloat("MoveX", animDirection.x);
            animator.SetFloat("MoveY", animDirection.y);
        }

        // Colocar bomba solo si no se excede el maximo
        if (Input.GetKeyDown(KeyCode.Space) && bombPrefab != null && mapTilemap != null && currentBombs < maxBombs)
        {
            Vector3Int cellPos = mapTilemap.WorldToCell(transform.position);
            Vector3 spawnPos = mapTilemap.CellToWorld(cellPos) + mapTilemap.cellSize / 2f;

            GameObject bomba = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
            Bomb bombScript = bomba.GetComponent<Bomb>();
            if (bombScript != null)
            {
                bombScript.explosionRange = bombRange;
                bombScript.Initialize(destructibleTilemap, indestructibleTilemap, explosionPrefab);
                bombScript.SetOwner(this); // importante: asigna dueño
            }

            currentBombs++;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        Vector2 newPos = rb.position + direction * velocity * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Monedas
        if (other.CompareTag("Moneda"))
        {
            Moneda moneda = other.GetComponent<Moneda>();
            if (moneda != null)
            {
                score += moneda.valor;
                UpdateScoreText();
                
            }
       
    if (audioSource != null && coinSound != null)
        audioSource.PlayOneShot(coinSound);

            Destroy(other.gameObject);
        }
    }

    public void IncreaseBombRange(int amount)
    {
        bombRange += amount;
    }

    public void IncreaseMaxBombs(int amount)
    {
        maxBombs += amount;
    }

    public void BombExploded()
    {
        currentBombs--;
        if (currentBombs < 0) currentBombs = 0;
    }

    public int GetScore() => score;

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = " => " + score;
    }
}
