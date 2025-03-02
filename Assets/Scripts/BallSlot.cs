using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    [Header("Elements")]
    private PathCreator pathCreator;

    [Header("Data")]
    private float distanceTraveled;
    public Ball ball;
    public float speedMultiplier = 1f;

    private void Start()
    {
        pathCreator = Object.FindFirstObjectByType<PathCreator>();
    }

    public void SetDistanceTraveled(float distanceTraveled)
    {
        this.distanceTraveled = distanceTraveled;
    }

    public float GetDistanceTraveled()
    {
        return distanceTraveled;
    }

    private void Update()
    {
        if (pathCreator)
        {
            distanceTraveled += speedMultiplier *  GameProperties.ballSlotsSpeed * Time.deltaTime;

            if (distanceTraveled > pathCreator.path.length)
            {
                distanceTraveled = 0;
            }

            if (speedMultiplier < 0 && distanceTraveled < 1f && ball)
            {
                if (distanceTraveled < .5f)
                {
                    Destroy(ball.gameObject);
                }
                else
                {
                    ball.StartDestroying();
                }

                AssignBall(null);
            }

            if (distanceTraveled < 0)
            {
                distanceTraveled = pathCreator.path.length;
            }

            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
        }
    }

    public void AssignBall(Ball newBall)
    {
        ball = newBall;

        if (ball)
        {
            ball.slot = this;
        }
    }
}
