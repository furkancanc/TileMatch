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
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite blueSprite;

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

        return CreateBallAt(point, GetRandomBallType());
    }

    private BallType GetRandomBallType()
    {
        return Colors[Random.Range(0, Colors.Length)];
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
            default:
                return blueSprite;
        }
    }
}
