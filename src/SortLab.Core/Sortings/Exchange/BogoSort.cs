namespace SortLab.Core.Sortings;

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
    public override SortMethod Method => SortMethod.Exchange;
    protected override string Name => nameof(BogoSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, Method, Name);
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
            Statistics.AddIndexCount();
            Swap(ref array[i], ref array[Random.Shared.Next(0, array.Length - 1)]);
        }
    }

    private bool IsSorted(T[] array)
    {
        for (var i = 0; i < array.Length - 1; i++)
        {
            Statistics.AddIndexCount();
            if (Compare(array[i], array[i + 1]) > 0)
            {
                return false;
            }
        }
        return true;
    }
}
