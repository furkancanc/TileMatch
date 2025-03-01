using PathCreation;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallSlot ballSlotPrefab; 
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private Transform ballSlotContainer;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private BallFactory BallFactory; 

    private BallSlot[] ballSlots;

    private void Start()
    {
        InitializeBallSlots();
    }

    private void Update()
    {
        BallSlot zeroSlot = ballSlots.OrderBy(bs => bs.GetDistanceTraveled()).ToArray()[0];
        if (!zeroSlot.ball)
        {
            Ball ball = BallFactory.CreateRandomBallAt(zeroSlot.transform.position);
            zeroSlot.ball = ball;
            ball.transform.parent = zeroSlot.transform;
            ball.transform.localScale = Vector3.zero;
            ball.state = BallState.Spawning;
        }
    }

    private void InitializeBallSlots()
    {
        float pathLength = pathCreator.path.length;
        int slotsCount = (int)pathLength;
        float step = pathLength / slotsCount;

        ballSlots = new BallSlot[slotsCount];

        for (int i = 0; i < slotsCount; ++i)
        {
            float distanceTraveled = i * step;

            Vector3 slotPosition = pathCreator.path.GetPointAtDistance(i);
            BallSlot ballSlot = Instantiate(ballSlotPrefab, slotPosition, Quaternion.identity, ballSlotContainer);
            ballSlot.SetDistanceTraveled(distanceTraveled);
            ballSlots[i] = ballSlot;
        }
    }
}
