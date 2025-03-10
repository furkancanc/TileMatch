using UnityEngine;

public class BallDestroyer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Board board;

    private void OnTriggerEnter2D(Collider2D other)
    {
        BallSlot ballSlot = other.GetComponent<BallSlot>();
        if (!ballSlot || !ballSlot.ball)
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
