namespace DancingLinks
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A C# tuple based Sudoku Solver based upon "Algorithm X in 30 lines!" by Ali Assaf
    /// More information can be found at: https://www.cs.mcgill.ca/~aassaf9/python/algorithm_x.html
    /// </summary>
    public class DancingLinks
    {
        public IEnumerable<int[][]> SolveSudoku((int x, int y) boxSize, int[][] grid)
        {
            var (gridRows, gridCols) = boxSize;
            var size = gridRows * gridCols;

            var xKeys = BuildSetMatrixKeys(size);

            var y = BuildSubsetMatrix(size, gridRows, gridCols);
            var x = ExactCover(xKeys, y);

            LoadCaseToSolve(grid, x, y);

            foreach (var solution in Solve(x, y))
            {
                // fill in the solution in original problem grid and then display
                var solvedGrid = CopyGrid(grid);

                foreach (var (r2, c2, n3) in solution)
                {
                    solvedGrid[r2][c2] = n3;
                }

                yield return solvedGrid;
            }
        }

        private int[][] CopyGrid(int[][] grid)
        {
            var rv = new List<int[]>();

            foreach (var array in grid)
            {
                var list = new List<int>();
                foreach (var num in array)
                {
                    list.Add(num);
                }

                rv.Add(list.ToArray());
            }

            return rv.ToArray();
        }

        public void LoadCaseToSolve(int[][] grid, Dictionary<(string, (int, int)), HashSet<(int, int, int)>> x,
            Dictionary<(int, int, int), List<(string, (int, int))>> y)
        {
            foreach (var (i, row) in Enumerate(grid))
            {
                foreach (var (j, n2) in Enumerate(row))
                {
                    if (n2 != 0)
                    {
                        Select(x, y, (i, j, n2));
                    }
                }
            }
        }

        public Dictionary<(int, int, int), List<(string, (int, int))>> BuildSubsetMatrix(int size, int gridRows,
            int gridCols)
        {
            var y = new Dictionary<(int, int, int), List<(string, (int, int))>>();

            foreach (var item in Product(Enumerable.Range(0, size), Enumerable.Range(0, size),
                Enumerable.Range(1, size)))
            {
                var (r1, c1, n1) = item;

                // ReSharper disable ArrangeRedundantParentheses
                var b = (r1 / gridRows) * gridRows + (c1 / gridCols); // box number
                // ReSharper restore ArrangeRedundantParentheses

                y[(r1, c1, n1)] = new List<(string, (int, int))>
                {
                    ("rc", (r1, c1)),
                    ("rn", (r1, n1)),
                    ("cn", (c1, n1)),
                    ("bn", (b, n1))
                };
            }

            return y;
        }

        public List<(string, (int, int))> BuildSetMatrixKeys(int size)
        {
            var xKey = new List<(string, (int, int))>(Comprehension("rc",
                Product(Enumerable.Range(0, size), Enumerable.Range(0, size))));
            xKey.AddRange(Comprehension("rn", Product(Enumerable.Range(0, size), Enumerable.Range(1, size))));
            xKey.AddRange(Comprehension("cn", Product(Enumerable.Range(0, size), Enumerable.Range(1, size))));
            xKey.AddRange(Comprehension("bn", Product(Enumerable.Range(0, size), Enumerable.Range(1, size))));

            return xKey;
        }

        public Dictionary<(string, (int, int)), HashSet<(int, int, int)>> ExactCover(
            List<(string, (int, int))> xKey, Dictionary<(int, int, int), List<(string, (int, int))>> y)
        {
            // X = {j: set() for j in X}
            var rv = new Dictionary<(string, (int, int)), HashSet<(int, int, int)>>();
            foreach (var key in xKey)
            {
                rv[key] = new HashSet<(int, int, int)>();
            }

            // for i, row in Y.items():
            foreach (var yItem in y)
            {
                var i = yItem.Key;
                var row = yItem.Value;

                foreach (var j in row)
                {
                    rv[j].Add(i);
                }
            }

            return rv;
        }

        public IEnumerable<List<(int r2, int c2, int n3)>> Solve(
            Dictionary<(string, (int, int)), HashSet<(int, int, int)>> x,
            Dictionary<(int, int, int), List<(string, (int, int))>> y, List<(int r2, int c2, int n3)> solution = null)
        {
            solution = solution ?? new List<(int r2, int c2, int n3)>();

            if (!x.Any())
            {
                yield return solution;
            }
            else
            {
                // c = min(X, key= lambda c: len(X[c]))
                var list = x.OrderBy(i => i.Value.Count).First().Value;

                // for r in list(X[c]) :
                foreach (var r in list)
                {
                    solution.Add(r);

                    var cols = Select(x, y, r);

                    foreach (var s in Solve(x, y, solution))
                    {
                        yield return s;
                    }

                    Deselect(x, y, r, cols);

                    // solution.pop()
                    if (solution.Any())
                    {
                        solution.RemoveAt(solution.Count - 1);
                    }
                }
            }
        }

        private List<HashSet<(int, int, int)>> Select(Dictionary<(string, (int, int)), HashSet<(int, int, int)>> x,
            Dictionary<(int, int, int), List<(string, (int, int))>> y, (int, int, int) r)
        {
            var cols = new List<HashSet<(int, int, int)>>();

            foreach (var j in y[r])
            {
                foreach (var i in x[j])
                {
                    foreach (var k in y[i])
                    {
                        if (!k.Equals(j))
                        {
                            x[k].Remove(i);
                        }
                    }
                }

                // cols.append(X.pop(j))
                if (x.ContainsKey(j))
                {
                    var item = x[j];
                    x.Remove(j);
                    cols.Add(item);
                }
                else
                {
                    cols.Add(new HashSet<(int, int, int)>());
                }
            }

            return cols;
        }

        private void Deselect(Dictionary<(string, (int, int)), HashSet<(int, int, int)>> x,
            Dictionary<(int, int, int), List<(string, (int, int))>> y, (int, int, int) r,
            List<HashSet<(int, int, int)>> cols)
        {
            var list = y[r].ToList();
            list.Reverse();
            foreach (var j in list)
            {
                // X[j] = cols.pop()
                x[j] = cols.LastOrDefault() ?? new HashSet<(int, int, int)>();
                if (cols.Any())
                {
                    cols.RemoveAt(cols.Count - 1);
                }

                foreach (var i in x[j])
                {
                    foreach (var k in y[i])
                    {
                        if (!k.Equals(j))
                        {
                            x[k].Add(i);
                        }
                    }
                }
            }
        }

        private static List<(T, T)> Product<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            var bList = b is List<T> list1 ? list1 : b.ToList();

            IEnumerable<(T, T)> InnerProduct()
            {
                foreach (var itemA in a)
                {
                    foreach (var itemB in bList)
                    {
                        yield return (itemA, itemB);
                    }
                }
            }

            return InnerProduct().ToList();
        }

        private List<(T, T, T)> Product<T>(IEnumerable<T> a, IEnumerable<T> b, IEnumerable<T> c)
        {
            var bList = b is List<T> list1 ? list1 : b.ToList();
            var cList = c is List<T> list2 ? list2 : c.ToList();

            IEnumerable<(T, T, T)> InnerProduct()
            {
                foreach (var itemA in a)
                {
                    foreach (var itemB in bList)
                    {
                        foreach (var itemC in cList)
                        {
                            yield return (itemA, itemB, itemC);
                        }
                    }
                }
            }

            return InnerProduct().ToList();
        }

        private IEnumerable<(T1, T2)> Comprehension<T1, T2>(T1 leader, IEnumerable<T2> data)
        {
            foreach (var item in data)
            {
                yield return (leader, item);
            }
        }

        private IEnumerable<(int, T)> Enumerate<T>(IEnumerable<T> list)
        {
            var counter = 0;

            foreach (var item in list)
            {
                yield return (counter, item);
                counter++;
            }
        }

        private static string ToString((string, (int, int)) k)
        {
            return $"{k.Item1}:{k.Item2.Item1},{k.Item2.Item2}";
        }
    }
}
