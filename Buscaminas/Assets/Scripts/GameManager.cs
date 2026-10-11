using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    InGame = 0,
    GameOver = 1,
    Menu = 2,
    Victory = 3
}
public class GameManager : MonoBehaviour
{
    public Canvas MenuCanvas;
    public Canvas GameCanvas;
    public Canvas GameOverCanvas;
    [SerializeField] private Button[] _difficultyButtons;
    [SerializeField] private Text _resultTitle;
    [SerializeField] private Text _resultMessage;
    [SerializeField] private Image _resultAccent;
    private Difficulty _selectedDifficulty = Difficulty.Normal;
    private double _timerStartedAt;
    private double _elapsedSeconds;
    private bool _timerRunning;

    public int ClickCount { get; private set; }
    public double ElapsedSeconds => _timerRunning
        ? Time.unscaledTimeAsDouble - _timerStartedAt : _elapsedSeconds;

    public void RecordCellClick()
    {
        if (CurrentGameState != GameState.InGame || !GridHelper.IsInitialized) return;
        ClickCount++;
        if (!_timerRunning)
        {
            _timerStartedAt = Time.unscaledTimeAsDouble;
            _timerRunning = true;
        }
    }

    private void ResetStatistics()
    {
        ClickCount = 0;
        _elapsedSeconds = 0;
        _timerRunning = false;
    }

    public static GameManager SharedInstance;
    public GameState CurrentGameState = GameState.Menu;

    private void Awake()
    {
        SharedInstance = this;
    }
    private void Start()
    {
        BackToMenu();
    }
    public void PlayGame()
    {
        StartDifficulty(_selectedDifficulty);
    }

    public void PlayEasy() => SelectMenuDifficulty(Difficulty.Easy);
    public void SelectNormal() => SelectMenuDifficulty(Difficulty.Normal);
    public void PlayHard() => SelectMenuDifficulty(Difficulty.Hard);

    private void SelectMenuDifficulty(Difficulty difficulty)
    {
        if (CurrentGameState != GameState.Menu) return;
        _selectedDifficulty = difficulty;
        UpdateDifficultySelection();
    }

    private void UpdateDifficultySelection()
    {
        if (_difficultyButtons == null) return;
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            Button button = _difficultyButtons[i];
            if (button == null) continue;
            bool selected = i == (int)_selectedDifficulty;
            button.image.color = selected ? new Color(0.27f, 0.45f, 0.25f) : new Color(0.20f, 0.25f, 0.28f);
            Outline border = button.GetComponent<Outline>();
            if (border != null) border.effectColor = selected
                ? new Color(0.62f, 0.85f, 0.48f) : new Color(0.46f, 0.52f, 0.56f);
        }
    }

    private void StartDifficulty(Difficulty difficulty)
    {
        if (!GridHelper.SelectDifficulty(difficulty))
        {
            BackToMenu();
            return;
        }
        ResetStatistics();
        SetGameState(GameState.InGame);
    }
    public void RestartGame()
    {
        if (!GridHelper.RestartCells())
        {
            BackToMenu();
            return;
        }
        ResetStatistics();
        SetGameState(GameState.InGame);

    }
    public void BackToMenu()
    {
        SetGameState(GameState.Menu);
        UpdateDifficultySelection();
    }
    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void WinGame()
    {
        SetGameState(GameState.Victory);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void SetGameState(GameState NewGameState)
    {
        if (NewGameState != GameState.InGame && _timerRunning)
        {
            _elapsedSeconds = ElapsedSeconds;
            _timerRunning = false;
        }
        bool showMenu = NewGameState == GameState.Menu;
        bool showResult = NewGameState == GameState.GameOver || NewGameState == GameState.Victory;
        MenuCanvas.enabled = true;
        MenuCanvas.gameObject.SetActive(showMenu);
        GameCanvas.enabled = true;
        GameCanvas.gameObject.SetActive(NewGameState == GameState.InGame || showResult);
        GameOverCanvas.enabled = true;
        GameOverCanvas.gameObject.SetActive(showResult);
        if (showResult)
        {
            bool won = NewGameState == GameState.Victory;
            Color accent = won ? new Color(0.62f, 0.85f, 0.48f) : new Color(0.89f, 0.36f, 0.36f);
            if (_resultTitle != null)
            {
                _resultTitle.text = won ? "YOU WIN!" : "GAME OVER";
                _resultTitle.color = accent;
            }
            if (_resultMessage != null)
                _resultMessage.text = won ? "All safe cells revealed" : "A mine exploded";
            if (_resultAccent != null) _resultAccent.color = accent;
        }
        CurrentGameState = NewGameState;
    }
}
