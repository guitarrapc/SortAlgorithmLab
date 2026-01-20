namespace SortLab.Core.Sortings;

/// <summary>
/// 配列の先頭、中央、末尾から中央値を求めてピボットとし、このピボットを基準に配列を左右に分割する。
/// 左側にはピボットより小さい要素、右側には大きい要素を配置し、再帰的にそれぞれをソートする分割統治法。
/// Median-of-3法でピボットを選択することで、最悪ケースの確率を低減している。
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes (Only uses O(log n) recursive stack space)
/// Compare : O(n log n)  (Average case, Worst case: O(n^2))
/// Swap    : O(n log n)  (Average case, Worst case: O(n^2))
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * best case : O(n log n)
///         * worst case: O(n^2)     (rare with median-of-3)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian3<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortMedian3<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length - 1);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length - 1);
    }

    private void SortCore(Span<T> span, int left, int right)
    {
        if (left >= right) return;

        // fase 1. decide pivot
        var pivot = Median3(Index(span, left), Index(span, (left + (right - left)) / 2), Index(span, right));
        var l = left;
        var r = right;

        while (l <= r)
        {
            while (l < right && Compare(Index(span, l), pivot) < 0)
            {
                l++;
            }

            while (r > left && Compare(Index(span, r), pivot) > 0)
            {
                r--;
            }

            if (l > r) break;
            Swap(ref Index(span, l), ref Index(span, r));
            l++;
            r--;
        }

        // fase 2. Sort Left and Right
        SortCore(span, left, l - 1);
        SortCore(span, l, right);
    }

    // less efficient compatison
    //    private T Median3(T low, T mid, T high)
    //    {
    //        if (Compare(low, mid) > 0)
    //        {
    //            Swap(ref low, ref mid);
    //        }
    //        if (Compare(low, high) > 0)
    //        {
    //            Swap(ref low, ref high);
    //        }
    //        if (Compare(mid, high) > 0)
    //        {
    //            Swap(ref mid, ref high);
    //        }
    //        return mid;
    //    }

    // much more efficient comparison
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
}
