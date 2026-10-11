public sealed class BoardCell
{
    internal BoardCell(int x, int y, bool hasMine, int adjacentMineCount)
    {
        X = x;
        Y = y;
        HasMine = hasMine;
        AdjacentMineCount = adjacentMineCount;
    }

    public int X { get; }
    public int Y { get; }
    public bool HasMine { get; }
    public int AdjacentMineCount { get; }
    public CellState State { get; private set; } = CellState.Covered;

    internal void Reveal()
    {
        State = CellState.Revealed;
    }
}
