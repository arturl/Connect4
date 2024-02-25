using Connect4AIEngine;

namespace ConnectAITests
{
    public class BoardTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BasicPlacement01()
        {
            var board = new Board();
            Assert.That(board[2,3], Is.EqualTo(Disk.Empty));
            board[2, 3] = Disk.Red;
            Assert.That(board[2, 3], Is.EqualTo(Disk.Red));
        }

        public void DropDiskAt01()
        {
            var board = new Board();
            Assert.That(board[2, 0], Is.EqualTo(Disk.Empty));
            board.DropDiskAt(Disk.Red, 2);

            Assert.That(board[2, 0], Is.EqualTo(Disk.Red));
        }

        [Test]
        public void DeadlySpotTest01()
        {
            var board = new Board();

            board.DropDiskAt(Disk.Red, 2);
            board.DropDiskAt(Disk.Red, 3);

            board.DropDiskAt(Disk.Blue, 0);
            board.DropDiskAt(Disk.Blue, 2);
            board.DropDiskAt(Disk.Blue, 2);
            board.DropDiskAt(Disk.Blue, 3);
            board.DropDiskAt(Disk.Blue, 3);
            board.DropDiskAt(Disk.Blue, 3);

            board.PrintToConsole();

            bool isDeadly = board.IsDeadlySpotFor(Disk.Red, 1);

            Assert.IsTrue(isDeadly);
        }
    }
}