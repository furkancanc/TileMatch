using UnityEngine;

public class GameProperties : MonoBehaviour
{
    private void Awake()
    {
        lastLevel = PlayerPrefs.GetInt(KeyLastLevel, 1);    
    }

    private const string KeyLastLevel = "LAST_LEVEL";

    public static float ballUpscaleSpeed = 2f;
    public static float ballDownslaceSpeed = 2f;
    public static float ballSlotsSpeed = 4f;
    public static float ballShootingSpeed = 20f;
    public static float ballLandingSpeed = 5f;
    public static float ballSlotSwitchingSpeed = 5f;
    public static float reverseDuration = 2f;
    public static float timeSlowDuration = 2f;
    public static float levelDurationSeconds = 10f;
    public static float slotSpeedUpPerLevel = .1f;


    public static int bombRadius = 1;

    private static int lastLevel = 1;

    public static int LastLevel => lastLevel;

    public static void IncrementLastLevel()
    {
        ++lastLevel;
        PlayerPrefs.SetInt(KeyLastLevel, lastLevel);
    }
    public static float GetSlotSpeedMultiplier(float effectMultiplier)
    {
        return effectMultiplier * (1 + lastLevel * slotSpeedUpPerLevel);
    }

    public static void ResetLastLevel()
    {
        lastLevel = 1;
        PlayerPrefs.SetInt(KeyLastLevel, LastLevel);
    }
}
