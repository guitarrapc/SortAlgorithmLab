namespace SortLab.Core.Sortings;

/*

Ref span ...

| Method           | Number | Mean          | Error        | StdDev      | Median        | Min           | Max           | Allocated |
|----------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|--------------:|----------:|
| InsertSort       | 100    |    113.133 us | 2,569.345 us | 140.8345 us |     41.300 us |     22.700 us |    275.400 us |     448 B |
| InsertSort       | 1000   |    187.400 us |    17.594 us |   0.9644 us |    187.800 us |    186.300 us |    188.100 us |     736 B |
| InsertSort       | 10000  | 16,535.367 us | 2,879.932 us | 157.8588 us | 16,513.800 us | 16,389.400 us | 16,702.900 us |     448 B |
 
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
public class InsertSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(InsertSort<T>);

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

    private void SortCore(Span<T> span, int first, int last)
    {
        for (var i = first + 1; i < last; i++)
        {
            for (var j = i; j > first && Compare(Index(ref span, j - 1), Index(ref span, j)) > 0; --j)
            {
                Swap(ref Index(ref span, j), ref Index(ref span, j - 1));
            }
        }
    }
}
