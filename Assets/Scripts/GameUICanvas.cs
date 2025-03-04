using TMPro;
using UnityEngine;

public class GameUICanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTime;
    [SerializeField] private TextMeshProUGUI levelNumber;

    public void UpdateLevelTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        levelTime.text = "" + minutes + ":" + seconds.ToString("D2");
    }

    public void UpdateLevelNumber(int level)
    {
        levelNumber.text = "Level " + level;
    }
}
