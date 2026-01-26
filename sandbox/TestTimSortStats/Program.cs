using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var testCases = new[] { 10, 20, 50, 100 };

Console.WriteLine("=== Sorted Data ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    TimSort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"n={n,3}: Compares={stats.CompareCount,4}, Writes={stats.IndexWriteCount,4}, Swaps={stats.SwapCount,4}");
}

Console.WriteLine("\n=== Reversed Data ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    TimSort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"n={n,3}: Compares={stats.CompareCount,4}, Writes={stats.IndexWriteCount,4}, Swaps={stats.SwapCount,4}");
}

Console.WriteLine("\n=== Random Data (10 trials) ===");
foreach (var n in testCases)
{
    var compareSum = 0UL;
    var writeSum = 0UL;
    var swapSum = 0UL;
    var trials = 10;
    
    for (int trial = 0; trial < trials; trial++)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        TimSort.Sort(random.AsSpan(), stats);
        compareSum += stats.CompareCount;
        writeSum += stats.IndexWriteCount;
        swapSum += stats.SwapCount;
    }
    
    Console.WriteLine($"n={n,3}: Avg Compares={compareSum/(ulong)trials,4}, Avg Writes={writeSum/(ulong)trials,4}, Avg Swaps={swapSum/(ulong)trials,4}");
}
