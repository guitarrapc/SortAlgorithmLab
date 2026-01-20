namespace SortLab.Core.Sortings;

/// <summary>
/// Contains Bug on HeapSort.
/// QuickSort + HeapSort + InsertSort によるQuickSortの最悪ケースでのO(n^2) を回避する実装。一定の深度以下になった場合にHeapSortにスイッチすることで最悪ケースを防ぎ、ほぼソート済み状況ではInsertSortで最速を狙う。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class IntroSortMedian3<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Hybrid;
    protected override string Name => nameof(IntroSortMedian3<T>);

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
            var partition = Partition(span, left, right, Median3(Index(span, left), Index(span, left + ((right - left) / 2) + 1), Index(span, right - 1)));
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

    private static int FloorLog(int length)
    {
        return (int)(Math.Floor(Math.Log(length) / Math.Log(2)));
    }
}
