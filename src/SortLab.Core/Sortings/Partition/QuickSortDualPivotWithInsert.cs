namespace SortLab.Core.Sortings;

/// <summary>
/// Dual-Pivot QuickSort に InsertSortを組み合わせて最速を狙う
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(nlog^2n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortDualPivotWithInsert<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortDualPivotWithInsert<T>);

    private const int InsertThreshold = 16;
    private InsertionSort<T> insertSort = new InsertionSort<T>();

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

    void SortCore(Span<T> span, int left, int right)
    {
        if (right <= left) return;

        // switch to insert sort
        if (right - left < InsertThreshold)
        {
            insertSort.Sort(span, left, right + 1);
            return;
        }

        // fase 0. Make sure left item is lower than right item
        if (Compare(Index(span, left), Index(span, right)) > 0)
        {
            Swap(ref Index(span, left), ref Index(span, right));
        }

        // fase 1. decide pivot
        var l = left + 1;
        var k = l;
        var g = right - 1;

        while (k <= g)
        {
            if (Compare(Index(span, k), Index(span, left)) < 0)
            {
                Swap(ref Index(span, k), ref Index(span, l));
                k++;
                l++;
            }
            else if (Compare(Index(span, right), Index(span, k)) < 0)
            {
                Swap(ref Index(span, k), ref Index(span, g));
                g--;
            }
            else
            {
                k++;
            }
        }

        l--;
        g++;
        Swap(ref Index(span, left), ref Index(span, l));
        Swap(ref Index(span, right), ref Index(span, g));

        // fase 2. Sort Left, Mid and righ
        SortCore(span, left, l - 1);
        if (Compare(Index(span, left), Index(span, right)) < 0)
        {
            SortCore(span, l + 1, g - 1);
        }
        SortCore(span, g + 1, right);
    }
}
