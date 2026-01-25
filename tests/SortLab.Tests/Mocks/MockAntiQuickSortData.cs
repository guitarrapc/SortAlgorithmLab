using System.Collections;

namespace SortLab.Tests;

/// <summary>
/// Generates anti-quicksort data based on McIlroy's algorithm.
/// Reference: https://www.cs.dartmouth.edu/~doug/mdmspe.pdf
/// This data is designed to cause quadratic behavior in naive quicksort implementations.
/// </summary>
public class MockAntiQuickSortData : IEnumerable<object[]>
{
    private List<object[]> testData = new List<object[]>();

    public MockAntiQuickSortData()
    {
        testData.Add([new InputSample<int>() { InputType = InputType.AntiQuickSort, Samples = GenerateAntiQuickSort(100) }]);
        testData.Add([new InputSample<int>() { InputType = InputType.AntiQuickSort, Samples = GenerateAntiQuickSort(1000) }]);
        testData.Add([new InputSample<int>() { InputType = InputType.AntiQuickSort, Samples = GenerateAntiQuickSort(10000) }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Generates an anti-quicksort array using McIlroy's algorithm.
    /// The algorithm works by tracking comparisons and assigning values that maximize the number of comparisons.
    /// </summary>
    private static int[] GenerateAntiQuickSort(int n)
    {
        var generator = new AntiQuickSortGenerator(n);
        return generator.Generate();
    }

    private class AntiQuickSortGenerator
    {
        private readonly int n;
        private readonly int[] val;
        private int nsolid;
        private int candidate;
        private readonly int gas;

        public AntiQuickSortGenerator(int size)
        {
            n = size;
            val = new int[n];
            gas = n - 1;
            nsolid = 0;
            candidate = 0;

            for (int i = 0; i < n; i++)
            {
                val[i] = gas;
            }
        }

        public int[] Generate()
        {
            // Create pointer array for indirect sorting
            var ptr = new int[n];
            for (int i = 0; i < n; i++)
            {
                ptr[i] = i;
            }

            // Sort using our custom comparison that creates anti-quicksort pattern
            Array.Sort(ptr, Comparer);
            return val;
        }

        private void Freeze(int x)
        {
            val[x] = nsolid++;
        }

        private int Comparer(int x, int y)
        {
            // x and y are indices into the val array
            if (val[x] == gas && val[y] == gas)
            {
                if (x == candidate)
                    Freeze(x);
                else
                    Freeze(y);
            }

            if (val[x] == gas)
                candidate = x;
            else if (val[y] == gas)
                candidate = y;

            return val[x] - val[y];
        }
    }
}
