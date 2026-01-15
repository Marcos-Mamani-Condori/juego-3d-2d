using UnityEngine;

public class SeguirCamara : MonoBehaviour
{
    public Transform target;      
    public float smoothSpeed = 0.125f; 
    public Vector3 offset = new Vector3(0, 0, -10);

    // Límites del mapa
    public float mapMinX = -15f;  // borde izquierdo
    public float mapMaxX = 24f;   // borde derecho
    public float mapMinY = -5f;    // borde inferior
    public float mapMaxY = 9f;   // borde superior

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float camHalfHeight = Camera.main.orthographicSize;

        // Limitar horizontalmente
        float clampedX = Mathf.Clamp(smoothedPosition.x, mapMinX + camHalfWidth, mapMaxX - camHalfWidth);

        // Limitar verticalmente
        float clampedY = Mathf.Clamp(smoothedPosition.y, mapMinY + camHalfHeight, mapMaxY - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
