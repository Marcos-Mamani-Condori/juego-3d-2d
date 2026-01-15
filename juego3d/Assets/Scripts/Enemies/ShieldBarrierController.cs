using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBarrierController : MonoBehaviour
{
    [Header("Shield & Barrier")]
    public ShieldModuleScript shieldModule;
    public GameObject barrierObject;
    [Header("Drop Animation")]
    public float dropDistance = 2f;
    public float dropDuration = 0.6f;
    public AnimationCurve dropCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool disableCollidersDuringDrop = true;

    [Header("Linked Enemies")]
    public List<EnemyHealth> linkedEnemies = new List<EnemyHealth>();

    private int remainingEnemies;
    private Vector3 barrierInitialPosition;
    private bool hasInitialBarrierPosition;
    private Collider[] barrierColliders;
    private Coroutine releaseCoroutine;

    void Awake()
    {
        if (shieldModule == null)
        {
            shieldModule = GetComponent<ShieldModuleScript>();
        }

        RegisterEnemies();
    }

    void OnEnable()
    {
        if (releaseCoroutine != null)
        {
            StopCoroutine(releaseCoroutine);
            releaseCoroutine = null;
        }

        RegisterEnemies();
    }

    void OnValidate()
    {
        if (shieldModule == null)
        {
            shieldModule = GetComponent<ShieldModuleScript>();
        }

        if (!Application.isPlaying)
        {
            PruneNullEnemies();
        }
    }

    private void RegisterEnemies()
    {
        PruneNullEnemies();
        remainingEnemies = 0;

        foreach (var enemy in linkedEnemies)
        {
            if (enemy == null) continue;

            enemy.shieldController = this;
            remainingEnemies++;
        }

        if (shieldModule != null)
        {
            shieldModule.ActivateShield();
        }

        if (barrierObject != null)
        {
            if (!hasInitialBarrierPosition)
            {
                barrierInitialPosition = barrierObject.transform.position;
                hasInitialBarrierPosition = true;
                barrierColliders = barrierObject.GetComponentsInChildren<Collider>(true);
            }
            else
            {
                barrierObject.transform.position = barrierInitialPosition;
            }

            barrierObject.SetActive(true);

            if (disableCollidersDuringDrop && barrierColliders != null)
            {
                foreach (var col in barrierColliders)
                {
                    if (col != null) col.enabled = true;
                }
            }
        }
    }

    private void PruneNullEnemies()
    {
        linkedEnemies.RemoveAll(enemy => enemy == null);
    }

    public void NotifyEnemyDestroyed(EnemyHealth enemy)
    {
        if (linkedEnemies.Contains(enemy))
        {
            linkedEnemies.Remove(enemy);
        }

        remainingEnemies = Mathf.Max(0, remainingEnemies - 1);

        if (remainingEnemies <= 0)
        {
            ReleaseBarrier();
        }
    }

    private void ReleaseBarrier()
    {
        if (releaseCoroutine != null)
        {
            return;
        }

        releaseCoroutine = StartCoroutine(DropBarrierRoutine());
    }

    /// <summary>
    /// Permite que sistemas externos (como PatternMemoryGame) liberen la barrera.
    /// Método público que no requiere destrucción de enemigos.
    /// </summary>
    public void ReleaseBarrierExternal()
    {
        Debug.Log("[ShieldBarrierController] Barrera liberada externamente (ej: desafío de patrones completado)");
        ReleaseBarrier();
    }


    private IEnumerator DropBarrierRoutine()
    {
        if (barrierObject == null)
        {
            yield break;
        }

        Vector3 startPos = barrierObject.transform.position;
        Vector3 endPos = startPos - Vector3.up * Mathf.Max(0f, dropDistance);

        if (disableCollidersDuringDrop && barrierColliders != null)
        {
            foreach (var col in barrierColliders)
            {
                if (col != null) col.enabled = false;
            }
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, dropDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = dropCurve != null ? dropCurve.Evaluate(t) : t;
            barrierObject.transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        barrierObject.transform.position = endPos;

        yield return new WaitForSeconds(0.1f);

        if (shieldModule != null)
        {
            shieldModule.DeactivateShield();
        }

        if (barrierObject.activeSelf)
        {
            barrierObject.SetActive(false);
        }

        releaseCoroutine = null;
    }
}
