using UnityEngine;

public class Cell : MonoBehaviour
{
    public Sprite[] EmptyTexture;
    public Sprite MineTexture;
    public Sprite Panel;

    private BoardCell _model;
    private SpriteRenderer _spriteRenderer;

    public CellState State => _model != null ? _model.State : CellState.Covered;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(BoardCell model)
    {
        _model = model;
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
        _spriteRenderer.sprite = _model != null && _model.HasMine
            ? MineTexture
            : EmptyTexture[AdjacentCount];
    }

    public void RestartTexture()
    {
        _spriteRenderer.sprite = Panel;
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
