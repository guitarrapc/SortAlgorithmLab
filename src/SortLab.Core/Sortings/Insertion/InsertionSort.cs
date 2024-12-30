namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertionSort    | 100    |    104.067 us | 2,522.505 us | 138.2670 us |     33.200 us |     15.600 us |    263.400 us |     448 B |
| InsertionSort    | 1000   |    142.833 us |    12.147 us |   0.6658 us |    143.000 us |    142.100 us |    143.400 us |     736 B |
| InsertionSort    | 10000  | 12,186.533 us | 2,295.526 us | 125.8255 us | 12,181.500 us | 12,063.300 us | 12,314.800 us |     448 B |

Ref span (Swap base) ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertionSort    | 100    |    113.133 us | 2,569.345 us | 140.8345 us |     41.300 us |     22.700 us |    275.400 us |     448 B |
| InsertionSort    | 1000   |    187.400 us |    17.594 us |   0.9644 us |    187.800 us |    186.300 us |    188.100 us |     736 B |
| InsertionSort    | 10000  | 16,535.367 us | 2,879.932 us | 157.8588 us | 16,513.800 us | 16,389.400 us | 16,702.900 us |     448 B |
 
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
        for (var i = first + 1; i < last; i++)
        {
            // Temporarily store the value to be inserted
            var tmp = Index(ref span, i);

            // Shift the elements larger than tmp to the right
            var j = i - 1;
            while (j >= 0 && Compare(Index(ref span, j), tmp) > 0)
            {
                Index(ref span, j + 1) = Index(ref span, j);
                j--;
            }

            // Insert tmp into the correct position
            Index(ref span, j + 1) = tmp;
        }
    }

    /// <summary>
    /// Not opmized, BubbleSort like Swap based insertion sort
    /// </summary>
    /// <param name="span"></param>
    /// <param name="first"></param>
    /// <param name="last"></param>
    private void SortCoreSwap(Span<T> span, int first, int last)
    {
        for (var i = first + 1; i < last; i++)
        {
            var tmp = Index(ref span, i);
            for (var j = i; j > first && Compare(Index(ref span, j - 1), Index(ref span, j)) > 0; --j)
            {
                Swap(ref Index(ref span, j), ref Index(ref span, j - 1));
            }
        }
    }
}
