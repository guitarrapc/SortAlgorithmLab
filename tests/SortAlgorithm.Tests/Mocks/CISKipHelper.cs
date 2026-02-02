namespace SortAlgorithm.Tests.Mocks;

internal static class CISKipHelper
{
    public static void IsCI()
    {
        Skip.When(Environment.GetEnvironmentVariable("CI")?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false, "Skip on CI");
    }
}
