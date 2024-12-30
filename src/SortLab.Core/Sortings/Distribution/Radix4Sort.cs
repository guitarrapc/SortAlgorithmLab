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
    public override SortMethod Method => SortMethod.Distributed;
    protected override string Name => nameof(RadixLSD4Sort<T>);

    public override int[] Sort(int[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
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
        var bucket = new List<int>[256];
        var digit = 4;

        for (int d = 0, logR = 0; d < digit; ++d, logR += 8)
        {
            // make bucket for possibly assigned number of int
            for (var i = 0; i < array.Length; i++)
            {
                Statistics.AddIndexCount();
                // pick 256 radix d's digit number
                var key = (array[i] >> logR) & 255;
                if (bucket[key] == null) bucket[key] = new List<int>();
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
                bucket[j] = null;
            }
        }

        return array;
    }

    private int[] SortImplNegative(int[] array)
    {
        var positiveBucket = new List<int>[256];
        var negativeBucket = new List<int>[256];
        var digit = 4;

        for (int d = 0, logR = 0; d < digit; ++d, logR += 8)
        {
            var offset = array.Length;
            // make bucket for possibly assigned number of int
            for (var i = 0; i < array.Length; i++)
            {
                if (Compare(array[i], 0) >= 0)
                {
                    Statistics.AddIndexCount();
                    // pick 256 radix d's digit number
                    var key = (array[i] >> logR) & 255;
                    if (positiveBucket[key] == null) positiveBucket[key] = new List<int>();
                    positiveBucket[key].Add(array[i]);
                    offset--;
                }
                else
                {
                    Statistics.AddIndexCount();
                    // pick 256 radix d's digit number
                    var key = (array[i] >> logR) & 255;
                    if (negativeBucket[key] == null) negativeBucket[key] = new List<int>();
                    negativeBucket[key].Add(array[i]);
                }
            }

            // put array int to each bucket.
            // negative bucket
            for (int j = 0, i = 0; j < negativeBucket.Length; ++j)
            {
                if (negativeBucket[j] != null)
                {
                    foreach (var item in negativeBucket[j])
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
            // positive bucket
            for (int j = 0, i = offset; j < positiveBucket.Length; ++j)
            {
                if (positiveBucket[j] != null)
                {
                    foreach (var item in positiveBucket[j])
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

            for (var j = 0; j < positiveBucket.Length; ++j)
            {
                Statistics.AddIndexCount();
                positiveBucket[j] = null;
                negativeBucket[j] = null;
            }
        }

        return array;
    }

    private int GetValue(int key, int i)
    {
        return (UnsignedRightShift(key, (8 * i))) & 255;
    }
}
