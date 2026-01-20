namespace SortLab.Core.Sortings;

/// <summary>
/// 配列の先頭から、n番目の要素をn+1番目の要素と比較して交換を続ける。末尾から1ずつ増やしていくことで、毎ループで配列の末尾には確定した若い要素が必ず入る。次の配列の末尾から、n-1番目の要素と比較して交換を続ける。末尾から1ずつおろしていくことで、毎ループで配列の頭には確定した若い要素が必ず入る。ICompatibleの性質から、n > n-1 = -1 となり、< 0 で元順序を保証している安定ソート。
/// 両方向のバブルソートで、ほとんど整列済みのデータに対してはバブルソートより高速になる。
/// 別名Cocktail Sort
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : O(n^2) (Average n(n-1)/4)
/// Order : O(n^2)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CocktailShakerSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(CocktailShakerSort<T>);

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
        var min = 0;
        var max = span.Length - 1;

        while (min != max)
        {
            // 順方向スキャン
            var lastSwapIndex = min;
            for (var i = min; i < max; i++)
            {
                if (Compare(Index(span, i), Index(span, i + 1)) > 0)
                {
                    Swap(ref Index(span, i), ref Index(span, i + 1));
                    lastSwapIndex = i;
                }
            }

            max = lastSwapIndex;
            if (min == max) break;

            // 逆方向スキャン
            lastSwapIndex = max;
            for (var i = max; i > min; i--)
            {
                if (Compare(Index(span, i), Index(span, i - 1)) < 0)
                {
                    Swap(ref Index(span, i), ref Index(span, i - 1));
                    lastSwapIndex = i;
                }
            }
            min = lastSwapIndex;
            if (min == max) break;
        }
    }
}

/// <summary>
/// 実装はシンプルに見えても、最後に交換したポジションをとっていないので不要な要素へのアクセスを行っており非効率になっている
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : O(n^2) (Average n(n-1)/4)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class CocktailShakerSort2<T> : SortBase<T> where T : IComparable<T>
{
    public override SortMethod SortType => SortMethod.Exchange;
    protected override string Name => nameof(CocktailShakerSort2<T>);

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
        // half calculation
        for (int i = 0; i < span.Length / 2; i++)
        {
            var swapped = false;

            // Bubble Sort (To Min)
            for (int j = i; j < span.Length - i - 1; j++)
            {
                if (Compare(Index(span, j), Index(span, j + 1)) > 0)
                {
                    Swap(ref Index(span, j), ref Index(span, j + 1));
                    swapped = true;
                }
            }

            // Bubble Sort (To Max)
            for (int j = span.Length - 2 - i; j > i; j--)
            {
                if (Compare(Index(span, j), Index(span, j - 1)) < 0)
                {
                    Swap(ref Index(span, j), ref Index(span, j - 1));
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
    }
}
