using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.core.Benchmark.Handlers;

public sealed class TestStartedEventHandler : INeyrdEventHandler<TestStartedEvent, long>
{
    public Task Handle(TestStartedEvent @event)
    {
        TestSuite.BeginTest(@event.Payload);
        return Task.CompletedTask;
    }
}