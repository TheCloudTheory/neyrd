namespace neyrd.core.Messages;

public sealed class MessageEnvelope(
    DateTimeOffset timestamp,
    MessageType type,
    MessageOrigin.Kind origin,
    string[] message)
{
    public DateTimeOffset Timestamp { get; } = timestamp;
    public MessageType Type { get; } = type;
    public MessageOrigin.Kind Origin { get; } = origin;
    public string[] Segments { get; } = message;

    public static MessageEnvelope From(string message)
    {
        var decoded = MessageFactory.Decode(message);
        var timestamp = new DateTimeOffset(long.Parse(decoded[0]), TimeSpan.Zero);
        var type = (MessageType)int.Parse(decoded[1]);
        var origin = decoded[2] == MessageOrigin.Emitter ? MessageOrigin.Kind.Emitter : MessageOrigin.Kind.Receiver;

        return new MessageEnvelope(timestamp,
            type, origin, decoded[3..]);
    }
}