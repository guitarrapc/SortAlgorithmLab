namespace SortLab.Core.Sortings;

/// <summary>
/// ピボットを2つにすることで再帰の深さを浅くすることで、QuickSortを高速化する。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (log n)
/// Compare :
/// Swap :
/// Order : O(n log n) (Worst case : O(nlog^2n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class QuickSortDualPivot<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Partitioning;
    protected override string Name => nameof(QuickSortDualPivot<T>);

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
        if (right <= left) return;

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
