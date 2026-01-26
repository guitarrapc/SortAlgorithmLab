namespace SandboxBenchmark;

/// <summary>
/// Data patterns for benchmark testing.
/// </summary>
public enum DataPattern
{
    Random,
    Sorted,
    Reversed,
    NearlySorted
}

public static class BenchmarkData
{
    public static int[] GenerateIntArray(int size, DataPattern pattern)
    {
        return pattern switch
        {
            DataPattern.Random => Enumerable.Range(0, size).Sample(size).ToArray(),
            DataPattern.Sorted => Enumerable.Range(0, size).ToArray(),
            DataPattern.Reversed => Enumerable.Range(0, size).Reverse().ToArray(),
            DataPattern.NearlySorted => GenerateNearlySortedInt(size),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
    }

    public static string[] GenerateStringArray(int size, DataPattern pattern)
    {
        var baseArray = pattern switch
        {
            DataPattern.Random => Enumerable.Range(0, size).Sample(size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.Sorted => Enumerable.Range(0, size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.Reversed => Enumerable.Range(0, size).Reverse().Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.NearlySorted => GenerateNearlySortedString(size),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
        return baseArray;
    }

    public static int[] GenerateNearlySortedInt(int size)
    {
        var array = Enumerable.Range(0, size).ToArray();
        var swaps = size / 20; // Swap 5% of elements
        var random = new Random(42);
        for (int i = 0; i < swaps; i++)
        {
            var idx1 = random.Next(size);
            var idx2 = random.Next(size);
            (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
        }
        return array;
    }

    public static string[] GenerateNearlySortedString(int size)
    {
        var array = Enumerable.Range(0, size).Select(i => $"String_{i:D6}").ToArray();
        var swaps = size / 20; // Swap 5% of elements
        var random = new Random(42);
        for (int i = 0; i < swaps; i++)
        {
            var idx1 = random.Next(size);
            var idx2 = random.Next(size);
            (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
        }
        return array;
    }

}
