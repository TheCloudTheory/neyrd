using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class TestReceivedEvent(long timestamp) : INeyrdEvent<long>
{
    public static string Type => "TestReceived";
    public long Payload { get; } = timestamp;
    
    public static TestReceivedEvent From(MessageEnvelope message)
    {
        var timestamp = long.Parse(message.Segments[0]);
        return new TestReceivedEvent(timestamp);
    }
}