using System.Globalization;
using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class PointerWheelChangedMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static PointerWheelChangedMessage ToMessage(double deltaLength, double deltaX, double deltaY)
    {
        return new PointerWheelChangedMessage(MessageFactory.Encode(
            [
                MessageOrigin.Receiver, deltaLength.ToString(CultureInfo.InvariantCulture),
                deltaX.ToString(CultureInfo.InvariantCulture), deltaY.ToString(CultureInfo.InvariantCulture)
            ],
            MessageType.PointerWheel));
    }
}