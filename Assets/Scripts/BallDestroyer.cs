using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BallSlot>())
        {
            BallSlot ballSlot = other.GetComponent<BallSlot>();

            if (ballSlot.ball)
            {
                ballSlot.ball.state = BallState.Destroying;
                ballSlot.ball = null;
            }
        }
    }
}
