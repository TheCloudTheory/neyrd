using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.core.Benchmark.Handlers;

public sealed class TestCompletedHandler : INeyrdEventHandler<TestCompletedEvent, bool>
{
    public Task Handle(TestCompletedEvent @event)
    {
        TestSuite.Complete();
        return Task.CompletedTask;
    }
}