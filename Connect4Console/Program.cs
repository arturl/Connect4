using Connect4AIEngine;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using static Connect4AIEngine.GameEngine;

namespace Connect4Console
{
    class AITimer : IDisposable
    {
        Stopwatch stopwatch = new Stopwatch();

        public AITimer()
        {
            stopwatch.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
        }
    }

    internal class ConsoleProgressReport : IProgressReport
    {
        int total;
        int current;
        public void TotalMoves(int moves)
        {
            this.total = moves;
            this.current = 0;
        }

        public void UpdateProgress()
        {
            Console.Write(".");
            this.current++;
        }

        public void EndProgress()
        {
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var board = new Board();
            var nextToPlay = Disk.Blue;

#if false
            board[0, 0] = Disk.Empty; board[1, 0] = Disk.Empty; board[2, 0] = Disk.Red;   board[3, 0] = Disk.Red;   board[4, 0] = Disk.Empty; board[5, 0] = Disk.Empty; board[6, 0] = Disk.Empty;
            board[0, 1] = Disk.Empty; board[1, 1] = Disk.Empty; board[2, 1] = Disk.Red;   board[3, 1] = Disk.Blue;  board[4, 1] = Disk.Empty; board[5, 1] = Disk.Empty; board[6, 1] = Disk.Empty;
            board[0, 2] = Disk.Red;   board[1, 2] = Disk.Empty; board[2, 2] = Disk.Blue;  board[3, 2] = Disk.Red;   board[4, 2] = Disk.Empty; board[5, 2] = Disk.Red;   board[6, 2] = Disk.Empty;
            board[0, 3] = Disk.Blue;  board[1, 3] = Disk.Empty; board[2, 3] = Disk.Red;   board[3, 3] = Disk.Blue;  board[4, 3] = Disk.Empty; board[5, 3] = Disk.Blue;  board[6, 3] = Disk.Empty;
            board[0, 4] = Disk.Blue;  board[1, 4] = Disk.Empty; board[2, 4] = Disk.Red;   board[3, 4] = Disk.Red; ; board[4, 4] = Disk.Empty; board[5, 4] = Disk.Blue; board[6, 4] = Disk.Blue;
            board[0, 5] = Disk.Blue;  board[1, 5] = Disk.Blue;  board[2, 5] = Disk.Red;   board[3, 5] = Disk.Blue;  board[4, 5] = Disk.Empty; board[5, 5] = Disk.Blue;  board[6, 5] = Disk.Red;;

            nextToPlay = Disk.Red;
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
                    if(!board.GetAvailableMoves().Any())
                    {
                        Console.WriteLine("Game over - no more moves");
                        break;
                    }
                }

                bool gotValidChoice = false;
                var progress = new ConsoleProgressReport();

                while (!gotValidChoice)
                {
                    int move;
                    if (nextToPlay == Disk.Red)
                    {
                        // AI plays for Red
                        //move = GameEngine.GetBestMoveBasic(board, nextToPlay);

                        var evalResultWithTimer = GameEngine.NegaMax(board, nextToPlay, 12, progress);
                        move = evalResultWithTimer.evalResult.Move;

                        Console.WriteLine($"AI chose {evalResultWithTimer.evalResult.Move}.");

                        if (evalResultWithTimer.forcedMove)
                        {
                            Console.WriteLine("Forced move.");
                        }
                        else
                        {
                            Console.WriteLine($"Score = {evalResultWithTimer.evalResult.Score}. Elapsed time = {(int)evalResultWithTimer.elapsedTime.TotalMilliseconds} ms. ");
                            if (evalResultWithTimer.evalResult.Score == GameEngine.WinValue)
                            {
                                Console.WriteLine($"AI will win.");
                            }
                            else if (evalResultWithTimer.evalResult.Score == -GameEngine.WinValue)
                            {
                                Console.WriteLine($"AI might lose if you play right.");
                            }
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

                        var availableMoves = board.GetAvailableMoves();
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
