using System.Diagnostics;
using System.Drawing;

namespace Connect4AIEngine
{
    public interface IProgressReport
    {
        void TotalMoves(int moves);
        void UpdateProgress();
        void EndProgress();
    }

    public class DoNothingProgressReport : IProgressReport
    {
        public void TotalMoves(int moves) { }

        public void UpdateProgress() { }

        public void EndProgress() { }
    }

    public class GameEngine
    {
		public const int WinValue = 100;
        private const int ProgressReportDepth = 1;

        public static EvalResult GetBestMoveBasic(Board board, Disk disk)
        {
            // Level 1: 1 move

            var possible_moves = board.GetAvailableMoves();

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
            }

            foreach (var col in possible_moves)
            {
                Disk possibleWinner = Disk.Empty;
                string dummy = string.Empty;

                // Will the opponent win if they place there?
                var newBoard2 = board.Clone();
                var opponent = disk.Opposite();
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

        private static int GetAvailableMovesAtDepth(Board board, Disk color, int depth)
        {
            var moves = board.GetAvailableMovesExcludingWins();
            if(depth == 0)
            {
                return moves.Count;
            }
            int total = 0;
            foreach(var move in moves)
            {
                var child = board.Clone();
                child.DropDiskAt(color, move);
                Disk opposite = color.Opposite();
                total += GetAvailableMovesAtDepth(board, opposite, depth - 1);
            }
            return total;
        }

        public static EvalResultWithTime NegaMax(Board board, Disk color, int depth, IProgressReport progress)
        {
            EvalResultWithTime result = new EvalResultWithTime();
            var stopWatch = Stopwatch.StartNew();

            int reportingDepth = 1;
            if (depth >= reportingDepth)
            {
                var moveCount = GetAvailableMovesAtDepth(board, color, reportingDepth);
                progress.TotalMoves(moveCount);
            }

            var simple = GetBestMoveBasic(board, color);
            if (simple.Move == -1)
            {
                // Need full analysis
                EvalResult evalResult = NegaMaxWorker(board, color, depth > reportingDepth ? depth - reportingDepth : -1, depth, -999, 999, progress);

                if(evalResult.Score == -WinValue && depth > 0)
                {
                    // AI might lose. Try a move that will not lose immediately
                    for (var lowerLevel = depth-1; lowerLevel >0; lowerLevel--)
                    {
                        var lowerDepthevalResult = NegaMaxWorker(board, color, 0, lowerLevel, -999, 999, new DoNothingProgressReport());
                        if (lowerDepthevalResult.Score >= 0)
                        {
                            evalResult.Move = lowerDepthevalResult.Move;
                            break;
                        }
                    }
                }

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

            if (depth >= reportingDepth)
            {
                progress.EndProgress();
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

            int deadlySpotsForPlayer = 0;
            int deadlySpotsForOpponent = 0;
            Disk opponent = player.Opposite();
            for(int col=0; col<7; col++)
            {
                if (board.IsDeadlySpotFor(player, col)) deadlySpotsForPlayer++;
                if (board.IsDeadlySpotFor(opponent, col)) deadlySpotsForOpponent++;
            }

            return deadlySpotsForOpponent - deadlySpotsForPlayer;
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

        private static EvalResult NegaMaxWorker(Board board, Disk color, int reportingDepth, int currentDepth, int alpha, int beta, IProgressReport progress)
        {
            var possible_moves = board.GetAvailableMoves();

            bool terminate;

            if (currentDepth == 0)
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
            int i = 0;
            foreach (var move in possible_moves)
            {
                var child = board.Clone();
                child.DropDiskAt(color, move);

                if(currentDepth == reportingDepth)
                {
                    progress.UpdateProgress();
                }

                Disk opposite = color.Opposite();
                EvalResult evalResult = NegaMaxWorker(child, opposite, reportingDepth, currentDepth - 1, -beta, -alpha, progress);
                int score = -evalResult.Score;

                if (score > value)
                {
                    value = score;
                    best_move = move;
                }

                alpha = int.Max(alpha, value);
                if (alpha >= beta)
                {
                    // Report progress for all skipped moves
                    if (currentDepth == reportingDepth)
                    {
                        for (int j = i+1; j < possible_moves.Count; j++)
                        {
                            progress.UpdateProgress();
                        }
                    }
                    break; // cut-off
                }
                i++;
            }

            return new EvalResult { Score = value, Move = best_move };
        }
    }
}
