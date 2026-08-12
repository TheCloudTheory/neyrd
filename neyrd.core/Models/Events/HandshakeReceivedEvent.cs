using System.Net;
using neyrd.core.Events;

namespace neyrd.core.Models.Events;

public sealed class HandshakeReceivedEvent(IPAddress ipAddress) : INeyrdEvent<IPAddress>
{
    public static string Type => "HandshakeReceived";
    public IPAddress Payload => ipAddress;

    public static HandshakeReceivedEvent From(string message)
    {
        var segments = message.Split(':');
        var emitterIp = segments[0];
        
        return new HandshakeReceivedEvent(IPAddress.Parse(emitterIp));
    }
}