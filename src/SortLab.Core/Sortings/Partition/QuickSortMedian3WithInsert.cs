namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSort + InsertionSort によるハイブリッドソート。
/// QuickSortでだいたいソート済みになった小さな部分配列でInsertionSortに切り替えることで、最高のパフォーマンスを目指す。
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes (Only uses O(log n) recursive stack space)
/// Compare : O(n log n)  (Average case, Worst case: O(n^2))
/// Swap    : O(n log n)  (Average case, Worst case: O(n^2))
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * best case : O(n log n)
///         * worst case: O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian3WithInsert<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortMedian3WithInsert<T>);

    // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
    private const int InsertThreshold = 16;
    private InsertionSort<T> insertSort = new InsertionSort<T>();

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
