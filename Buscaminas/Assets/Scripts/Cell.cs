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
        LoadTexture(1);
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
}