using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    [Header("Elements")]
    private PathCreator pathCreator;

    [Header("Data")]
    private float distanceTraveled;
    public Ball ball;

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
            distanceTraveled += GameProperties.ballSlotsSpeed * Time.deltaTime;

            if (distanceTraveled > pathCreator.path.length)
            {
                distanceTraveled = 0;
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
