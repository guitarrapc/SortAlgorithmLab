namespace SortLab.Core.Sortings;

/*

Ref span (Iterative) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |    50.567 us |   121.459 us |   6.6576 us |    50.200 us |    44.100 us |    57.400 us |   31688 B |
| BalancedTreeSort | 1000   |   645.367 us |   245.080 us |  13.4337 us |   639.000 us |   636.300 us |   660.800 us |  541816 B |
| BalancedTreeSort | 10000  | 8,448.633 us | 2,596.190 us | 142.3059 us | 8,494.300 us | 8,289.100 us | 8,562.500 us | 5874912 B |

Ref span (Recursive) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |    13.633 us |     8.622 us |   0.4726 us |    13.800 us |    13.100 us |    14.000 us |    4736 B |
| BalancedTreeSort | 1000   |   187.633 us |   382.550 us |  20.9689 us |   190.700 us |   165.300 us |   206.900 us |   40736 B |
| BalancedTreeSort | 10000  | 2,028.033 us |   159.111 us |   8.7214 us | 2,032.300 us | 2,018.000 us | 2,033.800 us |  400736 B |

*/

/// <summary>
/// 平衡二分木を用いた二本木ソート。挿入時に回転を行って常に高さが O(log n) に保たれ、木を中順巡回することで配列に要素を昇順で再割り当てします。二本木ソートに比べて、平均および最悪ケースでも O(n log n) のソートが期待できます。
/// 平衡二分木（AVL 木）= 左の子ノードは現ノードより小さく、右の子ノードは大きいという性質を持ちます。
/// </summary>
/// <remarks>
/// stable : no  
/// inplace : no (ノードを生成するため追加メモリを使用)  
/// Compare : n log n  
/// Swap : 0 (配列自体でのスワップは行わない)  
/// Order : O(n log n) (平均・最悪ケースともにO(n log n))  
/// <typeparam name="T"></typeparam>
public class BalancedTreeSort<T> : SortBase<T> where T : IComparable<T>
{
    public override SortType SortType => SortType.Insertion;
    protected override string Name => nameof(BalancedTreeSort<T>);

    public override T[] Sort(T[] array)
    {
        Statistics.Reset(array.Length, SortType, Name);
        SortCore(array.AsSpan());
        return array;
    }

    /// <summary>
    /// Insert elements into an AVL tree, then traverse it in-order.
    /// </summary>
    private void SortCore(Span<T> span)
    {
        Node? root = null;

        // Insert each element in the array into the AVL tree.
        for (int i = 0; i < span.Length; i++)
        {
            // root = InsertIterative(root, Index(ref span, i));
            root = InsertRecursive(root, Index(ref span, i));
        }

        // Traverse in order and write back into the array.
        int n = 0;
        Inorder(span, root, ref n);
    }

    /// <summary>
    /// Insert a value into the AVL tree iteratively and rebalance if necessary.
    /// </summary>
    private Node InsertIterative(Node? root, T value)
    {
        // If the tree is empty, just create a new node.
        if (root is null)
        {
            return new Node(value);
        }

        // We'll track the path in a stack so we can rebalance on the way back up.
        // Stack items: (node, direction) where direction=-1 for left, +1 for right, 0 for "node itself"
        var stack = new Stack<(Node node, int dir)>();
        Node current = root;

        while (true)
        {
            // push the current node and direction to the stack
            stack.Push((current, 0));

            // Go left
            if (Compare(value, current.Item) < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new Node(value);
                    // Inserted a new node, so we need to rebalance on the way back up.
                    stack.Push((current.Left, 0));
                    break;
                }
                current = current.Left;
            }
            else
            {
                // Go right
                if (current.Right is null)
                {
                    current.Right = new Node(value);
                    stack.Push((current.Right, 0));
                    break;
                }
                current = current.Right;
            }
        }

        // After insertion, pop from the stack and rebalance each node if needed.
        Node? newRoot = root;

        // pop the last node from the stack
        var (childNode, _) = stack.Pop();

