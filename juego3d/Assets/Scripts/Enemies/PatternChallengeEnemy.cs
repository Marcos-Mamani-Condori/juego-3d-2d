using UnityEngine;

/// <summary>
/// Enemigo especial que activa el desafío de memoria de patrones al ser DISPARADO.
/// Se integra con el sistema de disparo del robot (Raycast) y EnemyHealth.
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class PatternChallengeEnemy : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Sistema de desafío de patrones")]
    public PatternMemoryGame patternGame;

    [Header("Configuración")]
    [Tooltip("¿Puede activarse múltiples veces?")]
    public bool canActivateMultipleTimes = false;

    [Tooltip("Efecto visual al activar (opcional)")]
    public ParticleSystem activationEffect;

    [Tooltip("Sonido al activar (opcional)")]
    public AudioClip activationSound;

    [Header("Visual")]
    [Tooltip("Material especial para distinguir este enemigo (opcional)")]
    public Material specialMaterial;

    [Tooltip("¿Hacer que el enemigo brille/pulse?")]
    public bool enablePulseEffect = true;

    [Tooltip("Velocidad del pulso")]
    public float pulseSpeed = 2f;

    [Tooltip("Intensidad del pulso (escala)")]
    public float pulseIntensity = 0.1f;

    private bool hasBeenActivated = false;
    private AudioSource audioSource;
    private Renderer enemyRenderer;
    private Vector3 originalScale;
    private Material originalMaterial;
    private EnemyHealth enemyHealth;

    void Start()
    {
        // Buscar PatternMemoryGame si no está asignado
        if (patternGame == null)
        {
            patternGame = FindFirstObjectByType<PatternMemoryGame>();
            if (patternGame == null)
            {
                Debug.LogError($"[{gameObject.name}] PatternMemoryGame no encontrado! Asigna la referencia manualmente.");
            }
        }

        // Obtener EnemyHealth y hacer este enemigo INMORTAL
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Hacer inmortal: establecer vida muy alta
            enemyHealth.health = 999999;
            Debug.Log($"[{gameObject.name}] Enemigo especial configurado como INMORTAL");
        }

        // Setup visual
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalMaterial = enemyRenderer.material;
            originalScale = transform.localScale;

            // Aplicar material especial si está asignado
            if (specialMaterial != null)
            {
                enemyRenderer.material = specialMaterial;
            }
        }

        // Setup audio
        if (activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = activationSound;
        }
    }

    void Update()
    {
        // Efecto de pulso visual
        if (enablePulseEffect && !hasBeenActivated && enemyRenderer != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            transform.localScale = originalScale * pulse;
        }
    }

    /// <summary>
    /// Método llamado cuando este enemigo recibe daño del disparo.
    /// Se integra con el sistema de disparo existente del robot.
    /// </summary>
    public void OnShotHit()
    {
        Debug.LogError("🔴🔴🔴 ONSHOTIT LLAMADO EN PATTERNCHALLENGEENEMY 🔴🔴🔴");
        Debug.Log($"[{gameObject.name}] ⚡ OnShotHit() llamado - Intentando activar desafío...");
        TryActivateChallenge();
    }

    /// <summary>
    /// Intenta activar el desafío de patrones
    /// </summary>
    private void TryActivateChallenge()
    {
        Debug.Log($"[{gameObject.name}] 🔍 TryActivateChallenge() - hasBeenActivated: {hasBeenActivated}, canActivateMultipleTimes: {canActivateMultipleTimes}");
        
        // Verificar si ya fue activado
        if (hasBeenActivated && !canActivateMultipleTimes)
        {
            Debug.LogWarning($"[{gameObject.name}] ⚠️ Ya fue activado anteriormente.");
            return;
        }

        // Verificar que el sistema de patrones esté disponible
        if (patternGame == null)
        {
            Debug.LogError($"[{gameObject.name}] ❌ ERROR CRÍTICO: PatternMemoryGame es NULL! Asigna la referencia en el Inspector.");
            return;
        }

        Debug.Log($"[{gameObject.name}] ✅ ¡Enemigo especial disparado! Activando desafío de patrones...");

        // Marcar como activado
        hasBeenActivated = true;

        // Efectos visuales/sonoros
        PlayActivationEffects();

        // Iniciar el desafío
        Debug.Log($"[{gameObject.name}] 🎮 Llamando a patternGame.StartChallenge()...");
        patternGame.StartChallenge();

        // Opcional: Desactivar el pulso visual
        if (enablePulseEffect)
        {
            enablePulseEffect = false;
            transform.localScale = originalScale;
        }
    }


    /// <summary>
    /// Reproduce efectos de activación
    /// </summary>
    private void PlayActivationEffects()
    {
        // Efecto de partículas
        if (activationEffect != null)
        {
            activationEffect.Play();
        }

        // Sonido
        if (audioSource != null && activationSound != null)
        {
            audioSource.Play();
        }

        // Cambio de color/material (opcional)
        if (enemyRenderer != null)
        {
            // Puedes cambiar el color aquí si quieres feedback visual
            // enemyRenderer.material.color = Color.gray;
        }
    }

    /// <summary>
    /// Resetea el estado del enemigo (útil para testing)
    /// </summary>
    public void ResetEnemy()
    {
        hasBeenActivated = false;
        enablePulseEffect = true;
        
        if (enemyRenderer != null && originalMaterial != null)
        {
            enemyRenderer.material = originalMaterial;
        }

        Debug.Log($"[{gameObject.name}] Enemigo reseteado.");
    }

    void OnDrawGizmosSelected()
    {
        // Visualización en el editor
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.5f); // Amarillo/dorado
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

