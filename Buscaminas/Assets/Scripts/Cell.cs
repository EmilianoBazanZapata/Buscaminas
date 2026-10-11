using UnityEngine;

public class Cell : MonoBehaviour
{
    public Sprite[] EmptyTexture;
    public Sprite MineTexture;
    public Sprite Panel;

    [SerializeField] private Color _coveredColor = new Color(0.28f, 0.35f, 0.40f);
    [SerializeField] private Color _revealedColor = new Color(0.68f, 0.76f, 0.82f);
    [SerializeField] private Color _mineColor = new Color(0.89f, 0.36f, 0.36f);

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
        bool hasMine = _model != null && _model.HasMine;
        _spriteRenderer.sprite = hasMine
            ? MineTexture
            : EmptyTexture[AdjacentCount];
        _spriteRenderer.color = hasMine ? _mineColor : _revealedColor;
    }

    public void RestartTexture()
    {
        _spriteRenderer.sprite = Panel;
        _spriteRenderer.color = _coveredColor;
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
