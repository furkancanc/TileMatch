using NUnit.Framework;
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
    [SerializeField] private BallFactory BallFactory; 

    private BallSlot[] ballSlots;

    private void Start()
    {
        InitializeBallSlots();
    }

    private void Update()
    {
        BallSlot zeroSlot = BallSlotsByDistance[0];
        if (!zeroSlot.ball)
        {
            Ball ball = BallFactory.CreateRandomBallAt(zeroSlot.transform.position);
            zeroSlot.AssignBall(ball);
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

        foreach (BallSlot ballSlot in ballSlotsByDistance.Where(bs => bs.ball && bs.ball.state == BallState.InSlot))
        {
            ballSlot.ball.MoveToSlot();
        }

        StartCoroutine(DestroyMatchingBallsCo(landingBall.slot));
    }

    private IEnumerator DestroyMatchingBallsCo(BallSlot landedBallSlot)
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs =>
            !bs.ball || bs.ball.state != BallState.Landing && bs.ball.state != BallState.SwitchingSlots));
        Debug.Log("Ready To Destroy Matching Balls!");

        List<BallSlot> ballsToDestroySlots = new List<BallSlot>();
        ballsToDestroySlots.Add(landedBallSlot);
        int indexOfLandedBallSlot = Array.IndexOf(BallSlotsByDistance, landedBallSlot);

        for (int i = indexOfLandedBallSlot - 1; i >= 0; i--)
        {
            BallSlot ballSlot = BallSlotsByDistance[i];
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot) && ballSlot.ball.type == landedBallSlot.ball.type)
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
            if (ballSlot.ball && !ballsToDestroySlots.Contains(ballSlot) && ballSlot.ball.type == landedBallSlot.ball.type)
            {
                ballsToDestroySlots.Add(ballSlot);
            }
            else
            {
                break;
            }
        }

        if (ballsToDestroySlots.Count >= 3)
        {
            foreach (BallSlot ballsToDestroySlot in ballsToDestroySlots)
            {
                ballsToDestroySlot.ball.StartDestroying();
                ballsToDestroySlot.AssignBall(null);
            }
        }
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
