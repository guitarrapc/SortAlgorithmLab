namespace SortAlgorithm.Tests.Attributes;

/// <summary>
/// Theory attribute that skips the test when running in CI environment.
/// </summary>
public sealed class CISkippableTheoryAttribute : TheoryAttribute
{
    public CISkippableTheoryAttribute()
    {
        if (IsCI())
        {
            Skip = "Skipped in CI environment";
        }
    }

    static bool IsCI()
    {
        var ci = Environment.GetEnvironmentVariable("CI");
        return !string.IsNullOrEmpty(ci) && ci.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
