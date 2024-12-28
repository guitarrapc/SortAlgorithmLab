namespace SortLab.Core;

public interface IOutput
{
    string Header { get; }
    string Item { get; }
}

public class MarkdownOutput : IOutput
{
    private readonly string[] _itemBuffer = new string[8];
    private readonly string[] _headers = [nameof(InputType), nameof(ArraySize), nameof(SortType), nameof(Algorithm), nameof(IsSorted), nameof(IndexAccessCount), nameof(CompareCount), nameof(SwapCount)];

    public string Header => $"""
    {_headers.ToMarkdownString()}
    {Enumerable.Range(0, _headers.Length).Select(x => "---").ToMarkdownString()}
    """.ReplaceLineEndings();
    public string Item => GetItems().ToMarkdownString();

    public InputType InputType { get; set; }
    public int ArraySize { get; set; }
    public SortType SortType { get; set; }
    public string Algorithm { get; set; }
    public bool IsSorted { get; set; }
    public ulong IndexAccessCount { get; set; }
    public ulong CompareCount { get; set; }
    public ulong SwapCount { get; set; }

    public MarkdownOutput(IStatistics statics, InputType inputType)
    {
        InputType = inputType;
        ArraySize = statics.ArraySize;
        SortType = statics.SortType;
        Algorithm = statics.Algorithm;
        IsSorted = statics.IsSorted;
        IndexAccessCount = statics.IndexAccessCount;
        CompareCount = statics.CompareCount;
        SwapCount = statics.SwapCount;
    }

    private string[] GetItems()
    {
        _itemBuffer[0] = InputType.ToString();
        _itemBuffer[1] = ArraySize.ToString();
        _itemBuffer[2] = SortType.ToString();
        _itemBuffer[3] = Algorithm;
        _itemBuffer[4] = IsSorted.ToString();
        _itemBuffer[5] = IndexAccessCount.ToString();
        _itemBuffer[6] = CompareCount.ToString();
        _itemBuffer[7] = SwapCount.ToString();
        return _itemBuffer;
    }

    public void WriteHeader()
    {
        Console.WriteLine(Header);
    }

    public void WriteItem()
    {
        Console.WriteLine(Item);
    }
}

public class ConsoleOutput : IOutput
{
    private readonly static string separator = ", ";
    private readonly string[] _itemBuffer = new string[9];
    private string[] _headers = [nameof(InputType), nameof(ArraySize), nameof(SortType), nameof(Algorithm), nameof(IsSorted), nameof(IndexAccessCount), nameof(CompareCount), nameof(SwapCount), nameof(SortResult)];

    public string Header => $"| {_headers.ToJoinedString(separator)} |";
    public string Item => GetItems().ToJoinedString(separator);

    public InputType InputType { get; set; }
    public int ArraySize { get; set; }
    public string Algorithm { get; set; }
    public SortType SortType { get; set; }
    public bool IsSorted { get; set; }
    public ulong IndexAccessCount { get; set; }
    public ulong CompareCount { get; set; }
    public ulong SwapCount { get; set; }
    public string SortResult { get; set; }

    public ConsoleOutput(IStatistics statics, string sortResult, InputType inputType)
    {
        InputType = inputType;
        ArraySize = statics.ArraySize;
        SortType = statics.SortType;
        Algorithm = statics.Algorithm;
        IsSorted = statics.IsSorted;
        IndexAccessCount = statics.IndexAccessCount;
        CompareCount = statics.CompareCount;
        SwapCount = statics.SwapCount;
        SortResult = sortResult;
    }

    private string[] GetItems()
    {
        _itemBuffer[0] = InputType.ToString();
        _itemBuffer[1] = ArraySize.ToString();
        _itemBuffer[2] = SortType.ToString();
        _itemBuffer[3] = Algorithm;
        _itemBuffer[4] = IsSorted.ToString();
        _itemBuffer[5] = IndexAccessCount.ToString();
        _itemBuffer[6] = CompareCount.ToString();
        _itemBuffer[7] = SwapCount.ToString();
        _itemBuffer[8] = SortResult;
        return _itemBuffer;
    }

    public override string ToString()
    {
        var items = GetItems();
        var outputs = new string[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            outputs[i] = $"{_headers[i]} : {items[i]}";
        }
        return outputs.ToJoinedString(", ");
    }
}
