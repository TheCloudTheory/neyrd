using System;
using System.IO;

namespace neyrd.receiver;

internal static class NeyrdLogger
{
    public static void Log(string message)
    {
        File.AppendAllText("neyrd.log", $"[{DateTimeOffset.Now}] {message}{Environment.NewLine}");
    }
}