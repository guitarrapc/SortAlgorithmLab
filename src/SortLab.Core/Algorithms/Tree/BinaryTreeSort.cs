using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*

Ref span (Itelative) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BinaryTreeSort   | 100    |     7.000 us |     8.360 us |   0.4583 us |     7.100 us |     6.500 us |     7.400 us |    4448 B |
| BinaryTreeSort   | 1000   |   114.233 us |   162.629 us |   8.9142 us |   116.200 us |   104.500 us |   122.000 us |   40448 B |
| BinaryTreeSort   | 10000  |   653.500 us |    14.249 us |   0.7810 us |   653.900 us |   652.600 us |   654.000 us |  400736 B |

Ref span (Recursive) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BinaryTreeSort   | 100    |     7.767 us |     1.053 us |   0.0577 us |     7.800 us |     7.700 us |     7.800 us |    4736 B |
| BinaryTreeSort   | 1000   |   109.033 us |   319.199 us |  17.4964 us |   100.000 us |    97.900 us |   129.200 us |   40736 B |
| BinaryTreeSort   | 10000  | 1,283.600 us |   365.845 us |  20.0532 us | 1,275.700 us | 1,268.700 us | 1,306.400 us |  400448 B |

Span (Itelative) ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| BinaryTreeSort         | 100    |      8.267 us |    35.579 us |   1.9502 us |      7.400 us |      6.900 us |     10.50 us |    4448 B |
| BinaryTreeSort         | 1000   |    111.667 us |   206.286 us |  11.3072 us |    111.200 us |    100.600 us |    123.20 us |   40736 B |
| BinaryTreeSort         | 10000  |    665.733 us |    13.448 us |   0.7371 us |    666.000 us |    664.900 us |    666.30 us |  400736 B |

*/

/// <summary>
/// バイナリ検索木(Binary Search Tree, BST)を使用したソートアルゴリズム、二分木ソートとも呼ばれる。バイナリ検索木では、左の子ノードは親ノードより小さく、右の子ノードは親ノードより大きいことが保証される。
/// この特性により、木の中間順序走査 (in-order traversal) を行うことで配列がソートされる。
/// 外部ソートとしても利用可能であり、高速なソートが可能となる。
/// ただし、木が不均衡になると最悪ケースでO(n^2)の時間がかかる可能性がある。また、クラスを多用するためメモリアロケーションが多く、現実的なソートとしてはQuickSortやMergeSortを用いることが多い
/// <br/>
/// A sorting algorithm that uses a binary search tree. In a binary search tree, the left child node is guaranteed to be smaller than the parent node, and the right child node is guaranteed to be larger. This property ensures that performing an in-order traversal of the tree results in a sorted array. It can also be used as an external sort, enabling fast sorting.
/// </summary>
/// <remarks>
/// family  : tree
/// stable  : no  (Typically, Binary Tree Sort is not stable unless additional mechanisms are used.)
/// inplace : no  (Requires additional memory for the tree structure)
/// Compare : O(n log n)  (on average)
/// Swap    : 0        (Swaps are not used; instead, tree node assignments)
/// Index   : O(n)     (Accesses each element once during in-order traversal)
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * worst case: O(n^2) (if the tree becomes unbalanced)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class BinaryTreeSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        // The root node of the binary tree (null == the tree is empty).
        Node<T>? root = null;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            InsertIterative(ref root, value, s);
            //InsertResursive(ref root, value, s);
        }

        // Traverse the tree in inorder and write elements back into the array.
        var n = 0;
        Inorder(s, root, ref n);
    }

    /// <summary>
    /// Iterative insertion. Instead of using recursion, it loops to find the child nodes.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="value"></param>
    /// <param name="s"></param>
    private static void InsertIterative<T>(ref Node<T>? node, T value, SortSpan<T> s) where T : IComparable<T>
    {
        // If the tree is empty, create a new root and return.
        if (node is null)
        {
            node = new Node<T>(value);
            return;
        }

        // Iterate left & right node and insert.
        // If there's an existing tree, use 'current' to traverse down the children.
        Node<T> current = node;
        while (true)
        {
            // Compare value with current node's item using SortSpan for statistics tracking
            var cmp = s.Compare(value, current.Item);
            
            // If the value is smaller than the current node, go left.
            if (cmp < 0)
            {
                // If the left child is null, insert here.
                if (current.Left is null)
                {
                    current.Left = new Node<T>(value);
                    break;
                }
                // Otherwise, move further down to the left child.
                current = current.Left;
            }
            else
            {
                // If the value is greater or equal, go right.
                if (current.Right is null)
                {
                    current.Right = new Node<T>(value);
                    break;
                }
                // Otherwise, move further down to the right child.
                current = current.Right;
            }
        }
    }

    /// <summary>
    /// Recursive insertion. Recursion is slower than iteration.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="value"></param>
    /// <param name="s"></param>
    private static void InsertResursive<T>(ref Node<T>? node, T value, SortSpan<T> s) where T : IComparable<T>
    {
        // If the tree is empty, create a new root and return.
        if (node is null)
        {
            node = new Node<T>(value);
            return;
        }

        var cmp = s.Compare(value, node.Item);
        if (cmp < 0)
        {
            InsertIterative(ref node.Left, value, s);
        }
        else
        {
            InsertIterative(ref node.Right, value, s);
        }
    }

    private static void Inorder<T>(SortSpan<T> s, Node<T>? node, ref int i) where T : IComparable<T>
    {
        if (node is null) return;

        Inorder(s, node.Left, ref i);
        s.Write(i++, node.Item);
        Inorder(s, node.Right, ref i);
    }

    /// <summary>
    /// Represents a node in a binary tree structure that stores a value and references to left and right child nodes.
    /// </summary>
    /// <remarks>
    /// You cannot use struct here because of recursive definition.
    /// C# struct cannot contain an instance of itself directly or indirectly.
    /// </remarks>
    /// <typeparam name="T">The type of the value stored in the node.</typeparam>
    /// <param name="value">The value to store in the node.</param>
    private class Node<T>(T value)
    {
        public T Item = value;
        public Node<T>? Left;
        public Node<T>? Right;
    }
}
