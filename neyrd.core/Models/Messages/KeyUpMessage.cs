using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class KeyUpMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static IMessage ToMessage(string keySymbol, int keyModifier)
    {
        return new KeyUpMessage(MessageFactory.Encode([MessageOrigin.Receiver, keySymbol, keyModifier.ToString()], MessageType.KeyUp));
    }
}