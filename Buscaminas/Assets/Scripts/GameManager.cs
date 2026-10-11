using UnityEngine;

public enum GameState
{
    InGame = 0,
    GameOver = 1,
    Menu = 2,
    Victory = 3
}
public class GameManager : MonoBehaviour
{
    public Canvas MenuCanvas;
    public Canvas GameCanvas;
    public Canvas GameOverCanvas;
    public static GameManager SharedInstance;
    public GameState CurrentGameState = GameState.Menu;

    private void Awake()
    {
        SharedInstance = this;
    }
    private void Start()
    {
        BackToMenu();
    }
    public void PlayGame()
    {
        if (!GridHelper.IsInitialized)
        {
            Debug.LogError("Cannot start the game: the board is not initialized. Check the GridHelper board validation errors.", this);
            return;
        }
        SetGameState(GameState.InGame);
    }
    public void RestartGame()
    {
        if (!GridHelper.RestartCells())
        {
            BackToMenu();
            return;
        }
        PlayGame();

    }
    public void BackToMenu()
    {
        SetGameState(GameState.Menu);
    }
    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void WinGame()
    {
        SetGameState(GameState.Victory);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void SetGameState(GameState NewGameState)
    {
        bool showGame = NewGameState == GameState.InGame || NewGameState == GameState.Victory;
        MenuCanvas.enabled = NewGameState == GameState.Menu;
        GameCanvas.enabled = showGame;
        GameCanvas.gameObject.SetActive(showGame);
        GameOverCanvas.enabled = NewGameState == GameState.GameOver;
        this.CurrentGameState = NewGameState;
    }
}
