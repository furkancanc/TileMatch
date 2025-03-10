
using PathCreation;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private BallFactory ballFactory;
    [SerializeField] private BallSlot ballSlotPrefab;
    [SerializeField] private Transform ballSlotsContainer;

    [Header("Data")]
    private BallSlot[] ballSlots;
    private bool isReverse;

    private void Start()
    {
        InitializeBallSlots();
    }

    private void InitializeBallSlots()
    {
        float pathLength    = pathCreator.path.length;
        int slotsCount      = (int)pathLength;
        float step          = pathLength / slotsCount;
        ballSlots           = new BallSlot[slotsCount];

        for (int i = 0; i < slotsCount; ++i)
        {
            float distanceTraveled = i * step;
            Vector3 slotPosition = pathCreator.path.GetPointAtDistance(distanceTraveled);
            BallSlot ballSlot = Instantiate(ballSlotPrefab, slotPosition, Quaternion.identity);
            ballSlot.SetDistanceTraveled(distanceTraveled);
            ballSlot.transform.parent = ballSlotsContainer.transform;
            ballSlots[i] = ballSlot;
        }
    }

    private void Update()
    {
        ProduceBallsOnTrack();
    }

    private void ProduceBallsOnTrack()
    {
        if (isReverse)
        {
            return;
        }

        BallSlot zeroSlot = BallSlotsByDistance[0];
        if (!zeroSlot.ball)
        {
            Ball ball = ballFactory.CreateBallAt(zeroSlot.transform.position, ballFactory.GetRandomBallType());
            zeroSlot.AssignBall(ball);
            ball.transform.parent = zeroSlot.transform;
            ball.transform.localScale = Vector3.zero;
            ball.state = BallState.SpawningOnTrack;
        }
    }

    public BallSlot[] BallSlotsByDistance => ballSlots.OrderBy(bs => bs.GetDistanceTraveled()).ToArray();
}
