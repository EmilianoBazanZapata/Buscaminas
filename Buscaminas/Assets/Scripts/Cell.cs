using UnityEngine;

public enum CellState
{
    Covered = 0,
    Revealed = 1
}

public class Cell : MonoBehaviour
{
    public bool HasMine;
    public Sprite[] EmptyTexture;
    public Sprite MineTexture;
    public Sprite Panel;

    public CellState State { get; private set; } = CellState.Covered;

    private int _x;
    private int _y;
    private bool _isInitialized;

    public void Initialize(int x, int y, bool hasMine)
    {
        _isInitialized = false;
        _x = x;
        _y = y;
        HasMine = hasMine;
        RestartTexture();
        _isInitialized = true;
    }
    public void Reveal(int AdjacentCount)
    {
        if (!IsCovered())
        {
            return;
        }

        State = CellState.Revealed;
        LoadTexture(AdjacentCount);
    }

    public void LoadTexture(int AdjacentCount)
    {
        if (HasMine)
        {
            GetComponent<SpriteRenderer>().sprite = MineTexture;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = EmptyTexture[AdjacentCount];
        }
    }
    public void RestartTexture()
    {
        State = CellState.Covered;
        GetComponent<SpriteRenderer>().sprite = Panel;
    }
    //metodo para saber si la celda esta o no tapada
    public bool IsCovered()
    {
        return State == CellState.Covered;
    }
    //metodo que se llama al hacer click en una celda
    private void OnMouseUpAsButton()
    {
        if (_isInitialized && GridHelper.IsInitialized && GameManager.SharedInstance != null &&
            GameManager.SharedInstance.CurrentGameState == GameState.InGame && IsCovered())
        {
            if (HasMine)
            {
                //mostrar mensaje de gameover
                GridHelper.UncoverAllTheMines();
                GameManager.SharedInstance.GameOver();
                //Debug.Log("Hay Mina");
            }
            else
            {
                //descubrir toda el area sin minas de la celda abierta
                GridHelper.FloodFillUncover(_x, _y, new bool[GridHelper.w, GridHelper.h]);
                //comprobar si el juego a terminado o no
                if (GridHelper.HasTheGameEnded())
                {
                    GameManager.SharedInstance.WinGame();
                    Debug.Log("el juego ha terminado");
                }
            }
        }
    }
}
