#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net10.0
#:project ../../sandbox/SortAlgorithm.VisualizationWeb

using SortAlgorithm.VisualizationWeb.Models;
using SortAlgorithm.VisualizationWeb.Services;

var generator = new ArrayPatternGenerator();

Console.WriteLine("=== Mountain Shape (Size: 10) ===");
var mountain = generator.Generate(10, ArrayPattern.MountainShape);
Console.WriteLine($"Expected: Values increase to center, then decrease");
Console.WriteLine($"Result:   [{string.Join(", ", mountain)}]");
Console.WriteLine();

Console.WriteLine("=== Valley Shape (Size: 10) ===");
var valley = generator.Generate(10, ArrayPattern.ValleyShape);
Console.WriteLine($"Expected: Values decrease to center, then increase");
Console.WriteLine($"Result:   [{string.Join(", ", valley)}]");
Console.WriteLine();

Console.WriteLine("=== Mountain Shape (Size: 8) ===");
var mountain8 = generator.Generate(8, ArrayPattern.MountainShape);
Console.WriteLine($"Result:   [{string.Join(", ", mountain8)}]");
Console.WriteLine();

Console.WriteLine("=== Valley Shape (Size: 8) ===");
var valley8 = generator.Generate(8, ArrayPattern.ValleyShape);
Console.WriteLine($"Result:   [{string.Join(", ", valley8)}]");
