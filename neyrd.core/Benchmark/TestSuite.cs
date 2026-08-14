using System.Globalization;

namespace neyrd.core.Benchmark;

public static class TestSuite
{
    public static void BeginTest(long timestamp)
    {
        Beginning = timestamp;
    }

    private static long Beginning { get; set; }

    /// <summary>
    /// A collection that stores the results of recorded test timestamps.
    /// Each entry in the collection represents a tuple containing a
    /// timestamp and its corresponding processing time in ticks.
    /// </summary>
    /// <remarks>
    /// This property is used internally by the TestSuite class to track
    /// the timestamps of test recordings and their associated processing times.
    /// It is initialized as an empty list and updated with each call to the
    /// RecordTest method.
    /// </remarks>
    private static List<(long, long)> Results { get; } = [];

    public static void RecordTest(long timestamp)
    {
        Results.Add((timestamp, DateTimeOffset.Now.Ticks));
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
        var offsets = Results.Select(r =>
            ((r.Item2 - r.Item1) / (double)ticksPerMs))
            .ToArray();
        var average = offsets
            .Average()
            .ToString("F3", CultureInfo.InvariantCulture);

        return
            $"Duration: {durationMs.ToString("F3", CultureInfo.InvariantCulture)}ms{System.Environment.NewLine}Hits: [{string.Join(", ", offsets.Select(o => $"{o.ToString("F3", CultureInfo.InvariantCulture)}ms"))}]{System.Environment.NewLine}Average: {average}ms";
    }
}