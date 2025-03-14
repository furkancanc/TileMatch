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
    [SerializeField] private Shooter shooter;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameUICanvas gameUICanvas;

    private BallSlot[] ballSlots;
    private float levelTime;

    public bool isGameOver;

    [Header("Settings")]
    public bool isDestroyingMatchingBalls { get; private set; }
    public bool isReverse { get; private set; }

    public bool isPaused;

    private void Awake()
    {
        Ball.onBallCollided += LandBall;
    }

    private void OnDestroy()
    {
        Ball.onBallCollided -= LandBall;
    }
    private void Start()
    {
        InitializeBallSlots();
        Time.timeScale = 1;

    }

    private void Update()
    {
        if (!isGameOver)
        {
            levelTime += Time.deltaTime;
            gameUICanvas.UpdateLevelTime(levelTime);

            if (levelTime >= GameProperties.levelDurationSeconds)
            {
                GameProperties.IncrementLastLevel();
                gameUICanvas.UpdateLevelNumber(GameProperties.LastLevel);
                levelTime = 0;
                DestroyAllBallsInList(BallSlotsByDistance.Where(bs => bs.ball).ToList());
            }
        }

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
        ball.state = BallState.SpawningOnTrack;
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
            ballSlot.transform.parent = ballSlotContainer.transform;
            ballSlot.speedMultiplier = GameProperties.GetSlotSpeedMultiplier(1f);
            ballSlots[i] = ballSlot;
        }
    }

    public void LandBall(BallSlot collidedSlot, Ball landingBall)
    {
        audioManager.PlaySfx(0);

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
        shooter.isShooterDisabledFromOutside = true;

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
        shooter.isShooterDisabledFromOutside = false;
    }

    private void DestroyAllBallsInList(List<BallSlot> ballsToDestroySlots)
    {
        foreach (BallSlot ballsToDestroySlot in ballsToDestroySlots)
        {
            ballsToDestroySlot.ball.UpdateSprite(ballFactory.GetActiveSpriteByType(ballsToDestroySlot.ball.type));
            ballsToDestroySlot.ball.StartDestroying();
            ballsToDestroySlot.AssignBall(null);
        }
        audioManager.PlaySfx(1);
    }

    private IEnumerator TimeSlowCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs => 
        isDestroyingMatchingBalls == false 
        && isReverse == false 
        &&(!bs.ball
            || bs.ball.state != BallState.Landing
            && bs.ball.state != BallState.SwitchingSlots)));

        SpeedUpSlots(.5f);

        yield return new WaitForSeconds(GameProperties.timeSlowDuration);

        SpeedUpSlots(1f);
    }


    private IEnumerator StartReverseCo()
    {
        yield return new WaitUntil(() => BallSlotsByDistance.All(bs => isDestroyingMatchingBalls == false && (!bs.ball
        || bs.ball.state != BallState.Landing
        && bs.ball.state != BallState.SwitchingSlots)));

        isReverse = true;
        shooter.isShooterDisabledFromOutside = true;

        SpeedUpSlots(-1f);

        yield return new WaitForSeconds(GameProperties.reverseDuration);

        SpeedUpSlots(1f);

        isReverse = false;
        shooter.isShooterDisabledFromOutside = false;
    }

    private void SpeedUpSlots(float effectMultiplier)
    {
        foreach (BallSlot ballSlot in ballSlots)
        {
            ballSlot.speedMultiplier = GameProperties.GetSlotSpeedMultiplier(effectMultiplier);
        }
    }

    private void AddBalsIfThereIsBomb(List<BallSlot> ballsToDestroySlots)
    {
        List<BallSlot> ballSlots = ballsToDestroySlots.Where(bs => bs.ball.type == BallType.Bomb).ToList();
        foreach (BallSlot bombSlot in ballSlots)
        {
            if (bombSlot)
            {
                int indexOfBombSlot = Array.IndexOf(BallSlotsByDistance, bombSlot);
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

    public void GameOver()
    {
        StartCoroutine(GameOverCo());
    }

    private IEnumerator GameOverCo()
    {
        shooter.isShooterDisabledFromOutside = true;
        isGameOver = true;
        SpeedUpSlots(1.5f);
        yield return new WaitForSeconds(2);
        gameUICanvas.ShowGameOver();
    }

    private BallSlot[] BallSlotsByDistance => ballSlots.OrderBy(bs => bs.GetDistanceTraveled()).ToArray();
}
