// Es MUY IMPORTANTE añadir esta línea para poder manejar escenas.
using UnityEngine.SceneManagement;
using UnityEngine;

public class cambiadorEscena : MonoBehaviour
{
    // Esta función cargará la escena del juego principal.
    public void CargarEscenaJuego()
    {
        // Reemplaza "Nivel1" con el nombre EXACTO de tu escena de juego.
        SceneManager.LoadScene("SampleScene");
    }

    // Esta función cargará la escena de opciones.
    public void CargarPartidaOpciones()
    {
        // Reemplaza "MenuOpciones" con el nombre de tu escena de opciones.
        SceneManager.LoadScene("PartidasGuardadas");
    }

    // Esta función cerrará el juego.
    public void SalirDelJuego()
    {
        // Muestra un mensaje en la consola para saber que funciona.
        Debug.Log("Saliendo del juego...");

        // Esta línea cierra el juego (solo funciona fuera del editor de Unity).
        Application.Quit();
    }
}