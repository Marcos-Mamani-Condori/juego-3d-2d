using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de UI para el desafío de memoria de patrones.
/// Muestra el patrón, recibe input del jugador, y da feedback visual.
/// </summary>
public class PatternChallengeUI : MonoBehaviour
{
    [Header("Referencias del Canvas")]
    [Tooltip("Panel principal del desafío")]
    public GameObject mainPanel;

    [Tooltip("Texto del título (ej: 'MEMORIZA EL PATRÓN')")]
    public TextMeshProUGUI titleText;

    [Tooltip("Texto del contador de rondas (ej: 'RONDA 2/4')")]
    public TextMeshProUGUI roundCounterText;

    [Header("Display de Patrones")]
    [Tooltip("Contenedor de los números del patrón")]
    public Transform patternContainer;

    [Tooltip("Prefab de número individual (TextMeshProUGUI)")]
    public GameObject numberPrefab;

    [Header("Mensajes")]
    [Tooltip("Texto de mensaje de éxito")]
    public TextMeshProUGUI successText;

    [Tooltip("Texto de mensaje de fallo")]
    public TextMeshProUGUI failureText;

    [Tooltip("Texto de ronda completada")]
    public TextMeshProUGUI roundCompleteText;

    [Header("Configuración Visual")]
    [Tooltip("Color para números correctos")]
    public Color correctColor = Color.green;

    [Tooltip("Color para números incorrectos")]
    public Color incorrectColor = Color.red;

    [Tooltip("Color normal de números")]
    public Color normalColor = Color.white;

    [Tooltip("Tamaño de fuente para números")]
    public float numberFontSize = 80f;

    [Tooltip("Espaciado entre números")]
    public float numberSpacing = 100f;

