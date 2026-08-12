using System.Net;
using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class HandshakeReceivedEvent(IPAddress ipAddress) : INeyrdEvent<IPAddress>
{
    public static string Type => "HandshakeReceived";
    public IPAddress Payload => ipAddress;

    public static HandshakeReceivedEvent From(MessageEnvelope message)
    {
        var emitterIp = message.Segments[0];
        
        return new HandshakeReceivedEvent(IPAddress.Parse(emitterIp));
    }
}