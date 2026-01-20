namespace SortLab.Core.Sortings;

/// <summary>
/// 2^8のバケット対応基数ソート。基数の数だけバケットを用意して、基数の桁ごとにバケットソートを行うことで、バケットソートにあった値の範囲制限をゆるくしたもの。
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
public class RadixLSD4Sort<T> : SortBase<int> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Distributed;
    protected override string Name => nameof(RadixLSD4Sort<T>);

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
        var bucket = new List<int>[256];
        var digit = 4;

        for (int d = 0, logR = 0; d < digit; d++, logR += 8)
        {
            // Make bucket for possibly assigned number of int
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                // Pick 256 radix d's digit number
                var key = (value >> logR) & 255;
                bucket[key] ??= new List<int>();
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
        var positiveBucket = new List<int>[256];
        var negativeBucket = new List<int>[256];
        var digit = 4;

        for (int d = 0, logR = 0; d < digit; d++, logR += 8)
        {
            var offset = span.Length;
            
            // Make bucket for possibly assigned number of int
            for (var i = 0; i < span.Length; i++)
            {
                var value = Index(span, i);
                if (Compare(value, 0) >= 0)
                {
                    // Pick 256 radix d's digit number
                    var key = (value >> logR) & 255;
                    positiveBucket[key] ??= new List<int>();
                    positiveBucket[key].Add(value);
                    offset--;
                }
                else
                {
                    // Pick 256 radix d's digit number
                    var key = (value >> logR) & 255;
                    negativeBucket[key] ??= new List<int>();
                    negativeBucket[key].Add(value);
                }
            }

            // Put array int to each bucket
            // Negative bucket
            for (int j = 0, i = 0; j < negativeBucket.Length; j++)
            {
                if (negativeBucket[j] != null)
                {
                    foreach (var item in negativeBucket[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }
            
            // Positive bucket
            for (int j = 0, i = offset; j < positiveBucket.Length; j++)
            {
                if (positiveBucket[j] != null)
                {
                    foreach (var item in positiveBucket[j])
                    {
                        Index(span, i++) = item;
                    }
                }
            }

            for (var j = 0; j < positiveBucket.Length; j++)
            {
                positiveBucket[j]?.Clear();
                negativeBucket[j]?.Clear();
            }
        }
    }

    private int GetValue(int key, int i)
    {
        return (UnsignedRightShift(key, (8 * i))) & 255;
    }
}
