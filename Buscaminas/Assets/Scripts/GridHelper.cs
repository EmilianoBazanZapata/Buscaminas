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

    public static bool IsInitialized =>
        _owner != null && _owner.isActiveAndEnabled && _owner._isInitialized;

    private void Awake()
    {
        _owner = this;
        _isInitialized = false;
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

        cells = registeredCells;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                cells[x, y].Initialize(x, y, Random.value < MineWeight);
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

    //revelamos la posicion de todas las minas al perder
    public static void UncoverAllTheMines()
    {
        if (!IsInitialized)
        {
            return;
        }
        foreach (Cell c in cells)
        {
            if (c.HasMine)
            {
                c.Reveal(0);
            }
        }
    }
    public static bool HasMineAt(int x, int y)
    {
        if (IsInitialized && x >= 0 && y >= 0 && x < w && y < h)
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
        if (!IsInitialized)
        {
            return;
        }
        //solo debemos proceder si el punto (x,y) es valida

        if (x >= 0 && y >= 0 && x < w && y < h)
        {
            //si ya pase por la celda el algoritmo de flood fill no debera hacer nada
            if (visited[x, y] || cells[x, y].HasMine || !cells[x, y].IsCovered())
            {
                return;
            }
            visited[x, y] = true;
            //cuento la cantidad de minsas adjacentes
            int AdjacentMines = CountAdjacentMines(x, y);
            //muesto el numero de minas
            cells[x, y].Reveal(AdjacentMines);
            //verifico si tengo minas para saber si puedo destapar la celda
            if (AdjacentMines > 0)
            {
                return;
            }
            //visito todos los vecinos
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i != x || j != y)
                    {
                        FloodFillUncover(i, j, visited);
                    }
                }
            }
        }
    }
    public static bool HasTheGameEnded()
    {
        if (!IsInitialized)
        {
            return false;
        }
        foreach (Cell cell in cells)
        {
            if (!cell.HasMine && cell.IsCovered())
            {
                return false;
            }
        }
        return true;
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
