using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class PointerReleasedEvent(MouseButton button) : INeyrdEvent<MouseButton>
{
    public static string Type => "PointerReleased";
    public MouseButton Payload { get; } = button;
    
    public static PointerReleasedEvent From(MessageEnvelope message)
    {
        var button = Enum.Parse<MouseButton>(message.Segments[0]);

        return new PointerReleasedEvent(button);
    }
}