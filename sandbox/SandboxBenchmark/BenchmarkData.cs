namespace SandboxBenchmark;

public static class BenchmarkData
{
    public static int[] GenIntArray(int number)
    {
        return Enumerable.Range(0, number).Sample(number).ToArray();
    }
}
