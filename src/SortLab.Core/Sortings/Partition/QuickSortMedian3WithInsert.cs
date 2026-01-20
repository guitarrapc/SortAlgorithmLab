namespace SortLab.Core.Sortings;

/// <summary>
/// QuickSort + InsertSortによる Quick Searchでだいたいソート済みになった時に最速を目指す。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(n nlog n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortMedian3WithInsert<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortMedian3WithInsert<T>);

    // ref : https://github.com/nlfiedler/burstsort4j/blob/master/src/org/burstsort4j/Introsort.java
    private const int InsertThreshold = 16;
    private InsertionSort<T> insertSort = new InsertionSort<T>();

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var result = SortImpl(array, 0, array.Length - 1);
        Statistics.AddCompareCount(insertSort.Statistics.CompareCount);
        Statistics.AddIndexCount(insertSort.Statistics.IndexAccessCount);
        Statistics.AddSwapCount(insertSort.Statistics.SwapCount);
        return result;
    }

    private T[] SortImpl(T[] array, int left, int right)
    {
        if (left >= right) return array;

        // switch to insert sort
        if (right - left < InsertThreshold)
        {
            return insertSort.Sort(array, left, right + 1);
        }

        // fase 1. decide pivot
        Statistics.AddIndexCount();
        var pivot = Median3(array[left], array[(left + (right - left)) / 2], array[right]);
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
}
