using System.Buffers;
using SortLab.Core.Contexts;
using System.Runtime.CompilerServices;

namespace SortLab.Core.Algorithms;

/*

Arena-based (struct Node with ArrayPool) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |
| BalancedTreeSort | 1000   |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |
| BalancedTreeSort | 10000  |      TBD us  |      TBD us  |     TBD us  |      TBD us  |      TBD us  |      TBD us  |      TBD B |

Non-Optimized (class Node) ...

| Method           | Number | Mean         | Error        | StdDev      | Median       | Min          | Max          | Allocated |
|----------------- |------- |-------------:|-------------:|------------:|-------------:|-------------:|-------------:|----------:|
| BalancedTreeSort | 100    |    37.700 us |   274.936 us |  15.0702 us |    29.200 us |    28.800 us |    55.100 us |   21344 B |
| BalancedTreeSort | 1000   |   433.533 us |    93.762 us |   5.1394 us |   436.400 us |   427.600 us |   436.600 us |  342080 B |
| BalancedTreeSort | 10000  | 5,281.433 us | 3,870.582 us | 212.1597 us | 5,187.100 us | 5,132.800 us | 5,524.400 us | 3654352 B |

*/

/// <summary>
/// Arena-based balanced binary tree sort (AVL tree) using struct nodes for optimal memory performance.
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
/// This is guaranteed by the BST property and implemented iteratively to avoid stack overflow.</description></item>
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
/// <item><description>Index Reads : Θ(n log n) - With ItemIndex implementation, each comparison reads both values (2 reads per comparison) plus n traversal reads</description></item>
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
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int NULL_INDEX = -1;       // Represents null reference in arena

    // Note: Arena (Node array) operations are not tracked via SortSpan because:
    // 1. Node is an internal implementation detail (stores only indices, not actual data)
    // 2. Only accesses to the original data array (T[]) need statistics tracking via ItemIndex
    // 3. Arena operations are auxiliary and don't represent the algorithm's core operations on data

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

        // Allocate path stack for iterative insertion
        // AVL tree theoretical height: h ≤ 1.44 * log₂(n+2) - 0.328
        // During construction, use conservative estimate to handle temporary imbalance
        // Use: max(n, 2 * log₂(n+1) + 8) for small n, logarithmic for large n
        var avlMaxHeight = span.Length <= 16
            ? span.Length
            : Math.Max((int)(2.0 * Math.Log2(span.Length + 1)) + 8, 32);

        // Use ArrayPool for arena allocation
        // Note: Cannot use stackalloc with Node<T> when T might be a reference type
        var arena = ArrayPool<Node<T>>.Shared.Rent(span.Length);
        int[]? rentedPathStack = null;
        Span<int> pathStack = avlMaxHeight <= 128
            ? stackalloc int[avlMaxHeight]
            : (rentedPathStack = ArrayPool<int>.Shared.Rent(avlMaxHeight)).AsSpan(0, avlMaxHeight);
        try
        {
            var arenaSpan = arena.AsSpan(0, span.Length);
            SortCore(span, context, arenaSpan, pathStack);
        }
        finally
        {
            ArrayPool<Node<T>>.Shared.Return(arena);
            if (rentedPathStack is not null)
            {
                ArrayPool<int>.Shared.Return(rentedPathStack);
            }
        }
    }

    /// <summary>
    /// Core AVL tree sort implementation.
    /// Builds a balanced binary search tree iteratively, then performs in-order traversal.
    /// </summary>
    /// <param name="span">The span to sort</param>
    /// <param name="context">Sort context for statistics tracking</param>
    /// <param name="arena">Preallocated arena for tree nodes</param>
    /// <param name="pathStack">Preallocated stack for tracking insertion path</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SortCore<T>(Span<T> span, ISortContext context, Span<Node<T>> arena, Span<int> pathStack) where T : IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);
        var rootIndex = NULL_INDEX;
        var nodeCount = 0;

        // Insert each element into the AVL tree (iteratively with rebalancing)
        for (int i = 0; i < s.Length; i++)
        {
            rootIndex = InsertIterative(arena, rootIndex, ref nodeCount, i, s, pathStack);
        }

        // Traverse in order and write back into the array (iterative to avoid stack overflow)
        var writeIndex = 0;
        Inorder(s, arena, rootIndex, ref writeIndex);
    }

    /// <summary>
    /// Iteratively insert into the AVL tree using path stack, then rebalance.
    /// Completely recursion-free to avoid stack overhead.
    /// Uses ItemIndex to ensure all data access is tracked via SortSpan.
    /// Performance optimization: uses Node.CachedValue for direct comparison (avoids indirection).
    /// </summary>
    /// <param name="itemIndex">Index in the original span (not the value itself).</param>
    /// <returns>Index of the new root of the tree.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int InsertIterative<T>(Span<Node<T>> arena, int rootIndex, ref int nodeCount, int itemIndex, SortSpan<T> s, Span<int> pathStack) where T : IComparable<T>
    {
        // Read the value to insert (tracked by SortSpan for statistics)
        var insertValue = s.Read(itemIndex);

        // If tree is empty, create root
        if (rootIndex == NULL_INDEX)
        {
            var newIndex = nodeCount++;
            arena[newIndex] = new Node<T>(itemIndex, insertValue);
            return newIndex;
        }

        // Phase 1: Navigate to insertion point and track path
        var stackTop = 0;
        var currentIndex = rootIndex;

        while (true)
        {
            pathStack[stackTop++] = currentIndex;
            // Read current node's value once per iteration (tracked by SortSpan)
            var cmp = s.Compare(insertValue, arena[currentIndex].CachedValue);

            if (cmp < 0)
            {
                // Go left
                var leftIndex = arena[currentIndex].Left;
                if (leftIndex == NULL_INDEX)
                {
                    // Insert here
                    var newIndex = nodeCount++;
                    arena[newIndex] = new Node<T>(itemIndex, insertValue);
                    arena[currentIndex].Left = newIndex;
                    currentIndex = newIndex;
                    break;
                }
                currentIndex = leftIndex;
            }
            else
            {
                // Go right
                var rightIndex = arena[currentIndex].Right;
                if (rightIndex == NULL_INDEX)
                {
                    // Insert here
                    var newIndex = nodeCount++;
                    arena[newIndex] = new Node<T>(itemIndex, insertValue);
                    arena[currentIndex].Right = newIndex;
                    currentIndex = newIndex;
                    break;
                }
                currentIndex = rightIndex;
            }
        }

        // Phase 2: Rebalance upward along the insertion path
        // subtreeRoot: the (possibly rotated) root of the subtree we just finished processing
        // subtreeFrom: the original node index before any rotation (for parent to identify which child)
        var subtreeRoot = currentIndex;
        int subtreeFrom = currentIndex;

        while (stackTop > 0)
        {
            var nodeIndex = pathStack[--stackTop];

            // Ensure nodeIndex points to the correct child subtree root (propagate up)
            if (arena[nodeIndex].Left == subtreeFrom)
            {
                arena[nodeIndex].Left = subtreeRoot;
            }
            else if (arena[nodeIndex].Right == subtreeFrom)
            {
                arena[nodeIndex].Right = subtreeRoot;
            }

            // Update height of current node
            UpdateHeight(arena, nodeIndex);

            // Balance this subtree - may rotate and return a new subtree root
            var newRoot = Balance(arena, nodeIndex);

            subtreeFrom = nodeIndex;
            subtreeRoot = newRoot;
        }

        // After processing all nodes up to the root, subtreeRoot is the new tree root
        // (which may have changed if rotations occurred at the top level)
        return subtreeRoot;
    }

    /// <summary>
    /// Iterative in-order traversal using explicit stack to avoid recursion overhead.
    /// </summary>
    /// <remarks>
    /// Uses an explicit stack to track node indices during traversal, avoiding recursion overhead.
    /// Uses ArrayPool to avoid GC pressure.
    /// Reads actual data from original span using ItemIndex.
    /// </remarks>
    private static void Inorder<T>(SortSpan<T> s, Span<Node<T>> arena, int rootIndex, ref int writeIndex) where T : IComparable<T>
    {
        if (rootIndex == NULL_INDEX) return;

        // Maximum stack size needed for in-order traversal
        // For AVL tree: height ≤ 1.44 * log₁En+2), but use n for safety and simplicity
        var maxStackSize = s.Length;

        int[]? rented = null;
        Span<int> stack = maxStackSize <= 128
            ? stackalloc int[maxStackSize]
            : (rented = ArrayPool<int>.Shared.Rent(maxStackSize)).AsSpan(0, maxStackSize);
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
                // Use CachedValue for direct access (optimal performance)
                // Note: This reduces Read() count but improves performance significantly
                s.Write(writeIndex++, arena[currentIndex].CachedValue);

                // Move to right subtree
                currentIndex = arena[currentIndex].Right;
            }
        }
        finally
        {
            if (rented is not null)
            {
                ArrayPool<int>.Shared.Return(rented);
            }
        }
    }

    // methods used to maintain AVL tree balance

    /// <summary>
    /// Update the node's height based on the heights of its children using arena indices.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateHeight<T>(Span<Node<T>> arena, int nodeIndex)
    {
        var leftIndex = arena[nodeIndex].Left;
        var rightIndex = arena[nodeIndex].Right;

        // Get the heights of the left and right children.
        int leftHeight = (leftIndex == NULL_INDEX) ? 0 : arena[leftIndex].Height;
        int rightHeight = (rightIndex == NULL_INDEX) ? 0 : arena[rightIndex].Height;

        // node's height = max child's height + 1
        arena[nodeIndex].Height = 1 + Math.Max(leftHeight, rightHeight);
    }

    /// <summary>
    /// Returns the balance factor (left height - right height) using arena indices.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetBalance<T>(Span<Node<T>> arena, int nodeIndex)
    {
        var leftIndex = arena[nodeIndex].Left;
        var rightIndex = arena[nodeIndex].Right;

        int leftHeight = (leftIndex == NULL_INDEX) ? 0 : arena[leftIndex].Height;
        int rightHeight = (rightIndex == NULL_INDEX) ? 0 : arena[rightIndex].Height;
        return leftHeight - rightHeight;
    }

    /// <summary>
    /// Rebalance the node after insertion using AVL rotations (arena-based).
    /// </summary>
    /// <returns>Index of the new root after balancing.</returns>
    private static int Balance<T>(Span<Node<T>> arena, int nodeIndex)
    {
        int balance = GetBalance(arena, nodeIndex);

        // Left heavy (balance > 1)
        if (balance > 1)
        {
            var leftIndex = arena[nodeIndex].Left;
            // Left child is right heavy
            if (GetBalance(arena, leftIndex) < 0)
            {
                arena[nodeIndex].Left = RotateLeft(arena, leftIndex);
            }
            return RotateRight(arena, nodeIndex);
        }
        // Right heavy (balance < -1)
        else if (balance < -1)
        {
            var rightIndex = arena[nodeIndex].Right;
            // Right child is left heavy
            if (GetBalance(arena, rightIndex) > 0)
            {
                arena[nodeIndex].Right = RotateRight(arena, rightIndex);
            }
            return RotateLeft(arena, nodeIndex);
        }

        // Node is balanced, return as is.
        return nodeIndex;
    }

    /// <summary>
    /// Right rotation on the given node using arena indices.
    /// </summary>
    /// <returns>Index of the new root after rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int RotateRight<T>(Span<Node<T>> arena, int yIndex)
    {
        var xIndex = arena[yIndex].Left;
        var t2Index = arena[xIndex].Right;

        // Perform the rotation
        arena[xIndex].Right = yIndex;
        arena[yIndex].Left = t2Index;

        // Recompute heights
        UpdateHeight(arena, yIndex);
        UpdateHeight(arena, xIndex);

        // Return the new root
        return xIndex;
    }

    /// <summary>
    /// Left rotation on the given node using arena indices.
    /// </summary>
    /// <returns>Index of the new root after rotation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int RotateLeft<T>(Span<Node<T>> arena, int xIndex)
    {
        var yIndex = arena[xIndex].Right;
        var t2Index = arena[yIndex].Left;

        // Perform the rotation
        arena[yIndex].Left = xIndex;
        arena[xIndex].Right = t2Index;

        // Recompute heights
        UpdateHeight(arena, xIndex);
        UpdateHeight(arena, yIndex);

        // Return the new root
        return yIndex;
    }

    /// <summary>
    /// Arena-based node structure using index references for both tree structure and data.
    /// </summary>
    /// <remarks>
    /// This struct-based node eliminates GC pressure by using value semantics.
    /// Left and Right are indices into the arena array (-1 represents null).
    /// ItemIndex points to the original span element (used for statistics tracking in DEBUG builds).
    /// CachedValue stores the actual value for direct access (performance optimization in RELEASE builds).
    /// Height is maintained for AVL balancing.
    /// </remarks>
    private struct Node<T>
    {
        public int ItemIndex;   // Index in original span (for accessing actual data and DEBUG statistics)
        public T CachedValue;   // Cached value for direct access (avoids ItemIndex to span[ItemIndex] indirection)
        public int Left;        // Index in arena, -1 = null
        public int Right;       // Index in arena, -1 = null
        public int Height;      // For AVL balancing

        public Node(int itemIndex, T value)
        {
            ItemIndex = itemIndex;
            CachedValue = value;
            Left = -1;
            Right = -1;
            Height = 1;  // A new node starts with height = 1
        }
    }
}

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
public static class BalancedBinaryTreeSortNonOptimized
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
