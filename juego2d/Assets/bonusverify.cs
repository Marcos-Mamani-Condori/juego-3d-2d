using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MetaNivel : MonoBehaviour
{
    public int monedasObjetivo;                  
    public PlayerController jugador;             
    public GameObject mensajePanel;              
    public TextMeshProUGUI mensajeText;          
    public TextMeshProUGUI objetivoHUDText;      
    public float tiempoMensaje = 3f;             
    public Button botonSiguienteNivel;           

    private bool puedePasarNivel = false;        

    private void Start()
    {
        if (mensajePanel != null)
            mensajePanel.SetActive(false);

        if (objetivoHUDText != null)
            objetivoHUDText.text = $"Objetivo: {monedasObjetivo} ";

        if (botonSiguienteNivel != null)
            botonSiguienteNivel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown && botonSiguienteNivel != null && puedePasarNivel)
        {
            botonSiguienteNivel.onClick.Invoke();
            puedePasarNivel = false; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (jugador != null && mensajePanel != null && mensajeText != null)
        {
            int scoreActual = jugador.GetScore();

            if (scoreActual >= monedasObjetivo)
            {
                puedePasarNivel = true; 

                if (botonSiguienteNivel != null)
                    botonSiguienteNivel.gameObject.SetActive(true);

                StartCoroutine(PasarNivel("¡Muy bien! Pasaste de nivel", tiempoMensaje));
            }
            else
            {
                int faltan = monedasObjetivo - scoreActual;
                StartCoroutine(MostrarMensaje($"Faltan {faltan} monedas para pasar de nivel", tiempoMensaje));
            }
        }
    }

    private IEnumerator MostrarMensaje(string texto, float tiempo)
    {
        mensajeText.text = texto;
        mensajePanel.SetActive(true);
        yield return new WaitForSeconds(tiempo);
        mensajePanel.SetActive(false);
    }

    private IEnumerator PasarNivel(string texto, float tiempo)
    {
        mensajeText.text = texto;
        mensajePanel.SetActive(true);
        yield return new WaitForSeconds(tiempo);

        if (puedePasarNivel && botonSiguienteNivel != null)
        {
            botonSiguienteNivel.onClick.Invoke();
            puedePasarNivel = false;
        }
    }
}
