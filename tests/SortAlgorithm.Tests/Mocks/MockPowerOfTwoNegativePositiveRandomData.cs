namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoNegativePositiveRandomData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-8, 16).Sample(16).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-32, 64).Sample(64).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-128, 256).Sample(256).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-256, 512).Sample(512).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-512, 1024).Sample(1024).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Range(-1024, 2048).Sample(2048).ToArray()
        };
    }
}
