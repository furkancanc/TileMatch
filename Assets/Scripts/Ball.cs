using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isSpawning;
    private float upscaleCounter;

    private void Update()
    {
        if (isSpawning)
        {
            upscaleCounter += GameProperties.ballUpscaleSpeed * Time.deltaTime;
            if (upscaleCounter >= 1)
            {
                isSpawning = false;
                return;
            }

            transform.localScale = Vector3.one * upscaleCounter;
        }
    }
}
