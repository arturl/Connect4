﻿using System;
using System.Collections.Generic;
using System.Drawing;
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

    public static class DiskMethods
    {
        public static Disk Opposite(this Disk disk)
        {
            return disk == Disk.Blue ? Disk.Red : Disk.Blue;
        }
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

        public Board(string game)
        {
            for (int i = 0; i < game.Length; i++)
            {
                Disk player = game[i] == 'R' ? Disk.Red : game[i] == 'B' ? Disk.Blue : Disk.Empty;
                if (i == game.Length - 1) break; // Error in game history
                char colChar = game[i + 1];
                if (Int32.TryParse(colChar.ToString(), out int col))
                {
                    if (col >= 0 && col < 7)
                    {
                        this.DropDiskAt(player, col);
                    }
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

		public static string GetDiskColorLetter(Disk d)
		{
			switch (d)
			{
				case Disk.Red: return "R";
				case Disk.Blue: return "B";
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

        public List<int> GetAvailableMoves()
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
                return GetAvailableMoves();
            }
        }

        public int CountOfPositionsTaken()
        {
            int count = 0;
            for (int col = 0; col < Width; col++)
            {
                for (int row = 0; row < Height; row++)
                {
                    if (field[col, row] != Disk.Empty) count++;
                }
            }

            return count;
        }

        private int FindRowForColumn(int col)
        {
            if (field[col, 5] == Disk.Empty) return 5;
            if (field[col, 4] == Disk.Empty) return 4;
            if (field[col, 3] == Disk.Empty) return 3;
            if (field[col, 2] == Disk.Empty) return 2;
            if (field[col, 1] == Disk.Empty) return 1;
            if (field[col, 0] == Disk.Empty) return 0;
            return -1;
        }

        private bool CheckIfDisk(int col, int row, Disk player)
        {
            if(col >=0 && col < 7 && row > 0 && row < 6)
            {
                return field[col, row] == player;
            }
            return false;
        }

        public bool IsDeadlySpotFor(Disk player, int col)
        {
            // Find row for this column
            int row = FindRowForColumn(col);
            if (row <= 0) return false;
            Disk existingPiece = field[col, row];
            if (existingPiece != Disk.Empty) return false;

            try
            {
                field[col, row] = player;
                Disk opponent = player.Opposite();
                int rowAbove = row - 1;
                field[col, rowAbove] = opponent;

                // Check horizontal
                int horizontalCount = 1;
                if (CheckIfDisk(col + 1, rowAbove, opponent)) horizontalCount++;
                if (CheckIfDisk(col + 2, rowAbove, opponent)) horizontalCount++;
                if (CheckIfDisk(col + 3, rowAbove, opponent)) horizontalCount++;
                if (CheckIfDisk(col - 1, rowAbove, opponent)) horizontalCount++;
                if (CheckIfDisk(col - 2, rowAbove, opponent)) horizontalCount++;
                if (CheckIfDisk(col - 3, rowAbove, opponent)) horizontalCount++;
                if (horizontalCount >= 4) return true;

                // Check main diagonal
                int mainDiagonalCount = 1;
                if (CheckIfDisk(col + 1, rowAbove + 1, opponent)) mainDiagonalCount++;
                if (CheckIfDisk(col + 2, rowAbove + 2, opponent)) mainDiagonalCount++;
                if (CheckIfDisk(col + 3, rowAbove + 3, opponent)) mainDiagonalCount++;
                if (CheckIfDisk(col - 1, rowAbove - 1, opponent)) mainDiagonalCount++;
                if (CheckIfDisk(col - 2, rowAbove - 2, opponent)) mainDiagonalCount++;
                if (CheckIfDisk(col - 3, rowAbove - 3, opponent)) mainDiagonalCount++;
                if (mainDiagonalCount >= 4) return true;

                // Check secondary diagonal
                int secondaryDiagonalCount = 1;
                if (CheckIfDisk(col + 1, rowAbove - 1, opponent)) secondaryDiagonalCount++;
                if (CheckIfDisk(col + 2, rowAbove - 2, opponent)) secondaryDiagonalCount++;
                if (CheckIfDisk(col + 3, rowAbove - 3, opponent)) secondaryDiagonalCount++;
                if (CheckIfDisk(col - 1, rowAbove + 1, opponent)) secondaryDiagonalCount++;
                if (CheckIfDisk(col - 2, rowAbove + 2, opponent)) secondaryDiagonalCount++;
                if (CheckIfDisk(col - 3, rowAbove + 3, opponent)) secondaryDiagonalCount++;
                if (secondaryDiagonalCount >= 4) return true;

                return false;
            }
            finally
            {
                // Restore board
                field[col, row] = Disk.Empty;
                field[col, row-1] = Disk.Empty;
            }
        }
    }
}
