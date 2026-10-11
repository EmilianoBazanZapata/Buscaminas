using UnityEngine;

public class GridHelper : MonoBehaviour
{
    public static int w => DifficultySettings.For(SelectedDifficulty).Columns;
    public static int h => DifficultySettings.For(SelectedDifficulty).Rows;
    public static Cell[,] cells = new Cell[w, h];

    public static Difficulty SelectedDifficulty { get; private set; } = Difficulty.Normal;

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Transform _boardRoot;

    private const float CoordinateTolerance = 0.0001f;
    private static GridHelper _owner;
    private bool _isInitialized;
    private bool _cellsCreated;
    private Cell[] _createdCells;
    private Transform _generatedRoot;
    private Difficulty _createdDifficulty;
    public MinesweeperBoard Board { get; private set; }

    public static bool IsInitialized =>
        _owner != null && _owner.isActiveAndEnabled && _owner._isInitialized;

    private void Awake()
    {
        _owner = this;
        SelectedDifficulty = Difficulty.Normal;
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

        if (!CreateCells())
        {
            return false;
        }

        var registeredCells = new Cell[w, h];
        int ownerCount = 0;
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
        {
            ownerCount += root.GetComponentsInChildren<GridHelper>(true).Length;
            if (root.transform != _boardRoot && root.GetComponentsInChildren<Cell>(true).Length != 0)
            {
                return FailInitialization($"Unexpected cells outside the board root in '{root.name}'.");
            }
        }
        if (_boardRoot == null || !_boardRoot.gameObject.activeInHierarchy ||
            _boardRoot.gameObject.scene != gameObject.scene || _generatedRoot == null || !_generatedRoot.gameObject.activeInHierarchy ||
            _generatedRoot.parent != _boardRoot || _generatedRoot.childCount != h ||
            _generatedRoot.GetComponentsInChildren<Cell>(true).Length != w * h ||
            _boardRoot.GetComponentsInChildren<Cell>().Length != w * h)
        {
            return FailInitialization("The board root must remain active in this scene with all expected rows.");
        }
        for (int y = 0; y < h; y++)
        {
            if (_generatedRoot.GetChild(y).childCount != w)
            {
                return FailInitialization($"Board row '{_generatedRoot.GetChild(y).name}' must contain {w} cells.");
            }
        }
        foreach (Cell cell in _createdCells)
        {
            if (cell == null)
            {
                return FailInitialization($"Missing cell; expected {w * h} registered cells.");
            }
            if (cell.transform.parent == null || cell.transform.parent.parent != _generatedRoot ||
                cell.transform.childCount != 0 || cell.GetComponents<Cell>().Length != 1)
            {
                return FailInitialization($"Cell '{GetCellPath(cell)}' must remain in its board row with exactly one Cell component and no children.");
            }
            string path = GetCellPath(cell);

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
                cell.GetComponent<BoxCollider2D>() == null ||
                cell.Panel == null || cell.MineTexture == null ||
                cell.EmptyTexture == null || cell.EmptyTexture.Length != 9)
            {
                return FailInitialization($"Cell '{path}' must be active and have a SpriteRenderer, BoxCollider2D, Panel, MineTexture, and nine numbered sprites.");
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
        var positions = new int[w * h];
        for (int i = 0; i < positions.Length; i++) positions[i] = i;
        int mineCount = DifficultySettings.For(SelectedDifficulty).Mines;
        for (int i = 0; i < mineCount; i++)
        {
            int selected = Random.Range(i, positions.Length);
            int position = positions[selected];
            positions[selected] = positions[i];
            positions[i] = position;
            mines[position % w, position / w] = true;
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
        Camera camera = Camera.main;
        if (camera != null && camera.orthographic)
        {
            camera.transform.position = new Vector3((w - 1) * 0.5f, (h - 1) * 0.5f, camera.transform.position.z);
            camera.orthographicSize = Mathf.Max(h * 0.5f, w * 0.5f / camera.aspect) + 1f;
        }
        _isInitialized = true;
        return true;
    }

    private bool CreateCells()
    {
        // Restart validates and reuses the same views; changing difficulty replaces only generated views.
        if (_cellsCreated && _createdDifficulty == SelectedDifficulty) return true;
        if (_cellPrefab == null || _boardRoot == null ||
            _boardRoot.gameObject.scene != gameObject.scene || !_boardRoot.gameObject.activeInHierarchy)
        {
            return FailInitialization("A cell prefab and active board root in this scene are required.");
        }
        if (!_cellsCreated && _boardRoot.GetComponentsInChildren<Cell>(true).Length != 0)
        {
            return FailInitialization("The board root must not contain cells before generation.");
        }
        if (_generatedRoot != null)
        {
            _generatedRoot.gameObject.SetActive(false);
            Destroy(_generatedRoot.gameObject);
        }
        _generatedRoot = new GameObject("Generated board").transform;
        _generatedRoot.SetParent(_boardRoot, false);
        _generatedRoot.position = Vector3.zero;
        _createdCells = new Cell[w * h];
        for (int y = 0; y < h; y++)
        {
            Transform row = new GameObject($"Row {y}").transform;
            row.SetParent(_generatedRoot, false);
            row.localPosition = new Vector3(0, y, 0);
            for (int x = 0; x < w; x++)
            {
                Cell cell = Instantiate(_cellPrefab, row, false);
                cell.name = $"Cell ({x}, {y})";
                cell.transform.localPosition = new Vector3(x, 0, 0);
                _createdCells[y * w + x] = cell;
            }
        }
        _createdDifficulty = SelectedDifficulty;
        _cellsCreated = true;
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
                changedCell.AdjacentMineCount);
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

    public static bool SelectDifficulty(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        return RestartCells();
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
