using System.Net;
using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class HandshakeMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static HandshakeConfirmedMessage ToMessage(IPAddress ipAddress)
    {
        return new HandshakeConfirmedMessage(MessageFactory.Encode(["e", ipAddress.ToString()], MessageType.Acknowledgement));
    }
}