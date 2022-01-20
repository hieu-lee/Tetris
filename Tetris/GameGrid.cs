namespace Tetris
{
    public class GameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[Rows, Columns];
        }

        public bool IsInside(int r, int c) => (r >= 0 && r < Rows && c >= 0 && c < Columns);

        public bool IsEmpty(int r, int c) => (IsInside(r, c) && grid[r, c] == 0);

        public bool IsRowFull(int r)
        {
            for (int i = 0; i < Columns; i++)
            {
                if (grid[r, i] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRowEmpty(int r)
        {
            for (int i = 0; i < Columns; i++)
            {
                if (grid[r, i] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearRow(int r)
        {
            for (int i = 0; i < Columns; i++)
            {
                grid[r, i] = 0;
            }
        }

        public void MoveRowDown(int r, int NumRows)
        {
            for (int i = 0; i < Columns; i++)
            {
                grid[r + NumRows, i] = grid[r, i];
                grid[r, i] = 0;
            }
        }

        public int ClearFullRows()
        {
            var Cleared = 0;

            for (int r = Rows-1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    Cleared++;
                }
                else if (Cleared > 0) 
                {
                    MoveRowDown(r, Cleared);
                }
            }

            return Cleared;
        }
    }
}
