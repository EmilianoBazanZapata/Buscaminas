using UnityEngine;

public class GridHelper : MonoBehaviour
{
    public static readonly int w = 21;
    public static readonly int h = 21;
    //guardo cuantas celdas hay en total
    public static Cell[,] cells = new Cell[w, h];

    [Range(0.0f, 1.0f)]
    public float MineWeight = 0.15f;
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
        int count = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if ((i != x || j != y) && HasMineAt(i, j))
                    count++;
            }
        }
        return count;
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
            FloodFillUncover(x - 1, y, visited); //izquierda
            FloodFillUncover(x + 1, y, visited); //derecha
            FloodFillUncover(x, y - 1, visited); //abajo
            FloodFillUncover(x, y + 1, visited); //arriba
        }
    }
    public static bool HasTheGameEnded()
    {
        foreach (Cell cell in cells)
        {
            if (cell.IsCovered() && !cell.HasMine)
            {
                return false;
            }
        }
        return true;
    }
    public static void RestartCells()
    {
        foreach (Cell cell in cells)
        {
            cell.RestartTexture();
            cell.ReloadMines();
        }
    }
}
