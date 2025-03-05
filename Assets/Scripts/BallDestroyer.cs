using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    [SerializeField] private Board board;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<BallSlot>())
        {
            return;
        }

        BallSlot ballSlot = other.GetComponent<BallSlot>();

        if (!ballSlot.ball)
        {
            return;
        }

        ballSlot.ball.state = BallState.Destroying;
        ballSlot.ball = null;

        if (!board.isGameOver)
        {
            board.GameOver();
        }
    }
}
