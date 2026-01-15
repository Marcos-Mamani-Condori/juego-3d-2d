using UnityEngine;

public class SpawnerMoneda : MonoBehaviour
{
    [Header("Moneda")]
    public GameObject monedaPrefab;      // Prefab de moneda
    public int cantidadMonedas = 50;     // Cantidad total de monedas
    public int valorMoneda = 1;          // Valor de cada moneda

    [Header("Mapa")]
    public float mapMinX = -15f;
    public float mapMaxX = 24f;
    public float mapMinY = -6f;
    public float mapMaxY = 9f;

    [Header("Grid & Spawn Settings")]
    public float gridSize = 1f;          // Tamaño de cada celda (1 unidad)
    public float checkRadius = 0.3f;     // Radio para comprobar colisiones

    void Start()
    {
        GenerarMonedas();
    }

    void GenerarMonedas()
    {
        int generadas = 0;
        int intentos = 0;
        int maxIntentos = 1000;

        // Número de celdas disponibles en X y Y
        int celdasX = Mathf.FloorToInt((mapMaxX - mapMinX) / gridSize);
        int celdasY = Mathf.FloorToInt((mapMaxY - mapMinY) / gridSize);

        while (generadas < cantidadMonedas && intentos < maxIntentos)
        {
            intentos++;

            // Elegir celda aleatoria
            int cellX = Random.Range(0, celdasX);
            int cellY = Random.Range(0, celdasY);

            // Convertir a posición en unidades (centro de la celda)
            float x = mapMinX + cellX * gridSize + gridSize / 2f;
            float y = mapMinY + cellY * gridSize + gridSize / 2f;

            Vector2 spawnPos = new Vector2(x, y);

            // Evitar colisiones con cualquier collider (navbar, paredes, objetos)
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, checkRadius);
            if (hit == null)
            {
                GameObject nuevaMoneda = Instantiate(monedaPrefab, spawnPos, Quaternion.identity);

                // Asignar valor
                Moneda monedaScript = nuevaMoneda.GetComponent<Moneda>();
                if (monedaScript != null)
                    monedaScript.valor = valorMoneda;

                generadas++;
            }
        }

        if (generadas < cantidadMonedas)
            Debug.LogWarning("No se pudieron generar todas las monedas, revisa mapSize, gridSize o checkRadius");
    }
}
