namespace SortLab.Core.Sortings;

/*

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BinaryTreeSort   | 100    |     7.367 us |     5.574 us |   0.3055 us |     7.300 us |     7.100 us |     7.700 us |    4736 B |
| BinaryTreeSort   | 1000   |   112.233 us |   165.107 us |   9.0500 us |   112.200 us |   103.200 us |   121.300 us |   40736 B |
| BinaryTreeSort   | 10000  |   649.833 us |    59.574 us |   3.2655 us |   648.100 us |   647.800 us |   653.600 us |  400448 B |

*/

/// <summary>
/// バイナリツリーを作成して、Leftは現ノードより小さく。Rightは大きいことを保証する。これで、ツリーによって並び替えが保証されるので、ツリー作成結果を配列に割り当て直す。高速な安定の外部ソート。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n)
/// Compare : n log n
/// Swap : 0
/// Order : O(n log n) (Better case : O(n log n)) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinaryTreeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(BinaryTreeSort<T>);

    private Node root = null;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());

        return array;
    }
    private void SortCore(Span<T> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            Insert(Index(ref span, i));
        }

        var n = 0;
        Inorder(span, root, ref n);
    }

    private class Node
    {
        public T item;
        public Node left;
        public Node right;
    }

    private void Insert(T id)
    {
        var node = new Node();
        node.item = id;
        if (root == null)
        {
            root = node;
        }
        else
        {
            var current = root;
            Node parent;
            while (true)
            {
                parent = current;
                if (Compare(id, current.item) < 0)
                {
                    current = current.left;
                    if (current == null)
                    {
                        parent.left = node;
                        return;
                    }
                }
                else
                {
                    current = current.right;
                    if (current == null)
                    {
                        parent.right = node;
                        return;
                    }
                }
            }
        }
    }

    private void Inorder(Span<T> span, Node Root, ref int i)
    {
        if (Root != null)
        {
            Inorder(span, Root.left, ref i);
            Index(ref span, i++) = Root.item;
            Inorder(span, Root.right, ref i);
        }
    }
}
