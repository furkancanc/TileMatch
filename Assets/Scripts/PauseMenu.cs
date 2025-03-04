using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Board board;
    private void Start()
    {
        pausePanel.SetActive(false);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Pause()
    {
        board.isPaused = true;
        Time.timeScale = 0;
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
    }
}
