using SortAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SortAlgorithm
{
    public interface IOutput
    {
        string Header { get; }
        string Item { get; }
        void WriteHeader();
        void WriteItem();
    }

    public class MarkdownOutput : IOutput
    {
        private readonly static string separator = " | ";
        private readonly string[] itemBuffer = new string[8];

        private string[] Headers = new[] { nameof(InputType), nameof(ArraySize), nameof(IsSorted), nameof(SortType), nameof(Algorithm), nameof(IndexAccessCount), nameof(CompareCount), nameof(SwapCount) };
        public string Header => Headers.ToJoinedString(separator) + Environment.NewLine + $"{Enumerable.Range(0, Headers.Length).Select(x => "----").ToJoinedString(separator)}";
        public string Item => GetItems().ToJoinedString(separator);

        public InputType InputType { get; set; }
        public int ArraySize { get; set; }
        public bool IsSorted { get; set; }
        public SortType SortType { get; set; }
        public string Algorithm { get; set; }
        public ulong IndexAccessCount { get; set; }
        public ulong CompareCount { get; set; }
        public ulong SwapCount { get; set; }

        public MarkdownOutput(IStatistics statics, InputType inputType)
        {
            InputType = inputType;
            ArraySize = statics.ArraySize;
            IsSorted = statics.IsSorted;
            SortType = statics.SortType;
            Algorithm = statics.Algorithm;
            IndexAccessCount = statics.IndexAccessCount;
            CompareCount = statics.CompareCount;
            SwapCount = statics.SwapCount;
        }

        private string[] GetItems()
        {
            itemBuffer[0] = InputType.ToString();
            itemBuffer[1] = ArraySize.ToString();
            itemBuffer[2] = IsSorted.ToString();
            itemBuffer[3] = SortType.ToString();
            itemBuffer[4] = Algorithm;
            itemBuffer[5] = IndexAccessCount.ToString();
            itemBuffer[6] = CompareCount.ToString();
            itemBuffer[7] = SwapCount.ToString();
            return itemBuffer;
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
        private readonly string[] itemBuffer = new string[9];

        private string[] Headers = new[] { nameof(InputType), nameof(ArraySize), nameof(IsSorted), nameof(SortType), nameof(Algorithm), nameof(IndexAccessCount), nameof(CompareCount), nameof(SwapCount), nameof(SortResult) };
        public string Header => Headers.ToJoinedString(separator);
        public string Item => GetItems().ToJoinedString(separator);

        public InputType InputType { get; set; }
        public int ArraySize { get; set; }
        public bool IsSorted { get; set; }
        public SortType SortType { get; set; }
        public string Algorithm { get; set; }
        public ulong IndexAccessCount { get; set; }
        public ulong CompareCount { get; set; }
        public ulong SwapCount { get; set; }
        public string SortResult { get; set; }

        public ConsoleOutput(IStatistics statics, string sortResult, InputType inputType)
        {
            InputType = inputType;
            ArraySize = statics.ArraySize;
            IsSorted = statics.IsSorted;
            SortType = statics.SortType;
            Algorithm = statics.Algorithm;
            IndexAccessCount = statics.IndexAccessCount;
            CompareCount = statics.CompareCount;
            SwapCount = statics.SwapCount;
            SortResult = SortResult;
        }

        private string[] GetItems()
        {
            itemBuffer[0] = InputType.ToString();
            itemBuffer[1] = ArraySize.ToString();
            itemBuffer[2] = IsSorted.ToString();
            itemBuffer[3] = SortType.ToString();
            itemBuffer[4] = Algorithm;
            itemBuffer[5] = IndexAccessCount.ToString();
            itemBuffer[6] = CompareCount.ToString();
            itemBuffer[7] = SwapCount.ToString();
            itemBuffer[8] = SortResult;
            return itemBuffer;
        }

        public void WriteHeader()
        {
            Console.WriteLine(Header);
        }

        public void WriteItem()
        {
            Console.WriteLine(Item);
        }

        public override string ToString()
        {
            var items = GetItems();
            var outputs = new string[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                outputs[i] = $"{Headers[i]} : {items[i]}";
            }
            return outputs.ToJoinedString(", ");
        }
    }
}
