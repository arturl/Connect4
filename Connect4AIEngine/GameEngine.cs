using System.Diagnostics;
using System.Drawing;

namespace Connect4AIEngine
{
    public class GameEngine
    {
		public const int WinValue = 100;
        public static EvalResult GetBestMoveBasic(Board board, Disk disk)
        {
            // Level 1: 1 move

            var possible_moves = board.GetAvailableMovesForPlayer();

            foreach (var col in possible_moves)
            {
                Disk possibleWinner = Disk.Empty;
                string dummy = string.Empty;

                // Will I win if I place there?
                var newBoard = board.Clone();
                newBoard.DropDiskAt(disk, col);
                if (newBoard.IsWinReached(ref possibleWinner, ref dummy))
                {
                    // I will win if I place here, yay!
                    return new EvalResult { Move = col, Score = WinValue };
                }

                // Will the opponent win if they place there?
                var newBoard2 = board.Clone();
                var opponent = (disk == Disk.Red) ? Disk.Blue : Disk.Red;
                newBoard2.DropDiskAt(opponent, col);
                if (newBoard2.IsWinReached(ref possibleWinner, ref dummy))
                {
                    // don't let them - place there myself
                    return new EvalResult { Move = col, Score = -WinValue };
                }
            }

            // More analysis needed
            return new EvalResult { Move = -1, Score = 0 };
        }

        public struct EvalResult
        {
            public int Score;
            public int Move;
        }

        public struct EvalResultWithTime
        {
            public EvalResult evalResult;
            public TimeSpan elapsedTime;
            public bool forcedMove;
        }

        public static EvalResultWithTime NegaMax(Board board, Disk color, int depth)
        {
            EvalResultWithTime result = new EvalResultWithTime();
            var stopWatch = Stopwatch.StartNew();

            var simple = GetBestMoveBasic(board, color);
            if (simple.Move == -1)
            {
                // Need full analysis
                EvalResult evalResult = NegaMaxWorker(board, color, depth, -999, 999);

                result.evalResult = evalResult;
                result.elapsedTime = stopWatch.Elapsed;
                result.forcedMove = false;
            }
            else
            {
                result.evalResult = simple;
                result.elapsedTime = stopWatch.Elapsed;
                result.forcedMove = true;
            }
            return result;
        }

        private static int Eval(Board board, Disk player)
        {
            Disk winner = Disk.Empty;
            string dummy = string.Empty;
            bool isWinReached = board.IsWinReached(ref winner, ref dummy);
            if (isWinReached)
            {
                if (winner == player) return WinValue;
                return - WinValue;
            }
            return 0;
        }

        private static int EvalForOrdering2(Board board, int move, Disk color)
        {
            if (board[move, 5] == Disk.Empty) return 1;
            return 0;
        }

        private static void OrderMoves(Board board, List<int> moves, Disk color)
        {
            moves = [.. moves.OrderBy(move => EvalForOrdering2(board, move, color))];
        }

        private static EvalResult NegaMaxWorker(Board board, Disk color, int depth, int alpha, int beta)
        {
            var possible_moves = board.GetAvailableMovesForPlayer();

            bool terminate;

            if (depth == 0)
            {
                terminate = true;
            }
            else
            {
                if (!possible_moves.Any())
                {
                    terminate = true;
                }
                else
                {
                    Disk winner = Disk.Empty;
                    string dummy = string.Empty;
                    if (board.IsWinReached(ref winner, ref dummy))
                    {
                        terminate = true;
                    }
                    else
                    {
                        terminate = false;
                    }
                }
            }

            if (terminate)
            {
                var score = Eval(board, color);
                if (!possible_moves.Any())
                {
                    return new EvalResult { Score = score, Move = -1 };
                }
                // Pick the first move among the possible ones
                var index = 0;
                return new EvalResult { Score = score, Move = possible_moves[index] };
            }

            var value = -999;
            int best_move = -1;

            // OrderMoves(board, possible_moves, color);

            foreach (var move in possible_moves)
            {
                var child = board.Clone();
                child.DropDiskAt(color, move);

                Disk opposite = color == Disk.Blue ? Disk.Red : Disk.Blue;
                EvalResult evalResult = NegaMaxWorker(child, opposite, depth - 1, -beta, -alpha);
                int score = -evalResult.Score;

                if (score > value)
                {
                    value = score;
                    best_move = move;
                }

                alpha = int.Max(alpha, value);
                if (alpha >= beta)
                {
                    break; // cut-off
                }
            }

            return new EvalResult { Score = value, Move = best_move };
        }
    }
}
