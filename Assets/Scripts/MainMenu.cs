using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject confirmationPanel;

    private void Start()
    {
        audioManager.PlayMenuMusic();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartNewGame()
    {
        confirmationPanel.SetActive(true);
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
    }
}
