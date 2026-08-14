using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class PointerPressedEvent(double x, double y) : INeyrdEvent<(double, double)>
{
    public static string Type => "PointerPressed";
    public (double, double) Payload { get; } = (x, y);
    
    public static PointerPressedEvent From(MessageEnvelope message)
    {
        var x = double.Parse(message.Segments[0]);
        var y = double.Parse(message.Segments[1]);

        return new PointerPressedEvent(x, y);
    }
}