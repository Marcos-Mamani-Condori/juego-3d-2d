using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Script para el hoyo de golf (meta del nivel).
/// Detecta cuando la pelota entra y completa el nivel.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GolfHole : MonoBehaviour
{
    [Header("Configuración del Hoyo")]
    [Tooltip("Radio del hoyo (para detección)")]
    public float holeRadius = 0.5f;
    
    [Tooltip("¿Qué pasa al completar?")]
    public WinAction winAction = WinAction.ShowVictoryScreen;
    
    [Tooltip("Nombre de la siguiente escena (si es LoadNextLevel o ShowVictoryScreen)")]
    public string nextSceneName = "VictoryScreen";
    
    [Tooltip("Tiempo de espera antes de la acción (segundos)")]
    public float delayBeforeAction = 1.5f;

    [Header("Efectos Visuales")]
    [Tooltip("Color del hoyo")]
    public Color holeColor = Color.black;
    
    [Tooltip("Partículas cuando la pelota entra")]
    public ParticleSystem winParticles;

    [Header("Efectos de Sonido")]
    [Tooltip("Sonido cuando la pelota entra al hoyo")]
    public AudioClip holeInSound;
    
    [Tooltip("Música de victoria (opcional)")]
    public AudioClip victoryMusic;

    [Header("UI")]
    [Tooltip("Canvas de victoria (opcional - se crea automáticamente si es null)")]
    public GameObject victoryCanvas;
    
    [Tooltip("Texto del mensaje de victoria")]
    public string victoryText = "¡FELICIDADES!\n¡HAS COMPLETADO EL JUEGO!";

    public enum WinAction
    {
        LoadNextLevel,    // Cargar siguiente nivel
        RestartLevel,     // Reiniciar nivel actual
        ShowVictoryScreen,// Mostrar pantalla de victoria
        Nothing           // Solo efectos (para testing)
    }

    private bool ballInHole = false;
    private AudioSource audioSource;
    private GameObject createdVictoryCanvas;

    void Start()
    {
        // Configurar collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Configurar material del hoyo
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = holeColor;
        }

        // Configurar audio
        if (holeInSound != null || victoryMusic != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si es la pelota
        if (other.CompareTag("Pelota") && !ballInHole)
        {
            // Verificar que la pelota esté casi detenida (velocidad baja)
            Rigidbody ballRb = other.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Solo cuenta si la pelota va despacio
                if (ballRb.linearVelocity.magnitude < 5f)
                {
                    BallEnteredHole(other.gameObject);
                }
            }
        }
    }

    void BallEnteredHole(GameObject ball)
    {
        ballInHole = true;

        Debug.Log("¡HOYO! La pelota entró en el hoyo.");

        // Detener la pelota
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.isKinematic = true;
        }

        // Animar la pelota cayendo en el hoyo
        StartCoroutine(BallFallAnimation(ball));

        // Reproducir sonido
        if (audioSource != null && holeInSound != null)
        {
            audioSource.PlayOneShot(holeInSound);
        }

        // Efectos de partículas
        if (winParticles != null)
        {
            winParticles.transform.position = transform.position;
            winParticles.Play();
        }

        // Mostrar mensaje de victoria
        ShowVictoryUI();

        // Ejecutar acción después del delay
        Invoke(nameof(ExecuteWinAction), delayBeforeAction);
    }

    System.Collections.IEnumerator BallFallAnimation(GameObject ball)
    {
        Vector3 startPos = ball.transform.position;
        Vector3 endPos = transform.position - new Vector3(0f, 0.5f, 0f); // Caer dentro del hoyo

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Interpolar posición con curva suave
            ball.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Reducir escala gradualmente
            float scale = Mathf.Lerp(1f, 0.5f, t);
            ball.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        // Ocultar la pelota completamente
        ball.SetActive(false);
    }

    void ShowVictoryUI()
    {
        // Pausar el juego
        Time.timeScale = 0f;

        // Si ya hay un canvas asignado, usarlo
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
            return;
        }

        // Crear Canvas automáticamente
        createdVictoryCanvas = new GameObject("VictoryCanvas");
        Canvas canvas = createdVictoryCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = createdVictoryCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        createdVictoryCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Panel de fondo oscuro
        GameObject panel = new GameObject("Background");
        panel.transform.SetParent(createdVictoryCanvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // Texto de victoria
        GameObject textObj = new GameObject("VictoryText");
        textObj.transform.SetParent(panel.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(1000, 300);
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = victoryText;
        tmp.fontSize = 72;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.yellow;
        tmp.fontStyle = FontStyles.Bold;

        Debug.Log("✅ UI de victoria creado y mostrado");
    }

    void ExecuteWinAction()
    {
        // Reproducir música de victoria
        if (audioSource != null && victoryMusic != null)
        {
            audioSource.PlayOneShot(victoryMusic);
        }

        // Reanudar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        switch (winAction)
        {
            case WinAction.LoadNextLevel:
                LoadNextLevel();
                break;

            case WinAction.RestartLevel:
                RestartCurrentLevel();
                break;

            case WinAction.ShowVictoryScreen:
                ShowVictoryScreen();
                break;

            case WinAction.Nothing:
                Debug.Log("Victoria detectada - Sin acción configurada");
                break;
        }
    }

    void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Cargando siguiente nivel: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No se especificó el nombre de la siguiente escena.");
        }
    }

    void RestartCurrentLevel()
    {
        Debug.Log("Reiniciando nivel...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ShowVictoryScreen()
    {
        Debug.Log("Cargando pantalla de victoria...");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No se especificó escena de victoria. Usa 'Next Scene Name' en el Inspector.");
        }
    }



    void OnDrawGizmos()
    {
        // Dibujar el radio del hoyo como esfera de alambre
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, holeRadius);

        // Dibujar círculo en el suelo para marcar el área
        Gizmos.color = Color.green;
        DrawCircle(transform.position, holeRadius, 32);

        // Dibujar flecha hacia arriba (indicar posición del hoyo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
        
        // Dibujar punta de flecha
        Vector3 arrowTop = transform.position + Vector3.up * 2f;
        Gizmos.DrawLine(arrowTop, arrowTop + new Vector3(0.2f, -0.3f, 0f));
        Gizmos.DrawLine(arrowTop, arrowTop + new Vector3(-0.2f, -0.3f, 0f));
    }

    // Helper para dibujar círculo en el suelo
    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }


}
