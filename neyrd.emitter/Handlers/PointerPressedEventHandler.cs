using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class PointerPressedEventHandler(IPuppeter puppeter) : INeyrdEventHandler<PointerPressedEvent, (double, double)>
{
    public Task Handle(PointerPressedEvent @event)
    {
        puppeter.HandleClick(@event.Payload.Item1,  @event.Payload.Item2);
        return Task.CompletedTask;
    }
}