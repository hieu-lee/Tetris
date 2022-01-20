using Tetris.Models;

namespace Tetris
{
    public class GameState
    {
        private Block curentBlock;
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; } = true;
        public Difficulty difficulty { get; set; } = Difficulty.Easy;
        public int Score { get; private set; }
        const int MediumScore = 20;
        const int HardScore = 40;

        public Block CurrentBlock
        {
            get => curentBlock;
            private set
            {
                curentBlock = value;
                curentBlock.Reset();

                for (var i = 0; i < 2; i++)
                {
                    curentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        curentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; } = false;

        public GameState()
        {
            GameGrid = new(22, 10);
            BlockQueue = new();
            CurrentBlock = BlockQueue.GetAndUpdate();
        }

        private bool BlockFits()
        {
            foreach (Position p in curentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver() => !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));

        private int CalculateScore(int NumClearedRows)
        {
            var res = 0;
            var c = 1;
            while (NumClearedRows > 0)
            {
                res += c;
                NumClearedRows--;
                c++;
            }
            return res;
        }

        public void HoldBlock()
        {
            if (CanHold)
            {
                if (HeldBlock == null)
                {
                    HeldBlock = curentBlock;
                    curentBlock = BlockQueue.GetAndUpdate();
                }
                else
                {
                    (CurrentBlock, HeldBlock) = (HeldBlock, CurrentBlock);
                }

                CanHold = false;
            }
        }

        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += CalculateScore(GameGrid.ClearFullRows());
            
            switch (difficulty)
            {
                case Difficulty.Easy:
                    if (Score >= MediumScore)
                    {
                        difficulty = Difficulty.Medium;
                    }
                    break;
                case Difficulty.Medium:
                    if (Score >= HardScore)
                    {
                        difficulty = Difficulty.Hard;
                    }
                    break;
                default:
                    break;
            }

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                var tmp = TileDropDistance(p);
                drop = (tmp < drop) ? tmp : drop;
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
