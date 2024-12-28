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
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(CocktailShakerSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        var min = 0;
        var max = array.Length - 1;

        while (min != max)
        {
            // 順方向スキャン
            var lastSwapIndex = min;
            for (var i = min; i < max; i++)
            {
                Statistics.AddIndexCount();
                if (Compare(array[i], array[i + 1]) > 0)
                {
                    //array.Dump($"min -> {i} : {array[i]}, {i + 1} : {array[i+1]}");
                    Swap(ref array[i], ref array[i + 1]);
                    lastSwapIndex = i;
                }
            }

            max = lastSwapIndex;
            if (min == max) break;

            // 逆方向スキャン
            lastSwapIndex = max;
            for (var i = max; i > min; i--)
            {
                Statistics.AddIndexCount();
                if (Compare(array[i], array[i - 1]) < 0)
                {
                    //array.Dump($"max -> {i} : {array[i]}, {i + 1} : {array[i + 1]}");
                    Swap(ref array[i], ref array[i - 1]);
                    lastSwapIndex = i;
                }
            }
            min = lastSwapIndex;
            if (min == max) break;
        }

        return array;
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
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(CocktailShakerSort2<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);

        // half calculation
        for (int i = 0; i < array.Length / 2; i++)
        {
            var swapped = false;

            // Bubble Sort (To Min)
            for (int j = i; j < array.Length - i - 1; j++)
            {
                Statistics.AddIndexCount();
                if (Compare(array[j], array[j + 1]) > 0)
                {
                    //array.Dump($"min -> {j} : {array[j]}, {j + 1} : {array[j + 1]}");
                    Swap(ref array[j], ref array[j + 1]);
                    swapped = true;
                }
            }

            // Bubble Sort (To Max)
            for (int j = array.Length - 2 - i; j > i; j--)
            {
                Statistics.AddIndexCount();
                if (Compare(array[j], array[j - 1]) < 0)
                {
                    //array.Dump($"max -> {j} : {array[j]}, {j - 1} : {array[j-1]}");
                    Swap(ref array[j], ref array[j - 1]);
                    swapped = true;
                }
            }
            if (!swapped) break;
        }
        return array;
    }
}
