namespace SortLab.Core.Sortings;

/// <summary>
/// 配列の先頭と末尾を比較し先頭が大きければ交換。処理配列が3要素以上なら先頭2/3、末尾2/3、先頭2/3の順にソート。激遅、不安定で外部メモリと配列の同長つかうだめだめソート。
/// </summary>
/// <remarks>
/// stable : no
/// inplace : no (n)
/// Compare : n^(log3/log1.5)
/// Swap : n^2
/// Order : O(n^(log3/log1.5)) = O(n^2.7095)
/// </remarks>
/// <typeparam name="T"></typeparam>
public class StoogeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Exchange;
    protected override string Name => nameof(StoogeSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        for (var i = 0; i < array.Length; i++)
        {
            for (var j = array.Length - 1; j > i; j--)
            {
                Statistics.AddIndexCount();
                //array.Dump($"{j} : {array[j]}, {j - 1} : {array[j - 1]}, {array[j - 1].CompareTo(array[j]) > 0}");
                if (Compare(array[j], array[j - 1]) < 0)
                {
                    Swap(ref array[j], ref array[j - 1]);
                }
            }
        }
        return array;
    }
}
