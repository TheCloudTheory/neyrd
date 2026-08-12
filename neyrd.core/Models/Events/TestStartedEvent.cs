using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class TestStartedEvent(long timestamp) : INeyrdEvent<long>
{
    public static string Type  => "TestStarted";
    public long Payload => timestamp;

    public static TestStartedEvent From(MessageEnvelope message)
    {
        var timestamp = long.Parse(message.Segments[0]);
        return new TestStartedEvent(timestamp);
    }
}