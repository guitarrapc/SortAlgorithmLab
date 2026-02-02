namespace SortAlgorithm.Tests;

public static class MockMountainData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = Enumerable.Range(0, 50).Concat(Enumerable.Range(0, 50).Reverse()).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = Enumerable.Range(0, 500).Concat(Enumerable.Range(0, 500).Reverse()).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = Enumerable.Range(0, 5000).Concat(Enumerable.Range(0, 5000).Reverse()).ToArray()
        };
    }
}
