using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

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
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
