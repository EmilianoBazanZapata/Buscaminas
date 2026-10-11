using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GridHelper _grid;
    [SerializeField] private Camera _boardCamera;
    [SerializeField] private RectTransform _safeArea;
    [SerializeField] private RectTransform _boardArea;
    [SerializeField] private Text _time;
    [SerializeField] private Text _clicks;
    [SerializeField] private Text _mines;
    [SerializeField] private Text _resetFace;
    [SerializeField] private Text _resultStats;

    private readonly Vector3[] _corners = new Vector3[4];
    private Rect _lastSafeArea;
    private Vector2Int _lastScreen;
    private int _lastSeconds = -1;
    private int _lastClicks = -1;
    private Difficulty? _lastDifficulty;
    private GameState? _lastState;
    private Rect _lastViewport;
    private int _lastWidth;
    private int _lastHeight;

    private void OnEnable()
    {
        _lastScreen = Vector2Int.zero;
        _lastSeconds = _lastClicks = -1;
        _lastState = null;
        _lastDifficulty = null;
        _lastWidth = 0;
    }

    private void LateUpdate()
    {
        if (_gameManager == null || _grid == null || _boardCamera == null ||
            _safeArea == null || _boardArea == null || Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safe = Screen.safeArea;
        var screen = new Vector2Int(Screen.width, Screen.height);
        if (_lastSafeArea != safe || _lastScreen != screen)
        {
            _safeArea.anchorMin = new Vector2(safe.xMin / screen.x, safe.yMin / screen.y);
            _safeArea.anchorMax = new Vector2(safe.xMax / screen.x, safe.yMax / screen.y);
            _safeArea.offsetMin = _safeArea.offsetMax = Vector2.zero;
            _lastSafeArea = safe;
            _lastScreen = screen;
            Canvas.ForceUpdateCanvases();
        }

        _boardArea.GetWorldCorners(_corners);
        Vector2 min = RectTransformUtility.WorldToScreenPoint(null, _corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(null, _corners[2]);
        Rect viewport = Rect.MinMaxRect(min.x / screen.x, min.y / screen.y,
            max.x / screen.x, max.y / screen.y);
        MinesweeperBoard board = _grid.Board;
        if (board != null && viewport.width > 0 && viewport.height > 0 &&
            (_lastViewport != viewport || _lastWidth != board.Width || _lastHeight != board.Height))
        {
            _boardCamera.rect = viewport;
            float aspect = (max.x - min.x) / (max.y - min.y);
            _boardCamera.transform.position = new Vector3((board.Width - 1) * 0.5f,
                (board.Height - 1) * 0.5f, _boardCamera.transform.position.z);
            _boardCamera.orthographicSize = Mathf.Max(board.Height * 0.5f,
                board.Width * 0.5f / aspect) + 0.5f;
            _lastViewport = viewport;
            _lastWidth = board.Width;
            _lastHeight = board.Height;
        }

        int seconds = (int)_gameManager.ElapsedSeconds;
        int clicks = _gameManager.ClickCount;
        GameState state = _gameManager.CurrentGameState;
        Difficulty difficulty = GridHelper.SelectedDifficulty;
        if (seconds == _lastSeconds && clicks == _lastClicks &&
            state == _lastState && difficulty == _lastDifficulty) return;

        if (_time != null) _time.text = seconds.ToString("D3");
        if (_clicks != null) _clicks.text = "CLICKS  " + clicks.ToString("D3");
        if (_mines != null) _mines.text = DifficultySettings.For(difficulty).Mines.ToString("D3");
        if (_resetFace != null) _resetFace.text = state == GameState.GameOver ? ":(" : ":)";
        if (_resultStats != null) _resultStats.text = "TIME " + seconds.ToString("D3") +
            " s   /   CLICKS " + clicks.ToString("D3");
        _lastSeconds = seconds;
        _lastClicks = clicks;
        _lastState = state;
        _lastDifficulty = difficulty;
    }
}
