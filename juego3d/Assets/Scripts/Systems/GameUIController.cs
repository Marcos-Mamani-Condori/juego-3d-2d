using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Sistema de UI en pantalla con controles, menú de pausa y confirmaciones.
/// Muestra header con instrucciones y permite pausar el juego.
/// 
/// INSTRUCCIONES:
/// 1. Crea un GameObject vacío llamado "GameUI"
/// 2. Adjunta este script
/// 3. ¡Funciona automáticamente!
/// </summary>
public class GameUIController : MonoBehaviour
{
    [Header("Teclas de Control")]
    [Tooltip("Tecla para abrir/cerrar menú de pausa")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    [Tooltip("Tecla para reiniciar nivel rápido")]
    public KeyCode quickRestartKey = KeyCode.R;

    [Header("Configuración de Escenas")]
    [Tooltip("Nombre de la escena del menú principal")]
    public string mainMenuSceneName = "MenuPrincipal";
    
    [Tooltip("¿Mostrar botón de salir? (solo funciona en builds)")]
    public bool showQuitButton = true;

    [Header("Estilo Visual")]
    [Tooltip("¿Mostrar header con controles?")]
    public bool showControlsHeader = true;
    
    [Tooltip("Posición del header")]
    public HeaderPosition headerPosition = HeaderPosition.TopLeft;
    
    public enum HeaderPosition { TopLeft, TopCenter, TopRight }

    [Header("Textos Personalizables")]
    public string headerTitle = "CONTROLES";
    public string[] controlLines = new string[]
    {
        "F - Cambiar modo (Golf/Disparo)",
        "Tab - Disparar (Modo Shooter)",
        "Space - Destruir nave (Shooter) / Cargar potencia (Golf)",
        "V - Modo cámara libre",
        "R - Reiniciar nivel",
        "Backspace - Reset completo",
        "ESC - Menú de pausa"
    };

    // Estado del menú
    private bool isPaused = false;
    private bool showConfirmRestart = false;
    private bool showConfirmMenu = false;
    private bool showConfirmQuit = false;

    // Estilos de GUI
    private GUIStyle headerStyle;
    private GUIStyle controlStyle;
    private GUIStyle buttonStyle;
    private GUIStyle titleStyle;
    private GUIStyle confirmStyle;
    private bool stylesInitialized = false;

    void Update()
    {
        // Abrir/cerrar menú de pausa
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

        // Reinicio rápido (sin confirmación)
        if (!isPaused && Input.GetKeyDown(quickRestartKey))
        {
            showConfirmRestart = true;
            PauseGame();
        }
    }

    void OnGUI()
    {
        InitializeStyles();

        // Header con controles (siempre visible)
        if (showControlsHeader && !isPaused)
        {
            DrawControlsHeader();
        }

        // Menú de pausa
        if (isPaused)
        {
            DrawPauseMenu();
        }

        // Confirmaciones
        if (showConfirmRestart)
        {
            DrawConfirmRestart();
        }
        else if (showConfirmMenu)
        {
            DrawConfirmMenu();
        }
        else if (showConfirmQuit)
        {
            DrawConfirmQuit();
        }
    }

    void InitializeStyles()
    {
        if (stylesInitialized) return;

        // Estilo del header
        headerStyle = new GUIStyle(GUI.skin.box);
        headerStyle.fontSize = 14;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        headerStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.7f));
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.padding = new RectOffset(10, 10, 5, 5);

        // Estilo de controles
        controlStyle = new GUIStyle(GUI.skin.label);
        controlStyle.fontSize = 12;
        controlStyle.normal.textColor = Color.white;
        controlStyle.alignment = TextAnchor.MiddleLeft;

