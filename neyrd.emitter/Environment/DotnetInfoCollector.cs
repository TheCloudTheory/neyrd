using System.Runtime;

namespace neyrd.emitter.Environment;

internal sealed class DotnetInfoCollector
{
    public int AvailableThreads => GetAvailableThreads();
    public string GarbageCollectorMode => GCSettings.IsServerGC ? "Server" : "Workstation";

    private int GetAvailableThreads()
    {
        ThreadPool.GetAvailableThreads(out _, out var availableThreads);
        return availableThreads;
    }

    public override string ToString()
    {
        return $"Threads: {AvailableThreads}, GC Mode: {GarbageCollectorMode}";
    }
}