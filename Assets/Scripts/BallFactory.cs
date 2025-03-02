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

    [Header("Data")]
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite reverseSprite;
    [SerializeField] private Sprite timeSlowSprite;


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
}
