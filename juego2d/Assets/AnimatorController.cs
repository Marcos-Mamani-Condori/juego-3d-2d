using UnityEngine;

public class animatorcontroller : MonoBehaviour
{
    public Animator anim;

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
}
