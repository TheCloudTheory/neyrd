using System.Globalization;

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

    public static void Complete()
    {
        End = DateTimeOffset.Now.Ticks;
    }

    private static long End { get; set; }
    
    public static bool HasCompleted => End > 0;

    public static string DisplayResults()
    {
        const long ticksPerMs = TimeSpan.TicksPerMillisecond;
        var durationMs = (End - Beginning) / (double)ticksPerMs;
        var offsets = Results.Select(r => $"{(r - Beginning) / (double)ticksPerMs:F3}ms");
        
        return $"Duration: {durationMs.ToString("F3", CultureInfo.InvariantCulture)}ms | Hits: [{string.Join(", ", offsets)}]";
    }
}