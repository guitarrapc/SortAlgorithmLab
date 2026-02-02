namespace SortAlgorithm.Tests;

public static class MockSortedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 100).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 1000).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 10000).ToArray()
        };
    }
}
