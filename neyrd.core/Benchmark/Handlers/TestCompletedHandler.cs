using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.core.Benchmark.Handlers;

public sealed class TestCompletedHandler : INeyrdEventHandler<TestCompletedEvent, long>
{
    public Task Handle(TestCompletedEvent @event)
    {
        TestSuite.Complete(@event.Payload);
        return Task.CompletedTask;
    }
}