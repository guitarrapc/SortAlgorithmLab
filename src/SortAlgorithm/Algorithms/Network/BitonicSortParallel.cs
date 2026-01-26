using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// バイトニックソート（並列版・2のべき乗専用） - バイトニック列を構築し、並列実行で再帰的にマージして整列列に変換するソーティングネットワークアルゴリズムです。
/// 入力サイズは2のべき乗（2^n）でなければなりません。対象の要素が大きい場合、並列実行を活用します。
/// <br/>
/// Bitonic Sort (Parallel, Power-of-2 Only) - A sorting network algorithm that builds a bitonic sequence and merges it in parallel using Parallel.For.
/// Input length must be a power of 2 (2^n). Leverages parallel execution for large datasets.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Bitonic Sequence Definition:</strong> A sequence is bitonic if it first monotonically increases then monotonically decreases,
/// or can be circularly rotated to achieve this property.</description></item>
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (n = 2^m for some integer m ≥ 0).
/// This ensures the divide-and-conquer structure maintains balanced splits at each recursive level.</description></item>
/// <item><description><strong>Recursive Bitonic Construction:</strong> Divide the input into two halves. Recursively sort the first half in ascending order
/// and the second half in descending order to form a bitonic sequence.</description></item>
/// <item><description><strong>Parallel Bitonic Merge:</strong> The compare-and-swap operations within BitonicMerge are independent and can be executed in parallel.
/// Uses Parallel.For to distribute work across multiple threads.</description></item>
/// <item><description><strong>Thread Safety:</strong> Since Parallel cannot capture ref struct (Span/SortSpan), this implementation accepts T[] array
/// and creates thread-local SortSpan instances within parallel loops for statistics tracking.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort / Exchange</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order of equal elements)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space - sorts directly on input array)</description></item>
/// <item><description>Sequential time: Θ(n log² n) - Same comparison count as non-parallel version</description></item>
/// <item><description>Parallel time: O(log³ n) - Theoretical parallel depth with O(n) processors</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly (log² n × (log n + 1)) / 4 × n comparisons for n = 2^k</description></item>
/// <item><description>Parallelism : High - Compare-and-swap operations at each merge level are independent</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Accepts T[] array instead of Span due to Parallel.For limitation with ref struct</description></item>
/// <item><description>Uses Parallel.For for compare-and-swap loops (threshold: 1024 elements minimum for parallelization)</description></item>
/// <item><description>Creates thread-local SortSpan instances within parallel regions for statistics tracking</description></item>
/// <item><description>Strictly requires power-of-2 input length. Throws ArgumentException otherwise.</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Large datasets (≥ 1024 elements) on multi-core systems</description></item>
/// <item><description>Scenarios where predictable parallel performance is required</description></item>
/// <item><description>When input size is guaranteed to be a power of 2</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Bitonic_sorter</para>
/// </remarks>
public static class BitonicSortParallel
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    // Threshold for parallelization - below this size, use sequential sorting
    private const int PARALLEL_THRESHOLD = 512;

    // Parallel options with max degree of parallelism set to number of processors
    private static readonly ParallelOptions parallelOptions = new ParallelOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount,
    };

    /// <summary>
    /// Sorts the elements in the specified array in ascending order using the default comparer.
    /// Uses parallel execution for improved performance on multi-core systems.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">The array of elements to sort in place.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not a power of 2.</exception>
    public static void Sort<T>(T[] array) where T : IComparable<T>
    {
        Sort(array, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified array using the provided sort context and parallel execution.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">The array of elements to sort. The elements within this array will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the array length is not a power of 2.</exception>
    public static void Sort<T>(T[] array, ISortContext context) where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(context);

        if (array.Length <= 1) return;

        // Verify that length is a power of 2
        if (!IsPowerOfTwo(array.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {array.Length}", nameof(array));

        SortCore(array, 0, array.Length, ascending: true, context);
    }

    /// <summary>
    /// Recursively builds and merges a bitonic sequence with parallel execution.
    /// </summary>
    /// <param name="array">The array to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort in ascending order, false for descending.</param>
    /// <param name="context">The sort context for statistics tracking.</param>
    internal static void SortCore<T>(T[] array, int low, int count, bool ascending, ISortContext context) where T : IComparable<T>
    {
        if (count > 1)
        {
            int k = count / 2;

            // For large enough sequences, use parallel tasks for left and right halves
            if (count >= PARALLEL_THRESHOLD * 2)
            {
                Parallel.Invoke(
                    parallelOptions,
                    () => SortCore(array, low, k, ascending: true, context),
                    () => SortCore(array, low + k, k, ascending: false, context)
                );
            }
            else
            {
                // Recursively sort first half in ascending order
                SortCore(array, low, k, ascending: true, context);
                // Recursively sort second half in descending order to create bitonic sequence
                SortCore(array, low + k, k, ascending: false, context);
            }

            // Merge the bitonic sequence in the desired order
            BitonicMerge(array, low, count, ascending, context);
        }
    }

    /// <summary>
    /// Merges a bitonic sequence into a sorted sequence with parallel execution.
    /// </summary>
    /// <param name="array">The array containing the bitonic sequence.</param>
    /// <param name="low">The starting index of the bitonic sequence.</param>
    /// <param name="count">The length of the bitonic sequence.</param>
    /// <param name="ascending">True to merge in ascending order, false for descending.</param>
    /// <param name="context">The sort context for statistics tracking.</param>
    private static void BitonicMerge<T>(T[] array, int low, int count, bool ascending, ISortContext context) where T : IComparable<T>
    {
        if (count > 1)
        {
            int k = count / 2;

            // Parallelize the compare-and-swap loop if count is large enough
            if (count >= PARALLEL_THRESHOLD)
            {
                Parallel.For(low, low + k, parallelOptions, i =>
                {
                    // Create thread-local SortSpan for this iteration
                    var span = new SortSpan<T>(array.AsSpan(), context, BUFFER_MAIN);
                    CompareAndSwap(span, i, i + k, ascending);
                });
            }
            else
            {
                // Sequential compare-and-swap for small sequences
                var span = new SortSpan<T>(array.AsSpan(), context, BUFFER_MAIN);
                for (int i = low; i < low + k; i++)
                {
                    CompareAndSwap(span, i, i + k, ascending);
                }
            }

            // Recursively merge both halves (can also be parallelized)
            if (k >= PARALLEL_THRESHOLD)
            {
                Parallel.Invoke(
                    () => BitonicMerge(array, low, k, ascending, context),
                    () => BitonicMerge(array, low + k, k, ascending, context)
                );
            }
            else
            {
                BitonicMerge(array, low, k, ascending, context);
                BitonicMerge(array, low + k, k, ascending, context);
            }
        }
    }

    /// <summary>
    /// Compares two elements and swaps them if they are in the wrong order.
    /// </summary>
    /// <param name="span">The span containing the elements.</param>
    /// <param name="i">The index of the first element.</param>
    /// <param name="j">The index of the second element.</param>
    /// <param name="ascending">True if elements should be in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CompareAndSwap<T>(SortSpan<T> span, int i, int j, bool ascending) where T : IComparable<T>
    {
        int cmp = span.Compare(i, j);

        // If ascending and i > j, or if descending and i < j, then swap
        if ((ascending && cmp > 0) || (!ascending && cmp < 0))
        {
            span.Swap(i, j);
        }
    }

    /// <summary>
    /// Checks if a number is a power of 2.
    /// </summary>
    /// <param name="n">The number to check.</param>
    /// <returns>True if n is a power of 2, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }
}
