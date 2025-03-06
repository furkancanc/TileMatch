using System.Collections.Generic;
using UnityEngine;

public class BallFactory : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Ball ballPrefab;

    [Header("Settings")]
    private static readonly BallType[] Colors = new[]
    {
        BallType.Red,
        BallType.Green,
        BallType.Blue
    };
    private static readonly BallType[] SpecialTypes = new[]
    {
        BallType.Bomb,
        BallType.Reverse,
        BallType.TimeSlow
    };

    private readonly Stack<BallType> spawningStack = new Stack<BallType>();

    [Header("Data")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite reverseSprite;
    [SerializeField] private Sprite timeSlowSprite;
    
    [SerializeField] private Sprite activeRedSprite;
    [SerializeField] private Sprite activeGreenSprite;
    [SerializeField] private Sprite activeBlueSprite;
    [SerializeField] private Sprite activeBombSprite;
    [SerializeField] private Sprite activeReverseSprite;
    [SerializeField] private Sprite activeTimeSlowSprite;


    public Ball CreateBallAt(Vector3 point, BallType ballType)
    {
        Ball ball = Instantiate(ballPrefab, point, Quaternion.identity);
        ball.type = ballType;
        SpriteRenderer spriteRenderer = ball.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.sprite = GetSpriteByType(ballType);
        }
        return ball;
    }

    public Ball CreateRandomBallAt(Vector3 point)
    {

        return CreateBallAt(point, GetRandomBallColor());
    }

    private BallType GetRandomBallColor()
    {
        return Colors[Random.Range(0, Colors.Length)];
    }

    private BallType GetRandomBallSpecialType()
    {
        return SpecialTypes[Random.Range(0, SpecialTypes.Length)];
    }

    public BallType GetRandomBallType()
    {
        if (spawningStack.Count > 0)
        {
            return spawningStack.Pop();
        }

        return Random.Range(0f, 1f) > 0.2f ? GetRandomBallColor() : GetRandomBallSpecialType();
    }

    private Sprite GetSpriteByType(BallType type)
    {
        switch (type)
        {
            case BallType.Red:
                return redSprite;
            case BallType.Green:
                return greenSprite;
            case BallType.Blue:
                return blueSprite;
            case BallType.Bomb:
                return bombSprite;
            case BallType.Reverse:
                return reverseSprite;
            case BallType.TimeSlow:
                return timeSlowSprite;
            default:
                return blueSprite;
        }
    }

    public Sprite GetActiveSpriteByType(BallType type)
    {
        switch (type)
        {
            case BallType.Red:
                return activeRedSprite;
            case BallType.Green:
                return activeGreenSprite;
            case BallType.Blue:
                return activeBlueSprite;
            case BallType.Bomb:
                return activeBombSprite;
            case BallType.Reverse:
                return activeReverseSprite;
            case BallType.TimeSlow:
                return activeTimeSlowSprite;
            default:
                return blueSprite;
        }
    }

    public void AddTypeToStack(BallType type)
    {
        spawningStack.Push(type);
    }
}