        // Estilo de botones
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 16;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.9f));
        buttonStyle.hover.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.9f));
        buttonStyle.active.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.9f));
        buttonStyle.padding = new RectOffset(10, 10, 10, 10);

        // Estilo de título
        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 36;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.yellow;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        // Estilo de confirmación
        confirmStyle = new GUIStyle(GUI.skin.box);
        confirmStyle.fontSize = 18;
        confirmStyle.fontStyle = FontStyle.Bold;
        confirmStyle.normal.textColor = Color.white;
        confirmStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.9f));
        confirmStyle.alignment = TextAnchor.MiddleCenter;
        confirmStyle.padding = new RectOffset(20, 20, 20, 20);

        stylesInitialized = true;
    }

    void DrawControlsHeader()
{
    // Dimensiones responsivas (basadas en el ancho de pantalla)
    float screenScale = Screen.width / 1920f; // Base 1920x1080
    
    float width = 300 * Mathf.Max(1f, screenScale);
    float lineHeight = 25 * Mathf.Max(1f, screenScale);
    float headerHeight = 40 * Mathf.Max(1f, screenScale);
    float totalHeight = headerHeight + (controlLines.Length * lineHeight) + 20;

    // *** MODIFICACIÓN CLAVE: ESCALAR EL MARGEN ***
    // Definimos el margen base (20px) y le aplicamos el factor de escala.
    float margin = 20 * screenScale;

    // Ajustar tamaño de fuente dinámicamente
    headerStyle.fontSize = (int)(18 * Mathf.Max(1f, screenScale));
    controlStyle.fontSize = (int)(14 * Mathf.Max(1f, screenScale));

    // Calcular posición relativa
    float x = margin; 
    float y = margin; // El margen superior ya está bien (depende de 'margin')

    switch (headerPosition)
    {
        case HeaderPosition.TopCenter:
            x = (Screen.width - width) / 2f;
            break;
        case HeaderPosition.TopRight:
            // *** ESTA ES LA FÓRMULA CORREGIDA Y CONSISTENTE ***
            // La posición X es el ancho total, menos el ancho de la caja, menos el margen escalado.
            x = Screen.width - width - margin; 
            break;
    }

    Rect headerRect = new Rect(x, y, width, totalHeight);

    // Fondo semi-transparente
    GUI.Box(headerRect, "", headerStyle);

    // Título del header
    GUI.Label(new Rect(x, y + 5, width, headerHeight), headerTitle, headerStyle);

    // Líneas de controles
    float currentY = y + headerHeight + 10;
    foreach (string line in controlLines)
    {
        GUI.Label(new Rect(x + 15, currentY, width - 30, lineHeight), line, controlStyle);
        currentY += lineHeight;
    }
}

    void DrawPauseMenu()
    {
        // Fondo oscuro que cubre TODA la pantalla
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", 
            new GUIStyle { normal = { background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.8f)) } });

        // Escala responsiva
        float screenScale = Screen.height / 1080f;
        float menuWidth = 500 * Mathf.Max(1f, screenScale);
        float menuHeight = 600 * Mathf.Max(1f, screenScale);
        
        float x = (Screen.width - menuWidth) / 2f;
        float y = (Screen.height - menuHeight) / 2f;

        // Título grande
        titleStyle.fontSize = (int)(48 * Mathf.Max(1f, screenScale));
        GUI.Label(new Rect(x, y, menuWidth, 80 * screenScale), "PAUSA", titleStyle);

        float buttonWidth = 350 * Mathf.Max(1f, screenScale);
        float buttonHeight = 60 * Mathf.Max(1f, screenScale);
        float buttonX = x + (menuWidth - buttonWidth) / 2f;
        float currentY = y + (100 * screenScale);
        float spacing = 30 * screenScale;

        buttonStyle.fontSize = (int)(24 * Mathf.Max(1f, screenScale));

        // Botón Continuar
        if (GUI.Button(new Rect(buttonX, currentY, buttonWidth, buttonHeight), "Continuar", buttonStyle))
        {
            ResumeGame();
        }
        currentY += buttonHeight + spacing;

        // Botón Reiniciar
        if (GUI.Button(new Rect(buttonX, currentY, buttonWidth, buttonHeight), "Reiniciar Nivel", buttonStyle))
        {
            showConfirmRestart = true;
        }
        currentY += buttonHeight + spacing;

        // Botón Menú Principal
        if (GUI.Button(new Rect(buttonX, currentY, buttonWidth, buttonHeight), "Menú Principal", buttonStyle))
        {
            showConfirmMenu = true;
        }
        currentY += buttonHeight + spacing;

        // Botón Salir
        if (showQuitButton)
        {
            if (GUI.Button(new Rect(buttonX, currentY, buttonWidth, buttonHeight), "Salir del Juego", buttonStyle))
            {
                showConfirmQuit = true;
            }
        }
    }

    void DrawConfirmRestart()
    {
        DrawConfirmDialog(
            "¿REINICIAR NIVEL?",
            "¿Estás seguro de que quieres reiniciar el nivel?\nTodo el progreso actual se perderá.",
            () => { RestartLevel(); },
            () => { showConfirmRestart = false; }
        );
    }

    void DrawConfirmMenu()
    {
        DrawConfirmDialog(
            "¿VOLVER AL MENÚ?",
            "¿Estás seguro de que quieres volver al menú principal?\nTodo el progreso actual se perderá.",
            () => { GoToMainMenu(); },
            () => { showConfirmMenu = false; }
        );
    }

    void DrawConfirmQuit()
    {
        DrawConfirmDialog(
            "¿SALIR DEL JUEGO?",
            "¿Estás seguro de que quieres salir del juego?",
            () => { QuitGame(); },
            () => { showConfirmQuit = false; }
        );
    }

    void DrawConfirmDialog(string title, string message, System.Action onConfirm, System.Action onCancel)
    {
        float dialogWidth = 500;
        float dialogHeight = 250;
        float x = (Screen.width - dialogWidth) / 2f;
        float y = (Screen.height - dialogHeight) / 2f;

        // Fondo del diálogo
        GUI.Box(new Rect(x, y, dialogWidth, dialogHeight), "", confirmStyle);

        // Título
        GUIStyle titleConfirm = new GUIStyle(titleStyle);
        titleConfirm.fontSize = 24;
        titleConfirm.normal.textColor = Color.red;
        GUI.Label(new Rect(x, y + 20, dialogWidth, 40), title, titleConfirm);

        // Mensaje
        GUIStyle messageStyle = new GUIStyle(controlStyle);
        messageStyle.fontSize = 14;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        messageStyle.wordWrap = true;
        GUI.Label(new Rect(x + 20, y + 70, dialogWidth - 40, 80), message, messageStyle);

        // Botones
        float buttonWidth = 150;
        float buttonHeight = 40;
        float buttonY = y + dialogHeight - 60;
        float spacing = 20;
        float totalButtonWidth = (buttonWidth * 2) + spacing;
        float buttonStartX = x + (dialogWidth - totalButtonWidth) / 2f;

        // Botón SÍ
        GUIStyle yesButtonStyle = new GUIStyle(buttonStyle);
        yesButtonStyle.normal.background = MakeTex(2, 2, new Color(0.8f, 0.2f, 0.2f, 0.9f));
        yesButtonStyle.hover.background = MakeTex(2, 2, new Color(1f, 0.3f, 0.3f, 0.9f));
        
        if (GUI.Button(new Rect(buttonStartX, buttonY, buttonWidth, buttonHeight), "SÍ", yesButtonStyle))
        {
            onConfirm();
        }

        // Botón NO
        GUIStyle noButtonStyle = new GUIStyle(buttonStyle);
        noButtonStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.6f, 0.2f, 0.9f));
        noButtonStyle.hover.background = MakeTex(2, 2, new Color(0.3f, 0.8f, 0.3f, 0.9f));
        
        if (GUI.Button(new Rect(buttonStartX + buttonWidth + spacing, buttonY, buttonWidth, buttonHeight), "NO", noButtonStyle))
        {
            onCancel();
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        // Mostrar cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void ResumeGame()
    {
        isPaused = false;
        showConfirmRestart = false;
        showConfirmMenu = false;
        showConfirmQuit = false;
        Time.timeScale = 1f;

        // El cursor se maneja según el modo del juego
        // (el PersonajeControlador lo controla)
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("No se especificó el nombre de la escena del menú principal.");
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Helper para crear texturas de color
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
