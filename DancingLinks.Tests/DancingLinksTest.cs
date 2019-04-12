namespace DancingLinks.Tests
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DancingLinksTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void DancingLinksConversionTest1()
        {
            var sut = new DancingLinks();

            var grid = new[]
            {
                new[] {5, 3, 0, 0, 7, 0, 0, 0, 0},
                new[] {6, 0, 0, 1, 9, 5, 0, 0, 0},
                new[] {0, 9, 8, 0, 0, 0, 0, 6, 0},
                new[] {8, 0, 0, 0, 6, 0, 0, 0, 3},
                new[] {4, 0, 0, 8, 0, 3, 0, 0, 1},
                new[] {7, 0, 0, 0, 2, 0, 0, 0, 6},
                new[] {0, 6, 0, 0, 0, 0, 2, 8, 0},
                new[] {0, 0, 0, 4, 1, 9, 0, 0, 5},
                new[] {0, 0, 0, 0, 8, 0, 0, 7, 9}
            };

            var solution = sut.SolveSudoku((3, 3), grid).ToList();

            // should be one and only one solution for a valid Sudoku
            Assert.AreEqual(1, solution.Count);

            solution.First().Dump(TestContext.WriteLine, @"9x9 Solution");
        }

        [TestMethod]
        public void BuildSetMatrixKeysTest1()
        {
            var sut = new DancingLinks();

            var actualXKeys = sut.BuildSetMatrixKeys(4);

            Assert.AreEqual(64, actualXKeys.Count);

            actualXKeys.Dump(TestContext.WriteLine, $"Dumping actualXKeys, item count = {actualXKeys.Count}");
        }

        [TestMethod]
        public void BuildSubsetMatrixTest1()
        {
            var sut = new DancingLinks();

            var actualY = sut.BuildSubsetMatrix(4, 2, 2);

            Assert.AreEqual(64, actualY.Count);

            actualY.Dump(TestContext.WriteLine, $"Dumping actualY, item count = {actualY.Count}");
        }

        [TestMethod]
        public void ExactCoverTest()
        {
            var sut = new DancingLinks();
            var xKeys = sut.BuildSetMatrixKeys(4);
            var y = sut.BuildSubsetMatrix(4, 2, 2);

            var x = sut.ExactCover(xKeys, y);

            x.Dump(TestContext.WriteLine, $"Dumping x, item count = {x.Count}");
        }

        [TestMethod]
        public void LoadCaseToSolveTest()
        {
            var grid = new[]
            {
                new[] {1, 0, 3, 4},
                new[] {3, 4, 1, 2},
                new[] {2, 1, 4, 3},
                new[] {4, 3, 2, 1}
            };

            var sut = new DancingLinks();
            var xKeys = sut.BuildSetMatrixKeys(4);
            var y = sut.BuildSubsetMatrix(4, 2, 2);
            var x = sut.ExactCover(xKeys, y);

            sut.LoadCaseToSolve(grid, x, y);

            x.Dump(TestContext.WriteLine, $"Dumping x, item count = {x.Count}");
        }

        [TestMethod]
        public void SolveTest()
        {
            var grid = new[]
            {
                new[] {1, 0, 3, 4},
                new[] {3, 4, 1, 2},
                new[] {2, 1, 4, 3},
                new[] {4, 3, 2, 1}
            };

            var sut = new DancingLinks();
            var xKeys = sut.BuildSetMatrixKeys(4);
            var y = sut.BuildSubsetMatrix(4, 2, 2);
            var x = sut.ExactCover(xKeys, y);

            sut.LoadCaseToSolve(grid, x, y);

            var solution = sut.Solve(x, y).First();

            solution.Dump(TestContext.WriteLine, $"Dumping solution, item count = {solution.Count}");
        }
    }
}