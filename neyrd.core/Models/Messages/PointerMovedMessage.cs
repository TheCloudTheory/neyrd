using System.Globalization;
using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class PointerMovedMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static PointerMovedMessage ToMessage(double x, double y)
    {
        return new PointerMovedMessage(MessageFactory.Encode(
            [MessageOrigin.Emitter, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture)],
            MessageType.Pointer));
    }
}