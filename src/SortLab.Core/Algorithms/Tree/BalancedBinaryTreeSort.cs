using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*

Ref span (Iterative) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |    37.700 us |   274.936 us |  15.0702 us |    29.200 us |    28.800 us |    55.100 us |   21344 B |
| BalancedTreeSort | 1000   |   433.533 us |    93.762 us |   5.1394 us |   436.400 us |   427.600 us |   436.600 us |  342080 B |
| BalancedTreeSort | 10000  | 5,281.433 us | 3,870.582 us | 212.1597 us | 5,187.100 us | 5,132.800 us | 5,524.400 us | 3654352 B |

Ref span (Recursive) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |    13.633 us |     8.622 us |   0.4726 us |    13.800 us |    13.100 us |    14.000 us |    4736 B |
| BalancedTreeSort | 1000   |   187.633 us |   382.550 us |  20.9689 us |   190.700 us |   165.300 us |   206.900 us |   40736 B |
| BalancedTreeSort | 10000  | 2,028.033 us |   159.111 us |   8.7214 us | 2,032.300 us | 2,018.000 us | 2,033.800 us |  400736 B |

Span (Iterative) ...

| Method                 | Number | Mean          | Error        | StdDev      | Median        | Min           | Max          | Allocated |
|----------------------- |------- |--------------:|-------------:|------------:|--------------:|--------------:|-------------:|----------:|
| BalancedBinaryTreeSort | 100    |     13.600 us |    11.963 us |   0.6557 us |     13.700 us |     12.900 us |     14.20 us |    4736 B |
| BalancedBinaryTreeSort | 1000   |    174.467 us |   199.636 us |  10.9427 us |    170.200 us |    166.300 us |    186.90 us |   40448 B |
| BalancedBinaryTreeSort | 10000  |  2,074.300 us |   387.592 us |  21.2452 us |  2,084.300 us |  2,049.900 us |  2,088.70 us |  400736 B |

*/

/// <summary>
/// 平衡二分木を用いた二分木ソート。挿入時に回転を行って常に高さが O(log n) に保たれ、木を中順巡回することで配列に要素を昇順で再割り当てします。
/// 平衡二分木（AVL木）は、左の子ノードが親ノードより小さく、右の子ノードが大きいという性質を持ちます。  
/// 二分木ソートに比べて、平均および最悪ケースでも O(n log n) のソートが期待できます。
/// <br/>
/// Balanced binary tree sort algorithm. During insertion, rotations are performed to maintain a height of O(log n), and an in-order traversal of the tree reassigns the array elements in ascending order. Compared to binary tree sort, it can achieve O(n log n) sorting in both average and worst cases.
/// Balanced binary trees (AVL trees) have the property that the left child node is smaller than the parent node and the right child node is larger.
/// Compared to binary tree sort, it can achieve O(n log n) sorting in both average and worst cases.
/// </summary>
/// <remarks>
/// family  : tree
/// stable  : no  (Binary Tree Sort is not stable as it does not preserve the relative order of equal elements)  
/// inplace : no  (Requires additional memory for the tree structure)
/// Compare : O(n log n)  
/// Swap    : 0        (No swaps are performed in the array itself)  
/// Index   : O(n)     (Each element is accessed once during in-order traversal)  
/// Order   : O(n log n)
///         * average   : O(n log n)
///         * worst case: O(n log n) (due to tree balancing)
/// </remarks>
/// <typeparam name="T"></typeparam>
public static class BalancedBinaryTreeSort
{
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Insert elements into an AVL tree, then traverse it in-order.
    /// </summary>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context);

        Node<T>? root = null;

        // Insert each element in the array into the AVL tree.
        for (int i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            // root = InsertIterative(root, value, s);
            root = InsertRecursive(root, value, s);
        }

