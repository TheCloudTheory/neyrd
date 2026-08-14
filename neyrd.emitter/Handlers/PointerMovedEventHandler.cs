using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class PointerMovedEventHandler(IPuppeter puppeter) : INeyrdEventHandler<PointerMovedEvent, (double, double)>
{
    public Task Handle(PointerMovedEvent @event)
    {
        puppeter.MovePointer(@event.Payload.Item1, @event.Payload.Item2);
        return Task.CompletedTask;
    }
}