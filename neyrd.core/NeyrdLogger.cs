namespace neyrd.core;

public static class NeyrdLogger
{
    public static void Log(string message)
    {
        File.AppendAllText("neyrd.log", $"[{DateTimeOffset.Now}] {message}{Environment.NewLine}");
    }
}