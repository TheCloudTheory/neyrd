namespace neyrd.emitter.Environment;

internal sealed class EnvironmentInfoCollector
{
    internal string OsName => System.Environment.OSVersion.Platform.ToString();
    internal string OsVersion => System.Environment.OSVersion.Version.ToString();
    internal string Cpu => System.Environment.ProcessorCount.ToString();

    public override string ToString()
    {
        return $"OS {OsName} {OsVersion} | CPU cores: {Cpu}";
    }
}