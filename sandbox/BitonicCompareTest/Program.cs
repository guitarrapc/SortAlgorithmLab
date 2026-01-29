using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var sizes = new[] { 64, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536 };
var iterations = 10;

Console.WriteLine("BitonicSort vs BitonicSortParallel Performance Comparison");
Console.WriteLine("=========================================================\n");
Console.WriteLine($"Iterations per size: {iterations}");
Console.WriteLine($"Processor count: {Environment.ProcessorCount}\n");

foreach (var size in sizes)
{
    var sequentialTimes = new List<long>();
    var parallelTimes = new List<long>();

    for (int iter = 0; iter < iterations; iter++)
    {
        // Generate random data
        var random = new Random(42 + iter);
        var data = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();

        // Test BitonicSort (sequential)
        var seqData = data.ToArray();
        var seqSw = Stopwatch.StartNew();
        BitonicSort.Sort(seqData.AsSpan(), NullContext.Default);
        seqSw.Stop();
        sequentialTimes.Add(seqSw.ElapsedTicks);

        // Test BitonicSortParallel
        var parData = data.ToArray();
        var parSw = Stopwatch.StartNew();
        BitonicSortParallel.Sort(parData, NullContext.Default);
        parSw.Stop();
        parallelTimes.Add(parSw.ElapsedTicks);

        // Verify both sorted correctly
        if (!seqData.SequenceEqual(parData))
        {
            Console.WriteLine($"ERROR: Results differ for size {size}!");
            return;
        }
    }

    var avgSeq = sequentialTimes.Average();
    var avgPar = parallelTimes.Average();
    var speedup = avgSeq / avgPar;
    var winner = speedup > 1.0 ? "Parallel" : "Sequential";

    Console.WriteLine($"Size: {size,5}");
    Console.WriteLine($"  Sequential avg: {avgSeq,10:F2} ticks");
    Console.WriteLine($"  Parallel avg:   {avgPar,10:F2} ticks");
    Console.WriteLine($"  Speedup:        {speedup,10:F2}x ({winner} faster)");
    Console.WriteLine();
}

Console.WriteLine("Test completed successfully!");
