using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class PointerPressedEvent(double x, double y, MouseButton button) : INeyrdEvent<(double, double, MouseButton)>
{
    public static string Type => "PointerPressed";
    public (double, double, MouseButton) Payload { get; } = (x, y, button);
    
    public static PointerPressedEvent From(MessageEnvelope message)
    {
        var x = double.Parse(message.Segments[0]);
        var y = double.Parse(message.Segments[1]);
        var button = Enum.Parse<MouseButton>(message.Segments[2]);

        return new PointerPressedEvent(x, y, button);
    }
}