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

    // Start is called before the first frame update
    void Start()
    {
        GridHelper helper = GameObject.Find("GridHelper").GetComponent<GridHelper>();
        HasMine = (Random.value < helper.MineWeight);
        int x = (int)this.transform.position.x;
        int y = (int)this.transform.parent.transform.position.y;
        //asigno la posicion en la grilla
        GridHelper.cells[x, y] = this;
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
    public void ReloadMines()
    {
        Start();
    }
    //metodo para saber si la celda esta o no tapada
    public bool IsCovered()
    {
        return State == CellState.Covered;
    }
    //metodo que se llama al hacer click en una celda
    private void OnMouseUpAsButton()
    {
        if (GameManager.SharedInstance.CurrentGameState == GameState.InGame && IsCovered())
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
                //cambiar la textura de la celda
                int x = (int)this.transform.position.x;
                int y = (int)this.transform.parent.transform.position.y;
                //descubrir toda el area sin minas de la celda abierta
                GridHelper.FloodFillUncover(x, y, new bool[GridHelper.w, GridHelper.h]);
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
