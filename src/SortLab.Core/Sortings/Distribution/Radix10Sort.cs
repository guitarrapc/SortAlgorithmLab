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

    public override int[] Sort(int[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        if (array.Min() >= 0)
        {
            return SortImplPositive(array);
        }
        else
        {
            return SortImplNegative(array);
        }
    }

    private int[] SortImplPositive(int[] array)
    {
        var digit = 1 + (int)array.Max(x => Math.Log10(x));

        var bucket = new List<int>[10];

        for (int d = 0, r = 1; d < digit; ++d, r *= 10)
        {
            // make bucket for possibly assigned number of int
            for (var i = 0; i < array.Length; i++)
            {
                Statistics.AddIndexCount();
                var key = (array[i] / r) % 10;
                if (bucket[key] == null) bucket[key] = [];
                bucket[key].Add(array[i]);
            }

            // put array int to each bucket.
            for (int j = 0, i = 0; j < bucket.Length; ++j)
            {
                if (bucket[j] != null)
                {
                    foreach (var item in bucket[j])
                    {
                        Statistics.AddIndexCount();
                        array[i++] = item;
                    }
                }
                else
                {
                    Statistics.AddIndexCount();
                }
            }

            for (var j = 0; j < bucket.Length; ++j)
            {
                Statistics.AddIndexCount();
                bucket[j]?.Clear();
            }
        }

        return array;
    }

    private int[] SortImplNegative(int[] array)
    {
        var bucket = new List<int>[20];
        // findmax
        var max = 0;
        int digit = 0;
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            digit = GetDigit(array[i]);
            if (digit > max)
            {
                max = digit;
            }
        }

        for (var r = 1; r <= max; r++)
        {
            for (var i = 0; i < array.Length; i++)
            {
                Statistics.AddIndexCount();
                var tmp = array[i];
                var radix = tmp < 0
                    ? -(int)(Math.Abs(tmp) / Math.Pow(10, r - 1)) % 10
                    : (int)(tmp / Math.Pow(10, r - 1)) % 10;
                radix += 9;
                if (bucket[radix] == null) bucket[radix] = new List<int>();
                bucket[radix].Add(tmp);
            }

            for (int j = 0, i = 0; j < bucket.Length; ++j)
            {
                if (bucket[j] != null)
                {
                    foreach (var item in bucket[j])
                    {
                        Statistics.AddIndexCount();
                        array[i++] = item;
                    }
                }
                else
                {
                    Statistics.AddIndexCount();
                }
            }

            for (var j = 0; j < bucket.Length; ++j)
            {
                Statistics.AddIndexCount();
                bucket[j]?.Clear();
            }
        }

        return array;
    }

    private int GetDigit(int i)
    {
        return Math.Abs(i) < 10 ? 1 : 1 + GetDigit(i / 10);
    }
}
