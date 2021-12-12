using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool HasMine;
    public Sprite[] EmptyTexture;
    public Sprite MineTexture;
    // Start is called before the first frame update
    void Start()
    {
        HasMine = (Random.value < 0.15);
        int x = (int)this.transform.position.x;
        int y = (int)this.transform.parent.transform.position.y;
        //asigno la posicion en la grilla
        GridHelper.cells[x, y] = this;
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
    //metodo para saber si la celda esta o no tapada
    public bool IsCovered()
    {
        return GetComponent<SpriteRenderer>().sprite.texture.name == "MinesweeoweSpritesheet_19";
    }
    //metodo que se llama al hacer click en una celda
    private void OnMouseUpAsButton()
    {
        if (HasMine)
        {
            //mostrar mensaje de gameover
            GridHelper.UncoverAllTheMines();
            Debug.Log("Hay Mina");
        }
        else
        {
            //cambiar la textura de la celda
            int x = (int)this.transform.position.x;
            int y = (int)this.transform.parent.transform.position.y;
            LoadTexture(GridHelper.CountAdjacentMines(x,y));
            //descubrir toa el area sein minas de la celda abierta
            //comprobar si el juego a terminado o no
        }
    }
}