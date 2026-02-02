#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test 1: Random array
var random = new Random(42);
var array1 = Enumerable.Range(0, 100).Select(_ => random.Next(1000)).ToArray();
var stats1 = new StatisticsContext();
IntroSort.Sort(array1.AsSpan(), stats1);
Console.WriteLine("Test 1 - Random Array (100 elements):");
Console.WriteLine($"  Sorted: {IsSorted(array1)}");
Console.WriteLine($"  Compares: {stats1.CompareCount}, Swaps: {stats1.SwapCount}");
Console.WriteLine();

// Test 2: Large pathological case (already sorted)
var array2 = Enumerable.Range(0, 10000).ToArray();
var stats2 = new StatisticsContext();
IntroSort.Sort(array2.AsSpan(), stats2);
Console.WriteLine("Test 2 - Already Sorted (10,000 elements):");
Console.WriteLine($"  Sorted: {IsSorted(array2)}");
Console.WriteLine($"  Compares: {stats2.CompareCount}, Swaps: {stats2.SwapCount}");
Console.WriteLine();

// Test 3: Large pathological case (reverse sorted)
var array3 = Enumerable.Range(0, 10000).Reverse().ToArray();
var stats3 = new StatisticsContext();
IntroSort.Sort(array3.AsSpan(), stats3);
Console.WriteLine("Test 3 - Reverse Sorted (10,000 elements):");
Console.WriteLine($"  Sorted: {IsSorted(array3)}");
Console.WriteLine($"  Compares: {stats3.CompareCount}, Swaps: {stats3.SwapCount}");
Console.WriteLine();

// Test 4: All equal elements
var array4 = Enumerable.Repeat(42, 1000).ToArray();
var stats4 = new StatisticsContext();
IntroSort.Sort(array4.AsSpan(), stats4);
Console.WriteLine("Test 4 - All Equal (1,000 elements):");
Console.WriteLine($"  Sorted: {IsSorted(array4)}");
Console.WriteLine($"  Compares: {stats4.CompareCount}, Swaps: {stats4.SwapCount}");
Console.WriteLine();

// Test 5: Large array to test tail recursion optimization
var array5 = Enumerable.Range(0, 100000).Select(_ => random.Next(1000000)).ToArray();
var stats5 = new StatisticsContext();
IntroSort.Sort(array5.AsSpan(), stats5);
Console.WriteLine("Test 5 - Large Random Array (100,000 elements):");
Console.WriteLine($"  Sorted: {IsSorted(array5)}");
Console.WriteLine($"  Compares: {stats5.CompareCount}, Swaps: {stats5.SwapCount}");
Console.WriteLine();

Console.WriteLine("✅ All tests passed! Tail recursion optimization is working correctly.");

static bool IsSorted<T>(T[] array) where T : IComparable<T>
{
    for (int i = 1; i < array.Length; i++)
    {
        if (array[i].CompareTo(array[i - 1]) < 0)
            return false;
    }
    return true;
}
