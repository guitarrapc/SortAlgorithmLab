using SortLab.Core.Contexts;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Algorithms;

/// <summary>
/// 10進数基数のLSD（Least Significant Digit）基数ソート。
/// 値を10進数の桁として扱い、最下位桁から最上位桁まで順に安定なバケットソートを繰り返します。
/// 人間が理解しやすい10進数ベースのアルゴリズムで、デバッグや教育目的に適しています。
/// <br/>
/// Decimal-based LSD (Least Significant Digit) radix sort.
/// Treats values as decimal digits and performs stable bucket sorting repeatedly from the least significant digit to the most significant digit.
/// This decimal-based algorithm is easy for humans to understand and is suitable for debugging and educational purposes.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct LSD Radix Sort (Decimal Base):</strong></para>
/// <list type="number">
/// <item><description><strong>Stable Sorting per Digit:</strong> Each pass must be stable (preserve relative order of equal keys).
/// This implementation uses counting sort to maintain insertion order, ensuring stability.</description></item>
/// <item><description><strong>Digit Extraction Consistency:</strong> For a given position, the digit must be extracted consistently across all values.
/// This uses (value / divisor) % 10 where divisor = 10^d (d = 0, 1, 2, ...).</description></item>
/// <item><description><strong>LSD Processing Order:</strong> Process digits from least significant (ones place) to most significant (highest decimal digit).
/// This ensures that lower-order digits are already sorted when processing higher-order digits.</description></item>
/// <item><description><strong>Complete Pass Coverage:</strong> Must perform d passes where d = ⌈log₁₀(max)⌉ + 1 (number of decimal digits in the maximum value).
/// Incomplete passes result in partially sorted arrays.</description></item>
/// <item><description><strong>Negative Number Handling:</strong> For signed integers, negative values require special treatment.
/// This implementation separates negative and non-negative values, sorts them independently, then concatenates (negatives reversed, then non-negatives).</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Distribution (Non-comparison based)</description></item>
/// <item><description>Stable      : Yes (insertion order preserved in buckets)</description></item>
/// <item><description>In-place    : No (O(n + 10) auxiliary space for buckets)</description></item>
/// <item><description>Best case   : Θ(d × n) - d = number of decimal digits (d = ⌈log₁₀(max)⌉ + 1)</description></item>
/// <item><description>Average case: Θ(d × n) - Linear in input size, independent of value distribution</description></item>
/// <item><description>Worst case  : Θ(d × n) - Performance depends on digit count, not comparisons</description></item>
/// <item><description>Comparisons : 0 (Non-comparison sort; uses only arithmetic operations)</description></item>
/// <item><description>Swaps       : 0 (Elements moved via bucket redistribution, not swaps)</description></item>
/// <item><description>Writes      : d × n (Each element written once per digit pass)</description></item>
/// <item><description>Reads       : d × n (Each element read once per digit pass)</description></item>
/// </list>
/// <para><strong>Note:</strong> Uses decimal arithmetic (division and modulo), which may be slower than binary-based radix sorts (e.g., RadixLSD4Sort with bit shifts).
/// However, it is more intuitive for understanding and debugging.</para>
/// </remarks>
public static class RadixLSD10Sort
{
    private const int RadixBase = 10;       // Decimal base
    
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;           // Main input array
    private const int BUFFER_TEMP = 1;           // Temporary buffer for digit redistribution
    private const int BUFFER_NEGATIVE = 2;       // Negative numbers buffer
    private const int BUFFER_NONNEGATIVE = 3;    // Non-negative numbers buffer


    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/> and <see cref="IBinaryInteger{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>, IBinaryInteger<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/> and <see cref="IBinaryInteger{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>, IBinaryInteger<T>
    {
        if (span.Length <= 1) return;

        // Rent buffers from ArrayPool
        var tempArray = ArrayPool<T>.Shared.Rent(span.Length);
        var bucketCountsArray = ArrayPool<int>.Shared.Rent(RadixBase);
        var negativeArray = ArrayPool<T>.Shared.Rent(span.Length);
        var nonNegativeArray = ArrayPool<T>.Shared.Rent(span.Length);

        try
        {
            var tempBuffer = tempArray.AsSpan(0, span.Length);
            var bucketCounts = bucketCountsArray.AsSpan(0, RadixBase);
            var negativeBuffer = negativeArray.AsSpan(0, span.Length);
            var nonNegativeBuffer = nonNegativeArray.AsSpan(0, span.Length);

            SortCore(span, tempBuffer, bucketCounts, negativeBuffer, nonNegativeBuffer, context);
        }
        finally
        {
            ArrayPool<T>.Shared.Return(tempArray);
            ArrayPool<int>.Shared.Return(bucketCountsArray);
            ArrayPool<T>.Shared.Return(negativeArray);
            ArrayPool<T>.Shared.Return(nonNegativeArray);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(Span<T> span, Span<T> tempBuffer, Span<int> bucketCounts, Span<T> negativeBuffer, Span<T> nonNegativeBuffer, ISortContext context) where T : IComparable<T>, IBinaryInteger<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // Check if we have negative numbers
        var hasNegative = false;
        var zero = T.Zero;
        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (value.CompareTo(zero) < 0)
            {
                hasNegative = true;
                break;
            }
        }

        if (hasNegative)
        {
            SortCoreNegative(s, tempBuffer, bucketCounts, negativeBuffer, nonNegativeBuffer, context);
        }
        else
        {
            SortCorePositive(s, tempBuffer, bucketCounts);
        }
    }

    private static void SortCorePositive<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketCounts) where T : IComparable<T>, IBinaryInteger<T>
    {
        if (s.Length <= 1) return;

        // Find max to determine number of digits
        var max = s.Read(0);
        for (var i = 1; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (value.CompareTo(max) > 0)
            {
                max = value;
            }
        }

        // Calculate number of decimal digits
        var digitCount = GetDigitCount(max);
        
        Span<int> bucketStarts = stackalloc int[RadixBase];
        var divisor = T.One;
        var ten = T.CreateChecked(10);

        for (var d = 0; d < digitCount; d++)
        {
            // Clear bucket counts
            bucketCounts.Slice(0, RadixBase).Clear();

            // Count elements per bucket
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(value, divisor);
                bucketCounts[digit]++;
            }

            // Calculate starting positions (cumulative sum)
            var offset = 0;
            for (var i = 0; i < RadixBase; i++)
            {
                bucketStarts[i] = offset;
                offset += bucketCounts[i];
            }

            // Distribute elements into temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(value, divisor);
                var pos = bucketStarts[digit]++;
                tempBuffer[pos] = value;
            }

