using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool HasMine;
    public Sprite[] EmptyTexture;
    public Sprite MineTexture;
    public Sprite Panel;

    private BoardCell _model;

    public CellState State => _model != null ? _model.State : CellState.Covered;

    public void Initialize(BoardCell model)
    {
        _model = model;
        // Retain the serialized field as a presentation mirror; the board owns mine data.
        HasMine = model.HasMine;
        RefreshPresentation(0);
    }

    public void RefreshPresentation(int adjacentCount)
    {
        if (IsCovered())
        {
            RestartTexture();
        }
        else
        {
            LoadTexture(adjacentCount);
        }
    }

    public void LoadTexture(int AdjacentCount)
    {
        GetComponent<SpriteRenderer>().sprite = _model != null && _model.HasMine
            ? MineTexture
            : EmptyTexture[AdjacentCount];
    }

    public void RestartTexture()
    {
        GetComponent<SpriteRenderer>().sprite = Panel;
    }

    public bool IsCovered()
    {
        return State == CellState.Covered;
    }

    private void OnMouseUpAsButton()
    {
        if (_model != null && GridHelper.IsInitialized && GameManager.SharedInstance != null &&
            GameManager.SharedInstance.CurrentGameState == GameState.InGame)
        {
            GridHelper.RevealCell(_model.X, _model.Y);
        }
    }
}
