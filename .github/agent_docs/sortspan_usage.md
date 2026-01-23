# SortSpan Usage Guidelines

When implementing sorting algorithms, **always use SortSpan<T>** for all array/span operations.

## Why SortSpan?

- **Accurate statistics** for algorithm analysis via ISortContext
- **Clean abstraction** for tracking operations
- **Minimal performance impact** with NullContext (empty method calls)
- **Consistent code style** across all sorting implementations
- **Separation of concerns** - algorithm logic vs observation

## Required Operations

### 1. Reading Elements: `s.Read(i)`

Notifies context via `OnIndexRead(i)`

```csharp
// ✅ Correct - uses Read
var value = s.Read(i);

// ❌ Incorrect - bypasses context
var value = span[i];
```

### 2. Writing Elements: `s.Write(i, value)`

Notifies context via `OnIndexWrite(i)`

```csharp
// ✅ Correct - uses Write
s.Write(i, value);

// ❌ Incorrect - bypasses context
span[i] = value;
```

### 3. Comparing Elements: `s.Compare(i, j)`

Reads both elements, compares, and notifies context via `OnCompare(i, j, result)`

```csharp
// ✅ Correct - comparing two indices
if (s.Compare(i, j) < 0) { ... }

// ❌ Incorrect - bypasses context
if (span[i].CompareTo(span[j]) < 0) { ... }
```

For comparing with a value (not an index):

```csharp
var value = s.Read(someIndex);

// ✅ Correct - comparing index with value
if (s.Compare(i, value) < 0) { ... }

// ✅ Correct - comparing value with index
if (s.Compare(value, i) < 0) { ... }

// ❌ Incorrect - direct CompareTo bypasses context
if (s.Read(i).CompareTo(value) < 0) { ... }
```

**Important:** Never use `CompareTo()` directly. All comparisons must go through `SortSpan` methods.

### 4. Swapping Elements: `s.Swap(i, j)`

Reads both elements, notifies context via `OnSwap(i, j)`, then writes

```csharp
// ✅ Correct
s.Swap(i, j);

// ❌ Incorrect - bypasses context
(span[i], span[j]) = (span[j], span[i]);
```

## Buffer Management

When using internal temporary buffers, always track them with a unique `bufferId`:

```csharp
public static class MySort
{
    // Buffer identifiers for visualization
    private const int BUFFER_MAIN = 0;       // Main input array
    private const int BUFFER_TEMP = 1;       // Temporary merge buffer
    private const int BUFFER_AUX = 2;        // Auxiliary buffer

    public static void Sort<T>(Span<T> span, ISortContext context) where T : IComparable<T>
    {
        var s = new SortSpan<T>(span, context, BUFFER_MAIN);

        // For temporary buffers
        Span<T> tempBuffer = stackalloc T[span.Length];
        var temp = new SortSpan<T>(tempBuffer, context, BUFFER_TEMP);
    }
}
```

**Rules:**

1. ✅ **Always use SortSpan for internal buffers** - even if they're temporary arrays
2. ✅ **Assign unique bufferIds** - starting from 0 for main array
3. ✅ **Document buffer purpose** - use clear constant names
4. ❌ **Never bypass SortSpan** - direct array access loses statistics

## Usage Examples

```csharp
// Production - no statistics
MySort.Sort<int>(array);

// With statistics
var stats = new StatisticsContext();
MySort.Sort(array.AsSpan(), stats);
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");

// With visualization
var viz = new VisualizationContext(
    onSwap: (i, j) => RenderSwap(i, j),
    onCompare: (i, j, result) => HighlightCompare(i, j)
);
MySort.Sort(array.AsSpan(), viz);
```
