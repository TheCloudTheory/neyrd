using neyrd.core.Events;
using neyrd.core.Models;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class PointerReleasedEventHandler(IPuppeter puppeter) : INeyrdEventHandler<PointerReleasedEvent, MouseButton>
{
    public Task Handle(PointerReleasedEvent @event)
    {
        puppeter.HandleClickReleased(@event.Payload);
        return Task.CompletedTask;
    }
}