using UnityEngine;

public class GridHelper : MonoBehaviour
{
    public static readonly int w = 21;
    public static readonly int h = 21;
    //guardo cuantas celdas hay en total
    public static Cell[,] cells = new Cell[w, h];

    [Range(0.0f, 1.0f)]
    public float MineWeight = 0.15f;

    private const float CoordinateTolerance = 0.0001f;
    private static GridHelper _owner;
    private bool _isInitialized;
    public MinesweeperBoard Board { get; private set; }

    public static bool IsInitialized =>
        _owner != null && _owner.isActiveAndEnabled && _owner._isInitialized;

    private void Awake()
    {
        _owner = this;
        _isInitialized = false;
        Board = null;
        cells = new Cell[w, h];
    }

    private void Start()
    {
        InitializeBoard();
    }

    private void OnDestroy()
    {
        if (_owner == this)
        {
            _owner = null;
            cells = new Cell[w, h];
        }
    }

    public bool InitializeBoard()
    {
        _isInitialized = false;
        Board = null;
        cells = new Cell[w, h];

        if (_owner != this || !isActiveAndEnabled || !gameObject.scene.IsValid())
        {
            return FailInitialization("The board owner is missing, inactive, or belongs to an invalid scene.");
        }

        var registeredCells = new Cell[w, h];
        int ownerCount = 0;
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
        {
            ownerCount += root.GetComponentsInChildren<GridHelper>(true).Length;
            foreach (Cell cell in root.GetComponentsInChildren<Cell>(true))
            {
                string path = GetCellPath(cell);
                if (cell.transform.parent == null)
                {
                    return FailInitialization($"Cell '{path}' is missing its row parent.");
                }

                float rawX = cell.transform.position.x;
                float rawY = cell.transform.parent.position.y;
                if (float.IsNaN(rawX) || float.IsInfinity(rawX) ||
                    float.IsNaN(rawY) || float.IsInfinity(rawY) ||
                    rawX < -CoordinateTolerance || rawX > w - 1 + CoordinateTolerance ||
                    rawY < -CoordinateTolerance || rawY > h - 1 + CoordinateTolerance)
                {
                    return FailInitialization($"Cell '{path}' has out-of-range coordinates ({rawX}, {rawY}); expected x=0..{w - 1}, y=0..{h - 1}.");
                }

                int x = Mathf.RoundToInt(rawX);
                int y = Mathf.RoundToInt(rawY);
                if (Mathf.Abs(rawX - x) > CoordinateTolerance ||
                    Mathf.Abs(rawY - y) > CoordinateTolerance)
                {
                    return FailInitialization($"Cell '{path}' has non-integer coordinates ({rawX}, {rawY}).");
                }
                if (registeredCells[x, y] != null)
                {
                    return FailInitialization($"Duplicate cell coordinates ({x}, {y}): '{GetCellPath(registeredCells[x, y])}' and '{path}'.");
                }
                if (!cell.isActiveAndEnabled || cell.GetComponent<SpriteRenderer>() == null ||
                    cell.Panel == null || cell.MineTexture == null ||
                    cell.EmptyTexture == null || cell.EmptyTexture.Length != 9)
                {
                    return FailInitialization($"Cell '{path}' must be active and have a SpriteRenderer, Panel, MineTexture, and nine numbered sprites.");
                }
                foreach (Sprite sprite in cell.EmptyTexture)
                {
                    if (sprite == null)
                    {
                        return FailInitialization($"Cell '{path}' has a missing numbered sprite.");
                    }
                }

                registeredCells[x, y] = cell;
            }
        }

        if (ownerCount != 1)
        {
            return FailInitialization($"Expected one GridHelper board owner, found {ownerCount}.");
        }
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (registeredCells[x, y] == null)
                {
                    return FailInitialization($"Missing cell at ({x}, {y}); expected {w * h} uniquely registered cells.");
                }
            }
        }

        var mines = new bool[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                mines[x, y] = Random.value < MineWeight;
            }
        }
        Board = new MinesweeperBoard(mines);
        cells = registeredCells;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                cells[x, y].Initialize(Board.GetCell(x, y));
            }
        }
        _isInitialized = true;
        return true;
    }

    private bool FailInitialization(string message)
    {
        if (_owner != null)
        {
            _owner._isInitialized = false;
        }
        Debug.LogError($"Board initialization failed in scene '{gameObject.scene.path}': {message} Cell input remains disabled.", this);
        return false;
    }

    private static string GetCellPath(Cell cell)
    {
        string path = cell.name;
        Transform parent = cell.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    public static void RevealCell(int x, int y)
    {
        if (!IsInitialized || GameManager.SharedInstance == null ||
            GameManager.SharedInstance.CurrentGameState != GameState.InGame)
        {
            return;
        }

        MinesweeperBoard board = _owner.Board;
        foreach (BoardCell changedCell in board.Reveal(x, y))
        {
            cells[changedCell.X, changedCell.Y].RefreshPresentation(
                board.CountAdjacentMines(changedCell.X, changedCell.Y));
        }

        switch (board.Result)
        {
            case GameResult.Victory:
                GameManager.SharedInstance.WinGame();
                break;
            case GameResult.Defeat:
                GameManager.SharedInstance.GameOver();
                break;
        }
    }

    public static bool HasMineAt(int x, int y)
    {
        return IsInitialized && _owner.Board.HasMineAt(x, y);
    }

    public static int CountAdjacentMines(int x, int y)
    {
        return IsInitialized ? _owner.Board.CountAdjacentMines(x, y) : 0;
    }

    public static bool HasTheGameEnded()
    {
        return IsInitialized && _owner.Board.Result == GameResult.Victory;
    }
    public static bool RestartCells()
    {
        if (_owner == null)
        {
            Debug.LogError("Cannot restart the board: GridHelper owner is missing. Cell input remains disabled.");
            return false;
        }
        return _owner.InitializeBoard();
    }
}
