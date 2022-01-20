using System.Collections.Generic;

namespace Tetris
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffSet { get; }
        public abstract int Id { get; }

        private int RotationState;
        private Position OffSet;

        public Block()
        {
            OffSet = new(StartOffSet.Row, StartOffSet.Column);
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[RotationState])
            {
                yield return new(p.Row + OffSet.Row, p.Column + OffSet.Column);
            }
        }

        public void RotateCW() => RotationState = (RotationState + 1) % Tiles.Length;

        public void RotateCCW() => RotationState = (RotationState == 0) ? Tiles.Length - 1 : RotationState - 1;

        public void Move(int row, int col)
        {
            OffSet.Row += row;
            OffSet.Column += col;
        }

        public void Reset()
        {
            RotationState = 0;
            OffSet.Row = StartOffSet.Row;
            OffSet.Column = StartOffSet.Column; 
        }
    }
}
