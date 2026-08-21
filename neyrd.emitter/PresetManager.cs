using System.Net;
using Spectre.Console;

namespace neyrd.emitter;

internal static class PresetManager
{
    public static IPAddress LoadPreset(string presetName)
    {
        AnsiConsole.WriteLine("Loading preset `" + presetName + "`...");
        
        var fileName = presetName + ".neyrd";
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(fileName);
        }
        
        var lines = File.ReadAllLines(fileName);
        var ipLine = lines[1].Split('=');
        
        return IPAddress.Parse(ipLine[1]);
    }
}