using System.Collections;
using UnityEngine;

public class MovingBarrier : MonoBehaviour
{
    public enum MovementAxis { UpDown, LeftRight, ForwardBackward, Custom }

    [Header("Movimiento Básico")]
    public MovementAxis movementAxis = MovementAxis.UpDown;
    [Tooltip("Desplazamiento total desde la posición inicial.")]
    public float distance = 3f;
    [Tooltip("Vector personalizado utilizado cuando MovementAxis está en Custom.")]
    public Vector3 customDirection = Vector3.up;
    [Tooltip("Tiempo total para completar el recorrido (ida o vuelta).")]
    public float travelTime = 1.5f;
    [Tooltip("Curve define la suavidad del movimiento (0-1) en el tiempo.")]
    public AnimationCurve motionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Tooltip("Retraso opcional antes de iniciar el movimiento.")]
    public float startDelay = 0f;

    [Header("Comportamiento")]
    public bool loop = true;
    [Tooltip("Esperar este tiempo antes de invertir la dirección en modo loop.")]
    public float pauseBetweenLoops = 0.5f;
    [Tooltip("Si está activo, desactiva colliders mientras el muro se mueve.")]
    public bool disableCollidersWhileMoving = false;

    private Vector3 initialPosition;
    private Vector3 targetOffset;
    private Coroutine movementCoroutine;
    private Collider[] cachedColliders;
    private bool movingForward = true;

    void Awake()
    {
        initialPosition = transform.position;
        targetOffset = GetDirectionVector() * distance;
        cachedColliders = GetComponentsInChildren<Collider>(true);
    }

    void OnEnable()
    {
        StopMovement();
        movementCoroutine = StartCoroutine(MoveRoutine());
    }

    void OnDisable()
    {
        StopMovement();
    }

    private Vector3 GetDirectionVector()
    {
        switch (movementAxis)
        {
            case MovementAxis.UpDown:
                return Vector3.up;
            case MovementAxis.LeftRight:
                return Vector3.right;
            case MovementAxis.ForwardBackward:
                return Vector3.forward;
            case MovementAxis.Custom:
                return customDirection.sqrMagnitude > 0.0001f ? customDirection.normalized : Vector3.up;
            default:
                return Vector3.up;
        }
    }

    private IEnumerator MoveRoutine()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        while (enabled)
        {
            yield return MoveOnce(movingForward ? 1f : -1f);

            if (!loop)
            {
                yield break;
            }

            movingForward = !movingForward;

            if (pauseBetweenLoops > 0f)
            {
                yield return new WaitForSeconds(pauseBetweenLoops);
            }
        }
    }

    private IEnumerator MoveOnce(float directionSign)
    {
        Vector3 startPos = movingForward ? initialPosition : initialPosition + targetOffset;
        Vector3 endPos = movingForward ? initialPosition + targetOffset : initialPosition;

        if (directionSign < 0f)
        {
            (startPos, endPos) = (endPos, startPos);
        }

        float duration = Mathf.Max(0.01f, travelTime);
        float elapsed = 0f;

        if (disableCollidersWhileMoving && cachedColliders != null)
        {
            foreach (var col in cachedColliders)
            {
                if (col != null) col.enabled = false;
            }
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = motionCurve != null ? motionCurve.Evaluate(t) : t;
            transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        transform.position = endPos;

        if (disableCollidersWhileMoving && cachedColliders != null)
        {
            foreach (var col in cachedColliders)
            {
                if (col != null) col.enabled = true;
            }
        }
    }

    public void StopMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    public void RestartMovement()
    {
        StopMovement();
        initialPosition = transform.position;
        targetOffset = GetDirectionVector() * distance;
        movementCoroutine = StartCoroutine(MoveRoutine());
    }
}
