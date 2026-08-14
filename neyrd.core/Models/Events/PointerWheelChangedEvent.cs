using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class PointerWheelChangedEvent(double deltaLength, double deltaX, double deltaY)
    : INeyrdEvent<(double, double, double)>
{
    public static string Type => "PointerWheelChangedEvent";
    public (double, double, double) Payload { get; } = (deltaLength, deltaX, deltaY);

    public static PointerWheelChangedEvent From(MessageEnvelope message)
    {
        var deltaLength = double.Parse(message.Segments[0]);
        var deltaX = double.Parse(message.Segments[1]);
        var deltaY = double.Parse(message.Segments[2]);

        return new PointerWheelChangedEvent(deltaLength, deltaX, deltaY);
    }
}