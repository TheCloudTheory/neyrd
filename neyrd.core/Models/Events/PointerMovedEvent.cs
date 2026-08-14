using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class PointerMovedEvent(double x, double y) : INeyrdEvent<(double, double)>
{
    public static string Type =>  "PointerMoved";
    public (double, double) Payload { get; } = (x, y);

    public static PointerMovedEvent From(MessageEnvelope message)
    {
        var x = double.Parse(message.Segments[0]);
        var y = double.Parse(message.Segments[1]);

        return new PointerMovedEvent(x, y);
    }
}