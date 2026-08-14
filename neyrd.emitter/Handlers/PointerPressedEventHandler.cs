using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class PointerPressedEventHandler(IPuppeter puppeter) : INeyrdEventHandler<PointerPressedEvent, (double, double)>
{
    public Task Handle(PointerPressedEvent @event)
    {
        return Task.CompletedTask;
    }
}