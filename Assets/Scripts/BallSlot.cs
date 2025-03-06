using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    [Header("Elements")]
    private PathCreator pathCreator;
    private BallFactory ballFactory;

    [Header("Data")]
    private float distanceTraveled;
    public Ball ball;
    public float speedMultiplier = 1f;

    private void Start()
    {
        pathCreator = Object.FindFirstObjectByType<PathCreator>();
        ballFactory = Object.FindFirstObjectByType<BallFactory>();
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
            distanceTraveled += speedMultiplier * GameProperties.ballSlotsSpeed * Time.deltaTime;


            if (speedMultiplier < 0 && distanceTraveled < 1f && ball)
            {
                ballFactory.AddTypeToStack(ball.type);
                DestroyBall(distanceTraveled < 0.5);
            }

            TrimDistanceTraveled();

            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);

            LookOppositePathDirection();
        }
    }

    private void LookOppositePathDirection()
    {
        Vector3 bPos = pathCreator.path.GetPointAtDistance(distanceTraveled);
        Vector3 aPos = pathCreator.path.GetPointAtDistance(distanceTraveled - .1f);
        Vector3 lookDirection = aPos - bPos;
        transform.up = new Vector2(lookDirection.x, lookDirection.y);
    }

    private void TrimDistanceTraveled()
    {
        if (distanceTraveled > pathCreator.path.length)
        {
            distanceTraveled = 0;
        }

        else if (distanceTraveled < 0)
        {
            distanceTraveled = pathCreator.path.length;
        }
    }

    private void DestroyBall(bool immediately)
    {
        if (immediately)
        {
            Destroy(ball.gameObject);
        }
        else
        {
            ball.StartDestroying();
        }

        AssignBall(null);
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
