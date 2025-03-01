using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Ball ballPrefab;

    public Ball CreateBallAt(Vector3 point)
    {
        Ball ball = Instantiate(ballPrefab, point, Quaternion.identity);
        return ball;
    }
}
