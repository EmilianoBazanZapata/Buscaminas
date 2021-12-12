using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper : MonoBehaviour
{
    public static int w = 21;
    public static int h = 21;
    //gusrdo cuantas celdas hay en total
    public static Cell[,] cells = new Cell[w, h];
    //revelamos la posicion de todas las minas al perder
    public static void UncoverAllTheMines()
    {
        foreach (Cell c in cells)
        {
            if (c.HasMine)
            {
                c.LoadTexture(0);
            }
        }
    }
    public static bool HasMineAt(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < w && y < h)
        {
            //estoy dentro de la parrilla
            Cell c = cells[x, y];
            return c.HasMine;
        }
        else
        {
            //estoy fuera d ela parrilla
            return false;
        }
    }
    public static int CountAdjacentMines(int x, int y)
    {
        int Count = 0;
        if (HasMineAt(x - 1, y - 1)) Count++;//abajo-izquierda
        if (HasMineAt(x - 1, y)) Count++;//abajo-centro
        if (HasMineAt(x - 1, y + 1)) Count++;//abajo-derecha
        if (HasMineAt(x, y + 1)) Count++;//medio-izquierda
        if (HasMineAt(x, y - 1)) Count++;//medio-derecha
        if (HasMineAt(x + 1, y - 1)) Count++;//arriba-izquierda
        if (HasMineAt(x + 1, y)) Count++;//arriba-centro
        if (HasMineAt(x + 1, y + 1)) Count++;//arriba-derecha
        return Count;
    }
    public static void FloodFillUncover(int x, int y, bool[,] visited)
    {
        //solo debemos proceder si el punto (x,y) es valida

        if (x >= 0 && y >= 0 && x < w && y < h)
        {
            //si ya pase por la celda el algoritmo de flood fill no debera hacer nada
            if (visited[x, y])
            {
                return;
            }
            //cuento la cantidad de minsas adjacentes
            int AdjacentMines = CountAdjacentMines(x, y);
            //muesto el numero de minas
            cells[x, y].LoadTexture(AdjacentMines);
            //verifico si tengo minas para saber si puedo destapar la celda
            if (AdjacentMines > 0)
            {
                return;
            }
            //marco como visitada a la celda
            visited[x, y] = true;
            //visito todos los vecinos
            FloodFillUncover(x - 1, y, visited);//izquierda
            FloodFillUncover(x + 1, y, visited);//derecha
            FloodFillUncover(x, y - 1, visited);//abajo
            FloodFillUncover(x, y + 1, visited);//arriba
        }
    }
}