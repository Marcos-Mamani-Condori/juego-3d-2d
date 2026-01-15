using UnityEngine;
using UnityEngine.UI; // Necesario para mostrar el contador en la pantalla (Opcional)

public class GameManager : MonoBehaviour
{
    // Permite que otros scripts accedan a esta instancia fácilmente
    public static GameManager Instance; 
    
    // Contador de golpes
    public int strokes = 0;
    
    // Asigna aquí el Text component en el Inspector si usas UI
    public Text strokesText; 

    void Awake()
    {
        // Configura la instancia única
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        UpdateStrokeUI(); // Llama esto al inicio para mostrar "0"
    }

    // Llama a esta función desde el script de la pelota cuando el jugador golpea
    public void AddStroke()
    {
        strokes++;
        UpdateStrokeUI();
    }

    private void UpdateStrokeUI()
    {
        if (strokesText != null)
        {
            strokesText.text = "GOLPES: " + strokes.ToString();
        }
    }
}