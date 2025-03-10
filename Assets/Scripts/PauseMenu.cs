using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Board board;

    [Header("Data")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameUIPanel;
    
    private void Start()
    {
        pausePanel.SetActive(false);
    }
    public void GoToMainMenu()
    {
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void Pause()
    {
        board.isPaused = true;

        Time.timeScale = 0;

        gameUIPanel.SetActive(false);
        pausePanel.SetActive(true);

    }

    public void UnPause()
    {
        StartCoroutine(UnpauseCo());
    }

    private IEnumerator UnpauseCo()
    {
        yield return new WaitForSecondsRealtime(1);

        board.isPaused = false;
        Time.timeScale = 1;

        pausePanel.SetActive(false);
        gameUIPanel.SetActive(true);
    }
}
