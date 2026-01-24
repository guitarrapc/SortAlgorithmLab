using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SortLab.Core.Contexts;

namespace SortLab.Core.Algorithms;

/*

Arena-based (struct Node with ArrayPool) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BinaryTreeSort   | 100    |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |
| BinaryTreeSort   | 1000   |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |
| BinaryTreeSort   | 10000  |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |

Non-Optimized (class Node) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BinaryTreeSort   | 100    |     7.000 us |     8.360 us |   0.4583 us |     7.100 us |     6.500 us |     7.400 us |    4448 B |
| BinaryTreeSort   | 1000   |   114.233 us |   162.629 us |   8.9142 us |   116.200 us |   104.500 us |   122.000 us |   40448 B |
| BinaryTreeSort   | 10000  |   653.500 us |    14.249 us |   0.7810 us |   653.900 us |   652.600 us |   654.000 us |  400736 B |

*/

/// <summary>
/// アリーナベース(配列)のバイナリツリーソート、構造体ノードを使用して最適なメモリパフォーマンスを目指している。
/// バイナリ検索木(Binary Search Tree, BST)を使用したソートアルゴリズム、二分木ソートとも呼ばれる。
/// バイナリ検索木では、左の子ノードは親ノードより小さく、右の子ノードは親ノードより大きいことが保証される。
/// この特性により、木の中間順序走査 (in-order traversal) を行うことで配列がソートされる。
/// ただし、木が不均衡になると最悪ケースでO(n²)の時間がかかる可能性がある。
/// <br/>
/// Arena-based (array) binary tree sort aiming for optimal memory performance using struct nodes.
/// A sorting algorithm that uses a binary search tree. In a binary search tree, the left child node is guaranteed to be smaller than the parent node, and the right child node is guaranteed to be larger.
/// This property ensures that performing an in-order traversal of the tree results in a sorted array.
/// However, an unbalanced tree can lead to O(n²) worst-case time complexity in terms of comparisons.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Binary Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binary Search Tree Property (BST Invariant):</strong> For every node in the tree, all values in the left subtree must be strictly less than the node's value,
/// and all values in the right subtree must be greater than or equal to the node's value.
/// This implementation maintains this invariant during iterative insertion by comparing the new value with the current node and navigating left (value &lt; current) or right (value ≥ current).</description></item>
/// <item><description><strong>Complete Tree Construction:</strong> All n elements from the input array must be inserted into the BST before traversal.
/// Each insertion operation locates the correct position in O(h) comparisons, where h is the current tree height.</description></item>
/// <item><description><strong>In-Order Traversal Correctness:</strong> The tree must be traversed in strict in-order sequence (left subtree → root → right subtree).
/// This ordering, combined with the BST property, guarantees that elements are visited in ascending sorted order.
/// This implementation uses iterative in-order traversal with an explicit stack to avoid recursion overhead.</description></item>
/// <item><description><strong>Comparison Transitivity:</strong> The comparison operation must be consistent and transitive.
/// For all elements a, b, c: if a &lt; b and b &lt; c, then a &lt; c. This ensures that the BST property is preserved throughout all insertions.</description></item>
/// <item><description><strong>Tree Height Impact:</strong> The tree height h determines insertion performance.
/// Balanced tree (random input): h = O(log n), giving O(n log n) total time.
/// Degenerate tree (sorted/reversed input): h = O(n), degrading to O(n²) total time.</description></item>
/// </list>
/// <para><strong>Mathematical Proof of Correctness:</strong></para>
/// <list type="bullet">
/// <item><description><strong>BST Property Preservation:</strong> During insertion, if value &lt; current.value, it goes to left subtree, ensuring all left descendants are smaller.
/// If value ≥ current.value, it goes to right subtree, ensuring all right descendants are larger. This recursively maintains the BST property.</description></item>
/// <item><description><strong>In-Order Traversal Property:</strong> For any node N with left child L and right child R:
/// All values in L's subtree &lt; N.value &lt; all values in R's subtree (by BST property).
/// In-order traversal visits: L's subtree → N → R's subtree, producing ascending sequence.</description></item>
/// <item><description><strong>Sorted Output Guarantee:</strong> By induction: Base case (1 node) trivially sorted. Inductive step: if left and right subtrees produce sorted sequences,
/// in-order traversal concatenates them as: sorted_left &lt; root &lt; sorted_right = fully sorted output.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree-based sorting</description></item>
/// <item><description>Stable      : No (equal elements may be reordered based on insertion order)</description></item>
/// <item><description>In-place    : No (Requires O(n) auxiliary space for tree nodes in arena)</description></item>
/// <item><description>Best case   : Θ(n log n) - Balanced tree (e.g., random input or middle-out insertion)</description></item>
/// <item><description>Average case: Θ(n log n) - Tree height is O(log n), each insertion takes O(log n) comparisons</description></item>
/// <item><description>Worst case  : Θ(n²) - Completely unbalanced tree (e.g., sorted or reverse-sorted input forms a linear chain)</description></item>
/// <item><description>Comparisons : Best Θ(n log n), Average Θ(n log n), Worst Θ(n²)</description></item>
/// <item><description>  - Sorted input: n(n-1)/2 comparisons (each insertion compares with all previous elements)</description></item>
/// <item><description>  - Random input: ~1.39n log n comparisons (empirically, for balanced trees)</description></item>
/// <item><description>Index Reads : Θ(n log n) - With ItemIndex implementation, each comparison reads both values (2 reads per comparison) plus n traversal reads</description></item>
/// <item><description>Index Writes: Θ(n) - Each element is written once during in-order traversal</description></item>
/// <item><description>Swaps       : 0 (No swapping; elements are copied to tree nodes and then back to array)</description></item>
/// <item><description>Space       : O(n) - Arena allocation for n nodes (struct-based, minimal per-node overhead)</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative insertion instead of recursive insertion to reduce call stack overhead</description></item>
/// <item><description>Uses iterative in-order traversal with explicit stack to avoid recursion overhead and stack overflow risk</description></item>
/// <item><description>Tree nodes are struct-based, stored in an arena (array) to eliminate per-node GC pressure</description></item>
/// <item><description>Node references are index-based (int) instead of pointers, enabling struct usage</description></item>
/// <item><description>Arena uses ArrayPool for memory efficiency (zero allocations when pooled buffer available)</description></item>
/// <item><description>Equal elements are inserted to the right subtree (value ≥ current), making the sort unstable</description></item>
/// <item><description>No tree balancing is performed; for guaranteed O(n log n) performance, consider using AVL or Red-Black tree variants</description></item>
/// <item><description><strong>Optimized:</strong> This version uses arena-based struct nodes with iterative traversal. See <see cref="BinaryTreeSortNonOptimized"/> for comparison with class-based nodes.</description></item>
/// </list>
/// </remarks>
public static class BinaryTreeSort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int NULL_INDEX = -1;       // Represents null reference in arena
    
    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        // Use stackalloc for small arrays, ArrayPool for large arrays
        // Node struct size: 12 bytes (3 ints), so 85 nodes ≈ 1020 bytes (safe for stack)
        Node[]? rentedArena = null;
        Span<Node> arena = span.Length <= 85
            ? stackalloc Node[span.Length]
            : (rentedArena = ArrayPool<Node>.Shared.Rent(span.Length)).AsSpan(0, span.Length);
        try
        {
            SortCore(span, context, arena);
        }
        finally
        {
            if (rentedArena is not null)
            {
                ArrayPool<Node>.Shared.Return(rentedArena);
            }
        }
    }

    /// <summary>
    /// Core binary tree sort implementation.
    /// Builds a binary search tree iteratively, then performs in-order traversal.
    /// </summary>
    /// <param name="span">The span to sort</param>
    /// <param name="context">Sort context for statistics tracking</param>
    /// <param name="arena">Preallocated arena for tree nodes</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(Span<T> span, ISortContext context, Span<Node> arena) where T : IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        var rootIndex = NULL_INDEX;
        var nodeCount = 0;

        // Build tree by inserting each element (using ItemIndex)
        for (var i = 0; i < s.Length; i++)
        {
            InsertIterative(arena, ref rootIndex, ref nodeCount, i, s);
        }

        // Traverse tree in-order and write elements back
        var writeIndex = 0;
        InorderTraversal(s, arena, rootIndex, ref writeIndex);
    }

    /// <summary>
    /// Iterative insertion using arena-based index references.
    /// Uses ItemIndex to ensure all data access is tracked via SortSpan.
    /// </summary>
    /// <param name="itemIndex">Index in the original span (not the value itself).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InsertIterative<T>(Span<Node> arena, ref int rootIndex, ref int nodeCount, int itemIndex, SortSpan<T> s) where T : IComparable<T>
    {
        // If tree is empty, create root
        if (rootIndex == NULL_INDEX)
        {
            arena[nodeCount] = new Node(itemIndex);
            rootIndex = nodeCount;
            nodeCount++;
            return;
        }

        // Traverse tree to find insertion point
        var currentIndex = rootIndex;
        while (true)
        {
            ref var current = ref arena[currentIndex];
            // Compare using ItemIndex - all data access via SortSpan
            var cmp = s.Compare(itemIndex, current.ItemIndex);

            if (cmp < 0)
            {
                // Go left
                if (current.Left == NULL_INDEX)
                {
                    arena[nodeCount] = new Node(itemIndex);
                    current.Left = nodeCount;
                    nodeCount++;
                    break;
                }
                currentIndex = current.Left;
            }
            else
            {
                // Go right (includes equal values)
                if (current.Right == NULL_INDEX)
                {
                    arena[nodeCount] = new Node(itemIndex);
                    current.Right = nodeCount;
                    nodeCount++;
                    break;
                }
                currentIndex = current.Right;
            }
        }
    }

    /// <summary>
    /// Iterative in-order traversal of the arena-based tree using an explicit stack.
    /// </summary>
    /// <remarks>
    /// Iterative implementation avoids recursion overhead and stack overflow risk.
    /// Uses an explicit stack to track nodes during traversal.
    /// Worst case requires O(n) stack space for completely unbalanced trees.
    /// For small arrays (≤128), uses stackalloc for zero heap allocation.
    /// For large arrays (>128), uses ArrayPool to avoid GC pressure.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InorderTraversal<T>(SortSpan<T> s, Span<Node> arena, int rootIndex, ref int writeIndex) where T : IComparable<T>
    {
        if (rootIndex == NULL_INDEX) return;

        // Explicit stack for iterative traversal (worst case: all nodes)
        int[]? rentedStack = null;
        Span<int> stack = arena.Length <= 128
            ? stackalloc int[arena.Length]
            : (rentedStack = ArrayPool<int>.Shared.Rent(arena.Length)).AsSpan(0, arena.Length);
        try
        {
            var stackTop = 0;
            var currentIndex = rootIndex;

            // Iterative in-order traversal
            while (stackTop > 0 || currentIndex != NULL_INDEX)
            {
                // Traverse left subtree: push all left nodes onto stack
                while (currentIndex != NULL_INDEX)
                {
                    stack[stackTop++] = currentIndex;
                    currentIndex = arena[currentIndex].Left;
                }

                // Process the node at top of stack
                currentIndex = stack[--stackTop];
                // Read actual data from original span using ItemIndex (tracked by SortSpan)
                var value = s.Read(arena[currentIndex].ItemIndex);
                s.Write(writeIndex++, value);

                // Move to right subtree
                currentIndex = arena[currentIndex].Right;
            }
        }
        finally
        {
            if (rentedStack is not null)
            {
                ArrayPool<int>.Shared.Return(rentedStack);
            }
        }
    }

    /// <summary>
    /// Arena-based node structure using index references for both tree structure and data.
    /// </summary>
    /// <remarks>
    /// This struct-based node eliminates GC pressure by using value semantics.
    /// Left and Right are indices into the arena array (-1 represents null).
    /// ItemIndex points to the original span element, ensuring all data access is tracked via SortSpan.
    /// </remarks>
    private struct Node
    {
        public int ItemIndex; // Index in original span (for accessing actual data)
        public int Left;      // Index in arena, -1 = null
        public int Right;     // Index in arena, -1 = null

        public Node(int itemIndex)
        {
            ItemIndex = itemIndex;
            Left = -1;
            Right = -1;
        }
    }
}

