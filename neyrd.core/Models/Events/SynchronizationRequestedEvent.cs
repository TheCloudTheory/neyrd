using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class SynchronizationRequestedEvent(long timestamp, long echoedTimestamp) : INeyrdEvent<(long, long)>
{
    public static string Type => "SynchronizationRequested";
    public (long, long) Payload { get; set; } = (timestamp, echoedTimestamp);
    
    public static SynchronizationRequestedEvent From(MessageEnvelope message)
    {
        var timestamp = long.Parse(message.Segments[0]);
        return new SynchronizationRequestedEvent(message.Timestamp.Ticks, timestamp);
    }
}