namespace SortAlgorithm.Tests;

/// <summary>
/// Generates worst-case patterns specifically designed for middle-pivot QuickSort implementations.
/// These patterns create highly unbalanced partitions, leading to O(n²) behavior.
/// </summary>
/// <remarks>
/// Middle-pivot QuickSort (selecting pivot at (left + right) / 2) performs poorly on:
/// 1. Alternating patterns (sawtooth)
/// 2. Sorted arrays with repeated median values
/// 3. Zigzag patterns that create unbalanced partitions
/// </remarks>
public static class MockQuickSortWorstCaseData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        // Pattern 1: Sawtooth - alternating low/high values
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateSawtoothPattern(100)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateSawtoothPattern(1000)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateSawtoothPattern(10000)
        };

        // Pattern 2: Pipe organ - creates poor middle pivot choices
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GeneratePipeOrganPattern(100)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GeneratePipeOrganPattern(1000)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GeneratePipeOrganPattern(10000)
        };

        // Pattern 3: Interleaved halves - splits poorly with middle pivot
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateInterleavedPattern(100)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateInterleavedPattern(1000)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = QuickSortWorstCaseDataGenerator.GenerateInterleavedPattern(10000)
        };
    }

    public class QuickSortWorstCaseDataGenerator
    {
        /// <summary>
        /// Sawtooth pattern: [0, n-1, 1, n-2, 2, n-3, ...]
        /// Middle element will be either a small or large value, creating unbalanced partitions.
        /// </summary>
        public static int[] GenerateSawtoothPattern(int n)
        {
            var array = new int[n];
            int low = 0, high = n - 1;
            for (int i = 0; i < n; i++)
            {
                array[i] = (i % 2 == 0) ? low++ : high--;
            }
            return array;
        }

        /// <summary>
        /// Pipe organ pattern: [0, 1, 2, ..., n/2, n/2-1, ..., 2, 1, 0]
        /// Creates a mountain shape where the middle pivot is always near the peak,
        /// resulting in one small partition and one large partition.
        /// </summary>
        public static int[] GeneratePipeOrganPattern(int n)
        {
            var array = new int[n];
            int half = n / 2;

            // Ascending to middle
            for (int i = 0; i < half; i++)
            {
                array[i] = i;
            }

            // Descending from middle
            for (int i = half; i < n; i++)
            {
                array[i] = n - 1 - i;
            }

            return array;
        }

        /// <summary>
        /// Interleaved halves: [0, n/2, 1, n/2+1, 2, n/2+2, ...]
        /// The middle element alternates between being near min or max of its partition,
        /// creating consistently poor pivot choices.
        /// </summary>
        public static int[] GenerateInterleavedPattern(int n)
        {
            var array = new int[n];
            int half = n / 2;

            for (int i = 0; i < half; i++)
            {
                if (i * 2 < n)
                    array[i * 2] = i;
                if (i * 2 + 1 < n)
                    array[i * 2 + 1] = half + i;
            }

            // Fill remaining elements if n is odd
            if (n % 2 != 0)
            {
                array[n - 1] = n - 1;
            }

            return array;
        }

        /// <summary>
        /// Sorted with duplicates at median positions: [1,1,1,...,2,2,2,...,3,3,3,...]
        /// Middle pivot will be a repeated value, causing poor partitioning.
        /// </summary>
        private static int[] GenerateRepeatedMedianPattern(int n)
        {
            var array = new int[n];
            int groups = (int)Math.Sqrt(n);
            int groupSize = n / groups;

            for (int i = 0; i < n; i++)
            {
                array[i] = i / groupSize;
            }

            return array;
        }
    }
}
