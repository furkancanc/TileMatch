using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float upscaleCounter;
    private float downscaleCounter = 1;
    public BallState state;
    public BallType type;
    private Vector3 shootDirection;

    private void Update()
    {
        if (state == BallState.Spawning)
        {
            upscaleCounter += GameProperties.ballUpscaleSpeed * Time.deltaTime;
            if (upscaleCounter >= 1)
            {
                state = BallState.InSlot;
                return;
            }

            transform.localScale = Vector3.one * upscaleCounter;
        }
        else if (state == BallState.Destroying)
        {
            downscaleCounter -= GameProperties.ballDownslaceSpeed * Time.deltaTime;
            if (downscaleCounter < 0)
            {
                Destroy(gameObject);
                return;
            }

            transform.localScale = Vector3.one * downscaleCounter;
        }
        else if (state == BallState.Shooting)
        {
            transform.position += shootDirection * (GameProperties.ballShootingSpeed * Time.deltaTime);
        }
    }

    public void Shoot(Vector3 direction)
    {
        shootDirection = direction;
        state = BallState.Shooting;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<BallSlot>())
        {
            BallSlot ballSlot = collision.GetComponent<BallSlot>();

            if (ballSlot.ball && state == BallState.Shooting)
            {
                Debug.Log("Boo!");
            }
        }
    }
}