        // Traverse in order and write back into the array.
        int n = 0;
        Inorder(s, root, ref n);
    }

    /// <summary>
    /// Insert a value into the AVL tree iteratively and rebalance if necessary.
    /// </summary>
    private static Node<T> InsertIterative<T>(Node<T>? root, T value, SortSpan<T> s) where T : IComparable<T>
    {
        // If the tree is empty, just create a new node.
        if (root is null)
        {
            return new Node<T>(value);
        }

        // We'll track the path in a stack so we can rebalance on the way back up.
        var stack = new Stack<Node<T>>();
        Node<T> current = root;

        while (true)
        {
            // push the current node to the stack
            stack.Push(current);

            // Go left
            if (s.Compare(value, current.Item) < 0)
            {
                if (current.Left is null)
                {
                    current.Left = new Node<T>(value);
                    // Inserted a new node, so we need to rebalance on the way back up.
                    stack.Push(current.Left);
                    break;
                }
                current = current.Left;
            }
            else
            {
                // Go right
                if (current.Right is null)
                {
                    current.Right = new Node<T>(value);
                    stack.Push(current.Right);
                    break;
                }
                current = current.Right;
            }
        }

        // After insertion, pop from the stack and rebalance each node if needed.
        Node<T>? newRoot = root;

        // pop the last node from the stack
        var childNode = stack.Pop();

        while (stack.Count > 0)
        {
            var parent = stack.Pop();

            UpdateHeight(parent);
            var balanced = Balance(parent);

            // The parent's subtree might have changed to 'balanced' node
            // Stack is not empty => parent of node should exist => update parent's Left or Right
            if (stack.Count > 0)
            {
                // There is a parent node, update its child
                var upper = stack.Pop();
                if (ReferenceEquals(upper.Left, parent))
                {
                    upper.Left = balanced;
                }
                else
                {
                    upper.Right = balanced;
                }

                stack.Push(upper);
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
    private static Node<T> InsertRecursive<T>(Node<T>? node, T value, SortSpan<T> s) where T : IComparable<T>
    {
        // If the tree is empty, return a new node.
        if (node is null)
        {
            return new Node<T>(value);
        }

        // Find the correct position to insert.
        if (s.Compare(value, node.Item) < 0)
        {
            node.Left = InsertRecursive(node.Left, value, s);
        }
        else
        {
            node.Right = InsertRecursive(node.Right, value, s);
        }

        // Update the height and rebalance if necessary.
        UpdateHeight(node);
        return Balance(node);
    }

    /// <summary>
    /// Traverse the tree in order to collect elements in ascending order.
    /// </summary>
    private static void Inorder<T>(SortSpan<T> s, Node<T>? node, ref int index) where T : IComparable<T>
    {
        if (node is null) return;

        Inorder(s, node.Left, ref index);
        s.Write(index++, node.Item);
        Inorder(s, node.Right, ref index);
    }

    // methods used to maintain AVL tree balance

    /// <summary>
    /// Update the node's height based on the heights of its children.
    /// </summary>
    private static void UpdateHeight<T>(Node<T> node) where T : IComparable<T>
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
    private static int GetBalance<T>(Node<T> node) where T : IComparable<T>
    {
        int leftHeight = (node.Left is null) ? 0 : node.Left.Height;
        int rightHeight = (node.Right is null) ? 0 : node.Right.Height;
        return leftHeight - rightHeight;
    }

    /// <summary>
    /// Rebalance the node after insertion using AVL rotations.
    /// </summary>
    private static Node<T> Balance<T>(Node<T> node) where T : IComparable<T>
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
    private static Node<T> RotateRight<T>(Node<T> y) where T : IComparable<T>
    {
        Node<T> x = y.Left!;
        Node<T>? t2 = x.Right;

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
    private static Node<T> RotateLeft<T>(Node<T> x) where T : IComparable<T>
    {
        Node<T> y = x.Right!;
        Node<T>? t2 = y.Left;

        // Perform the rotation
        y.Left = x;
        x.Right = t2;

        // Recompute heights
        UpdateHeight(x);
        UpdateHeight(y);

        // Return the new root
        return y;
    }

    private class Node<T> where T : IComparable<T>
    {
        public T Item;
        public Node<T>? Left;
        public Node<T>? Right;
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