            // Copy back from temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                s.Write(i, tempBuffer[i]);
            }

            divisor *= ten;
        }
    }

    private static void SortCoreNegative<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketCounts, Span<T> negativeBuffer, Span<T> nonNegativeBuffer, ISortContext context) where T : IComparable<T>, IBinaryInteger<T>
    {
        // Separate negative and non-negative numbers using index-based approach
        var negativeCount = 0;
        var nonNegativeCount = 0;
        var zero = T.Zero;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (value.CompareTo(zero) < 0)
            {
                negativeBuffer[negativeCount++] = value;
            }
            else
            {
                nonNegativeBuffer[nonNegativeCount++] = value;
            }
        }

        // Sort negative numbers using their absolute values
        // Create SortSpan for internal buffer to track statistics with specific buffer ID
        if (negativeCount > 0)
        {
            var negativeSpan = new SortSpan<T>(negativeBuffer.Slice(0, negativeCount), context, BUFFER_NEGATIVE);
            SortNegativeValues(negativeSpan, tempBuffer, bucketCounts);
        }

        // Sort non-negative numbers
        // Create SortSpan for internal buffer to track statistics with specific buffer ID
        if (nonNegativeCount > 0)
        {
            var nonNegativeSpan = new SortSpan<T>(nonNegativeBuffer.Slice(0, nonNegativeCount), context, BUFFER_NONNEGATIVE);
            SortPositiveValues(nonNegativeSpan, tempBuffer, bucketCounts);
        }

        // Merge: reversed negative numbers + non-negative numbers
        var writeIndex = 0;
        for (var i = negativeCount - 1; i >= 0; i--)
        {
            s.Write(writeIndex++, negativeBuffer[i]);
        }
        for (var i = 0; i < nonNegativeCount; i++)
        {
            s.Write(writeIndex++, nonNegativeBuffer[i]);
        }
    }

    /// <summary>
    /// Sort negative values by their absolute values (smallest absolute value first).
    /// After sorting, the array will be in order: -1, -2, -3, ... (smallest to largest absolute value)
    /// </summary>
    private static void SortNegativeValues<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketCounts) where T : IBinaryInteger<T>
    {
        if (s.Length <= 1) return;

        // Find max absolute value to determine number of digits
        var maxAbs = T.Abs(s.Read(0));
        for (var i = 1; i < s.Length; i++)
        {
            var abs = T.Abs(s.Read(i));
            if (abs.CompareTo(maxAbs) > 0)
            {
                maxAbs = abs;
            }
        }

        var digitCount = GetDigitCount(maxAbs);
        
        Span<int> bucketStarts = stackalloc int[RadixBase];
        var divisor = T.One;
        var ten = T.CreateChecked(10);

        for (var d = 0; d < digitCount; d++)
        {
            // Clear bucket counts
            bucketCounts.Slice(0, RadixBase).Clear();

            // Count elements per bucket based on absolute value
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(T.Abs(value), divisor);
                bucketCounts[digit]++;
            }

            // Calculate starting positions (cumulative sum)
            var offset = 0;
            for (var i = 0; i < RadixBase; i++)
            {
                bucketStarts[i] = offset;
                offset += bucketCounts[i];
            }

            // Distribute elements into temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(T.Abs(value), divisor);
                var pos = bucketStarts[digit]++;
                tempBuffer[pos] = value;
            }

            // Copy back from temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                s.Write(i, tempBuffer[i]);
            }

            divisor *= ten;
        }
    }

    /// <summary>
    /// Sort non-negative values (standard LSD radix sort)
    /// </summary>
    private static void SortPositiveValues<T>(SortSpan<T> s, Span<T> tempBuffer, Span<int> bucketCounts) where T : IBinaryInteger<T>
    {
        if (s.Length <= 1) return;

        // Find max to determine number of digits
        var max = s.Read(0);
        for (var i = 1; i < s.Length; i++)
        {
            var value = s.Read(i);
            if (value.CompareTo(max) > 0)
            {
                max = value;
            }
        }

        // Calculate number of decimal digits
        var digitCount = GetDigitCount(max);
        
        Span<int> bucketStarts = stackalloc int[RadixBase];
        var divisor = T.One;
        var ten = T.CreateChecked(10);

        for (var d = 0; d < digitCount; d++)
        {
            // Clear bucket counts
            bucketCounts.Slice(0, RadixBase).Clear();

            // Count elements per bucket
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(value, divisor);
                bucketCounts[digit]++;
            }

            // Calculate starting positions (cumulative sum)
            var offset = 0;
            for (var i = 0; i < RadixBase; i++)
            {
                bucketStarts[i] = offset;
                offset += bucketCounts[i];
            }

            // Distribute elements into temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                var value = s.Read(i);
                var digit = GetDecimalDigit(value, divisor);
                var pos = bucketStarts[digit]++;
                tempBuffer[pos] = value;
            }

            // Copy back from temp buffer
            for (var i = 0; i < s.Length; i++)
            {
                s.Write(i, tempBuffer[i]);
            }

            divisor *= ten;
        }
    }

    /// <summary>
    /// Get the number of decimal digits in a value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDigitCount<T>(T value) where T : IBinaryInteger<T>
    {
        if (value == T.Zero)
            return 1;

        var count = 0;
        var temp = T.Abs(value);
        var zero = T.Zero;
        var ten = T.CreateChecked(10);

        while (temp.CompareTo(zero) > 0)
        {
            temp /= ten;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Extract a decimal digit at the given position (divisor = 10^position)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDecimalDigit<T>(T value, T divisor) where T : IBinaryInteger<T>
    {
        var ten = T.CreateChecked(10);
        var digit = (value / divisor) % ten;
        return int.CreateChecked(digit);
    }
}
