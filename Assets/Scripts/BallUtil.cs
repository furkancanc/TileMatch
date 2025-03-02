using System;
public static class BallUtil
{
    public static BallType GetColorByType(BallType type)
    {
        switch (type)
        {
            case BallType.Red:
                return BallType.Red;
            case BallType.Green:
                return BallType.Green;
            case BallType.Blue:
                return BallType.Blue;
            case BallType.Bomb:
                return BallType.Red;
            case BallType.Reverse:
                return BallType.Blue;
            case BallType.TimeSlow:
                return BallType.Green;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
