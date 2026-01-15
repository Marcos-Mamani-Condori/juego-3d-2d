using UnityEngine;
using UnityEngine.UI;

public class cambiarsprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; 
    public Sprite nuevoSprite;

    public void CambiarImagen()
    {
        spriteRenderer.sprite = nuevoSprite;
    }
}
