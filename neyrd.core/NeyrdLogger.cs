namespace neyrd.core;

public static class NeyrdLogger
{
    private const int MaxLogFiles = 10;
    private const int MaxLinesPerLogFile = 10000;

    private static readonly Lock Lock = new();
    private static int _currentLogLine;
    private static int _currentLogFileIndex;
    
    static NeyrdLogger()
    {
        File.WriteAllText("neyrd.0.log", string.Empty);    
    }
    
    public static void Log(string message)
    {
        lock (Lock)
        {
            File.AppendAllText($"neyrd.{_currentLogFileIndex}.log", $"[{DateTimeOffset.Now}] {message}{System.Environment.NewLine}");
            _currentLogLine++;

            if (_currentLogLine < MaxLinesPerLogFile) return;
            
            _currentLogLine = 0;
            _currentLogFileIndex = (_currentLogFileIndex + 1) % MaxLogFiles;
            File.WriteAllText($"neyrd.{_currentLogFileIndex}.log", string.Empty);
        }
    }
}