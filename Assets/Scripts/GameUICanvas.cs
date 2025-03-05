using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUICanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTime;
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameUIPanel;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        gameUIPanel.SetActive(true);
    }

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

    public void ShowGameOver()
    {
        gameUIPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
