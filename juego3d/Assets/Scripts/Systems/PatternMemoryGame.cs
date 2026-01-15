using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de mini-juego de memoria de patrones (estilo Simon Says).
/// 4 rondas acumulativas con números aleatorios 1-4.
/// Éxito: Baja el muro. Fallo: Reinicia el juego completo.
/// </summary>
public class PatternMemoryGame : MonoBehaviour
{
    public static PatternMemoryGame Instance;

    [Header("Referencias")]
    [Tooltip("Controlador del muro/barrera que se abrirá al completar el desafío")]
    public ShieldBarrierController shieldBarrierController;
    
    [Tooltip("UI del desafío de patrones")]
    public PatternChallengeUI challengeUI;

    [Tooltip("Controlador de la pelota de golf (OPCIONAL - solo para modo golf)")]
    public GolfBallController golfBallController;

    [Tooltip("Controlador del robot/personaje (OPCIONAL - solo para modo shooter)")]
    public PersonajeControlador personajeControlador;

    [Header("Configuración del Juego")]
    [Tooltip("Número total de rondas")]
    public int totalRounds = 4;

    [Tooltip("Tiempo para mostrar el patrón (segundos)")]
    public float patternDisplayTime = 4f;

    [Tooltip("Rango de números a usar (1-4)")]
    public int minNumber = 1;
    public int maxNumber = 4;

    // Estado del juego
    private List<int> masterPattern = new List<int>(); // Patrón maestro completo
    private List<int> currentInput = new List<int>(); // Input actual del jugador
    private int currentRound = 0;
    private bool isGameActive = false;
    private bool isWaitingForInput = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicia el desafío de patrones
    /// </summary>
    public void StartChallenge()
    {
        if (isGameActive)
        {
            Debug.LogWarning("El desafío ya está activo.");
            return;
        }

        Debug.Log("=== INICIANDO DESAFÍO DE PATRONES ===");
        
        // Generar patrón maestro aleatorio de 4 números
        GenerateMasterPattern();
        
        // Resetear estado
        currentRound = 0;
        currentInput.Clear();
        isGameActive = true;

        // PAUSAR EL JUEGO durante el desafío
        Time.timeScale = 0f;
        Debug.Log("⏸️ Juego pausado (Time.timeScale = 0)");

        // Mostrar UI
        if (challengeUI != null)
        {
            Debug.Log("✅ challengeUI encontrado, llamando ShowUI()...");
            challengeUI.ShowUI();
        }
        else
        {
            Debug.LogError("❌ ERROR CRÍTICO: challengeUI es NULL! Asigna el Canvas en el Inspector del GameManager.");
            Debug.LogError("   → Ve a GameManager → Pattern Memory Game → Challenge UI");
            Debug.LogError("   → Arrastra 'PatternChallengeCanvas' a ese campo");
        }

        // Iniciar primera ronda (usar WaitForSecondsRealtime porque el juego está pausado)
        StartCoroutine(StartRound());
    }

    /// <summary>
    /// Genera el patrón maestro aleatorio de 4 números
    /// </summary>
    private void GenerateMasterPattern()
    {
        masterPattern.Clear();
        for (int i = 0; i < totalRounds; i++)
        {
            int randomNumber = Random.Range(minNumber, maxNumber + 1);
            masterPattern.Add(randomNumber);
        }

        Debug.Log($"Patrón maestro generado: {string.Join(" → ", masterPattern)}");
    }

    /// <summary>
    /// Inicia una ronda mostrando el patrón acumulativo
    /// </summary>
    private IEnumerator StartRound()
    {
        currentRound++;
        currentInput.Clear();
        isWaitingForInput = false;

        Debug.Log($"--- RONDA {currentRound}/{totalRounds} ---");

        // Obtener el patrón para esta ronda (acumulativo)
        List<int> currentPattern = masterPattern.GetRange(0, currentRound);
        
        Debug.Log($"Patrón a mostrar: {string.Join(" → ", currentPattern)}");

        // Mostrar el patrón en el UI
        if (challengeUI != null)
        {
            challengeUI.ShowPattern(currentPattern, currentRound, totalRounds);
        }

        // Esperar el tiempo de visualización (usar Realtime porque el juego está pausado)
        yield return new WaitForSecondsRealtime(patternDisplayTime);

        // Ocultar el patrón y esperar input
        if (challengeUI != null)
        {
            challengeUI.HidePattern();
            challengeUI.ShowInputPhase(currentRound);
        }

        isWaitingForInput = true;
        Debug.Log("Esperando input del jugador...");
    }

