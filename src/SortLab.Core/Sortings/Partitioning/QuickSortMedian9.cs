namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSortのMedian-3 Killer対策に、Median9を採用する。ランダムデータでは若干おそくなるものの、山型データでは高速化、および最悪ケースの頻度は下がる
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(nlog^2n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian9<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Partition;
    protected override string Name => nameof(QuickSortMedian9<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        return SortImpl(array, 0, array.Length - 1);
    }

    private T[] SortImpl(T[] array, int left, int right)
    {
        if (left >= right) return array;

        // fase 1. decide pivot
        var pivot = Median9(array, left, right);
        var l = left;
        var r = right;

        while (l <= r)
        {
            while (l < right && Compare(array[l], pivot) < 0)
            {
                Statistics.AddIndexCount();
                l++;
            }

            while (r > left && Compare(array[r], pivot) > 0)
            {
                Statistics.AddIndexCount();
                r--;
            }

            if (l > r) break;
            Swap(ref array[l], ref array[r]);
            l++;
            r--;
        }

        // fase 2. Sort Left and Right
        SortImpl(array, left, l - 1);
        SortImpl(array, l, right);
        return array;
    }

    private T Median3(T low, T mid, T high)
    {
        if (Compare(low, mid) > 0)
        {
            if (Compare(mid, high) > 0)
            {
                return mid;
            }
            else
            {
                return Compare(low, high) > 0 ? high : low;
            }
        }
        else
        {
            if (Compare(mid, high) > 0)
            {
                return Compare(low, high) > 0 ? low : high;
            }
            else
            {
                return mid;
            }
        }
    }

    private T Median9(T[] array, int low, int high)
    {
        var m2 = (high - low) / 2;
        var m4 = m2 / 2;
        var m8 = m4 / 2;
        var a = array[low];
        var b = array[low + m8];
        var c = array[low + m4];
        var d = array[low + m2 - m8];
        var e = array[low + m2];
        var f = array[low + m2 + m8];
        var g = array[high - m4];
        var h = array[high - m8];
        var i = array[high];
        return Median3(Median3(a, b, c), Median3(d, e, f), Median3(g, h, i));
    }
}
