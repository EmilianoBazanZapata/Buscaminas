using System;
using System.Collections.Generic;

// Owns gameplay state and rules. The caller supplies mine data and renders changed cells.
public sealed class MinesweeperBoard
{
    private readonly BoardCell[,] _cells;
    private int _coveredSafeCells;

    public MinesweeperBoard(bool[,] mines)
    {
        if (mines == null)
        {
            throw new ArgumentNullException(nameof(mines));
        }
        Width = mines.GetLength(0);
        Height = mines.GetLength(1);
        if (Width == 0 || Height == 0)
        {
            throw new ArgumentException("The board must have cells in both dimensions.", nameof(mines));
        }

        _cells = new BoardCell[Width, Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _cells[x, y] = new BoardCell(x, y, mines[x, y], CalculateAdjacentMines(mines, x, y));
                if (!mines[x, y])
                {
                    _coveredSafeCells++;
                }
            }
        }
    }

    public int Width { get; }
    public int Height { get; }
    public GameResult Result { get; private set; } = GameResult.InProgress;
    public BoardCell GetCell(int x, int y) => _cells[x, y];

    public bool HasMineAt(int x, int y)
    {
        return Contains(x, y) && _cells[x, y].HasMine;
    }

    private int CalculateAdjacentMines(bool[,] mines, int x, int y)
    {
        int count = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if ((i != x || j != y) && Contains(i, j) && mines[i, j])
                {
                    count++;
                }
            }
        }
        return count;
    }

    public IReadOnlyList<BoardCell> Reveal(int x, int y)
    {
        if (Result != GameResult.InProgress || !Contains(x, y) ||
            _cells[x, y].State == CellState.Revealed)
        {
            return Array.Empty<BoardCell>();
        }

        var changedCells = new List<BoardCell>();
        if (_cells[x, y].HasMine)
        {
            Result = GameResult.Defeat;
            foreach (BoardCell cell in _cells)
            {
                if (cell.HasMine)
                {
                    cell.Reveal();
                    changedCells.Add(cell);
                }
            }
        }
        else
        {
            RevealSafeArea(x, y, changedCells);
            if (_coveredSafeCells == 0)
            {
                Result = GameResult.Victory;
            }
        }
        return changedCells;
    }

    private void RevealSafeArea(int x, int y, List<BoardCell> changedCells)
    {
        if (!Contains(x, y))
        {
            return;
        }
        BoardCell cell = _cells[x, y];
        if (cell.HasMine || cell.State == CellState.Revealed)
        {
            return;
        }

        cell.Reveal();
        changedCells.Add(cell);
        _coveredSafeCells--;
        if (cell.AdjacentMineCount > 0)
        {
            return;
        }
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i != x || j != y)
                {
                    RevealSafeArea(i, j, changedCells);
                }
            }
        }
    }

    private bool Contains(int x, int y)
    {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}
