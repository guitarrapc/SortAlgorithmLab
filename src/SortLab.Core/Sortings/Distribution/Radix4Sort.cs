using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SortLab.Core.Sortings;

/// <summary>
/// 2^8 (256) 基数のLSD基数ソート。
/// 値をビット列として扱い、8ビットずつ（256種類）の桁に分けてバケットソートを行います。
/// 各桁ごとにバケットソートを繰り返すことで、値の範囲に依存しない効率的なソートを実現します。
/// </summary>
/// <remarks>
/// stable  : yes
/// inplace : no (n + 2^8)
/// Compare : 0        (No comparison operations, only bitwise operations)
/// Swap    : 0
/// Order   : O(d * n) where d is the number of 8-bit digits (d = ⌈sizeof(T)/1⌉ bytes)
/// Note    : ビット演算を使用するため、整数型専用のアルゴリズムです。
/// </remarks>
/// <typeparam name="T">ソート対象の整数型（IBinaryInteger制約）</typeparam>
public class RadixLSD4Sort<T> : SortBase<T>
    where T : IBinaryInteger<T>, IMinMaxValue<T>, IComparable<T>
{
    private const int RadixBits = 8;        // 8 bits per digit
    private const int RadixSize = 256;      // 2^8 = 256 buckets
    
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(RadixLSD4Sort<T>);

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
            SortCoreNegative(span, digitCount);
        }
        else
        {
            SortCorePositive(span, digitCount);
        }
    }

    private void SortCorePositive(Span<T> span, int digitCount)
    {
        var buckets = new List<T>[RadixSize];

        for (int d = 0; d < digitCount; d++)
        {
            var shift = d * RadixBits;
            
            // Initialize buckets for this digit
            for (var i = 0; i < RadixSize; i++)
            {
                buckets[i]?.Clear();
            }

            // Distribute elements into buckets
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                var key = GetDigit(value, shift);
                buckets[key] ??= new List<T>();
                buckets[key].Add(value);
            }

            // Collect elements back from buckets
            for (int j = 0, i = 0; j < RadixSize; j++)
            {
                if (buckets[j] != null)
                {
                    foreach (var item in buckets[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }
        }
    }

    private void SortCoreNegative(Span<T> span, int digitCount)
    {
        // Separate into negative and positive arrays first
        var negativeList = new List<T>();
        var positiveList = new List<T>();
        var zero = T.Zero;

        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (Compare(value, zero) < 0)
            {
                negativeList.Add(value);
            }
            else
            {
                positiveList.Add(value);
            }
        }

        // Sort negative numbers (using absolute values, then reverse)
        if (negativeList.Count > 0)
        {
            var negativeSpan = CollectionsMarshal.AsSpan(negativeList);
            SortNegativeValues(negativeSpan, digitCount);
        }

        // Sort positive numbers
        if (positiveList.Count > 0)
        {
            var positiveSpan = CollectionsMarshal.AsSpan(positiveList);
            SortCorePositive(positiveSpan, digitCount);
        }

        // Merge back: negatives first, then positives
        var writeIndex = 0;
        foreach (var item in negativeList)
        {
            Index(span, writeIndex++) = item;
        }
        foreach (var item in positiveList)
        {
            Index(span, writeIndex++) = item;
        }
    }

    private void SortNegativeValues(Span<T> span, int digitCount)
    {
        var buckets = new List<T>[RadixSize];

        for (int d = 0; d < digitCount; d++)
        {
            var shift = d * RadixBits;
            
            // Clear buckets
            for (var i = 0; i < RadixSize; i++)
            {
                buckets[i]?.Clear();
            }

            // Distribute elements into buckets (using absolute value)
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                var absValue = T.Abs(value);
                var key = GetDigit(absValue, shift);
                buckets[key] ??= new List<T>();
                buckets[key].Add(value);
            }

            // Collect elements back from buckets in reverse order (for negatives)
            for (int j = RadixSize - 1, i = 0; j >= 0; j--)
            {
                if (buckets[j] != null)
                {
                    foreach (var item in buckets[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extract a digit (8 bits) from the value at the given bit position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDigit(T value, int shift)
    {
        // Convert to ulong for unsigned bit operations
        var bits = Convert.ToUInt64(value);
        return (int)((bits >> shift) & 0xFF);
    }
}
