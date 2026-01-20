namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSort + BinaryInsertSort によるハイブリッドソート。Median-of-9版。
/// QuickSortでだいたいソート済みになった小さな部分配列でBinaryInsertSortに切り替える。
/// Median-of-9法により、山型データなどのエッジケースに対して堅牢である。
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes (Only uses O(log n) recursive stack space)
/// Compare : O(n log n)  (Binary search reduces comparisons slightly)
/// Swap    : O(n log n)  (Average case, Worst case: O(n^2))
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * best case : O(n log n)
///         * worst case: O(n^2)     (very rare with median-of-9)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian9WithBinaryInsert<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortMedian9WithBinaryInsert<T>);

    // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
    private const int InsertThreshold = 16;
    private BinaryInsertSort<T> insertSort = new BinaryInsertSort<T>();

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length - 1);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length - 1);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
    }

    private void SortCore(Span<T> span, int left, int right)
    {
        if (left >= right) return;

        // switch to insert sort
        if (right - left < InsertThreshold)
        {
            insertSort.Sort(span, left, right + 1);
            return;
        }

        // fase 1. decide pivot
        var pivot = Median9(span, left, right);
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

    private T Median9(Span<T> span, int low, int high)
    {
        var m2 = (high - low) / 2;
        var m4 = m2 / 2;
        var m8 = m4 / 2;
        var p1 = Index(span, low);
        var p2 = Index(span, low + m8);
        var p3 = Index(span, low + m4);
        var p4 = Index(span, low + m2 - m8);
        var p5 = Index(span, low + m2);
        var p6 = Index(span, low + m2 + m8);
        var p7 = Index(span, high - m4);
        var p8 = Index(span, high - m8);
        var p9 = Index(span, high);
        return Median3(Median3(p1, p2, p3), Median3(p4, p5, p6), Median3(p7, p8, p9));
    }
}
