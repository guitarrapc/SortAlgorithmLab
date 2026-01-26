using SortAlgorithm.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Algorithms;

/// <summary>
/// 2^8 (256) 基数のLSD基数ソート。
/// 値をビット列として扱い、8ビットずつ（256種類）の桁に分けてバケットソートを行います。
/// 最下位桁（Least Significant Digit）から最上位桁へ向かって処理することで、安定なソートを実現します。
/// <br/>
/// LSD Radix Sort with radix 2^8 (256).
/// Treats values as bit sequences, dividing them into 8-bit digits (256 buckets) and performing bucket sort for each digit.
/// Processing from the Least Significant Digit to the most significant ensures a stable sort.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct LSD Radix Sort (Base-256):</strong></para>
/// <list type="number">
/// <item><description><strong>Digit Extraction Correctness:</strong> For each digit position d (from 0 to digitCount-1), extract the d-th 8-bit digit using bitwise operations:
/// digit = (value >> (d × 8)) &amp; 0xFF. This ensures each byte of the integer is processed independently.</description></item>
/// <item><description><strong>Stable Distribution (Counting Sort per Digit):</strong> Within each digit pass, elements are distributed into 256 buckets (0-255) based on the current digit value.
/// The distribution must preserve the relative order of elements with the same digit value (stable). This is achieved by processing elements in forward order and appending to buckets.</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Digits must be processed from least significant (d=0) to most significant (d=digitCount-1).
/// This bottom-up approach ensures that after processing digit d, all digits 0 through d are correctly sorted, with stability maintained by previous passes.</description></item>
/// <item><description><strong>Digit Count Determination:</strong> The number of passes (digitCount) must cover all significant bits of the largest value.
/// digitCount = ⌈bitSize / 8⌉ where bitSize is the bit width of type T (8, 16, 32, or 64 bits).</description></item>
/// <item><description><strong>Signed Integer Handling:</strong> For signed types, negative values must be separated and sorted by absolute value in reverse order.
/// After sorting negatives (in descending order by absolute value) and positives (in ascending order), negatives are concatenated before positives to produce the final sorted sequence.</description></item>
/// <item><description><strong>Bucket Collection Order:</strong> After distributing elements for a digit, buckets must be collected in ascending order (bucket 0, 1, 2, ..., 255).
/// For negative numbers sorted by absolute value, buckets are collected in reverse order to achieve descending order.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Radix Sort, LSD variant)</description></item>
/// <item><description>Stable      : Yes (maintains relative order of elements with equal keys)</description></item>
/// <item><description>In-place    : No (O(n + 256) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Θ(d × n) - d = ⌈bitSize/8⌉ is constant for fixed-width integers</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, independent of value distribution</description></item>
/// <item><description>Worst case  : Θ(d × n) - Same complexity regardless of input order</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort, uses bitwise operations only)</description></item>
/// <item><description>Digit Passes: d = ⌈bitSize/8⌉ (1 for byte, 2 for short, 4 for int, 8 for long)</description></item>
/// <item><description>Reads       : d × n (one read per element per digit pass)</description></item>
/// <item><description>Writes      : d × n (one write per element per digit pass)</description></item>
/// <item><description>Memory      : O(n) for bucket storage (List&lt;T&gt; per bucket)</description></item>
/// </list>
/// <para><strong>Radix-256 Advantages:</strong></para>
/// <list type="bullet">
/// <item><description>Fewer passes than radix-10: 4 passes for 32-bit vs 10 passes for decimal</description></item>
/// <item><description>Efficient bit operations: Shift and mask are faster than division/modulo</description></item>
/// <item><description>Cache-friendly bucket size: 256 buckets fit well in L1/L2 cache</description></item>
/// <item><description>Type-agnostic: Works with any IBinaryInteger type (byte, short, int, long)</description></item>
/// </list>
/// <para><strong>Reference:</strong></para>
/// <para>Wiki: https://en.wikipedia.org/wiki/Radix_sort#Least_significant_digit</para>
/// </remarks>
public static class RadixLSD4Sort
{
    private const int RadixBits = 8;        // 8 bits per digit
    private const int RadixSize = 256;      // 2^8 = 256 buckets

    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;           // Main input array
    private const int BUFFER_TEMP = 1;           // Temporary buffer for digit redistribution
    private const int BUFFER_NEGATIVE = 2;       // Negative values buffer
    private const int BUFFER_NONNEGATIVE = 3;    // Non-negative values buffer

