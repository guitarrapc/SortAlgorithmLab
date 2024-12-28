using System;

namespace SortLab.Core.Logics;

/// <summary>
/// バイナリツリーを作成して、Leftは現ノードより小さく。Rightは大きいことを保証する。これで、ツリーによって並び替えが保証されるので、ツリー作成結果を配列に割り当て直す。高速な安定の外部ソート。
/// </summary>
/// <remarks>
/// stable : yes
/// inplace : no (n)
/// Compare : n log n
/// Swap :
/// Order : O(n log n) (Better case : O(n log n)) (Worst case : O(n log n))
/// </remarks>
/// <typeparam name="T"></typeparam>
public class BinaryTreeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    private Node root = null;

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, nameof(BinaryTreeSort<T>));
        return SortImpl(array);
    }
    private T[] SortImpl(T[] array)
    {
        for (var i = 0; i < array.Length; i++)
        {
            Statistics.AddIndexCount();
            Insert(array[i]);
        }

        var n = 0;
        Inorder(array, root, ref n);
        return array;
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

    private void Inorder(T[] array, Node Root, ref int i)
    {
        if (Root != null)
        {
            Inorder(array, Root.left, ref i);
            array[i++] = Root.item;
            Inorder(array, Root.right, ref i);
        }
    }
}
