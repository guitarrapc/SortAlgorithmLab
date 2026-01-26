using System.Collections;

namespace SortAlgorithm.Tests;

public class MockStabilityAllEqualsData : IEnumerable<object[]>
{
    public static int[] Sorted => _sorted;
    private static int[] _sorted = [0, 1, 2, 3, 4];

    private List<object[]> testData = new List<object[]>();

    public MockStabilityAllEqualsData()
    {
        testData.Add([new StabilityTestItem[]
        {
            new (1, 0),
            new (1, 1),
            new (1, 2),
            new (1, 3),
            new (1, 4),
        }]);
    }

    public IEnumerator<object[]> GetEnumerator() => testData.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
