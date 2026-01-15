using UnityEngine;

public class CharacterAnimationHelper : MonoBehaviour
{
    public GolfBallController golfBallController;

    public void HitEventFromCharacter()
    {
        if (golfBallController != null)
        {
            golfBallController.ExecuteHitFromAnimation();
        }
    }
}