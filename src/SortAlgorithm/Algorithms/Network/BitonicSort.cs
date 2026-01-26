using System.Runtime.CompilerServices;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// Bitonic Sort - A parallel sorting network algorithm that works on sequences of length 2^n.
/// Builds a bitonic sequence (monotonically increasing then decreasing, or vice versa) and then 
/// recursively merges it into a sorted sequence.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Bitonic Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Power-of-Two Requirement:</strong> The input length must be a power of 2 (2^n).
/// This ensures the divide-and-conquer structure creates balanced bitonic sequences at each level.</description></item>
/// <item><description><strong>Bitonic Sequence Construction:</strong> Recursively divide the sequence into halves,
/// sorting one half in ascending order and the other in descending order. This creates a bitonic sequence
/// (first increasing, then decreasing).</description></item>
/// <item><description><strong>Bitonic Merge:</strong> Given a bitonic sequence, compare and conditionally swap 
/// elements at distance k/2 apart (where k is the sequence length). This splits the sequence into two 
/// bitonic sequences, each half the length, where all elements in the first half are ≤ all elements in the second half.</description></item>
/// <item><description><strong>Recursive Merging:</strong> Recursively apply bitonic merge to each half until 
/// the entire sequence is sorted.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Network Sort</description></item>
/// <item><description>Stable      : No (swapping non-adjacent elements can change relative order)</description></item>
/// <item><description>In-place    : Yes (O(1) auxiliary space)</description></item>
/// <item><description>Best case   : O(n log² n) - Same as worst case (data-independent comparisons)</description></item>
/// <item><description>Average case: O(n log² n)</description></item>
/// <item><description>Worst case  : O(n log² n)</description></item>
/// <item><description>Comparisons : Θ(n log² n) - Exactly (n log² n)/2 comparisons (input-independent)</description></item>
/// <item><description>Parallel    : Highly parallelizable - comparisons at each level are independent</description></item>
/// </list>
/// <para><strong>Limitations (Current Implementation):</strong></para>
/// <list type="bullet">
/// <item><description>Only supports input lengths that are powers of 2</description></item>
/// <item><description>No parallel execution (sequential implementation)</description></item>
/// </list>
/// <para><strong>Use Cases:</strong></para>
/// <list type="bullet">
/// <item><description>Parallel sorting on GPU or multi-core systems</description></item>
/// <item><description>Hardware sorting networks</description></item>
/// <item><description>Educational purposes to understand sorting networks</description></item>
/// </list>
/// </remarks>
public static class BitonicSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    /// <exception cref="ArgumentException">Thrown when the span length is not a power of 2.</exception>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    /// <exception cref="ArgumentException">Thrown when the span length is not a power of 2.</exception>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        // Verify that length is a power of 2
        if (!IsPowerOfTwo(span.Length))
            throw new ArgumentException($"Bitonic sort requires input length to be a power of 2. Actual length: {span.Length}", nameof(span));

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        SortCore(s, 0, span.Length, ascending: true);
    }

    /// <summary>
    /// Recursively builds and merges a bitonic sequence.
    /// </summary>
    /// <param name="span">The span to sort.</param>
    /// <param name="low">The starting index of the sequence.</param>
    /// <param name="count">The length of the sequence.</param>
    /// <param name="ascending">True to sort in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SortCore<T>(SortSpan<T> span, int low, int count, bool ascending) where T : IComparable<T>
    {
        if (count > 1)
        {
            int k = count / 2;

            // Recursively sort first half in ascending order
            SortCore(span, low, k, ascending: true);
            // Recursively sort second half in descending order to create bitonic sequence
            SortCore(span, low + k, k, ascending: false);

            // Merge the bitonic sequence in the desired order
            BitonicMerge(span, low, count, ascending);
        }
    }

    /// <summary>
    /// Merges a bitonic sequence into a sorted sequence.
    /// </summary>
    /// <param name="span">The span containing the bitonic sequence.</param>
    /// <param name="low">The starting index of the bitonic sequence.</param>
    /// <param name="count">The length of the bitonic sequence.</param>
    /// <param name="ascending">True to merge in ascending order, false for descending.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void BitonicMerge<T>(SortSpan<T> span, int low, int count, bool ascending) where T : IComparable<T>
    {
        if (count > 1)
        {
            int k = count / 2;
            
            // Compare and swap elements at distance k apart
            for (int i = low; i < low + k; i++)
            {
                CompareAndSwap(span, i, i + k, ascending);
            }
            
            // Recursively merge both halves
            BitonicMerge(span, low, k, ascending);
            BitonicMerge(span, low + k, k, ascending);
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
