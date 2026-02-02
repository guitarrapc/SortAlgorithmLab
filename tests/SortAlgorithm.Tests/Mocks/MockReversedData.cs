namespace SortAlgorithm.Tests;

public static class MockReversedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 100).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 1000).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 10000).Reverse().ToArray()
        };
    }
}
