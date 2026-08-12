using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class TestCompletedEvent : INeyrdEvent<bool>
{
    public static string Type => "TestCompleted";
    public bool Payload { get; } = true;
    
    public static TestCompletedEvent From(MessageEnvelope message)
    {
        return new TestCompletedEvent();
    }
}