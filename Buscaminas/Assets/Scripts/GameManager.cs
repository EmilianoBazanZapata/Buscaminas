using UnityEngine;

//enumerado
//posibles estado del juego
public enum GameState
{
    InGame = 0,
    GameOver = 1,
    Menu = 2,
    Victory = 3
}
public class GameManager : MonoBehaviour
{
    //referencia al canvas del menu
    public Canvas MenuCanvas;
    //referencia al canvas del juego
    public Canvas GameCanvas;
    //referencia al canvas del gameover
    public Canvas GameOverCanvas;
    //variable que hace referencia al manager
    public static GameManager SharedInstance;
    //variable para indicar en que estado se encuentra el juego
    public GameState CurrentGameState = GameState.Menu;

    private void Awake()
    {
        SharedInstance = this;
    }
    private void Start()
    {
        BackToMenu();
    }
    //metodo que llamare para iniciar el juego
    public void PlayGame()
    {
        if (!GridHelper.IsInitialized)
        {
            Debug.LogError("Cannot start the game: the board is not initialized. Check the GridHelper board validation errors.", this);
            return;
        }
        SetGameState(GameState.InGame);
    }
    //reiniciar jeugo
    public void RestartGame()
    {
        if (!GridHelper.RestartCells())
        {
            BackToMenu();
            return;
        }
        PlayGame();

    }
    //metodo que llamara para volver al menu
    public void BackToMenu()
    {
        SetGameState(GameState.Menu);
    }
    //metodo para el game over
    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void WinGame()
    {
        SetGameState(GameState.Victory);
    }
    //metodo para finalizar la aplicacion
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
        switch (NewGameState)
        {
            case GameState.Menu:
                MenuCanvas.enabled = true;
                GameCanvas.enabled = false;
                GameCanvas.gameObject.SetActive(false);
                GameOverCanvas.enabled = false;
                break;
            case GameState.InGame:
            case GameState.Victory:
                MenuCanvas.enabled = false;
                GameCanvas.enabled = true;
                GameCanvas.gameObject.SetActive(true);
                GameOverCanvas.enabled = false;
                break;
            case GameState.GameOver:
                MenuCanvas.enabled = false;
                GameCanvas.enabled = false;
                GameCanvas.gameObject.SetActive(false);
                GameOverCanvas.enabled = true;
                break;
        }
        //asignamos el estado de juego actual desde el parametro
        this.CurrentGameState = NewGameState;
    }
}
