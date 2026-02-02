namespace SortAlgorithm.Tests;

public static class MockNearlySortedData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = Enumerable.Range(0, 90).Concat(Enumerable.Range(0, 100).Sample(10)).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = Enumerable.Range(0, 990).Concat(Enumerable.Range(0, 1000).Sample(10)).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = Enumerable.Range(0, 9990).Concat(Enumerable.Range(0, 10000).Sample(10)).ToArray()
        };
    }
}
