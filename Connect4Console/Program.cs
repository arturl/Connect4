using Connect4AIEngine;
using System.Diagnostics;
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

                EvalResult evalResult;

                using (var timer = new AITimer())
                {
                    evalResult = GameEngine.NegaMax(board, Disk.Red, 12);
                }

                Console.WriteLine($"Move={evalResult.Move}, Score={evalResult.Score}");

                Disk winner = Disk.Empty;
                string direction = string.Empty;
                bool b = board.IsWinReached(ref winner, ref direction);

                return;
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

                        var evalResultWithTimer = GameEngine.NegaMax(board, nextToPlay, 12);
                        move = evalResultWithTimer.evalResult.Move;

                        Console.WriteLine($"AI chose {evalResultWithTimer.evalResult.Move}. Score = {evalResultWithTimer.evalResult.Score}. Elapsed time = {evalResultWithTimer.elapsedTime}");
                        if(evalResultWithTimer.evalResult.Score > 0)
                        {
                            Console.WriteLine("AI will win");
                        }
                        else if (evalResultWithTimer.evalResult.Score < 0)
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
