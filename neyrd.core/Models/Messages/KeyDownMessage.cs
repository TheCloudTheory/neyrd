using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class KeyDownMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static IMessage ToMessage(string keySymbol, int keyModifier)
    {
        return new KeyDownMessage(MessageFactory.Encode([MessageOrigin.Receiver, keySymbol, keyModifier.ToString()], MessageType.KeyDown));
    }
}