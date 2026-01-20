namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSort + InsertSortによる Quick Searchでだいたいソート済みになった時に最速を目指す。Median9バージョン
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(n nlog n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian9WithInsert<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortMedian9WithInsert<T>);

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

    void SortCore(Span<T> span, int left, int right)
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

    T Median9(Span<T> span, int low, int high)
    {
        var m2 = (high - low) / 2;
        var m4 = m2 / 2;
        var m8 = m4 / 2;
        var a = Index(span, low);
        var b = Index(span, low + m8);
        var c = Index(span, low + m4);
        var d = Index(span, low + m2 - m8);
        var e = Index(span, low + m2);
        var f = Index(span, low + m2 + m8);
        var g = Index(span, high - m4);
        var h = Index(span, high - m8);
        var i = Index(span, high);
        return Median3(Median3(a, b, c), Median3(d, e, f), Median3(g, h, i));
    }
}
