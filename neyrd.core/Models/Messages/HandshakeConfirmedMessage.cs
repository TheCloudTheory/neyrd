using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class HandshakeConfirmedMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static HandshakeConfirmedMessage ToMessage()
    {
        return new HandshakeConfirmedMessage(MessageFactory.Encode(["r", "1"], MessageType.Acknowledgement));
    }
}