using SortAlgorithm.VisualizationWeb.Models;
using SortAlgorithm.Contexts;
using SortAlgorithm.Algorithms;

namespace SortAlgorithm.VisualizationWeb.Services;

/// <summary>
/// 全ソートアルゴリズムのメタデータを管理するレジストリ
/// </summary>
public class AlgorithmRegistry
{
    private readonly List<AlgorithmMetadata> _algorithms = new();
    
    public AlgorithmRegistry()
    {
        RegisterAlgorithms();
    }
    
    public IReadOnlyList<AlgorithmMetadata> GetAllAlgorithms() => _algorithms.AsReadOnly();
    
    public IEnumerable<AlgorithmMetadata> GetByCategory(string category) 
        => _algorithms.Where(a => a.Category == category);
    
    public IEnumerable<string> GetCategories() 
        => _algorithms.Select(a => a.Category).Distinct().OrderBy(c => c);
    
    private void RegisterAlgorithms()
    {
        // 最大サイズは全て4096、推奨サイズは計算量に応じて設定
        const int MAX_SIZE = 4096;
        
        // Exchange Sorts - O(n²) - 推奨256
        Add("BubbleSort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BubbleSort.Sort(arr.AsSpan(), ctx));
        Add("CocktailShakerSort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CocktailShakerSort.Sort(arr.AsSpan(), ctx));
        Add("CombSort", "Exchange Sorts", "O(n²)", MAX_SIZE, 512, (arr, ctx) => CombSort.Sort(arr.AsSpan(), ctx));
        Add("OddEvenSort", "Exchange Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => OddEvenSort.Sort(arr.AsSpan(), ctx));
        
        // Selection Sorts - O(n²) - 推奨256
        Add("SelectionSort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => SelectionSort.Sort(arr.AsSpan(), ctx));
        Add("DoubleSelectionSort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => DoubleSelectionSort.Sort(arr.AsSpan(), ctx));
        Add("CycleSort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => CycleSort.Sort(arr.AsSpan(), ctx));
        Add("PancakeSort", "Selection Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => PancakeSort.Sort(arr.AsSpan(), ctx));
        
        // Insertion Sorts - O(n²) ~ O(n^1.5) - 推奨256-1024
        Add("InsertionSort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => InsertionSort.Sort(arr.AsSpan(), ctx));
        Add("BinaryInsertSort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => BinaryInsertSort.Sort(arr.AsSpan(), ctx));
        Add("ShellSortKnuth1973", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortKnuth1973.Sort(arr.AsSpan(), ctx));
        Add("ShellSortSedgewick1986", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortSedgewick1986.Sort(arr.AsSpan(), ctx));
        Add("ShellSortTokuda1992", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortTokuda1992.Sort(arr.AsSpan(), ctx));
        Add("ShellSortCiura2001", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortCiura2001.Sort(arr.AsSpan(), ctx));
        Add("ShellSortLee2021", "Insertion Sorts", "O(n^1.5)", MAX_SIZE, 1024, (arr, ctx) => ShellSortLee2021.Sort(arr.AsSpan(), ctx));
        Add("GnomeSort", "Insertion Sorts", "O(n²)", MAX_SIZE, 256, (arr, ctx) => GnomeSort.Sort(arr.AsSpan(), ctx));
        
        // Merge Sorts - O(n log n) - 推奨2048
        Add("MergeSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => MergeSort.Sort(arr.AsSpan(), ctx));
        Add("TimSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TimSort.Sort(arr.AsSpan(), ctx));
        Add("PowerSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => PowerSort.Sort(arr.AsSpan(), ctx));
        Add("DropMergeSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => DropMergeSort.Sort(arr.AsSpan(), ctx));
        Add("ShiftSort", "Merge Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => ShiftSort.Sort(arr.AsSpan(), ctx));
        
        // Heap Sorts - O(n log n) - 推奨2048
        Add("HeapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => HeapSort.Sort(arr.AsSpan(), ctx));
        Add("BottomupHeapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BottomupHeapSort.Sort(arr.AsSpan(), ctx));
        Add("WeakHeapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => WeakHeapSort.Sort(arr.AsSpan(), ctx));
        Add("SmoothSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => SmoothSort.Sort(arr.AsSpan(), ctx));
        Add("TernaryHeapSort", "Heap Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => TernaryHeapSort.Sort(arr.AsSpan(), ctx));
        
        // Partition Sorts - O(n log n) - 推奨2048-4096
        Add("QuickSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSort.Sort(arr.AsSpan(), ctx));
        Add("QuickSortMedian3", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian3.Sort(arr.AsSpan(), ctx));
        Add("QuickSortMedian9", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortMedian9.Sort(arr.AsSpan(), ctx));
        Add("QuickSortDualPivot", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => QuickSortDualPivot.Sort(arr.AsSpan(), ctx));
        Add("BlockQuickSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BlockQuickSort.Sort(arr.AsSpan(), ctx));
        Add("StableQuickSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => StableQuickSort.Sort(arr.AsSpan(), ctx));
        Add("IntroSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => IntroSort.Sort(arr.AsSpan(), ctx));
        Add("PDQSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => PDQSort.Sort(arr.AsSpan(), ctx));
        Add("StdSort", "Partition Sorts", "O(n log n)", MAX_SIZE, 4096, (arr, ctx) => StdSort.Sort(arr.AsSpan(), ctx));
        
        // Distribution Sorts - O(n) ~ O(nk) - 推奨4096
        // CountingSort and BucketSort require key selector function - omitted for now
        Add("RadixLSD4Sort", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD4Sort.Sort(arr.AsSpan(), ctx));
        Add("RadixLSD10Sort", "Distribution Sorts", "O(nk)", MAX_SIZE, 4096, (arr, ctx) => RadixLSD10Sort.Sort(arr.AsSpan(), ctx));
        
        // Network Sorts - O(log²n) - 推奨2048
        Add("BitonicSort", "Network Sorts", "O(log²n)", MAX_SIZE, 2048, (arr, ctx) => BitonicSort.Sort(arr.AsSpan(), ctx));
        Add("BitonicSortFill", "Network Sorts", "O(log²n)", MAX_SIZE, 2048, (arr, ctx) => BitonicSortFill.Sort(arr.AsSpan(), ctx));
        // BitonicSortParallel requires explicit type parameter - omitted for now
        
        // Tree Sorts - O(n log n) - 推奨1024
        Add("BinaryTreeSort", "Tree Sorts", "O(n log n)", MAX_SIZE, 1024, (arr, ctx) => BinaryTreeSort.Sort(arr.AsSpan(), ctx));
        Add("BalancedBinaryTreeSort", "Tree Sorts", "O(n log n)", MAX_SIZE, 2048, (arr, ctx) => BalancedBinaryTreeSort.Sort(arr.AsSpan(), ctx));
        
        // Joke Sorts - O(n!) ~ O(∞) - 推奨16（注意: 極めて遅い）
        Add("BogoSort", "Joke Sorts", "O(n!)", MAX_SIZE, 16, (arr, ctx) => BogoSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
        Add("SlowSort", "Joke Sorts", "O(n^(log n))", MAX_SIZE, 16, (arr, ctx) => SlowSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
        Add("StoogeSort", "Joke Sorts", "O(n^2.7)", MAX_SIZE, 16, (arr, ctx) => StoogeSort.Sort(arr.AsSpan(), ctx), "⚠️ Extremely slow!");
    }
    
    private void Add(string name, string category, string complexity, int maxElements, int recommendedSize,
        Action<int[], ISortContext> sortAction, string description = "")
    {
        _algorithms.Add(new AlgorithmMetadata
        {
            Name = name,
            Category = category,
            TimeComplexity = complexity,
            MaxElements = maxElements,
            RecommendedSize = recommendedSize,
            SortAction = sortAction,
            Description = description
        });
    }
}
