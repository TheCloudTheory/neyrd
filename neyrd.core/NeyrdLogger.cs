namespace neyrd.core;

public static class NeyrdLogger
{
    static NeyrdLogger()
    {
        File.WriteAllText("neyrd.log", string.Empty);    
    }
    
    public static void Log(string message)
    {
        File.AppendAllText("neyrd.log", $"[{DateTimeOffset.Now}] {message}{Environment.NewLine}");
    }
}