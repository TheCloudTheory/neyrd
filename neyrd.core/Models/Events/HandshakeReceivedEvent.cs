using System.Net;
using neyrd.core.Events;

namespace neyrd.core.Models.Events;

public sealed class HandshakeReceivedEvent(IPAddress ipAddress) : INeyrdEvent
{
    public string Type => "HandshakeReceived";
    public object Payload => ipAddress;

    public static INeyrdEvent From(string message)
    {
        var segments = message.Split(':');
        var emitterIp = segments[0];
        
        return new HandshakeReceivedEvent(IPAddress.Parse(emitterIp));
    }
}