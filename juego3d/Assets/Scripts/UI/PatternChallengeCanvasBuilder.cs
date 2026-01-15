using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Crea automáticamente el Canvas del desafío de patrones con efecto de blur en el fondo.
/// Ejecuta esto UNA VEZ en el editor para generar el Canvas, luego desactiva el script.
/// </summary>
[ExecuteInEditMode]
public class PatternChallengeCanvasBuilder : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Presiona este botón en el Inspector para crear el Canvas")]
    public bool createCanvas = false;

    [Header("Referencias de Fuentes (Opcional)")]
    public TMP_FontAsset customFont;

    void Update()
    {
        if (createCanvas)
        {
            createCanvas = false;
            BuildCanvas();
        }
    }

    void BuildCanvas()
    {
        Debug.Log("=== CREANDO CANVAS DE DESAFÍO DE PATRONES ===");

        // 1. Crear Canvas principal
        GameObject canvasObj = new GameObject("PatternChallengeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Asegurar que esté encima de todo

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Crear Panel de Fondo con BLUR (semi-transparente oscuro)
        GameObject panelObj = new GameObject("MainPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f); // Negro semi-transparente para efecto blur

        CanvasGroup canvasGroup = panelObj.AddComponent<CanvasGroup>();

        // 3. Crear contenedor central
        GameObject centerContainer = new GameObject("CenterContainer");
        centerContainer.transform.SetParent(panelObj.transform, false);
        
        RectTransform centerRect = centerContainer.AddComponent<RectTransform>();
        centerRect.anchorMin = new Vector2(0.5f, 0.5f);
        centerRect.anchorMax = new Vector2(0.5f, 0.5f);
        centerRect.sizeDelta = new Vector2(800, 600);
        centerRect.anchoredPosition = Vector2.zero;

        // 4. Título
        GameObject titleObj = CreateText("TitleText", "MEMORIZA EL PATRÓN", 60, TextAlignmentOptions.Center);
        titleObj.transform.SetParent(centerContainer.transform, false);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(800, 100);
        titleRect.anchoredPosition = new Vector2(0, -50);

        // 5. Contador de Rondas
        GameObject roundCounterObj = CreateText("RoundCounterText", "RONDA 1/4", 40, TextAlignmentOptions.Center);
        roundCounterObj.transform.SetParent(centerContainer.transform, false);
        RectTransform roundRect = roundCounterObj.GetComponent<RectTransform>();
        roundRect.anchorMin = new Vector2(0.5f, 1f);
        roundRect.anchorMax = new Vector2(0.5f, 1f);
        roundRect.sizeDelta = new Vector2(400, 60);
        roundRect.anchoredPosition = new Vector2(0, -130);

        // 6. Contenedor de Patrones (con HorizontalLayoutGroup)
        GameObject patternContainerObj = new GameObject("PatternContainer");
        patternContainerObj.transform.SetParent(centerContainer.transform, false);
        
        RectTransform patternRect = patternContainerObj.AddComponent<RectTransform>();
        patternRect.anchorMin = new Vector2(0.5f, 0.5f);
        patternRect.anchorMax = new Vector2(0.5f, 0.5f);
        patternRect.sizeDelta = new Vector2(600, 120);
        patternRect.anchoredPosition = Vector2.zero;

        HorizontalLayoutGroup layoutGroup = patternContainerObj.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 50;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter fitter = patternContainerObj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 7. Prefab de Número (para clonar)
        GameObject numberPrefabObj = CreateText("NumberPrefab", "1", 80, TextAlignmentOptions.Center);
        RectTransform numberRect = numberPrefabObj.GetComponent<RectTransform>();
        numberRect.sizeDelta = new Vector2(100, 100);
        
        // Guardar como prefab (debes hacerlo manualmente o usar PrefabUtility en editor)
        numberPrefabObj.SetActive(false); // Desactivar el template

        // 8. Mensaje de Éxito
        GameObject successObj = CreateText("SuccessText", "¡DESAFÍO COMPLETADO!\nABRIENDO MURO...", 50, TextAlignmentOptions.Center);
        successObj.transform.SetParent(centerContainer.transform, false);
        RectTransform successRect = successObj.GetComponent<RectTransform>();
        successRect.anchorMin = new Vector2(0.5f, 0.5f);
        successRect.anchorMax = new Vector2(0.5f, 0.5f);
        successRect.sizeDelta = new Vector2(700, 150);
        successRect.anchoredPosition = Vector2.zero;
        successObj.GetComponent<TextMeshProUGUI>().color = new Color(0, 1, 0, 1); // Verde
        successObj.SetActive(false);

        // 9. Mensaje de Fallo
        GameObject failureObj = CreateText("FailureText", "✗ FALLASTE\nREINICIANDO JUEGO...", 50, TextAlignmentOptions.Center);
        failureObj.transform.SetParent(centerContainer.transform, false);
        RectTransform failureRect = failureObj.GetComponent<RectTransform>();
        failureRect.anchorMin = new Vector2(0.5f, 0.5f);
        failureRect.anchorMax = new Vector2(0.5f, 0.5f);
        failureRect.sizeDelta = new Vector2(700, 150);
        failureRect.anchoredPosition = Vector2.zero;
        failureObj.GetComponent<TextMeshProUGUI>().color = new Color(1, 0, 0, 1); // Rojo
        failureObj.SetActive(false);

        // 10. Mensaje de Ronda Completada
        GameObject roundCompleteObj = CreateText("RoundCompleteText", "✓ RONDA COMPLETADA", 40, TextAlignmentOptions.Center);
        roundCompleteObj.transform.SetParent(centerContainer.transform, false);
        RectTransform roundCompleteRect = roundCompleteObj.GetComponent<RectTransform>();
        roundCompleteRect.anchorMin = new Vector2(0.5f, 0f);
        roundCompleteRect.anchorMax = new Vector2(0.5f, 0f);
        roundCompleteRect.sizeDelta = new Vector2(600, 80);
        roundCompleteRect.anchoredPosition = new Vector2(0, 100);
        roundCompleteObj.GetComponent<TextMeshProUGUI>().color = new Color(0, 1, 0, 1); // Verde
        roundCompleteObj.SetActive(false);

        // 11. Añadir el script PatternChallengeUI
        PatternChallengeUI uiScript = canvasObj.AddComponent<PatternChallengeUI>();
        uiScript.mainPanel = panelObj;
        uiScript.titleText = titleObj.GetComponent<TextMeshProUGUI>();
        uiScript.roundCounterText = roundCounterObj.GetComponent<TextMeshProUGUI>();
        uiScript.patternContainer = patternContainerObj.transform;
        uiScript.numberPrefab = numberPrefabObj;
        uiScript.successText = successObj.GetComponent<TextMeshProUGUI>();
        uiScript.failureText = failureObj.GetComponent<TextMeshProUGUI>();
        uiScript.roundCompleteText = roundCompleteObj.GetComponent<TextMeshProUGUI>();

        // Desactivar el panel inicialmente
        panelObj.SetActive(false);

        Debug.Log("✅ Canvas creado exitosamente! Revisa la jerarquía.");
        Debug.Log("⚠️ IMPORTANTE: Arrastra 'NumberPrefab' a la carpeta Prefabs y asígnalo en el Inspector.");
    }

    GameObject CreateText(string name, string text, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;

        if (customFont != null)
        {
            tmp.font = customFont;
        }

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        return textObj;
    }
}
