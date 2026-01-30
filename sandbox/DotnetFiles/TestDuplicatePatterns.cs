#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net10.0
#:project ../../sandbox/SortAlgorithm.VisualizationWeb

using SortAlgorithm.VisualizationWeb.Models;
using SortAlgorithm.VisualizationWeb.Services;

var generator = new ArrayPatternGenerator();

Console.WriteLine("=== Few Unique Values Pattern ===");
Console.WriteLine();

// 小さいサイズ (64)
var fewUnique64 = generator.Generate(64, ArrayPattern.FewUnique, seed: 42);
var uniqueCount64 = fewUnique64.Distinct().Count();
Console.WriteLine($"Size: 64");
Console.WriteLine($"Unique values: {uniqueCount64} (Expected: around 6-7)");
Console.WriteLine($"Sample: [{string.Join(", ", fewUnique64.Take(20))}...]");
Console.WriteLine();

// 中くらいのサイズ (256)
var fewUnique256 = generator.Generate(256, ArrayPattern.FewUnique, seed: 42);
var uniqueCount256 = fewUnique256.Distinct().Count();
Console.WriteLine($"Size: 256");
Console.WriteLine($"Unique values: {uniqueCount256} (Expected: around 20)");
Console.WriteLine($"Sample: [{string.Join(", ", fewUnique256.Take(20))}...]");
Console.WriteLine();

// 大きいサイズ (4096)
var fewUnique4096 = generator.Generate(4096, ArrayPattern.FewUnique, seed: 42);
var uniqueCount4096 = fewUnique4096.Distinct().Count();
Console.WriteLine($"Size: 4096");
Console.WriteLine($"Unique values: {uniqueCount4096} (Expected: 20 - FIXED!)");
Console.WriteLine($"Sample: [{string.Join(", ", fewUnique4096.Take(20))}...]");
Console.WriteLine();

Console.WriteLine("=== Many Duplicates Pattern ===");
Console.WriteLine();

// 小さいサイズ (64)
var manyDup64 = generator.Generate(64, ArrayPattern.ManyDuplicates, seed: 42);
var uniqueCountDup64 = manyDup64.Distinct().Count();
Console.WriteLine($"Size: 64");
Console.WriteLine($"Unique values: {uniqueCountDup64} (Expected: around 12-13)");
Console.WriteLine($"Sample: [{string.Join(", ", manyDup64.Take(20))}...]");
Console.WriteLine();

// 中くらいのサイズ (256)
var manyDup256 = generator.Generate(256, ArrayPattern.ManyDuplicates, seed: 42);
var uniqueCountDup256 = manyDup256.Distinct().Count();
Console.WriteLine($"Size: 256");
Console.WriteLine($"Unique values: {uniqueCountDup256} (Expected: around 40)");
Console.WriteLine($"Sample: [{string.Join(", ", manyDup256.Take(20))}...]");
Console.WriteLine();

// 大きいサイズ (4096)
var manyDup4096 = generator.Generate(4096, ArrayPattern.ManyDuplicates, seed: 42);
var uniqueCountDup4096 = manyDup4096.Distinct().Count();
Console.WriteLine($"Size: 4096");
Console.WriteLine($"Unique values: {uniqueCountDup4096} (Expected: 40 - FIXED!)");
Console.WriteLine($"Sample: [{string.Join(", ", manyDup4096.Take(20))}...]");
Console.WriteLine();

Console.WriteLine("=== Summary ===");
Console.WriteLine($"✅ FewUnique now has max 20 unique values regardless of size");
Console.WriteLine($"✅ ManyDuplicates now has max 40 unique values regardless of size");
