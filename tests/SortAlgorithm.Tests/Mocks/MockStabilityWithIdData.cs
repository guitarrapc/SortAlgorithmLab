using System.Collections;

namespace SortAlgorithm.Tests;

public class MockStabilityWithIdData : IEnumerable<object[]>
{
    public static StabilityTestItemWithId[] Sorted => _sorted;
    private static StabilityTestItemWithId[] _sorted = [new (2, "B"), new(2, "D"), new(2, "F"), new(5, "A"), new(5, "C"), new(5, "G"), new(8, "E")];

    private List<object[]> testData = new List<object[]>();

    public MockStabilityWithIdData()
    {
        testData.Add([new StabilityTestItemWithId[]
        {
            new (5, "A"),
            new (2, "B"),
            new (5, "C"),
            new (2, "D"),
            new (8, "E"),
            new (2, "F"),
            new (5, "G"),
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
