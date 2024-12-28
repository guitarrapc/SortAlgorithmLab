namespace SortLab.Core;

public class SortStatistics : IStatistics
{
    public SortType SortType { get; set; }
    public string Algorithm { get; set; }
    public int ArraySize { get; set; }
    public ulong IndexAccessCount { get; set; }
    public ulong CompareCount { get; set; }
    public ulong SwapCount { get; set; }
    public bool IsSorted { get; set; }

    public void Reset()
    {
        SortType = SortType.None;
        Algorithm = "";
        IndexAccessCount = 0;
        CompareCount = 0;
        SwapCount = 0;
        IsSorted = false;
    }

    public void Reset(int arraySize, SortType sortType, string algorithm)
    {
        SortType = sortType;
        Algorithm = algorithm;
        ArraySize = arraySize;
        IndexAccessCount = 0;
        CompareCount = 0;
        SwapCount = 0;
        IsSorted = false;
    }

    public void AddIndexAccess()
    {
        IndexAccessCount++;
    }
    public void AddIndexAccess(ulong count)
    {
        IndexAccessCount += count;
    }
    public void AddCompareCount()
    {
        CompareCount++;
    }
    public void AddCompareCount(ulong count)
    {
        CompareCount += count;
    }
    public void AddSwapCount()
    {
        SwapCount++;
    }
    public void AddSwapCount(ulong count)
    {
        SwapCount += count;
    }
}

public interface IStatistics
{
    SortType SortType { get; set; }
    string Algorithm { get; set; }
    int ArraySize { get; set; }
    ulong IndexAccessCount { get; set; }
    ulong CompareCount { get; set; }
    ulong SwapCount { get; set; }
    bool IsSorted { get; set; }

    void Reset();

    void Reset(int arraySize, SortType sortType, string algorithm);

    void AddIndexAccess();
    void AddIndexAccess(ulong count);
    void AddCompareCount();
    void AddCompareCount(ulong count);
    void AddSwapCount();
    void AddSwapCount(ulong count);
}