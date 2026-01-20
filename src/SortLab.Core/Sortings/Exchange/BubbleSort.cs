namespace SortLab.Core.Sortings;

/// <summary>
/// 配列の末尾から、n番目の要素をn-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証しているので安定ソート。
/// 単純だが低速
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : Average n(n-1)/4
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BubbleSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(BubbleSort<T>);

    public override void Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
    }

    public override void Sort(Span<T> span)
    {
        Statistics.Reset(span.Length, SortType, Name);
        SortCore(span);
    }

    private void SortCore(Span<T> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            for (var j = span.Length - 1; j > i; j--)
            {
                if (Compare(Index(span, j), Index(span, j - 1)) < 0)
                {
                    Swap(ref Index(span, j), ref Index(span, j - 1));
                }
            }
        }
    }
}
