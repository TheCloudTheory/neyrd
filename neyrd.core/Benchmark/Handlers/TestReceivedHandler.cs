using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.core.Benchmark.Handlers;

public sealed class TestReceivedHandler : INeyrdEventHandler<TestReceivedEvent, long>
{
    public Task Handle(TestReceivedEvent @event)
    {
        TestSuite.RecordTest(@event.Payload);
        return Task.CompletedTask;
    }
}