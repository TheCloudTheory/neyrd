using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class AcknowledgementReceivedEvent(long timestamp) : INeyrdEvent<long>
{
    public static string Type => "AcknowledgementReceived";
    public long Payload => timestamp;

    public static AcknowledgementReceivedEvent From(MessageEnvelope message)
    {
        var timestamp = long.Parse(message.Segments[0]);
        return new AcknowledgementReceivedEvent(timestamp);
    }
}