using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSalto : MonoBehaviour
{
    // Asegúrate de que aquí diga el nombre exacto de tu escena de menú
    public string nombreEscenaACargar = "MenuPrincipal";

    private bool escenaYaCargada = false;

    void Start()
    {
        // El script buscará el componente VideoPlayer en el mismo objeto
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();

        // Le decimos al script que nos avise cuando el video termine
        videoPlayer.loopPointReached += FinDelVideo;
    }

    void Update()
    {
        // Cada fotograma, revisa si se presiona la tecla Espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CargarSiguienteEscena();
        }
    }

    // Esta función se llama sola cuando el video termina
    void FinDelVideo(VideoPlayer vp)
    {
        CargarSiguienteEscena();
    }

    public void CargarSiguienteEscena()
    {
        if (!escenaYaCargada)
        {
            escenaYaCargada = true; // Evita que se cargue la escena dos veces
            SceneManager.LoadScene(nombreEscenaACargar);
        }
    }
}