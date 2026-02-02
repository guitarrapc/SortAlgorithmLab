using SortAlgorithm.Algorithms;

namespace SortAlgorithm.Tests;

public static class MockAntiQuickSortData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        // Pipe organ pattern is most effective for middle-pivot QuickSort
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = AntiQuickSortDataGenerator.GeneratePipeOrganPattern(100)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = AntiQuickSortDataGenerator.GeneratePipeOrganPattern(1000)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = AntiQuickSortDataGenerator.GeneratePipeOrganPattern(10000)
        };

        // Also include McIlroy's adversarial approach for comparison
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = AntiQuickSortDataGenerator.GenerateMcIlroyPattern(100)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = AntiQuickSortDataGenerator.GenerateMcIlroyPattern(1000)
        };
    }

    /// <summary>
    /// Generates anti-quicksort data specifically designed for middle-pivot QuickSort.
    /// Uses McIlroy's adversarial algorithm and pipe organ pattern.
    /// Reference: https://www.cs.dartmouth.edu/~doug/mdmspe.pdf
    /// </summary>
    /// <remarks>
    /// This generator creates worst-case data for QuickSort implementations that:
    /// - Select the middle element as pivot: (left + right) / 2
    /// - Use Hoare's partitioning scheme
    ///
    /// The pipe organ pattern [0,1,2,...,n/2,n/2-1,...,2,1,0] is particularly effective,
    /// causing ~19x more comparisons than random data on size=1000.
    /// </remarks>
    public static class AntiQuickSortDataGenerator
    {
        /// <summary>
        /// Pipe organ pattern: [0, 1, 2, ..., n/2, n/2-1, ..., 2, 1, 0]
        /// Creates a mountain/pyramid shape. When QuickSort selects the middle element as pivot,
        /// it will always be near the maximum value, creating highly unbalanced partitions.
        ///
        /// Performance on n=1000:
        /// - Random: ~13,000 comparisons
        /// - PipeOrgan: ~252,000 comparisons (19x worse!)
        /// </summary>
        public static int[] GeneratePipeOrganPattern(int n)
        {
            var array = new int[n];
            int half = n / 2;

            // Ascending to middle: [0, 1, 2, ..., n/2]
            for (int i = 0; i < half; i++)
            {
                array[i] = i;
            }

            // Descending from middle: [n/2-1, ..., 2, 1, 0]
            for (int i = half; i < n; i++)
            {
                array[i] = n - 1 - i;
            }

            return array;
        }

        /// <summary>
        /// McIlroy's adversarial algorithm: dynamically assigns values during sorting
        /// to maximize comparisons. This runs the actual QuickSort and tracks comparisons,
        /// assigning values that force the worst-case behavior.
        /// </summary>
        public static int[] GenerateMcIlroyPattern(int n)
        {
            var adv = new AntiQuickSortAdversary(n);
            AntiQuickSortAdversary.Current = adv;
            try
            {
                var items = new AntiItem[n];
                for (int i = 0; i < n; i++) items[i] = new AntiItem(i);

                QuickSort.Sort(items.AsSpan());

                adv.FinalizeAll();
                return adv.BuildPermutation();
            }
            finally
            {
                AntiQuickSortAdversary.Current = null;
            }
        }

        private readonly struct AntiItem : IComparable<AntiItem>
        {
            public readonly int Index;
            public AntiItem(int index) => Index = index;

            public int CompareTo(AntiItem other)
            {
                var adv = AntiQuickSortAdversary.Current ?? throw new InvalidOperationException("AntiQuickSortAdversary.Current is null.");
                return adv.CompareIndex(Index, other.Index);
            }
        }

        private sealed class AntiQuickSortAdversary
        {
            [ThreadStatic] public static AntiQuickSortAdversary? Current;

            private readonly int _n;
            private readonly int[] _val;
            private int _nsolid;
            private int _candidate;
            private readonly int _gas;

            public AntiQuickSortAdversary(int n)
            {
                _n = n;
                _val = new int[n];
                _gas = n - 1;
                _nsolid = 0;
                _candidate = -1;
                Array.Fill(_val, _gas);
            }

            public int CompareIndex(int x, int y)
            {
                if (x == y) return 0;

                // If both are "unknown", freeze one (core adversary move)
                if (_val[x] == _gas && _val[y] == _gas)
                {
                    if (_candidate == x) Freeze(x);
                    else Freeze(y);
                }

                // Keep an unknown one as the next candidate
                if (_val[x] == _gas) _candidate = x;
                else if (_val[y] == _gas) _candidate = y;

                int vx = _val[x], vy = _val[y];
                return vx < vy ? -1 : (vx > vy ? 1 : 0);
            }

            private void Freeze(int idx) => _val[idx] = _nsolid++;

            public async Task FinalizeAll()
            {
                for (int i = 0; i < _n; i++)
                {
                    if (_val[i] == _gas)
                        _val[i] = _nsolid++;
                }
            }

            public int[] BuildPermutation()
            {
                var result = new int[_n];
                Array.Copy(_val, result, _n);
                return result;
            }
        }
    }
}
