using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float upscaleCounter;
    private float downscaleCounter = 1;
    public BallState state;


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
    }
}
