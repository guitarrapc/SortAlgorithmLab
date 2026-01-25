using SortLab.Core;
using SortLab.Core.Algorithms;
using SortLab.Core.Contexts;
using SortLab.Tests;

Console.WriteLine("QuickSort Anti-Pattern Performance Test");
Console.WriteLine("========================================\n");

int size = 1000;

// Generate patterns
var patterns = new Dictionary<string, int[]>
{
    ["Random"] = Enumerable.Range(0, size).Sample(size).ToArray(),
    ["Sorted"] = Enumerable.Range(0, size).ToArray(),
    ["Reversed"] = Enumerable.Range(0, size).Reverse().ToArray(),
    ["Sawtooth"] = GenerateSawtooth(size),
    ["PipeOrgan"] = GeneratePipeOrgan(size),
    ["Interleaved"] = GenerateInterleaved(size),
};

Console.WriteLine($"Pattern          | Comparisons | Swaps    | IndexReads | IndexWrites");
Console.WriteLine("-----------------|-------------|----------|------------|------------");

ulong maxComparisons = 0;
string worstPattern = "";

foreach (var (name, pattern) in patterns)
{
    var stats = new StatisticsContext();
    var array = pattern.ToArray();
    
    QuickSort.Sort(array.AsSpan(), stats);
    
    Console.WriteLine($"{name,-16} | {stats.CompareCount,11:N0} | {stats.SwapCount,8:N0} | {stats.IndexReadCount,10:N0} | {stats.IndexWriteCount,11:N0}");
    
    if (stats.CompareCount > maxComparisons)
    {
        maxComparisons = stats.CompareCount;
        worstPattern = name;
    }
}

Console.WriteLine($"\nWorst pattern: {worstPattern} with {maxComparisons:N0} comparisons");
Console.WriteLine($"Theoretical worst case (n²/2): {size * size / 2:N0}");
Console.WriteLine($"Theoretical average case (2n ln n): {2.0 * size * Math.Log(size):N0}");

static int[] GenerateSawtooth(int n)
{
    var array = new int[n];
    int low = 0, high = n - 1;
    for (int i = 0; i < n; i++)
    {
        array[i] = (i % 2 == 0) ? low++ : high--;
    }
    return array;
}

static int[] GeneratePipeOrgan(int n)
{
    var array = new int[n];
    int half = n / 2;
    for (int i = 0; i < half; i++)
    {
        array[i] = i;
    }
    for (int i = half; i < n; i++)
    {
        array[i] = n - 1 - i;
    }
    return array;
}

static int[] GenerateInterleaved(int n)
{
    var array = new int[n];
    int half = n / 2;
    for (int i = 0; i < half; i++)
    {
        if (i * 2 < n)
            array[i * 2] = i;
        if (i * 2 + 1 < n)
            array[i * 2 + 1] = half + i;
    }
    if (n % 2 != 0)
    {
        array[n - 1] = n - 1;
    }
    return array;
}
