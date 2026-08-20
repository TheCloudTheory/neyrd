using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class PointerReleasedMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static PointerReleasedMessage ToMessage(MouseButton button)
    {
        return new PointerReleasedMessage(MessageFactory.Encode(
        [
            MessageOrigin.Receiver, button.ToString()], MessageType.PointerReleased));
    }
}