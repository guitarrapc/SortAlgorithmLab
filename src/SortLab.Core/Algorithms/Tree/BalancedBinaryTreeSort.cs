using System.Runtime.CompilerServices;
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
/// 平衡二分木（AVL木）を用いた二分木ソート。挿入時に回転を行って常に高さが O(log n) に保たれ、木を中順巡回することで配列に要素を昇順で再割り当てします。
/// 二分木ソートに比べて、平均および最悪ケースでも O(n log n) のソートが保証されます。
/// <br/>
/// Balanced binary tree sort using AVL tree. Rotations are performed during insertion to maintain a height of O(log n), and an in-order traversal reassigns array elements in ascending order.
/// Compared to binary tree sort, it guarantees O(n log n) sorting in both average and worst cases.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct AVL Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binary Search Tree Property:</strong> For every node, all values in the left subtree must be less than the node's value,
/// and all values in the right subtree must be greater than or equal to the node's value.
/// This property is maintained by comparing values during insertion (lines 165-171, 187-193).</description></item>
/// <item><description><strong>AVL Balance Property:</strong> For every node, the height difference between left and right subtrees (balance factor) must be at most 1.
/// Balance factor = height(left subtree) - height(right subtree) ∈ {-1, 0, 1}.
/// This is enforced by the Balance() method after every insertion (lines 229-261).</description></item>
/// <item><description><strong>Height Maintenance:</strong> Each node stores its height, which is updated after any structural change.
/// Height = 1 + max(height(left), height(right)).
/// This is computed by UpdateHeight() after insertions and rotations (lines 215-223).</description></item>
/// <item><description><strong>Rotation Correctness:</strong> When the balance factor violates the AVL property (|balance| > 1), rotations restore balance while preserving BST property:
/// <list type="bullet">
/// <item><description>Left-Left case (balance > 1, left child balanced): Single right rotation</description></item>
/// <item><description>Left-Right case (balance > 1, left child right-heavy): Left rotation on left child, then right rotation</description></item>
/// <item><description>Right-Right case (balance &lt; -1, right child balanced): Single left rotation</description></item>
/// <item><description>Right-Left case (balance &lt; -1, right child left-heavy): Right rotation on right child, then left rotation</description></item>
/// </list>
/// All rotations preserve the in-order traversal sequence (lines 263-303).</description></item>
/// <item><description><strong>In-order Traversal Correctness:</strong> Visiting nodes in left-root-right order produces elements in sorted ascending order.
/// This is guaranteed by the BST property and implemented recursively (lines 205-211).</description></item>
/// </list>
/// <para><strong>Mathematical Proof of O(log n) Height:</strong></para>
/// <list type="bullet">
/// <item><description>Let N(h) = minimum number of nodes in an AVL tree of height h</description></item>
/// <item><description>N(h) = N(h-1) + N(h-2) + 1 (Fibonacci-like recurrence)</description></item>
/// <item><description>N(h) ≥ F(h+2) - 1, where F is the Fibonacci sequence</description></item>
/// <item><description>Therefore, h ≤ 1.44 × log₂(n + 2) - 0.328 ≈ O(log n)</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree</description></item>
/// <item><description>Stable      : No (does not preserve relative order of equal elements)</description></item>
/// <item><description>In-place    : No (requires O(n) auxiliary space for tree structure)</description></item>
/// <item><description>Best case   : Θ(n log n) - Already sorted or reversed, still requires building balanced tree</description></item>
/// <item><description>Average case: Θ(n log n) - Each of n insertions takes O(log n) comparisons</description></item>
/// <item><description>Worst case  : O(n log n) - Guaranteed by AVL balancing property</description></item>
/// <item><description>Comparisons : O(n log n) - Each insertion performs at most log₂(n) comparisons</description></item>
/// <item><description>Index Reads : Θ(n) - Each element is read once during insertion</description></item>
/// <item><description>Index Writes: Θ(n) - Each element is written once during in-order traversal</description></item>
/// <item><description>Swaps       : 0 - No swaps performed; only tree node manipulations</description></item>
/// <item><description>Rotations   : O(n log n) worst case - At most 2 rotations per insertion (amortized O(1))</description></item>
/// </list>
/// <para><strong>Advantages over unbalanced BST:</strong></para>
/// <list type="bullet">
/// <item><description>Guaranteed O(n log n) time complexity even on sorted/reversed input (BST degrades to O(n²))</description></item>
/// <item><description>Predictable performance regardless of input distribution</description></item>
/// <item><description>Height always bounded by 1.44 × log₂(n)</description></item>
/// </list>
/// </remarks>
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

        // Track the path: (node, wentLeft) where wentLeft indicates which child we followed
        var path = new Stack<(Node<T> node, bool wentLeft)>();
        Node<T> current = root;

        // Navigate to insertion point
        while (true)
        {
            bool goLeft = s.Compare(value, current.Item) < 0;
            
            if (goLeft)
            {
                if (current.Left is null)
                {
                    current.Left = new Node<T>(value);
                    break;
                }
                path.Push((current, true));
                current = current.Left;
            }
            else
            {
                if (current.Right is null)
                {
                    current.Right = new Node<T>(value);
                    break;
                }
                path.Push((current, false));
                current = current.Right;
            }
        }

        // Start rebalancing from the parent of the inserted node
        UpdateHeight(current);
        var balanced = Balance(current);
        
        // Rebalance upward along the insertion path
        while (path.Count > 0)
        {
            var (parent, wentLeft) = path.Pop();
            
            // Connect the balanced child to its parent
            if (wentLeft)
                parent.Left = balanced;
            else
                parent.Right = balanced;
            
            // Update and balance the parent
            UpdateHeight(parent);
            balanced = Balance(parent);
        }

        // Return the new root (which might have changed due to rotations)
        return balanced;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