    public static void Sort<T>(Span<T> span) where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        if (span.Length <= 1) return;

        // Calculate maximum possible buckets needed
        var maxBuckets = span.Length * RadixSize; // Worst case: all elements in different buckets across all passes

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var negativeArray = ArrayPool<T>.Shared.Rent(span.Length);
        var nonNegativeArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketDataArray = ArrayPool<T>.Shared.Rent(span.Length); // Temporary storage for bucket distribution
        var bucketOffsetsArray = ArrayPool<int>.Shared.Rent(RadixSize + 1); // Start offsets for each bucket

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var negativeBuffer = negativeArray.AsSpan(0, span.Length);
            var nonNegativeBuffer = nonNegativeArray.AsSpan(0, span.Length);
            var bucketData = bucketDataArray.AsSpan(0, span.Length);
            var bucketOffsets = bucketOffsetsArray.AsSpan(0, RadixSize + 1);

            SortCore(span, tempBuffer, negativeBuffer, nonNegativeBuffer, bucketData, bucketOffsets, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray, clearArray: true);
            ArrayPool<T>.Shared.Return(negativeArray, clearArray: true);
            ArrayPool<T>.Shared.Return(nonNegativeArray, clearArray: true);
            ArrayPool<T>.Shared.Return(bucketDataArray, clearArray: true);
            ArrayPool<int>.Shared.Return(bucketOffsetsArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(Span<T> span, Span<T> tempBuffer, Span<T> negativeBuffer, Span<T> nonNegativeBuffer,
                                     Span<T> bucketData, Span<int> bucketOffsets, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // Determine the number of digits based on type size
        var bitSize = sizeof(int) * 8; // Default to 32
        if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            bitSize = 64;
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            bitSize = 32;
        else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            bitSize = 16;
        else if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            bitSize = 8;

        var digitCount = (bitSize + RadixBits - 1) / RadixBits;

        // Check if we have negative numbers (for signed types)
        var hasNegative = false;
        var zero = T.Zero;
        for (var i = 0; i < s.Length; i++)
        {
            if (s.Read(i).CompareTo(zero) < 0)
            {
                hasNegative = true;
                break;
            }
        }

        if (hasNegative)
        {
            SortCoreNegative(s, tempBuffer, negativeBuffer, nonNegativeBuffer, bucketOffsets, digitCount, context);
        }
        else
        {
            SortCorePositive(s, tempBuffer, bucketOffsets, digitCount, context);
        }
    }

    private static void SortCorePositive<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketOffsets, int digitCount, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        var temp = new SortSpan<T>(tempBuffer, context, BUFFER_TEMP);

        for (int d = 0; d < digitCount; d++)
        {
            var shift = d * RadixBits;

            // Clear bucket offsets
            bucketOffsets.Clear();

            // Count occurrences of each digit
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDigit(value, shift);
                bucketOffsets[digit + 1]++;
            }

            // Calculate cumulative offsets (prefix sum)
            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements into temp buffer based on current digit
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDigit(value, shift);
                var destIndex = bucketOffsets[digit]++;
                temp.Write(destIndex, value);
            }

