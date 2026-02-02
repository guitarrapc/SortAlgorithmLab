namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 16).Sample(16).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 64).Sample(64).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 256).Sample(256).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 512).Sample(512).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 1024).Sample(1024).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(0, 2048).Sample(2048).ToArray()
        };
    }
}
