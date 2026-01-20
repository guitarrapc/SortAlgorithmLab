namespace SortLab.Core.Sortings;

/// <summary>
/// 10進数のバケット対応基数ソート。基数の数だけバケットを用意して、基数の桁ごとにバケットソートを行うことで、バケットソートにあった値の範囲制限をゆるくしたもの。
/// これにより、桁ごとにバケットを用意すれば良くなり対象が楽になる。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n + 2^d)
/// Compare : 2kn
/// Swap : 0
/// Order : O(kn)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class RadixLSD10Sort<T> : SortBase<int> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(RadixLSD10Sort<T>);

    public override void Sort(int[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<int> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<int> span)
    {
        if (span.Length <= 1) return;

        // Check if we have negative numbers
        var hasNegative = false;
        for (var i = 0; i < span.Length; i++)
        {
            if (Index(span, i) < 0)
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

    private void SortCorePositive(Span<int> span)
    {
        // Find max to determine number of digits
        var max = int.MinValue;
        for (var i = 0; i < span.Length; i++)
        {
            var value = Index(span, i);
            if (value > max) max = value;
        }

        var digit = max == 0 ? 1 : 1 + (int)Math.Log10(max);
        var bucket = new List<int>[10];

        for (int d = 0, r = 1; d < digit; d++, r *= 10)
        {
            // Make bucket for possibly assigned number of int
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                var key = (value / r) % 10;
                bucket[key] ??= [];
                bucket[key].Add(value);
            }

            // Put array int to each bucket
            for (int j = 0, i = 0; j < bucket.Length; j++)
            {
                if (bucket[j] != null)
                {
                    foreach (var item in bucket[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }

            for (var j = 0; j < bucket.Length; j++)
            {
                bucket[j]?.Clear();
            }
        }
    }

    private void SortCoreNegative(Span<int> span)
    {
        var bucket = new List<int>[20];
        
        // Find max digit
        var max = 0;
        for (var i = 0; i < span.Length; i++)
        {
            var digit = GetDigit(Index(span, i));
            if (digit > max)
            {
                max = digit;
            }
        }

        for (var r = 1; r <= max; r++)
        {
            for (var i = 0; i < span.Length; i++)
            {
                var tmp = Index(span, i);
                var radix = tmp < 0
                    ? -(int)(Math.Abs(tmp) / Math.Pow(10, r - 1)) % 10
                    : (int)(tmp / Math.Pow(10, r - 1)) % 10;
                radix += 9;
                bucket[radix] ??= new List<int>();
                bucket[radix].Add(tmp);
            }

            for (int j = 0, i = 0; j < bucket.Length; j++)
            {
                if (bucket[j] != null)
                {
                    foreach (var item in bucket[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }

            for (var j = 0; j < bucket.Length; j++)
            {
                bucket[j]?.Clear();
            }
        }
    }

    private int GetDigit(int i)
    {
        return Math.Abs(i) < 10 ? 1 : 1 + GetDigit(i / 10);
    }
}
