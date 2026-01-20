using System.Numerics;

namespace SortLab.Core.Sortings;

/// <summary>
/// 10進数基数のLSD基数ソート。
/// 値を10進数の桁として扱い、各桁ごとにバケットソートを行います。
/// 人間が理解しやすい10進数ベースのアルゴリズムで、デバッグや教育目的に適しています。
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : no (n + 10)
/// Compare : 0        (No comparison operations, only division and modulo)
/// Swap    : 0
/// Order   : O(d * n) where d is the number of decimal digits (d = log₁₀(max))
/// Note    : 10進数演算を使用するため、2進数ベースのRadixLSD4Sortより遅い場合があります。
/// </remarks>
/// <typeparam name="T">ソート対象の整数型（IBinaryInteger制約）</typeparam>
public class RadixLSD10Sort<T> : SortBase<T> 
    where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
{
    private const int RadixBase = 10;       // Decimal base
    
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(RadixLSD10Sort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        if (span.Length <= 1) return;

        // Check if we have negative numbers
        var hasNegative = false;
        var zero = T.Zero;
        for (var i = 0; i < span.Length; i++)
        {
            if (Compare(Index(span, i), zero) < 0)
            {
                hasNegative = true;
                break;
            }
        }

        if (hasNegative)
        {
            SortCoreNegative(span);
        }
        else
        {
            SortCorePositive(span);
        }
    }

    private void SortCorePositive(Span<T> span)
    {
        // Find max to determine number of digits
        var max = T.MinValue;
        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (Compare(value, max) > 0)
            {
                max = value;
            }
        }

        // Calculate number of decimal digits
        var digitCount = GetDigitCount(max);
        var buckets = new List<T>[RadixBase];

        var divisor = T.One;
        var ten = T.CreateChecked(10);

        for (var d = 0; d < digitCount; d++)
        {
            // Clear buckets
            for (var i = 0; i < RadixBase; i++)
            {
                buckets[i]?.Clear();
            }

            // Distribute elements into buckets
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                var digit = GetDecimalDigit(value, divisor);
                buckets[digit] ??= new List<T>();
                buckets[digit].Add(value);
            }

            // Collect elements back from buckets
            for (int j = 0, i = 0; j < RadixBase; j++)
            {
                if (buckets[j] != null)
                {
                    foreach (var item in buckets[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }

            divisor *= ten;
        }
    }

    private void SortCoreNegative(Span<T> span)
    {
        // For negative numbers, use 20 buckets (10 for negative, 10 for positive)
        var buckets = new List<T>[20];
        
        // Find max absolute value to determine number of digits
        var maxAbs = T.Zero;
        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            var abs = T.Abs(value);
            if (Compare(abs, maxAbs) > 0)
            {
                maxAbs = abs;
            }
        }

        var digitCount = GetDigitCount(maxAbs);
        var divisor = T.One;
        var ten = T.CreateChecked(10);
        var zero = T.Zero;
        var nine = T.CreateChecked(9);

        for (var d = 0; d < digitCount; d++)
        {
            // Clear buckets
            for (var i = 0; i < 20; i++)
            {
                buckets[i]?.Clear();
            }

            // Distribute elements into buckets
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                var digit = GetDecimalDigit(T.Abs(value), divisor);
                
                // Negative numbers: negate digit and offset by 9
                // Positive numbers: offset by 9
                int bucketIndex;
                if (Compare(value, zero) < 0)
                {
                    bucketIndex = 9 - digit;  // 9, 8, 7, ..., 0 for digits 0-9
                }
                else
                {
                    bucketIndex = 10 + digit;  // 10, 11, 12, ..., 19 for digits 0-9
                }

                buckets[bucketIndex] ??= new List<T>();
                buckets[bucketIndex].Add(value);
            }

            // Collect elements back from buckets
            for (int j = 0, i = 0; j < 20; j++)
            {
                if (buckets[j] != null)
                {
                    foreach (var item in buckets[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }

            divisor *= ten;
        }
    }

    /// <summary>
    /// Get the number of decimal digits in a value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDigitCount(T value)
    {
        if (value == T.Zero)
            return 1;

        var count = 0;
        var temp = T.Abs(value);
        var zero = T.Zero;
        var ten = T.CreateChecked(10);

        while (Compare(temp, zero) > 0)
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
    private int GetDecimalDigit(T value, T divisor)
    {
        var ten = T.CreateChecked(10);
        var digit = (value / divisor) % ten;
        return int.CreateChecked(digit);
    }
}
