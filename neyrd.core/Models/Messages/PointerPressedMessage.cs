using System.Globalization;
using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class PointerPressedMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static PointerPressedMessage ToMessage(double x, double y)
    {
        return new PointerPressedMessage(MessageFactory.Encode(
        [
            MessageOrigin.Receiver, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture)
        ], MessageType.PointerPressed));
    }
}