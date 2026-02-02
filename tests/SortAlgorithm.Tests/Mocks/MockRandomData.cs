namespace SortAlgorithm.Tests;

public static class MockRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 100).Sample(100).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 1000).Sample(1000).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 10000).Sample(10000).ToArray()
        };
    }
}
