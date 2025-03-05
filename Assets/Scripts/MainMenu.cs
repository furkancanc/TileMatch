using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        audioManager.PlayMenuMusic();
        confirmationPanel.SetActive(false);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartNewGame()
    {
        confirmationPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ConfirmNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
