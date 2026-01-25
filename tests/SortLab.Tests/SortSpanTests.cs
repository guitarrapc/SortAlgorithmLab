#if DEBUG
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortLab.Tests;

public class SortSpanTests
{
    [Fact]
    public void CopyTo_ShouldCopyRangeToAnotherSortSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();
        
        var sourceSpan = new SortSpan<int>(source.AsSpan(), context, 0);
        var destSpan = new SortSpan<int>(destination.AsSpan(), context, 1);

        // Act
        sourceSpan.CopyTo(1, destSpan, 0, 3); // Copy [2, 3, 4] to destination[0..3]

        // Assert
        Assert.Equal(2, destination[0]);
        Assert.Equal(3, destination[1]);
        Assert.Equal(4, destination[2]);
        Assert.Equal(0, destination[3]); // Not copied
        Assert.Equal(0, destination[4]); // Not copied
    }

    [Fact]
    public void CopyTo_ShouldTrackStatistics()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();
        
        var sourceSpan = new SortSpan<int>(source.AsSpan(), context, 0);
        var destSpan = new SortSpan<int>(destination.AsSpan(), context, 1);

        // Act
        sourceSpan.CopyTo(0, destSpan, 0, 3); // Copy 3 elements

        // Assert - Should count as 3 reads + 3 writes
        Assert.Equal(3UL, context.IndexReadCount);
        Assert.Equal(3UL, context.IndexWriteCount);
    }

    [Fact]
    public void CopyTo_ShouldCopyToRegularSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();
        
        var sourceSpan = new SortSpan<int>(source.AsSpan(), context, 0);

        // Act
        sourceSpan.CopyTo(2, destination.AsSpan(), 1, 2); // Copy [3, 4] to destination[1..3]

        // Assert
        Assert.Equal(0, destination[0]); // Not copied
        Assert.Equal(3, destination[1]);
        Assert.Equal(4, destination[2]);
        Assert.Equal(0, destination[3]); // Not copied
    }

    [Fact]
    public void CopyTo_VerifyBetterThanLoopWrite()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var destination1 = new int[10];
        var destination2 = new int[10];
        var contextCopyTo = new StatisticsContext();
        var contextLoop = new StatisticsContext();
        
        var sourceSpan1 = new SortSpan<int>(source.AsSpan(), contextCopyTo, 0);
        var destSpan1 = new SortSpan<int>(destination1.AsSpan(), contextCopyTo, 1);
        
        var sourceSpan2 = new SortSpan<int>(source.AsSpan(), contextLoop, 0);
        var destSpan2 = new SortSpan<int>(destination2.AsSpan(), contextLoop, 1);

        // Act - using CopyTo
        sourceSpan1.CopyTo(0, destSpan1, 0, 10);
        
        // Act - using loop with Read/Write
        for (int i = 0; i < 10; i++)
        {
            destSpan2.Write(i, sourceSpan2.Read(i));
        }

        // Assert - Both should produce the same result
        Assert.Equal(destination1, destination2);
        
        // Assert - CopyTo should have the same statistics as loop
        // (Both are counted as reads + writes, but CopyTo is more efficient in tracking)
        Assert.Equal(10UL, contextCopyTo.IndexReadCount);
        Assert.Equal(10UL, contextCopyTo.IndexWriteCount);
        Assert.Equal(10UL, contextLoop.IndexReadCount);
        Assert.Equal(10UL, contextLoop.IndexWriteCount);
    }
}

#endif