/// <summary>
/// クラスベースノードを使用した非最適化バイナリツリーソート。
/// バイナリ検索木(Binary Search Tree, BST)を使用したソートアルゴリズム、二分木ソートとも呼ばれる。
/// バイナリ検索木では、左の子ノードは親ノードより小さく、右の子ノードは親ノードより大きいことが保証される。
/// この特性により、木の中間順序走査 (in-order traversal) を行うことで配列がソートされる。
/// ただし、木が不均衡になると最悪ケースでO(n²)の時間がかかる可能性がある。また、ノードごとにクラスインスタンスを生成するためメモリアロケーションが多く、現実的なソートとしてはQuickSortやMergeSortを用いることが多い。
/// <br/>
/// Non-optimized version of Binary Tree Sort using class-based nodes.
/// A sorting algorithm that uses a binary search tree. In a binary search tree, the left child node is guaranteed to be smaller than the parent node, and the right child node is guaranteed to be larger.
/// This property ensures that performing an in-order traversal of the tree results in a sorted array.
/// However, an unbalanced tree can lead to O(n²) worst-case time complexity. Additionally, because each node allocates a class instance, memory allocations are high, making QuickSort or MergeSort more practical for real-world sorting.
/// </summary>
/// <remarks>
/// <para><strong>Theoretical Conditions for Correct Binary Tree Sort:</strong></para>
/// <list type="number">
/// <item><description><strong>Binary Search Tree Property:</strong> For every node, all values in the left subtree must be less than the node's value, and all values in the right subtree must be greater than or equal to the node's value.
/// This implementation maintains this invariant during insertion (value &lt; current goes left, value ≥ current goes right).</description></item>
/// <item><description><strong>Complete Tree Construction:</strong> All n elements must be inserted into the BST.
/// Each insertion reads one element from the array (n reads total).</description></item>
/// <item><description><strong>In-Order Traversal:</strong> The tree must be traversed in in-order (left → root → right) to produce sorted output.
/// This traversal visits each node exactly once, writing n elements back to the array.</description></item>
/// <item><description><strong>Comparison Consistency:</strong> The comparison operation must be consistent and transitive.
/// For all elements a, b, c: if a &lt; b and b &lt; c, then a &lt; c.</description></item>
/// </list>
/// <para><strong>Performance Characteristics:</strong></para>
/// <list type="bullet">
/// <item><description>Family      : Tree-based sorting</description></item>
/// <item><description>Stable      : No (equal elements may be reordered based on insertion order)</description></item>
/// <item><description>In-place    : No (Requires O(n) auxiliary space for tree nodes)</description></item>
/// <item><description>Best case   : Θ(n log n) - Balanced tree (e.g., random input or middle-out insertion)</description></item>
/// <item><description>Average case: Θ(n log n) - Tree height is O(log n), each insertion takes O(log n) comparisons</description></item>
/// <item><description>Worst case  : Θ(n²) - Completely unbalanced tree (e.g., sorted or reverse-sorted input forms a linear chain)</description></item>
/// <item><description>Comparisons : Best Θ(n log n), Average Θ(n log n), Worst Θ(n²)</description></item>
/// <item><description>  - Sorted input: n(n-1)/2 comparisons (each insertion compares with all previous elements)</description></item>
/// <item><description>  - Random input: ~1.39n log n comparisons (empirically, for balanced trees)</description></item>
/// <item><description>Index Reads : Θ(n) - Each element is read once during tree construction</description></item>
/// <item><description>Index Writes: Θ(n) - Each element is written once during in-order traversal</description></item>
/// <item><description>Swaps       : 0 (No swapping; elements are copied to tree nodes and then back to array)</description></item>
/// <item><description>Space       : O(n) - One node allocated per element (worst case: n allocations of ~24-32 bytes each)</description></item>
/// </list>
/// <para><strong>Implementation Notes:</strong></para>
/// <list type="bullet">
/// <item><description>Uses iterative insertion instead of recursive insertion to reduce call stack overhead</description></item>
/// <item><description>Tree nodes are implemented as reference types (class) because C# structs cannot contain self-referencing fields</description></item>
/// <item><description>Equal elements are inserted to the right subtree (value ≥ current), making the sort unstable</description></item>
/// <item><description>No tree balancing is performed; for guaranteed O(n log n) performance, consider using AVL or Red-Black tree variants</description></item>
/// <item><description><strong>Non-Optimized:</strong> This version uses class-based nodes with reference type overhead. See <see cref="BinaryTreeSort"/> for an arena-based optimized version.</description></item>
/// </list>
/// </remarks>
public static class BinaryTreeSortNonOptimized
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array

    /// <summary>
    /// Sorts the elements in the specified span in ascending order using the default comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort in place.</param>
    public static void Sort<T>(Span<T> span) where T : IComparable<T>
    {
        Sort(span, NullContext.Default);
    }

    /// <summary>
    /// Sorts the elements in the specified span using the provided sort context.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span. Must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="span">The span of elements to sort. The elements within this span will be reordered in place.</param>
    /// <param name="context">The sort context that defines the sorting strategy or options to use during the operation. Cannot be null.</param>
    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        if (span.Length <= 1) return;

        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // The root node of the binary tree (null == the tree is empty).
        Node<T>? root = null;

        for (var i = 0; i < s.Length; i++)
        {
            var value = s.Read(i);
            InsertIterative(ref root, value, s);
        }

        // Traverse the tree in inorder and write elements back into the array.
        var n = 0;
        Inorder(s, root, ref n);
    }

    /// <summary>
    /// Iterative insertion. Instead of using recursion, it loops to find the child nodes.
    /// </summary>
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
    /// Class-based node with reference type overhead. Each node allocation incurs GC pressure.
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
