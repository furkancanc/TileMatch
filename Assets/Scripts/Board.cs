using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallSlot ballSlotPrefab; 
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private Transform ballSlotContainer;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private BallFactory ballFactory; 

    private BallSlot[] ballSlots;

    [Header("Settings")]
    public bool isDestroyingMatchingBalls { get; private set; }
    public bool isReverse { get; private set; }

    private void Start()
    {
        InitializeBallSlots();
    }

    private void Update()
    {
        ProduceBallsOnTrack();
    }

    private void ProduceBallsOnTrack()
    {
        if (isReverse) return;

        BallSlot zeroSlot = BallSlotsByDistance[0];
        if (zeroSlot.ball)
        {
            return;
        }

        Ball ball = ballFactory.CreateBallAt(zeroSlot.transform.position, ballFactory.GetRandomBallType());
        zeroSlot.AssignBall(ball);
        ball.transform.parent = zeroSlot.transform;
        ball.transform.localScale = Vector3.zero;
        ball.state = BallState.Spawning;
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

    public void LandBall(BallSlot collidedSlot, Ball landingBall)
    {
        BallSlot[] ballSlotsByDistance = BallSlotsByDistance;
        int indexOfCollidedSlot = Array.IndexOf(ballSlotsByDistance, collidedSlot);
        int firstEmptySlotIndexAfter = FirstEmptySlotIndexAfter(indexOfCollidedSlot, ballSlotsByDistance);

        for (int i = firstEmptySlotIndexAfter; i > indexOfCollidedSlot + 1; --i)
        {
            ballSlotsByDistance[i].AssignBall(ballSlotsByDistance[i - 1].ball);
        }

        if (collidedSlot.GetDistanceTraveled() < 
            pathCreator.path.GetClosestDistanceAlongPath(landingBall.transform.position))
        {
            ballSlotsByDistance[indexOfCollidedSlot + 1].AssignBall(landingBall);
        }
        else
        {
            ballSlotsByDistance[indexOfCollidedSlot + 1].AssignBall(collidedSlot.ball);
            collidedSlot.AssignBall(landingBall);
        }

        landingBall.Land();

        foreach (BallSlot ballSlot in ballSlotsByDistance
            .Where(bs => bs.ball && bs.ball.state == BallState.InSlot))
        {
            ballSlot.ball.MoveToSlot();
        }

        StartCoroutine(DestroyMatchingBallsCo(landingBall.slot));
    }

    private IEnumerator DestroyMatchingBallsCo(BallSlot landedBallSlot)
    {
        isDestroyingMatchingBalls = true;

        List<BallSlot> ballsToDestroySlots;
        BallSlot collidedBallSlot = landedBallSlot;

        do
        {
            yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
            !bs.ball || bs.ball.state != BallState.Landing && bs.ball.state != BallState.SwitchingSlots));

            ballsToDestroySlots = GetSimilarBalls(landedBallSlot);

            if (ballsToDestroySlots.Count < 3)
            {
                break;
            }

            AddBalsIfThereIsBomb(ballsToDestroySlots);
            if (ballsToDestroySlots.FindIndex(bs => bs.ball.type == BallType.Reverse) != -1)
            {
                StartCoroutine(StartReverseCo());
            }

            if (ballsToDestroySlots.FindIndex(bs => bs.ball.type == BallType.TimeSlow) != -1)
            {
                StartCoroutine(TimeSlowCo());
            }

            DestroyAllBallsInList(ballsToDestroySlots);

            collidedBallSlot = ballsToDestroySlots[0];

            yield return new WaitForSeconds(.5f);
            MoveSeperatedBallsBack();

        } while (ballsToDestroySlots.Count >= 3 && landedBallSlot);

        yield return new WaitUntil(() => BallSlotsByDistance
        .All(bs => !bs.ball || bs.ball.state != BallState.SwitchingSlots));

        isDestroyingMatchingBalls = false;
    }

    private void DestroyAllBallsInList(List<BallSlot> ballsToDestroySlots)
    {
        foreach (BallSlot ballsToDestroySlot in ballsToDestroySlots)
        {
            ballsToDestroySlot.ball.UpdateSprite(ballFactory.GetActiveSpriteByType(ballsToDestroySlot.ball.type));
            ballsToDestroySlot.ball.StartDestroying();
            ballsToDestroySlot.AssignBall(null);
        }
    }

    private IEnumerator TimeSlowCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs => 
        isDestroyingMatchingBalls == false 
        && isReverse == false 
        &&(!bs.ball
            || bs.ball.state != BallState.Landing
            && bs.ball.state != BallState.SwitchingSlots)));

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = .5f;
        }

        yield return new WaitForSeconds(GameProperties.timeSlowDuration);

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = 1;
        }

    }

    private IEnumerator StartReverseCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs => isDestroyingMatchingBalls == false && (!bs.ball 
        || bs.ball.state != BallState.Landing
        && bs.ball.state != BallState.SwitchingSlots)));

        isReverse = true;

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = -1;
        }

        yield return new WaitForSeconds(GameProperties.reverseDuration);

        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = 1;
        }

        isReverse = false;
    }

    private void AddBalsIfThereIsBomb(List<BallSlot> ballsToDestroySlots)
    {
        BallSlot ballSlot = ballsToDestroySlots.FirstOrDefault(bs => bs.ball.type == BallType.Bomb);
        if (ballSlot)
        {
            int indexOfBombSlot = Array.IndexOf(BallSlotsByDistance, ballSlot);
            for (int i = 1; i <= GameProperties.bombRadius; ++i)
            {
                int leftIndex = indexOfBombSlot - i;
                int rightIndex = indexOfBombSlot + i;
                if (leftIndex >= 0 && BallSlotsByDistance[leftIndex].ball &&
                    !ballsToDestroySlots.Contains(BallSlotsByDistance[leftIndex]))
                {
                    ballsToDestroySlots.Add(BallSlotsByDistance[leftIndex]);
                }

                if (rightIndex < BallSlotsByDistance.Length && BallSlotsByDistance[rightIndex].ball &&
                    !ballsToDestroySlots.Contains(BallSlotsByDistance[rightIndex]))
                {
                    ballsToDestroySlots.Add(BallSlotsByDistance[rightIndex]);
                }
            }
        }
    }

    private void MoveSeperatedBallsBack()
    {
        int firstEmptyIndex = Array.FindIndex(BallSlotsByDistance, 1, bs => !bs.ball);
        int firstNonEmptyIndexAfter = Array.FindIndex(BallSlotsByDistance, firstEmptyIndex, bs => bs.ball);
        int emptySlotsCount = firstNonEmptyIndexAfter - firstEmptyIndex;

        if (firstNonEmptyIndexAfter == -1 || firstEmptyIndex == -1)
        {
            return;
        }

        for (int i = firstEmptyIndex; i < BallSlotsByDistance.Length - emptySlotsCount; ++i)
        {
            BallSlotsByDistance[i].AssignBall(BallSlotsByDistance[i + emptySlotsCount].ball);
            if (BallSlotsByDistance[i].ball)
            {
                BallSlotsByDistance[i].ball.MoveToSlot();
            }
        }

        for (int i = BallSlotsByDistance.Length - emptySlotsCount; i < BallSlotsByDistance.Length; ++i)
        {
            BallSlotsByDistance[i].AssignBall(null);
        }
    }

    private List<BallSlot> GetSimilarBalls(BallSlot landedBallSlot)
    {
        List<BallSlot> ballsToDestroySlots = new List<BallSlot> { landedBallSlot };
        int indexOfLandedBallSlot = Array.IndexOf(BallSlotsByDistance, landedBallSlot);

        if (!landedBallSlot.ball)
        {
            return ballsToDestroySlots;
        }

        for (int i = indexOfLandedBallSlot - 1; i >= 0; i--)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot)
                && BallUtil.GetColorByType(ballSlot.ball.type) == 
                BallUtil.GetColorByType(landedBallSlot.ball.type))
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        for (int i = indexOfLandedBallSlot + 1; i < BallSlotsByDistance.Length; i++)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot) 
                && BallUtil.GetColorByType(ballSlot.ball.type) == 
                BallUtil.GetColorByType(landedBallSlot.ball.type))
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        return ballsToDestroySlots.OrderBy(bs => bs.GetDistanceTraveled()).ToList();
    }

    private int FirstEmptySlotIndexAfter(int indexOfCollidedSlot, BallSlot[] ballSlotsByDistance)
    {
        for (int i = indexOfCollidedSlot; i < ballSlotsByDistance.Length; ++i)
        {
            if (!ballSlotsByDistance[i].ball)
            {
                return i;
            }
        }

        return -1;
    }

    private BallSlot[] BallSlotsByDistance => ballSlots.OrderBy(bs => bs.GetDistanceTraveled()).ToArray();
}
