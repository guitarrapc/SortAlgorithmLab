using SortLab.Core.Contexts;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 値の分布状況を数え上げることを利用してインデックスを導きソートします。
/// 各要素からキーを抽出し、その出現回数をカウントして累積和を計算し、正しい位置に配置する安定なソートアルゴリズムです。
/// キーの範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Sorts elements by counting the distribution of extracted keys.
/// A stable sorting algorithm that extracts keys, counts occurrences, and uses cumulative sums to place elements.
/// Very fast when the key range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Counting Sort (Generic, Key-based):</strong></para>
/// <list type="number">
/// <item><description><strong>Key Extraction:</strong> Each element must have a deterministic integer key obtained via the key selector function.
/// The key must be stable (same element always produces the same key).</description></item>
/// <item><description><strong>Range Determination:</strong> The algorithm finds min and max keys to determine the range [min, max].
/// A count array of size (max - min + 1) is allocated to track occurrences.</description></item>
/// <item><description><strong>Offset Normalization:</strong> Keys are normalized using offset = -min, mapping keys to array indices [0, range-1].
/// This allows handling negative keys correctly.</description></item>
/// <item><description><strong>Counting Phase:</strong> For each element, its key is extracted and countArray[key + offset] is incremented.
/// This records how many times each key appears.</description></item>
/// <item><description><strong>Cumulative Sum:</strong> The count array is transformed into cumulative counts.
/// countArray[i] becomes the number of elements with keys ≤ i, indicating the final position.</description></item>
/// <item><description><strong>Placement Phase:</strong> Elements are placed in reverse order (for stability).
/// For each element with key k, it is placed at position countArray[k + offset] - 1, then the count is decremented.</description></item>
/// <item><description><strong>Stability:</strong> Processing elements in reverse order ensures that elements with equal keys maintain their original relative order.</description></item>
/// <item><description><strong>Range Limitation:</strong> The key range must be reasonable (≤ {MaxCountArraySize}).
/// Excessive ranges cause memory allocation failures.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes (reverse-order placement preserves relative order)</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of keys)</description></item>
/// <item><description>Comparisons : 0 (No comparison operations between keys)</description></item>
/// <item><description>Time        : O(n + k) where k is the range of keys</description></item>
/// <item><description>Memory      : O(n + k)</description></item>
/// <item><description>Note        : キーの範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxCountArraySize}です。</description></item>
/// </list>
/// </remarks>
public static class CountingSort
{
    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for count arrays smaller than this

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function.
    /// </summary>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector) where T : IComparable<T>
    {
        Sort(span, keySelector, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using a key selector function and sort context.
    /// </summary>
    public static void Sort<T>(Span<T> span, Func<T, int> keySelector, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        // Rent arrays from ArrayPool for temporary storage
        var keysArray = ArrayPool<int>.Shared.Rent(span.Length);
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        int[]? rentedCountArray = null;

        try
        {
            var keys = keysArray.AsSpan(0, span.Length);

            // Find min/max and cache keys in single pass
            var min = int.MaxValue;
            var max = int.MinValue;

            for (var i = 0; i < span.Length; i++)
            {
                var key = keySelector(s.Read(i));
                keys[i] = key;
                if (key < min) min = key;
                if (key > max) max = key;
            }

            // If all keys are the same, no need to sort
            if (min == max) return;

            // Check for overflow and validate range
            long range = (long)max - (long)min + 1;
            if (range > int.MaxValue)
                throw new ArgumentException($"Key range is too large for CountingSort: {range}. Maximum supported range is {int.MaxValue}.");
            if (range > MaxCountArraySize)
                throw new ArgumentException($"Key range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using QuickSort or another comparison-based sort.");

            var offset = -min; // Offset to normalize keys to 0-based index
            var size = (int)range;

            // Use stackalloc for small count arrays, ArrayPool for larger ones
            Span<int> countArray = size <= StackAllocThreshold
                ? stackalloc int[size]
                : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
            countArray.Clear();

            SortCore(s, keys, tempArray.AsSpan(0, span.Length), countArray, offset);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(keysArray);
            ArrayPool<T>.Shared.Return(tempArray);
            if (rentedCountArray != null)
            {
                ArrayPool<int>.Shared.Return(rentedCountArray);
            }
        }
    }

    /// <summary>
    /// Core counting sort implementation.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(SortSpan<T> s, Span<int> keys, Span<T> tempArray, Span<int> countArray, int offset) where T : IComparable<T>
    {
        // Count occurrences of each key
        for (var i = 0; i < s.Length; i++)
        {
            countArray[keys[i] + offset]++;
        }

        // Calculate cumulative counts (for stable sort)
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var key = keys[i];
            var index = key + offset;
            var pos = countArray[index] - 1;
            tempArray[pos] = s.Read(i);
            countArray[index]--;
        }

        // Write sorted data back to original span
        for (var i = 0; i < s.Length; i++)
        {
            s.Write(i, tempArray[i]);
        }
    }
}

/// <summary>
/// 整数値を直接カウンティングソートでソートします。
/// 各値の出現回数をカウントし、累積和を計算して正しい位置に配置する安定なソートアルゴリズムです。
/// 値の範囲が狭い場合に非常に高速ですが、範囲が広いとメモリを大量に消費します。
/// <br/>
/// Directly sorts integer values using counting sort.
/// A stable sorting algorithm that counts occurrences and uses cumulative sums to place elements.
/// Very fast when the value range is narrow, but consumes significant memory for wide ranges.
/// </summary>
/// <remarks>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution</description></item>
/// <item><description>Stable      : Yes</description></item>
/// <item><description>In-place    : No (O(n + k) where k = range of values)</description></item>
/// <item><description>Comparisons : 0 (No comparison operations)</description></item>
/// <item><description>Swaps       : 0</description></item>
/// <item><description>Time        : O(n + k) where k is the range of values</description></item>
/// <item><description>Memory      : O(n + k)</description></item>
/// <item><description>Note        : 値の範囲が大きいとメモリ使用量が膨大になります。最大範囲は{MaxCountArraySize}です。</description></item>
/// </list>
/// </remarks>
public static class CountingSortInteger
{
    private const int MaxCountArraySize = 10_000_000; // Maximum allowed count array size
    private const int StackAllocThreshold = 1024; // Use stackalloc for count arrays smaller than this

