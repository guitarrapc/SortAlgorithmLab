using System;

namespace SortLab.Core.Logics;

/// <summary>
/// ソートされるまでひたすらシャッフル。そして都度ソート確認をするというえげつなさ。これはひどい。10ソートで限界
/// Permutation Sort とも呼ばれる
/// </summary>
/// <remarks>
/// stable : no
/// inplace : yes
/// Compare :
/// Swap :
/// Order : O((n+1)!) (Worst : inifinite)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BogoSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;
    private Random random = new Random();

    public override T[] Sort(T[] array)
    {
        base.Statistics.Reset(array.Length, SortType, nameof(BogoSort<T>));
        while (!IsSorted(array))
        {
            Shuffle(array);
        }
        return array;
    }

    private void Shuffle(T[] array)
    {
        for (var i = 0; i <= array.Length - 1; i++)
        {
            base.Statistics.AddIndexAccess();
            Swap(ref array[i], ref array[random.Next(0, array.Length - 1)]);
        }
    }

    private bool IsSorted(T[] array)
    {
        for (var i = 0; i < array.Length - 1; i++)
        {
            base.Statistics.AddIndexAccess();
            base.Statistics.AddCompareCount();
            if (array[i].CompareTo(array[i + 1]) > 0)
            {
                return false;
            }
        }
        return true;
    }
}