    void Update()
    {
        if (!isGameActive || !isWaitingForInput) return;

        // Detectar input de números 1-4
        for (int i = minNumber; i <= maxNumber; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + (i - 1)) || Input.GetKeyDown(KeyCode.Keypad1 + (i - 1)))
            {
                ProcessInput(i);
                break;
            }
        }
    }

    /// <summary>
    /// Procesa el input del jugador
    /// </summary>
    private void ProcessInput(int number)
    {
        currentInput.Add(number);
        int inputIndex = currentInput.Count - 1;

        Debug.Log($"Input recibido: {number} (posición {inputIndex + 1}/{currentRound})");

        // Verificar si el input es correcto
        if (number != masterPattern[inputIndex])
        {
            // INPUT INCORRECTO - GAME OVER
            Debug.LogError($"¡INPUT INCORRECTO! Esperado: {masterPattern[inputIndex]}, Recibido: {number}");
            StartCoroutine(HandleFailure());
            return;
        }

        // Input correcto
        Debug.Log($"✓ Correcto! ({currentInput.Count}/{currentRound})");
        
        // Actualizar UI con feedback visual
        if (challengeUI != null)
        {
            challengeUI.ShowInputFeedback(number, true, currentInput.Count);
        }

        // Verificar si completó la ronda actual
        if (currentInput.Count == currentRound)
        {
            isWaitingForInput = false;

            if (currentRound >= totalRounds)
            {
                // ¡COMPLETÓ TODAS LAS RONDAS!
                StartCoroutine(HandleSuccess());
            }
            else
            {
                // Pasar a la siguiente ronda
                StartCoroutine(NextRound());
            }
        }
    }

    /// <summary>
    /// Pasa a la siguiente ronda
    /// </summary>
    private IEnumerator NextRound()
    {
        Debug.Log($"✓ Ronda {currentRound} completada!");
        
        if (challengeUI != null)
        {
            challengeUI.ShowRoundComplete();
        }

        yield return new WaitForSecondsRealtime(1.5f);

        StartCoroutine(StartRound());
    }

    /// <summary>
    /// Maneja el éxito (completó todas las rondas)
    /// </summary>
    private IEnumerator HandleSuccess()
    {
        Debug.Log("=== ¡DESAFÍO COMPLETADO! ===");
        isGameActive = false;

        if (challengeUI != null)
        {
            challengeUI.ShowSuccess();
        }

        yield return new WaitForSecondsRealtime(2f);

        // Bajar el muro
        if (shieldBarrierController != null)
        {
            shieldBarrierController.ReleaseBarrierExternal();
            Debug.Log("Muro liberado!");
        }

        // Ocultar UI
        if (challengeUI != null)
        {
            challengeUI.HideUI();
        }

        // REANUDAR EL JUEGO
        Time.timeScale = 1f;
        Debug.Log("▶️ Juego reanudado (Time.timeScale = 1)");
    }

    /// <summary>
    /// Maneja el fallo (input incorrecto) - REINICIA EL JUEGO
    /// </summary>
    private IEnumerator HandleFailure()
    {
        Debug.LogError("=== DESAFÍO FALLIDO - REINICIANDO JUEGO ===");
        isGameActive = false;
        isWaitingForInput = false;

        if (challengeUI != null)
        {
            challengeUI.ShowFailure();
        }

        yield return new WaitForSecondsRealtime(2f);

        // REINICIAR TODO EL JUEGO
        if (CheckpointSystem.Instance != null)
        {
            // Reanudar el tiempo ANTES de reiniciar
            Time.timeScale = 1f;
            CheckpointSystem.Instance.HardReset();
        }
        else
        {
            Debug.LogError("CheckpointSystem no encontrado! No se puede reiniciar el juego.");
            Time.timeScale = 1f; // Reanudar de todos modos
        }

        // Ocultar UI
        if (challengeUI != null)
        {
            challengeUI.HideUI();
        }
    }

    /// <summary>
    /// Cancela el desafío (por si necesitas resetear manualmente)
    /// </summary>
    public void CancelChallenge()
    {
        isGameActive = false;
        isWaitingForInput = false;
        
        if (challengeUI != null)
        {
            challengeUI.HideUI();
        }

        // Reanudar el juego
        Time.timeScale = 1f;

        Debug.Log("Desafío cancelado.");
    }
}
