﻿using SortLab.Core;
using SortLab.Core.Logics;

namespace SandboxBenchmark;

[ShortRunJob]
[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class CycleSortBenchmark
{
    [Params(100, 1000, 10000)]
    public int Number { get; set; }

    private int[] _array = default!;
    private CycleSort<int> _cycleSort = default!;

    [IterationSetup]
    public void Setup()
    {
        _array = Enumerable.Range(0, Number).Sample(Number).ToArray();
        _cycleSort = new();
    }

    [Benchmark]
    public void CycleSort()
    {
        _cycleSort.Sort(_array);
    }
}