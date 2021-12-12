using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper : MonoBehaviour
{
    //anchura
    public static int w = 21;
    //altura
    public static int h = 21;
    //
    public static Cell[,] cells = new Cell[w,h];

    public static void UncoverAllTheMines()
    {
        foreach (Cell c in cells)
        {
            if(c.HasMine)
            {
                c.LoadTexture(0);
            }
        }
    }
}
