using System.Threading.Tasks;

namespace LifeGame
{
    class CellBoard
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private byte[] bufferSecondary;
        public byte[] Cells { get; private set; }

        private static int[] d;

        public bool this[int x, int y]
        {
            get { return Cells[x + 1 + (y + 1) * (Height + 2)] != 0; }
            set { Cells[x + 1 + (y + 1) * (Height + 2)] = (byte)(value ? 1 : 0); }
        }

        public CellBoard(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new byte[(width + 2) * (height + 2)];
            bufferSecondary = new byte[(width + 2) * (height + 2)];

            d = new[]
            {
                -1 - (Height + 2), -(Height + 2), 1 - (Height + 2),
                -1, 1,
                -1 + Height + 2, Height + 2, 1 + Height + 2
            };
        }

        public void NextGeneration()
        {
            var next = bufferSecondary;
            var xlength = Width;
            var ylength = Height;
            Parallel.For(1, ylength - 1, y =>
            {
                var xbase = y * (Height + 2);
                unsafe
                {
                    fixed (byte* nextPtr = next)
                    {
                        byte* n = &nextPtr[1 + xbase];
                        for(int pos = 1 + xbase; pos < xlength - 1 + xbase; pos++, n++)
                        {
                            *n = IsAliveOrBorn(pos, Cells);
                        }
                    }
                }
            });

            Swap();
        }

        private void Swap()
        {
            var temp = Cells;
            Cells = bufferSecondary;
            bufferSecondary = temp;
        }

        private static byte IsAliveOrBorn(int pos, byte[] cells)
        {
            switch (GetAroundAliveCellCount(pos, cells))
            {
            case 2:
                return cells[pos];
            case 3:
                return 1;
            default:
                return 0;
            }
        }

        private static int GetAroundAliveCellCount(int pos, byte[] cells)
        {
            return cells[pos + d[0]] +
                   cells[pos + d[1]] +
                   cells[pos + d[2]] +
                   cells[pos + d[3]] +
                   cells[pos + d[4]] +
                   cells[pos + d[5]] +
                   cells[pos + d[6]] +
                   cells[pos + d[7]];
        }

        internal static unsafe void Render(uint* ptr, int stride, int width, int height, byte[] cells)
        {
            Parallel.For(0, height, y =>
            {
                var p = (uint*)((byte*)ptr + stride * y);
                var xbase = (y + 1) * (height + 2);
                fixed(byte* cellsPtr = cells)
                {
                    byte* c = &cellsPtr[1 + xbase];
                    for(int x = 0; x < width; x++, p++, c++)
                    {
                        *p = (uint)(0xff000000 + (1 - *c) * 0xf5f5f5);
                    }
                }
            });
        }
    }
}