namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertionSort    | 100    |     99.600 us | 2,663.800 us | 146.0119 us |     15.300 us |     15.300 us |    268.200 us |     736 B |
| InsertionSort    | 1000   |    148.700 us |   100.936 us |   5.5326 us |    147.600 us |    143.800 us |    154.700 us |     736 B |
| InsertionSort    | 10000  | 12,273.300 us | 3,278.699 us | 179.7166 us | 12,242.300 us | 12,111.100 us | 12,466.500 us |     448 B |

*/

/// <summary>
/// ソート済みとなる先頭に並ぶ配列がある。ソート済み配列の末尾から未ソートの配列の後ろに進み、各要素と比較して値が小さい限りソート済み配列と入れ替える。(つまり新しい要素の値が小さい限り前にいく)。ICompatibleの性質から、n > n-1 = -1 となり、> 0 で元順序を保証しているので安定ソート。
/// ソート済み配列には早いが、Reverse配列には遅い
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : yes
/// Compare : n(n-1) / 2
/// Swap : n^2/2
/// Order : O(n^2) (Better case : O(n)) (Worst case : O(n^2))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class InsertionSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(InsertionSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), 0, array.Length);
        return array;
    }

    public T[] Sort(T[] array, int first, int last)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan(), first, last);
        return array;
    }

    /// <summary>
    /// Optimized insertion sort
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    private void SortCore(Span<T> span, int first, int last)
    {
        if (last - first <= 1)
            return;

        for (var i = first + 1; i < last; i++)
        {
            // Temporarily store the value to be inserted
            var tmp = Index(ref span, i);

            // Shift the elements larger than tmp to the right
            var j = i - 1;
            while (j >= first && Compare(Index(ref span, j), tmp) > 0)
            {
                Index(ref span, j + 1) = Index(ref span, j);
                j--;
            }

            // Insert tmp into the correct position
            Index(ref span, j + 1) = tmp;
        }
    }
}
