using UnityEngine;

[RequireComponent(typeof(EnemyShipTarget))]
public class EnemyShipMover : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float directionChangeInterval = 2.5f;

    [Header("Límites del Área")]
    public Vector3 areaCenter;
    public Vector3 areaSize = new Vector3(10f, 6f, 0f);

    [Header("Suavizado")]
    public float turnSpeed = 4f;

    private Vector3 currentTargetPoint;
    private float timeToChangeDirection;

    void Start()
    {
        if (areaSize == Vector3.zero)
        {
            areaSize = new Vector3(10f, 6f, 0f);
        }

        if (areaCenter == Vector3.zero)
        {
            areaCenter = transform.position;
        }

        PickNewTargetPoint(true);
    }

    void Update()
    {
        if (Time.time >= timeToChangeDirection)
        {
            PickNewTargetPoint();
        }

        Vector3 direction = (currentTargetPoint - transform.position);
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Vector3 moveStep = direction.normalized * moveSpeed * Time.deltaTime;
            transform.position += moveStep;

            if (moveStep != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, moveStep.normalized));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }

    private void PickNewTargetPoint(bool immediate = false)
    {
        Vector3 halfSize = areaSize * 0.5f;
        float randomX = Random.Range(-halfSize.x, halfSize.x);
        float randomY = Random.Range(-halfSize.y, halfSize.y);

        currentTargetPoint = areaCenter + new Vector3(randomX, randomY, 0f);
        timeToChangeDirection = Time.time + (immediate ? 0f : Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.8f, 0.3f);
        Gizmos.DrawCube(areaCenter == Vector3.zero ? transform.position : areaCenter, areaSize);
    }
}
