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
    public override SortType SortType => SortType.Partition;
    protected override string Name => nameof(QuickSortDualPivot<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        return SortImpl(array, 0, array.Length - 1);
    }

    private T[] SortImpl(T[] array, int left, int right)
    {
        if (right <= left) return array;

        // fase 0. Make sure left item is lower than right item
        if (Compare(array[left], array[right]) > 0)
        {
            Swap(ref array[left], ref array[right]);
        }

        // fase 1. decide pivot
        var l = left + 1;
        var k = l;
        var g = right - 1;

        while (k <= g)
        {
            Statistics.AddIndexCount();
            if (Compare(array[k], array[left]) < 0)
            {
                Swap(ref array[k], ref array[l]);
                k++;
                l++;
            }
            else if (Compare(array[right], array[k]) < 0)
            {
                Swap(ref array[k], ref array[g]);
                g--;
            }
            else
            {
                k++;
            }
        }

        l--;
        g++;
        Swap(ref array[left], ref array[l]);
        Swap(ref array[right], ref array[g]);

        // fase 2. Sort Left, Mid and righ
        SortImpl(array, left, l - 1);
        if (Compare(array[left], array[right]) < 0)
        {
            SortImpl(array, l + 1, g - 1);
        }
        SortImpl(array, g + 1, right);
        return array;
    }
}
