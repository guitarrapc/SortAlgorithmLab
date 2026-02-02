namespace SortAlgorithm.Tests;

public static class MockNegativeRandomData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = Enumerable.Range(-100, 100).Sample(100).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = Enumerable.Range(-1000, 1000).Sample(1000).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = Enumerable.Range(-10000, 10000).Sample(10000).ToArray()
        };
    }
}