        while (stack.Count > 0)
        {
            var (parent, _) = stack.Pop();

            // determine the direction of the parent node before updating child's height and rotation
            int dir;
            if (object.ReferenceEquals(parent.Left, childNode))
            {
                dir = -1; // left
            }
            else
            {
                dir = 1;  // right
            }

            UpdateHeight(parent);
            var balanced = Balance(parent);

            // The parent's subtree might have changed to 'balanced' node
            // Stack is not empty => parent of node should exist => update parent's Left or Right
            if (stack.Count > 0)
            {
                // There is a parent node, update its child
                var (upper, _) = stack.Pop();
                if (object.ReferenceEquals(upper.Left, parent))
                {
                    upper.Left = balanced;
                }
                else
                {
                    upper.Right = balanced;
                }

                stack.Push((upper, 0));
            }
            else
            {
                newRoot = balanced;
            }

            // This balanced node is now the 'child' in the upper level
            childNode = balanced;
        }

        return newRoot;
    }

    /// <summary>
    /// Insert into the AVL tree, then rebalance.
    /// </summary>
    private Node InsertRecursive(Node? node, T value)
    {
        // If the tree is empty, return a new node.
        if (node is null)
        {
            return new Node(value);
        }

        // Find the correct position to insert.
        if (Compare(value, node.Item) < 0)
        {
            node.Left = InsertRecursive(node.Left, value);
        }
        else
        {
            node.Right = InsertRecursive(node.Right, value);
        }

        // Update the height and rebalance if necessary.
        UpdateHeight(node);
        return Balance(node);
    }

    /// <summary>
    /// Traverse the tree in order to collect elements in ascending order.
    /// </summary>
    private void Inorder(Span<T> span, Node? node, ref int index)
    {
        if (node is null) return;

        Inorder(span, node.Left, ref index);
        Index(ref span, index++) = node.Item;
        Inorder(span, node.Right, ref index);
    }

    // methods used to maintain AVL tree balance

    /// <summary>
    /// Update the node's height based on the heights of its children.
    /// </summary>
    private void UpdateHeight(Node node)
    {
        // Get the heights of the left and right children.
        int leftHeight = (node.Left is null) ? 0 : node.Left.Height;
        int rightHeight = (node.Right is null) ? 0 : node.Right.Height;

        // node's height = max child's height + 1
        node.Height = 1 + Math.Max(leftHeight, rightHeight);
    }

    /// <summary>
    /// Returns the balance factor (left height - right height).
    /// </summary>
    private int GetBalance(Node node)
    {
        int leftHeight = (node.Left is null) ? 0 : node.Left.Height;
        int rightHeight = (node.Right is null) ? 0 : node.Right.Height;
        return leftHeight - rightHeight;
    }

    /// <summary>
    /// Rebalance the node after insertion using AVL rotations.
    /// </summary>
    private Node Balance(Node node)
    {
        int balance = GetBalance(node);

        // Left heavy (balance > 1)
        if (balance > 1)
        {
            // Left child is right heavy
            if (GetBalance(node.Left!) < 0)
            {
                node.Left = RotateLeft(node.Left!);
            }
            return RotateRight(node);
        }
        // Right heavy (balance < -1)
        else if (balance < -1)
        {
            // Right child is left heavy
            if (GetBalance(node.Right!) > 0)
            {
                node.Right = RotateRight(node.Right!);
            }
            return RotateLeft(node);
        }

        // Node is balanced, return as is.
        return node;
    }

    /// <summary>
    /// Right rotation on the given node.
    /// </summary>
    private Node RotateRight(Node y)
    {
        Node x = y.Left!;
        Node? t2 = x.Right;

        // Perform the rotation
        x.Right = y;
        y.Left = t2;

        // Recompute heights
        UpdateHeight(y);
        UpdateHeight(x);

        // Return the new root
        return x;
    }

    /// <summary>
    /// Left rotation on the given node.
    /// </summary>
    private Node RotateLeft(Node x)
    {
        Node y = x.Right!;
        Node? t2 = y.Left;

        // Perform the rotation
        y.Left = x;
        x.Right = t2;

        // Recompute heights
        UpdateHeight(x);
        UpdateHeight(y);

        // Return the new root
        return y;
    }

    private class Node
    {
        public T Item;
        public Node? Left;
        public Node? Right;
        // For tracking the height in the AVL tree
        public int Height;


        public Node(T value)
        {
            Item = value;
            // A new node starts with height = 1
            Height = 1;
        }
    }
}