    public static void Sort(Span<int> span)
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort(Span<int> span, ISortContext context)
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<int>(span, context);

        // Rent arrays from ArrayPool for temporary storage
        var tempArray = ArrayPool<int>.Shared.Rent(span.Length);
        int[]? rentedCountArray = null;

        try
        {
            // Find min and max to determine range
            var min = int.MaxValue;
            var max = int.MinValue;

            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                if (value < min) min = value;
                if (value > max) max = value;
            }

            // If all elements are the same, no need to sort
            if (min == max) return;

            // Check for overflow and validate range
            long range = (long)max - (long)min + 1;
            if (range > int.MaxValue)
                throw new ArgumentException($"Value range is too large for CountingSort: {range}. Maximum supported range is {int.MaxValue}.");
            if (range > MaxCountArraySize)
                throw new ArgumentException($"Value range ({range}) exceeds maximum count array size ({MaxCountArraySize}). Consider using QuickSort or another comparison-based sort.");

            var offset = -min; // Offset to normalize values to 0-based index
            var size = (int)range;

            // Use stackalloc for small count arrays, ArrayPool for larger ones
            Span<int> countArray = size <= StackAllocThreshold
                ? stackalloc int[size]
                : (rentedCountArray = ArrayPool<int>.Shared.Rent(size)).AsSpan(0, size);
            countArray.Clear();

            SortCore(s, tempArray.AsSpan(0, span.Length), countArray, offset);
        }
        finally
        {
            ArrayPool<int>.Shared.Return(tempArray);
            if (rentedCountArray != null)
            {
                ArrayPool<int>.Shared.Return(rentedCountArray);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore(SortSpan<int> s, Span<int> tempArray, Span<int> countArray, int offset)
    {
        // Count occurrences and cache values to avoid redundant reads
        Span<int> cachedValues = tempArray.Slice(0, s.Length);
        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            cachedValues[i] = value;
            countArray[value + offset]++;
        }

        // Calculate cumulative counts (for stable sort)
        for (var i = 1; i < countArray.Length; i++)
        {
            countArray[i] += countArray[i - 1];
        }

        // Build result array in reverse order to maintain stability
        // Note: We need a separate temp array because cachedValues uses tempArray
        Span<int> result = stackalloc int[s.Length];
        for (var i = s.Length - 1; i >= 0; i--)
        {
            var value = cachedValues[i];
            var index = value + offset;
            var pos = countArray[index] - 1;
            result[pos] = value;
            countArray[index]--;
        }

        // Write sorted data back to original span
        for (var i = 0; i < s.Length; i++)
        {
            s.Write(i, result[i]);
        }
    }
}
