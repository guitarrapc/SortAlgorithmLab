namespace SortLab.Core.Sortings;

/// <summary>
/// Contains Bug on HeapSort.
/// 配列から、常に最大の要素をルートにもつ2分木構造(ヒープ)を作る(この時点で不安定)。あとは、ルート要素をソート済み配列の末尾に詰めて、ヒープの末端をルートに持ってきて再度ヒープ構造を作る。これを繰り返すことでヒープの最大値は常にルート要素になり、これをソート済み配列につめていくことで自然とソートができる。
/// Median3 は山データでエッジケース問題があるため、Median9が望ましい
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare : n log n
/// Swap : n log n
/// Order : O(n log n) (Worst case : O(n log n))
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
