using UnityEngine;

public class LogicaHuecoAvanzada : MonoBehaviour
{
    [Header("Configuración del Hueco")]
    [Tooltip("La velocidad MÁXIMA a la que la pelota puede ir para ser capturada.")]
    public float velocidadMaximaParaEntrar = 4.0f;

    [Header("Configuración de Rebote")]
    [Tooltip("La fuerza con la que el borde 'patea' la pelota si va muy rápido.")]
    public float fuerzaDeRebote = 4.0f;

    [Tooltip("Un pequeño salto vertical al rebotar, para más realismo.")]
    public float fuerzaSaltoRebote = 2.0f;

    void OnTriggerEnter(Collider other)
    {
        // 1. ¿Es la pelota la que entró?
        if (other.CompareTag("Pelota"))
        {
            Rigidbody pelotaRb = other.GetComponent<Rigidbody>();
            if (pelotaRb == null) return; 

            float velocidadActual = pelotaRb.linearVelocity.magnitude;

            // --- CASO 1: PELOTA LENTA (GANA) ---
            if (velocidadActual <= velocidadMaximaParaEntrar)
            {
                // ... (Esta parte es igual, la pelota gana)
                Debug.Log("¡ENTRÓ! Velocidad aceptable: " + velocidadActual);
                pelotaRb.isKinematic = true; 
                pelotaRb.linearVelocity = Vector3.zero;
                pelotaRb.angularVelocity = Vector3.zero;
                other.transform.position = this.transform.position; 
                // FindObjectOfType<GameManager>().NivelCompletado();
            }
            // --- CASO 2: PELOTA RÁPIDA (REBOTA) ---
            else
            {
                Debug.Log("¡DEMASIADO RÁPIDO! Velocidad: " + velocidadActual + ". Rebotando...");

                // --- ¡ESTAS SON LAS LÍNEAS NUEVAS! ---
                // 1. Calcula el vector desde el CENTRO DEL HUECO hacia la PELOTA
                Vector3 direccionImpacto = other.transform.position - this.transform.position;
                
                // 2. Aplanamos el vector (solo nos importa la dirección horizontal, no la altura)
                direccionImpacto.y = 0;

                // 3. Esta es la nueva dirección de rebote (la flecha "hacia afuera")
                Vector3 direccionRebote = direccionImpacto.normalized;
                // --- FIN DE LAS LÍNEAS NUEVAS ---

                // Detenemos la pelota un instante para un rebote limpio
                pelotaRb.linearVelocity = Vector3.zero;
                pelotaRb.angularVelocity = Vector3.zero;

                // Calculamos la fuerza total del rebote (hacia atrás y hacia arriba)
                Vector3 fuerzaTotalRebote = (direccionRebote * fuerzaDeRebote) + (Vector3.up * fuerzaSaltoRebote);
                
                // Aplicamos la "patada" (el rebote)
                pelotaRb.AddForce(fuerzaTotalRebote, ForceMode.Impulse);
            }
        }
    }
}