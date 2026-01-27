using SortAlgorithm.VisualizationWeb.Models;

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
        // Exchange Sorts - O(n²)
        Add("BubbleSort", "Exchange Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.BubbleSort");
        Add("CocktailShakerSort", "Exchange Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.CocktailShakerSort");
        Add("CombSort", "Exchange Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.CombSort");
        Add("OddEvenSort", "Exchange Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.OddEvenSort");
        
        // Selection Sorts - O(n²)
        Add("SelectionSort", "Selection Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.SelectionSort");
        Add("DoubleSelectionSort", "Selection Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.DoubleSelectionSort");
        Add("CycleSort", "Selection Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.CycleSort");
        Add("PancakeSort", "Selection Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.PancakeSort");
        
        // Insertion Sorts - O(n²) ~ O(n log n)
        Add("InsertionSort", "Insertion Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.InsertionSort");
        Add("BinaryInsertSort", "Insertion Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.BinaryInsertSort");
        Add("ShellSort", "Insertion Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.ShellSort");
        Add("GnomeSort", "Insertion Sorts", "O(n²)", 256, "SortAlgorithm.Algorithms.GnomeSort");
        
        // Merge Sorts - O(n log n)
        Add("MergeSort", "Merge Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.MergeSort");
        Add("TimSort", "Merge Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.TimSort");
        Add("PowerSort", "Merge Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.PowerSort");
        Add("DropMergeSort", "Merge Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.DropMergeSort");
        Add("ShiftSort", "Merge Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.ShiftSort");
        
        // Heap Sorts - O(n log n)
        Add("HeapSort", "Heap Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.HeapSort");
        Add("BottomupHeapSort", "Heap Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.BottomupHeapSort");
        Add("WeakHeapSort", "Heap Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.WeakHeapSort");
        Add("SmoothSort", "Heap Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.SmoothSort");
        Add("TernaryHeapSort", "Heap Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.TernaryHeapSort");
        
        // Partition Sorts - O(n log n)
        Add("QuickSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.QuickSort");
        Add("QuickSortMedian3", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.QuickSortMedian3");
        Add("QuickSortMedian9", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.QuickSortMedian9");
        Add("QuickSortDualPivot", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.QuickSortDualPivot");
        Add("BlockQuickSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.BlockQuickSort");
        Add("StableQuickSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.StableQuickSort");
        Add("IntroSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.IntroSort");
        Add("PDQSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.PDQSort");
        Add("StdSort", "Partition Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.StdSort");
        
        // Distribution Sorts - O(n) ~ O(nk)
        Add("CountingSort", "Distribution Sorts", "O(n+k)", 2048, "SortAlgorithm.Algorithms.CountingSort");
        Add("BucketSort", "Distribution Sorts", "O(n+k)", 2048, "SortAlgorithm.Algorithms.BucketSort");
        Add("RadixLSD4Sort", "Distribution Sorts", "O(nk)", 2048, "SortAlgorithm.Algorithms.RadixLSD4Sort");
        Add("RadixLSD10Sort", "Distribution Sorts", "O(nk)", 2048, "SortAlgorithm.Algorithms.RadixLSD10Sort");
        
        // Network Sorts - O(log²n)
        Add("BitonicSort", "Network Sorts", "O(log²n)", 2048, "SortAlgorithm.Algorithms.BitonicSort");
        Add("BitonicSortFill", "Network Sorts", "O(log²n)", 2048, "SortAlgorithm.Algorithms.BitonicSortFill");
        Add("BitonicSortParallel", "Network Sorts", "O(log²n)", 2048, "SortAlgorithm.Algorithms.BitonicSortParallel");
        
        // Tree Sorts - O(n log n)
        Add("BinaryTreeSort", "Tree Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.BinaryTreeSort");
        Add("BalancedBinaryTreeSort", "Tree Sorts", "O(n log n)", 2048, "SortAlgorithm.Algorithms.BalancedBinaryTreeSort");
        
        // Joke Sorts - O(n!) ~ O(∞)
        Add("BogoSort", "Joke Sorts", "O(n!)", 10, "SortAlgorithm.Algorithms.BogoSort", "⚠️ Extremely slow!");
        Add("SlowSort", "Joke Sorts", "O(n^(log n))", 10, "SortAlgorithm.Algorithms.SlowSort", "⚠️ Extremely slow!");
        Add("StoogeSort", "Joke Sorts", "O(n^2.7)", 10, "SortAlgorithm.Algorithms.StoogeSort", "⚠️ Extremely slow!");
    }
    
    private void Add(string name, string category, string complexity, int maxElements, string typeName, string description = "")
    {
        _algorithms.Add(new AlgorithmMetadata
        {
            Name = name,
            Category = category,
            TimeComplexity = complexity,
            MaxElements = maxElements,
            TypeName = typeName,
            Description = description
        });
    }
    
    /// <summary>
    /// アルゴリズムのインスタンスを作成
    /// </summary>
    public object? CreateInstance(string algorithmName)
    {
        var metadata = _algorithms.FirstOrDefault(a => a.Name == algorithmName);
        if (metadata == null) return null;
        
        var assemblyQualifiedName = $"{metadata.TypeName}, SortAlgorithm";
        var type = Type.GetType(assemblyQualifiedName);
        if (type == null) return null;
        
        return Activator.CreateInstance(type);
    }
}