            // Copy back from temp to source using CopyTo for efficiency
            temp.CopyTo(0, s, 0, s.Length);
        }
    }

    private static void SortCoreNegative<T>(SortSpan<T> s, Span<T> tempBuffer, Span<T> negativeBuffer, Span<T> nonNegativeBuffer,
                                             Span<int> bucketOffsets, int digitCount, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        // Wrap buffers with SortSpan for statistics tracking
        var negBuf = new SortSpan<T>(negativeBuffer, context, BUFFER_NEGATIVE);
        var nonNegBuf = new SortSpan<T>(nonNegativeBuffer, context, BUFFER_NONNEGATIVE);

        // Separate into negative and non-negative arrays
        var negativeCount = 0;
        var nonNegativeCount = 0;
        var zero = T.Zero;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (value.CompareTo(zero) < 0)
            {
                negBuf.Write(negativeCount++, value);
            }
            else
            {
                nonNegBuf.Write(nonNegativeCount++, value);
            }
        }

        // Sort negative numbers by their absolute values in reverse order
        if (negativeCount > 0)
        {
            var negativeSpan = negativeBuffer.Slice(0, negativeCount);
            var negativeSortSpan = new SortSpan<T>(negativeSpan, context, BUFFER_NEGATIVE);
            var negativeTempBuffer = tempBuffer.Slice(0, negativeCount);
            SortNegativeValues(negativeSortSpan, negativeTempBuffer, bucketOffsets, digitCount, context);
        }

        // Sort non-negative numbers normally
        if (nonNegativeCount > 0)
        {
            var nonNegativeSpan = nonNegativeBuffer.Slice(0, nonNegativeCount);
            var nonNegativeSortSpan = new SortSpan<T>(nonNegativeSpan, context, BUFFER_NONNEGATIVE);
            var nonNegativeTempBuffer = tempBuffer.Slice(0, nonNegativeCount);
            SortCorePositive(nonNegativeSortSpan, nonNegativeTempBuffer, bucketOffsets, digitCount, context);
        }

        // Merge back: negatives in REVERSE order (largest absolute value first), then non-negatives
        // Negative values are sorted by ascending absolute value: [-1, -2, -3, -5]
        // We merge them in reverse to get: [-5, -3, -2, -1]
        var writeIndex = 0;
        for (var i = negativeCount - 1; i >= 0; i--)
        {
            s.Write(writeIndex++, negBuf.Read(i));
        }
        // Non-negative numbers are in order, so we can use CopyTo for efficiency
        if (nonNegativeCount > 0)
        {
            nonNegBuf.CopyTo(0, s, negativeCount, nonNegativeCount);
        }
    }

    private static void SortNegativeValues<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketOffsets, int digitCount, ISortContext context)
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
    {
        var temp = new SortSpan<T>(tempBuffer, context, BUFFER_TEMP);

        for (int d = 0; d < digitCount; d++)
        {
            var shift = d * RadixBits;

            // Clear bucket offsets
            bucketOffsets.Clear();

            // Count occurrences of each digit (using absolute value)
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var absValue = T.Abs(value);
                var digit = GetDigit(absValue, shift);
                bucketOffsets[digit + 1]++;
            }

            // Calculate cumulative offsets (prefix sum)
            for (var i = 1; i <= RadixSize; i++)
            {
                bucketOffsets[i] += bucketOffsets[i - 1];
            }

            // Distribute elements into temp buffer in FORWARD order (for ascending sort by absolute value)
            // This preserves stability and sorts by absolute value in ascending order
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var absValue = T.Abs(value);
                var digit = GetDigit(absValue, shift);
                var destIndex = bucketOffsets[digit]++;
                temp.Write(destIndex, value);  // Via SortSpan for statistics
            }

            // Copy back from temp to source using CopyTo for efficiency
            // After all passes, array is sorted by absolute value (ascending)
            temp.CopyTo(0, s, 0, s.Length);
        }
    }

    /// <summary>
    /// Extract a digit (8 bits) from the value at the given bit position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigit<T>(T value, int shift) where T : IBinaryInteger<T>
    {
        // Convert to ulong for unsigned bit operations
        var bits = Convert.ToUInt64(value);
        return (int)((bits >> shift) & 0xFF);
    }
}
