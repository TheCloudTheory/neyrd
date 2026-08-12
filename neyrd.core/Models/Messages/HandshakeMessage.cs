using System.Net;
using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class HandshakeMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static HandshakeMessage ToMessage(IPAddress ipAddress)
    {
        return new HandshakeMessage(MessageFactory.Encode([MessageOrigin.Emitter, ipAddress.ToString()], MessageType.Handshake));
    }
}