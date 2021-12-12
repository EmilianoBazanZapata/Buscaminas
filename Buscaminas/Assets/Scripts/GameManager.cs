using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//enumerado
//posibles estado del juego
public enum GameState
{
    InGame,
    GameOver,
    Menu
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
        SetGameState(GameState.InGame);
    }
    //reiniciar jeugo
    public void RestartGame()
    {
        GridHelper.RestartCells();
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
                GameOverCanvas.enabled = false;
                break;
            case GameState.InGame:
                MenuCanvas.enabled = false;
                GameCanvas.enabled = true;
                GameOverCanvas.enabled = false;
                break;
            case GameState.GameOver:
                MenuCanvas.enabled = false;
                GameCanvas.enabled = false;
                GameOverCanvas.enabled = true;
                break;
        }
        //asignamos el estado de juego actual desde el parametro
        this.CurrentGameState = NewGameState;
    }
}