    private List<TextMeshProUGUI> currentNumberDisplays = new List<TextMeshProUGUI>();
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = mainPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = mainPanel.AddComponent<CanvasGroup>();
        }

        HideUI();
    }

    /// <summary>
    /// Muestra el UI del desafío
    /// </summary>
    public void ShowUI()
    {
        mainPanel.SetActive(true);
        canvasGroup.alpha = 1f;
        
        // Ocultar todos los mensajes
        if (successText != null) successText.gameObject.SetActive(false);
        if (failureText != null) failureText.gameObject.SetActive(false);
        if (roundCompleteText != null) roundCompleteText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Oculta el UI del desafío
    /// </summary>
    public void HideUI()
    {
        mainPanel.SetActive(false);
        ClearNumberDisplays();
    }

    /// <summary>
    /// Muestra el patrón en pantalla
    /// </summary>
    public void ShowPattern(List<int> pattern, int currentRound, int totalRounds)
    {
        // Actualizar título y contador
        if (titleText != null)
        {
            titleText.text = "MEMORIZA EL PATRÓN";
        }

        if (roundCounterText != null)
        {
            roundCounterText.text = $"RONDA {currentRound}/{totalRounds}";
        }

        // Limpiar displays anteriores
        ClearNumberDisplays();

        // Crear displays de números
        for (int i = 0; i < pattern.Count; i++)
        {
            GameObject numberObj = Instantiate(numberPrefab, patternContainer);
            TextMeshProUGUI numberText = numberObj.GetComponent<TextMeshProUGUI>();
            
            if (numberText != null)
            {
                numberText.text = pattern[i].ToString();
                numberText.fontSize = numberFontSize;
                numberText.color = normalColor;
                currentNumberDisplays.Add(numberText);
                
                // Mostrar inmediatamente sin animación (para evitar problemas con Time.timeScale = 0)
                numberText.transform.localScale = Vector3.one;
            }

            // Animación desactivada para evitar conflictos con Time.timeScale = 0
            // StartCoroutine(AnimateNumberAppear(numberText, i * 0.2f));
        }
    }

    /// <summary>
    /// Oculta el patrón (fade out)
    /// </summary>
    public void HidePattern()
    {
        // Limpiar inmediatamente sin animación (para evitar problemas con Time.timeScale = 0)
        ClearNumberDisplays();
    }

    /// <summary>
    /// Muestra la fase de input
    /// </summary>
    public void ShowInputPhase(int currentRound)
    {
        if (titleText != null)
        {
            titleText.text = "REPITE LA SECUENCIA";
        }

        // Limpiar y crear slots vacíos
        ClearNumberDisplays();

        for (int i = 0; i < currentRound; i++)
        {
            GameObject numberObj = Instantiate(numberPrefab, patternContainer);
            TextMeshProUGUI numberText = numberObj.GetComponent<TextMeshProUGUI>();
            
            if (numberText != null)
            {
                numberText.text = "_";
                numberText.fontSize = numberFontSize;
                numberText.color = normalColor;
                currentNumberDisplays.Add(numberText);
            }
        }
    }

    /// <summary>
    /// Muestra feedback visual del input del jugador
    /// </summary>
    public void ShowInputFeedback(int number, bool isCorrect, int position)
    {
        if (position > 0 && position <= currentNumberDisplays.Count)
        {
            TextMeshProUGUI display = currentNumberDisplays[position - 1];
            display.text = number.ToString();
            display.color = isCorrect ? correctColor : incorrectColor;

            // Animación desactivada para evitar conflictos con Time.timeScale = 0
            // StartCoroutine(PulseNumber(display));
        }
    }

    /// <summary>
    /// Muestra mensaje de ronda completada
    /// </summary>
    public void ShowRoundComplete()
    {
        if (roundCompleteText != null)
        {
            roundCompleteText.gameObject.SetActive(true);
            roundCompleteText.text = "✓ RONDA COMPLETADA";
            // Animación desactivada para evitar conflictos con Time.timeScale = 0
            // StartCoroutine(FadeOutMessage(roundCompleteText, 1.5f));
        }
    }

    /// <summary>
    /// Muestra mensaje de éxito
    /// </summary>
    public void ShowSuccess()
    {
        ClearNumberDisplays();
        
        if (titleText != null) titleText.text = "";
        if (roundCounterText != null) roundCounterText.text = "";

        if (successText != null)
        {
            successText.gameObject.SetActive(true);
            successText.text = "¡DESAFÍO COMPLETADO!\nABRIENDO MURO...";
        }
    }

    /// <summary>
    /// Muestra mensaje de fallo
    /// </summary>
    public void ShowFailure()
    {
        ClearNumberDisplays();
        
        if (titleText != null) titleText.text = "";
        if (roundCounterText != null) roundCounterText.text = "";

        if (failureText != null)
        {
            failureText.gameObject.SetActive(true);
            failureText.text = "✗ FALLASTE\nREINICIANDO JUEGO...";
        }
    }

    /// <summary>
    /// Limpia todos los displays de números
    /// </summary>
    private void ClearNumberDisplays()
    {
        foreach (var display in currentNumberDisplays)
        {
            if (display != null)
            {
                Destroy(display.gameObject);
            }
        }
        currentNumberDisplays.Clear();
    }

    // === ANIMACIONES ===

    private IEnumerator AnimateNumberAppear(TextMeshProUGUI numberText, float delay)
    {
        if (numberText == null) yield break;

        numberText.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(delay);

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            numberText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        numberText.transform.localScale = Vector3.one;
    }

    private IEnumerator FadeOutPattern()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            foreach (var display in currentNumberDisplays)
            {
                if (display != null)
                {
                    Color color = display.color;
                    color.a = 1f - t;
                    display.color = color;
                }
            }

            yield return null;
        }

        ClearNumberDisplays();
    }

    private IEnumerator PulseNumber(TextMeshProUGUI numberText)
    {
        if (numberText == null) yield break;

        Vector3 originalScale = numberText.transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;

        float duration = 0.2f;
        float elapsed = 0f;

        // Escalar hacia arriba
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            numberText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;

        // Escalar hacia abajo
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            numberText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        numberText.transform.localScale = originalScale;
    }

    private IEnumerator FadeOutMessage(TextMeshProUGUI message, float delay)
    {
        yield return new WaitForSeconds(delay);
        message.gameObject.SetActive(false);
    }
}
