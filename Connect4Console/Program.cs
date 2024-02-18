namespace Connect4Console
{
    public enum Disk
    {
        Empty,
        Red,
        Blue
    }

    public class Board
    {
        public const int Width = 7;
        public const int Height = 6;

        private Disk[,] field = new Disk[Width, Height];
        public Board() {
            for(int col=0; col<Width; col++)
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
        }

        public Disk this[int col, int row]
        {
            get => field[col, row];
            set => field[col, row] = value;
        }

        public Board Clone()
        {
            var newBoard = new Board(this);
            return newBoard;
        }

        public void DropDiskAt(Disk disk, int col)
        {
            for(int row = Height-1; row >= 0; row--)
            {
                if (field[col, row]==Disk.Empty)
                {
                    field[col, row] = disk;
                    return;
                }
            }
        }

        public static string GetPrintableDiskValue(Disk d)
        {
            switch(d)
            {
                case Disk.Red:   return "X";
                case Disk.Blue:  return "O";
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

        private bool IsWinReached(ref Disk disk, List<int[]> coordinateArrays)
        {
            foreach(var coordinateArray in coordinateArrays)
            {
                Disk lastValueSeen = Disk.Empty;
                int seqItemsDetected = 0;
                for(int i=0; i<coordinateArray.Length; i+=2)
                {
                    int row = coordinateArray[i];
                    int col = coordinateArray[i+1];

                    if (i == 0)
                    {
                        lastValueSeen = field[col, row];
                        seqItemsDetected = 1;
                    }
                    else
                    {
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
            }
            return false;
        }

        private bool IsWinReachedHorizontal(ref Disk disk, ref string direction)
        {
            List<int[]> horizontal =
            [
                [0, 0, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0, 6],
                [1, 0, 1, 1, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6],
                [2, 0, 2, 1, 2, 2, 2, 3, 2, 4, 2, 5, 2, 6],
                [3, 0, 3, 1, 3, 2, 3, 3, 3, 4, 3, 5, 3, 6],
                [4, 0, 4, 1, 4, 2, 4, 3, 4, 4, 4, 5, 4, 6],
                [5, 0, 5, 1, 5, 2, 5, 3, 5, 4, 5, 5, 5, 6],
            ];

            if( IsWinReached(ref disk, horizontal))
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

            if (IsWinReached(ref disk, vertical))
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

            if (IsWinReached(ref disk, mainDiagonal))
            {
                direction = "main diagonal";
                return true;
            }

            List<int[]> secondaryDiagonal =
            [
                [3,0, 2,1, 1,2, 0,3],
                [4,0, 3,1, 2,2, 1,3, 0,4],
                [5,0, 4,1, 3,2, 2,3, 1,4, 0,5],
                [5,1, 4,2, 3,3, 2,4, 1,5, 0,6],
                [5,2, 4,3, 3,4, 2,5, 1,6],
                [5,3, 4,4, 3,5, 2,6],
            ];

            if (IsWinReached(ref disk, secondaryDiagonal))
            {
                direction = "secondary diagonal";
                return true;
            }

            return false;
        }

        private bool IsWinReachedHorizontalOld(ref Disk disk, ref int winningRow)
        {
            for(int row = 0; row < Height; row++)
            {
                Disk lastValueSeen = Disk.Empty;
                int seqItemsDetected = 0;
                for (int col = 0; col < Width; col++)
                {
                    if(col == 0)
                    {
                        lastValueSeen = field[col, row];
                        seqItemsDetected = 1;
                    }
                    else
                    {
                        if (field[col,row] == lastValueSeen)
                        {
                            seqItemsDetected++;
                            if(lastValueSeen != Disk.Empty && seqItemsDetected == 4)
                            {
                                disk = lastValueSeen;
                                winningRow = row;
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
            }

            return false;
        }

        public bool IsWinReached(ref Disk disk, ref string direction)
        {
            if (IsWinReachedHorizontal(ref disk, ref direction)) return true;
            if (IsWinReachedVertical(ref disk, ref direction)) return true;
            if (IsWinReachedDiagonal(ref disk, ref direction)) return true;

            return false;
        }

        public List<int> GetAvailableMovesForPlayer(Disk not_used)
        {
            var possibleMoves = new List<int>();
            int[] positionsInCertainCleverOrder = [3, 2, 4, 1, 5, 0, 6];
            foreach (int col in positionsInCertainCleverOrder)
            {
                if (this[col, 0] == Disk.Empty) possibleMoves.Add(col);
            }
            return possibleMoves;
        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            var nextToPlay = Disk.Blue;
#if false
            {
                nextToPlay = Disk.Red;

                //board[1, 5] = Disk.Blue;
                //board[0, 5] = Disk.Blue;

                board[2, 5] = Disk.Blue;
                board[3, 5] = Disk.Blue;
                board[3, 3] = Disk.Blue;

                board[2, 4] = Disk.Red;
                board[3, 4] = Disk.Red;
                board[3, 2] = Disk.Red;

                //board[4, 3] = Disk.Red;
                //board[4, 4] = Disk.Red;
                //board[4, 5] = Disk.Red;

                board.PrintToConsole();

                var evalResult = GameEngine.NegaMax(board, Disk.Red, 7);

                Disk winner = Disk.Empty;
                string direction = string.Empty;
                bool b = board.IsWinReached(ref winner, ref direction);
            }
#endif
            Stack<Board> undoBuffer = new Stack<Board>();

            while (true)
            {
                board.PrintToConsole();

                Disk winner = Disk.Empty;
                string direction = string.Empty;
                bool isWinReached = board.IsWinReached(ref winner, ref direction);

                if (isWinReached)
                {
                    Console.WriteLine($"The winner is {Board.GetPrintableDiskValue(winner)}. Winning direction: {direction}");
                    break;
                }
                else
                {
                    if(!board.GetAvailableMovesForPlayer(nextToPlay).Any())
                    {
                        Console.WriteLine("Game over - no more moves");
                        break;
                    }
                }

                bool gotValidChoice = false;

                while (!gotValidChoice)
                {
                    int move;
                    if (nextToPlay == Disk.Red)
                    {
                        // AI plays for Red
                        //move = GameEngine.GetBestMoveBasic(board, nextToPlay);

                        var evalResult = GameEngine.NegaMax(board, nextToPlay, 12);
                        move = evalResult.Move;

                        Console.WriteLine($"AI chose {evalResult.Move}. Score = {evalResult.Score}");
                        if(evalResult.Score > 0)
                        {
                            Console.WriteLine("AI will win");
                        }
                        else if (evalResult.Score < 0)
                        {
                            Console.WriteLine("AI might lose");
                        }
                    }
                    else
                    {
                        Console.Write($"{Board.GetPrintableDiskValue(nextToPlay)}: pick your move: [0-7] or E to end game, B to take back your move: ");
                        string userChoice = Console.ReadLine();
                        if (string.IsNullOrEmpty(userChoice)) break;
                        if (userChoice.ToLower() == "e")
                        {
                            Console.WriteLine("You have decided to end the game. Good bye.");
                            goto end_of_game;
                        }

                        if (userChoice.ToLower() == "b")
                        {
                            Console.WriteLine("You have asked to take back your move - OK:");
                            if(!undoBuffer.Any() || !undoBuffer.TryPop(out board))
                            {
                                Console.WriteLine("Cannot take back your move at this point.");
                            }
                            break;
                        }

                        if (!int.TryParse(userChoice, out move))
                        {
                            Console.WriteLine("Wrong choice, try again");
                            break;
                        }

                        var availableMoves = board.GetAvailableMovesForPlayer(Disk.Empty);
                        if(!availableMoves.Contains(move))
                        {
                            Console.WriteLine($"Choice {move} is incorrect, try again");
                            break;
                        }

                        undoBuffer.Push(board.Clone());
                    }

                    gotValidChoice = true;
                    board.DropDiskAt(nextToPlay, move);

                    nextToPlay = (nextToPlay == Disk.Red) ? Disk.Blue : Disk.Red;
                }
            }
end_of_game:
            Console.WriteLine("Game over.");
        }
    }
}
