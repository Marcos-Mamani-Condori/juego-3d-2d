using UnityEngine;
using System.IO;

public class OpenHistoria : MonoBehaviour
{
    public void AbrirHistoria()
    {
        string ruta = Path.Combine(Application.streamingAssetsPath, "historia.html");

        string url = "file:///" + ruta.Replace("\\", "/");
        Application.OpenURL(url);
    }
}
