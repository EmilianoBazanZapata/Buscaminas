public readonly struct DifficultySettings
{
    private DifficultySettings(int columns, int rows, int mines)
    {
        Columns = columns;
        Rows = rows;
        Mines = mines;
    }

    public int Columns { get; }
    public int Rows { get; }
    public int Mines { get; }

    public static DifficultySettings For(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return new DifficultySettings(9, 9, 10);
            case Difficulty.Normal: return new DifficultySettings(16, 16, 40);
            case Difficulty.Hard: return new DifficultySettings(30, 16, 99);
            default: throw new System.ArgumentOutOfRangeException(nameof(difficulty));
        }
    }
}
