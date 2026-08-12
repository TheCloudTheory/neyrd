namespace neyrd.core.Benchmark;

public static class TestSuite
{
    public static void BeginTest(long timestamp)
    {
        Beginning = timestamp;
    }

    private static long Beginning { get; set; }
    private static IList<long> Results { get; set; } = new List<long>();

    public static void RecordTest(long timestamp)
    {
        Results.Add(timestamp);
    }

    public static void Complete(long timestamp)
    {
        End = timestamp;
    }

    private static long End { get; set; }
    
    public static bool HasCompleted => End > 0;

    public static string DisplayResults()
    {
        return $"Beginning: {Beginning}, End: {End}, Diff: {End - Beginning} Results: {string.Join(", ", Results)}";
    }
}