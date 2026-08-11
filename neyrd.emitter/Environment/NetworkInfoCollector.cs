using System.Net;
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
        var ips = unicastAddresses
            .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork
                         && !IPAddress.IsLoopback(ua.Address)
                         && ua.Address.GetAddressBytes() is [not 169, ..]);
        
        return [.. ips.Select(ip => ip.Address.ToString())];
    }
}