using Connect4Console;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Connect4Console
{
    public class GameEngine
    {
        public static int GetBestMoveBasic(Board board, Disk disk)
        {
            // Level 1: 1 move

            for(int col=0; col<Board.Width; col++)
            {
                Disk possibleWinner = Disk.Empty;
                string dummy = string.Empty;

                // Will I win if I place there?
                var newBoard = board.Clone();
                newBoard.DropDiskAt(disk, col);
                if (newBoard.IsWinReached(ref possibleWinner, ref dummy))
                {
                    // I will win if I place here, yay!
                    Console.WriteLine($"Found win at {col}");
                    return col;
                }

                // Will the opponent win if they place there?
                var newBoard2 = board.Clone();
                var opponent = (disk == Disk.Red) ? Disk.Blue : Disk.Red;
                newBoard2.DropDiskAt(opponent, col);
                if(newBoard2.IsWinReached(ref possibleWinner, ref dummy))
                {
                    Console.WriteLine($"Preventing opponent win at {col}");
                    // don't let them - place there myself
                    return col;
                }
            }

            // Not sure what to do - make a random move
            Console.WriteLine($"Fall back to l0");

            // Level 0 - random
            Random rnd = new Random();
            return rnd.Next(7);
        }

        public struct EvalResult
        {
            public int Score;
            public int Move;
        }

        public static EvalResult NegaMax(Board board, Disk color, int depth)
        {
            EvalResult evalResult = negamax_worker(board, color, depth, -100, 100);
            return evalResult;
        }

        private static int Eval(Board board, Disk player)
        {
            Disk winner = Disk.Empty;
            string dummy = string.Empty;
            bool isWinReached = board.IsWinReached(ref winner, ref dummy);
            if(isWinReached)
            {
                if (winner == player) return 1;
                return -1;
            }
            return 0;
        }

        private static EvalResult negamax_worker(Board board, Disk color, int depth, int alpha, int beta)
        {
            var possible_moves = board.GetAvailableMovesForPlayer(color);

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
                if(!possible_moves.Any())
                {
                    return new EvalResult { Score = score, Move = -1 };
                }
                // Pick the first move among the possible ones
                var index = 0;
                return new EvalResult { Score = score, Move = possible_moves[index] };
            }

            var value = -100;
            int best_move = -1;

            foreach(var move in possible_moves)
            {
                var child = board.Clone();
                child.DropDiskAt(color, move);

                Disk opposite = color == Disk.Blue ? Disk.Red : Disk.Blue;
                EvalResult evalResult = negamax_worker(child, opposite, depth - 1, -beta, -alpha);
                int score = -evalResult.Score;

                if(score > value) {
                    value = score;
                    best_move = move;
                }

                alpha = int.Max(alpha, value);
                if(alpha >= beta)
                {
                    break; // cut-off
                }
            }

            return new EvalResult { Score = value, Move = best_move };
        }
    }
}
