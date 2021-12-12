using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //variable que hace referencia al manager
    public static GameManager SharedInstance;
    //variable para indicar en que estado se encuentra el juego
    public GameState CurrentGameState = GameState.Menu;

    private void Awake()
    {
        SharedInstance = this;
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
}
