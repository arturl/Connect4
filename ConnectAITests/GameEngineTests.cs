using Connect4AIEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static Connect4AIEngine.GameEngine;

namespace ConnectAITests
{
    internal class GameEngineTests
    {
        [Test]
        public void BasicPlacement01()
        {
            var board = new Board();
            Assert.That(board[2, 3], Is.EqualTo(Disk.Empty));
            board[2, 3] = Disk.Red;
            Assert.That(board[2, 3], Is.EqualTo(Disk.Red));
        }

        private int GetBestMoveFork01(int level)
        {
            string game = "B3R3B3R3B2R3B2";
            var board = new Board(game);
            EvalResultWithTime evalResult;
            evalResult = GameEngine.NegaMax(board, Disk.Red, level, new DoNothingProgressReport());
            return evalResult.evalResult.Move;
        }


        [Test]
        public void BasicForkDetection01()
        {
            Assert.That(GetBestMoveFork01(3), Is.EqualTo(3));
            Assert.That(GetBestMoveFork01(4), Is.EqualTo(4));
            Assert.That(GetBestMoveFork01(5), Is.EqualTo(4));
            Assert.That(GetBestMoveFork01(6), Is.EqualTo(4));
        }

        [Test]
        public void DetectForcedWin()
        {
            string game = "B3R3B3R3B3R3B1R2B2R2B2R2B5R1B4";
            var board = new Board(game);
            EvalResultWithTime evalResult;
            evalResult = GameEngine.NegaMax(board, Disk.Red, 7, new DoNothingProgressReport());

            var move = evalResult.evalResult.Move;

            Assert.That(move, Is.EqualTo(0));
        }
    }
}
