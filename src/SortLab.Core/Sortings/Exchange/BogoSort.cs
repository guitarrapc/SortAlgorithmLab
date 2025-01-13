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
        SortCore(array.AsSpan());
        return array;
    }

    private void SortCore(Span<T> span)
    {
        while (!IsSorted(span))
        {
            Shuffle(span);
        }
    }

    private void Shuffle(Span<T> span)
    {
        var length = span.Length;
        for (var i = 0; i < length; i++)
        {
            Swap(ref Index(span, i), ref Index(span, Random.Shared.Next(0, length)));
        }
    }

    private bool IsSorted(Span<T> span)
    {
        var length = span.Length;
        for (var i = 0; i < length - 1; i++)
        {
            if (Compare(Index(span, i), Index(span, i + 1)) > 0)
            {
                return false;
            }
        }
        return true;
    }
}
