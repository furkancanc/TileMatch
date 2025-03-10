using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameUICanvas gameUICanvas;

    private void Start()
    {
        gameUICanvas.UpdateLevelNumber(GameProperties.LastLevel);
        // gameUICanvas.UpdateLevelTime(levelTime);
    }
}
