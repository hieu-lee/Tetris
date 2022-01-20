using System;

namespace Tetris
{
    public class BlockQueue
    {
        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private int[] BlockIndexes = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        private readonly Random random = new Random();

        public Block NextBlock { get; private set; }

        public Block RandomBlock()
        {
            var index = random.Next(6);
            var res = blocks[BlockIndexes[index]];
            (BlockIndexes[index], BlockIndexes[6]) = (BlockIndexes[6], BlockIndexes[index]);
            return res;
        }

        public Block GetAndUpdate()
        {
            var block = NextBlock;
            NextBlock = RandomBlock();
            return block;
        }
    }
}
