using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4AIEngine
{
    public enum Disk
    {
        Empty = 0,
        Red,
        Blue
    }

    public class Board
    {
        public const int Width = 7;
        public const int Height = 6;

        private int minCol = 0;
        private int maxCol = 0;
        private int minRow = 0;
        private int maxRow = 0;

        private readonly Disk[,] field = new Disk[Width, Height];
        public Board()
        {
            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    field[col, row] = Disk.Empty;
                }
            }
        }

        private Board(Board other)
        {
            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    field[col, row] = other.field[col, row];
                }
            }
            minCol = other.minCol;
            maxCol = other.maxCol;
            minRow = other.minRow;
            maxRow = other.maxRow;
        }

        private void SetValueAndAdjustBoundaries(int col, int row, Disk disk)
        {
            field[col, row] = disk;
            minCol = int.Min(minCol, col);
            maxCol = int.Max(maxCol, col);
            minRow = int.Min(minRow, row);
            maxRow = int.Max(maxRow, row);
    }

        public Disk this[int col, int row]
        {
            get => field[col, row];
            set => SetValueAndAdjustBoundaries(col, row, value);
        }

        public Board Clone()
        {
            var newBoard = new Board(this);
            return newBoard;
        }

        public void DropDiskAt(Disk disk, int col)
        {
            for (int row = Height - 1; row >= 0; row--)
            {
                if (field[col, row] == Disk.Empty)
                {
                    SetValueAndAdjustBoundaries(col, row, disk);
                    return;
                }
            }
        }

        public static string GetPrintableDiskValue(Disk d)
        {
            switch (d)
            {
                case Disk.Red: return "X";
                case Disk.Blue: return "O";
                default: return " ";
            }
        }

        public void PrintToConsole()
        {
            Console.WriteLine(VisualizeAsString());
        }

        private string VisualizeAsString()
        {
            string board = """
                       0     1     2     3     4     5     6 
                    ┌─────┬─────┬─────┬─────┬─────┬─────┬─────┐
                  0 │ 00  │ 01  │ 02  │ 03  │ 04  │ 05  │ 06  │
                    ├─────┼─────┼─────┼─────┼─────┼─────┼─────┤
                  1 │ 10  │ 11  │ 12  │ 13  │ 14  │ 15  │ 16  │
                    ├─────┼─────┼─────┼─────┼─────┼─────┼─────┤
                  2 │ 20  │ 21  │ 22  │ 23  │ 24  │ 25  │ 26  │
                    ├─────┼─────┼─────┼─────┼─────┼─────┼─────┤
                  3 │ 30  │ 31  │ 32  │ 33  │ 34  │ 35  │ 36  │
                    ├─────┼─────┼─────┼─────┼─────┼─────┼─────┤
                  4 │ 40  │ 41  │ 42  │ 43  │ 44  │ 45  │ 46  │
                    ├─────┼─────┼─────┼─────┼─────┼─────┼─────┤
                  5 │ 50  │ 51  │ 52  │ 53  │ 54  │ 55  │ 56  │
                    └─────┴─────┴─────┴─────┴─────┴─────┴─────┘
                """;

            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    string coordinates = $"{row}{col}";
                    board = board.Replace(coordinates, $" {GetPrintableDiskValue(field[col, row])}");
                }
            }

            return board;
        }

        private bool IsWinReachedByCoordinateArrays(ref Disk disk, List<int[]> coordinateArrays)
        {
            foreach (var coordinateArray in coordinateArrays)
            {
                Disk lastValueSeen = Disk.Empty;
                int seqItemsDetected = 0;

                if (coordinateArray.Length > 1)
                {
                    int row = coordinateArray[0];
                    int col = coordinateArray[1];
                    lastValueSeen = field[col, row];
                    seqItemsDetected = 1;
                }

                for (int i = 2; i < coordinateArray.Length; i += 2)
                {
                    int row = coordinateArray[i];
                    int col = coordinateArray[i + 1];

                    if (field[col, row] == lastValueSeen)
                    {
                        seqItemsDetected++;
                        if (lastValueSeen != Disk.Empty && seqItemsDetected == 4)
                        {
                            disk = lastValueSeen;
                            return true;
                        }
                    }
                    else
                    {
                        seqItemsDetected = 1;
                        lastValueSeen = field[col, row];
                    }
                }
            }
            return false;
        }

        private bool IsWinReachedHorizontal(ref Disk disk, ref string direction)
        {
            List<int[]> horizontal =
            [
#if true
                [0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6],
                [1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6],
                [2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6],
                [3, 0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3, 6],
                [4, 0, 4, 1, 4, 2, 4, 3, 4, 4, 4, 5, 4, 6],
                [5, 0, 5, 1, 5, 2, 5, 3, 5, 4, 5, 5, 5, 6],
#else
                // Faster order: bottom up:
                [5, 0, 5, 1, 5, 2, 5, 3, 5, 4, 5, 5, 5, 6], /* 5 */
                [4, 0, 4, 1, 4, 2, 4, 3, 4, 4, 4, 5, 4, 6], /* 4 */
                [3, 0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3, 6], /* 3 */
                [2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6], /* 2 */
                [1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6], /* 1 */
                [0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6], /* 0 */
#endif
            ];

            if (IsWinReachedByCoordinateArrays(ref disk, horizontal))
            {
                direction = "horizontal";
                return true;
            }
            return false;
        }

        private bool IsWinReachedVertical(ref Disk disk, ref string direction)
        {
            List<int[]> vertical =
            [
                [0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0],
                [0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1],
                [0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2],
                [0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3],
                [0, 4, 1, 4, 2, 4, 3, 4, 4, 4, 5, 4],
                [0, 5, 1, 5, 2, 5, 3, 5, 4, 5, 5, 5],
                [0, 6, 1, 6, 2, 6, 3, 6, 4, 6, 5, 6],
            ];

            if (IsWinReachedByCoordinateArrays(ref disk, vertical))
            {
                direction = "vertical";
                return true;
            }
            return false;
        }

        private bool IsWinReachedDiagonal(ref Disk disk, ref string direction)
        {
            List<int[]> mainDiagonal =
            [
                [2, 0, 3, 1, 4, 2, 5, 3],
                [1, 0, 2, 1, 3, 2, 4, 3, 5, 4],
                [0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5],
                [0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6],
                [0, 2, 1, 3, 2, 4, 3, 5, 4, 6],
                [0, 3, 1, 4, 2, 5, 3, 6]
            ];

            if (IsWinReachedByCoordinateArrays(ref disk, mainDiagonal))
            {
                direction = "main diagonal";
                return true;
            }

            List<int[]> secondaryDiagonal =
            [
                [3, 0, 2, 1, 1, 2, 0, 3],
                [4, 0, 3, 1, 2, 2, 1, 3, 0, 4],
                [5, 0, 4, 1, 3, 2, 2, 3, 1, 4, 0, 5],
                [5, 1, 4, 2, 3, 3, 2, 4, 1, 5, 0, 6],
                [5, 2, 4, 3, 3, 4, 2, 5, 1, 6],
                [5, 3, 4, 4, 3, 5, 2, 6],
            ];

            if (IsWinReachedByCoordinateArrays(ref disk, secondaryDiagonal))
            {
                direction = "secondary diagonal";
                return true;
            }

            return false;
        }

        private bool IsWinReachedHorizontalFast(ref Disk disk, ref string direction)
        {
            for (int row = this.maxRow; row >= this.minRow; row--)
            {
                Disk lastValueSeen = field[this.minCol, row];
                int seqItemsDetected = 1;
                for (int col = this.minCol+1; col <= this.maxCol; col++)
                {
                    if (field[col, row] == lastValueSeen)
                    {
                        seqItemsDetected++;
                        if (lastValueSeen != Disk.Empty && seqItemsDetected == 4)
                        {
                            disk = lastValueSeen;
                            direction = "horizontal";
                            return true;
                        }
                    }
                    else
                    {
                        seqItemsDetected = 1;
                        lastValueSeen = field[col, row];
                    }
                }
            }

            return false;
        }

        private bool IsWinReachedVerticalFast(ref Disk disk, ref string direction)
        {
            for (int col = 0; col < Width; col++)
            {
                Disk lastValueSeen = field[col, 0];
                int seqItemsDetected = 1;
                for (int row = 1; row < Height; row++)
                {
                    if (field[col, row] == lastValueSeen)
                    {
                        seqItemsDetected++;
                        if (lastValueSeen != Disk.Empty && seqItemsDetected == 4)
                        {
                            disk = lastValueSeen;
                            direction = "vertical";
                            return true;
                        }
                    }
                    else
                    {
                        seqItemsDetected = 1;
                        lastValueSeen = field[col, row];
                    }
                }
            }

            return false;
        }

        public bool IsWinReached(ref Disk disk, ref string direction)
        {
            if (IsWinReachedHorizontalFast(ref disk, ref direction)) return true;
            if (IsWinReachedVerticalFast(ref disk, ref direction)) return true;
            if (IsWinReachedDiagonal(ref disk, ref direction)) return true;

            return false;
        }

        public List<int> GetAvailableMovesForPlayer()
        {
            var possibleMoves = new List<int>();
            int[] positionsInCertainCleverOrder = [3, 2, 4, 1, 5, 0, 6];
            foreach (int col in positionsInCertainCleverOrder)
            {
                if (this[col, 0] == Disk.Empty) possibleMoves.Add(col);
            }
            return possibleMoves;
        }

        public List<int> GetAvailableMovesExcludingWins()
        {
            Disk winner = Disk.Empty;
            string dummy = string.Empty;
            if (IsWinReached(ref winner, ref dummy))
            {
                return [];
            }
            else
            {
                return GetAvailableMovesForPlayer();
            }
        }
    }
}
