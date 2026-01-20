namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSort + HeapSort + InsertionSort によるハイブリッドソート。
/// QuickSortの最悪ケースでのO(n^2)を回避するため、再帰の深度が一定以上になった場合はHeapSortに切り替え、
/// 小さな部分配列ではInsertionSortを使用することで、あらゆるケースで高速なソートを実現します。
/// Median-of-9法でピボットを選択するため、Median-of-3よりも偏ったデータに対して堅牢です。
/// </summary>
/// <remarks>
/// stable  : no
/// inplace : yes (Only uses O(log n) recursive stack space)
/// Compare : O(n log n)
/// Swap    : O(n log n)
/// Order   : O(n log n) (Worst case: O(n log n), Best case: O(n log n))
/// Note    : Median-of-9法により山型データなどのエッジケースに強い
/// </remarks>
/// <typeparam name="T"></typeparam>
public class IntroSortMedian9<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Hybrid;
    protected override string Name => nameof(IntroSortMedian9<T>);

    // ref : https://www.cs.waikato.ac.nz/~bernhard/317/source/IntroSort.java
    private const int IntroThreshold = 16;
    private HeapSort<T> heapSort = new HeapSort<T>();
    private InsertionSort<T> insertSort = new InsertionSort<T>();

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length - 1, 2 * FloorLog(array.Length));
        Statistics.AddCompareCount(heapSort.Statistics.CompareCount);
        Statistics.AddIndexCount(heapSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(heapSort.Statistics.SwapCount);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span, 0, span.Length - 1, 2 * FloorLog(span.Length));
        Statistics.AddCompareCount(heapSort.Statistics.CompareCount);
        Statistics.AddIndexCount(heapSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(heapSort.Statistics.SwapCount);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
    }

    private void SortCore(Span<T> span, int left, int right, int depthLimit)
    {
        while (right - left > IntroThreshold)
        {
            if (depthLimit == 0)
            {
                heapSort.Sort(span, left, right);
                return;
            }
            depthLimit--;
            var partition = Partition(span, left, right, Median9(span, left, right));
            SortCore(span, partition, right, depthLimit);
            right = partition;
        }
        insertSort.Sort(span, left, right + 1);
    }

    private int Partition(Span<T> span, int left, int right, T pivot)
    {
        var l = left;
        var r = right;
        while (true)
        {
            while (Compare(Index(span, l), pivot) < 0)
            {
                l++;
            }
            r--;
            while (Compare(pivot, Index(span, r)) < 0)
            {
                r--;
            }

            if (!(l < r))
            {
                return l;
            }

            Swap(ref Index(span, l), ref Index(span, r));
            l++;
        }
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

    private static int FloorLog(int length)
    {
        return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
    }
}
