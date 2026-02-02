namespace SortAlgorithm.Tests;

public static class MockNegativePositiveRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = Enumerable.Range(-50, 100).Sample(100).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = Enumerable.Range(-500, 1000).Sample(1000).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = Enumerable.Range(-5000, 10000).Sample(10000).ToArray()
        };
    }
}
