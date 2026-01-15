using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float velocidad = 1.5f;             
    public float tiempoCambioDireccion = 2f;   
    private Vector2 direccion;
    private Rigidbody2D rb;

    private float tiempoSiguienteCambio = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CambiarDireccionAleatoria();
        tiempoSiguienteCambio = Time.time + tiempoCambioDireccion;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direccion * velocidad;

        if (Time.time >= tiempoSiguienteCambio)
        {
            CambiarDireccionAleatoria();
            tiempoSiguienteCambio = Time.time + tiempoCambioDireccion;
        }
    }

    private void CambiarDireccionAleatoria()
    {
        Vector2[] posiblesDirecciones = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 nuevaDireccion;

        do
        {
            nuevaDireccion = posiblesDirecciones[Random.Range(0, posiblesDirecciones.Length)];
        } while (nuevaDireccion == direccion);

        direccion = nuevaDireccion;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            CambiarDireccionAleatoria();
        }
    }
}
