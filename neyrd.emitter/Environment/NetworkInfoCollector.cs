using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace neyrd.emitter.Environment;

internal sealed class NetworkInfoCollector
{
    internal string[] NetworkInterfaces => GetLocalIpAddresses();

    public override string ToString()
    {
        return $"Network interfaces: {string.Join(", ", NetworkInterfaces)}";
    }

    private static string[] GetLocalIpAddresses()
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces();
        var unicastAddresses = nics.Where(x => x.OperationalStatus == OperationalStatus.Up)
            .SelectMany(x => x.GetIPProperties().UnicastAddresses);
        var ips = unicastAddresses.Select(ua => ua.Address)
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        return [.. ips.Select(ip => ip.ToString())];
    }
}