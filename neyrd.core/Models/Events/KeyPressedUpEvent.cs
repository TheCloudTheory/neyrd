using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class KeyPressedUpEvent(string key, KeyModifier modifier) : INeyrdEvent<(string, KeyModifier)>
{
    public static string Type =>  "KeyPressedUp";
    public (string, KeyModifier) Payload { get; } = (key, modifier);

    public static KeyPressedUpEvent From(MessageEnvelope message)
    {
        var key = message.Segments[0];
        var modifier = (KeyModifier)int.Parse(message.Segments[1]);

        return new KeyPressedUpEvent(key, modifier);
    }
}