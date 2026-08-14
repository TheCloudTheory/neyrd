using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class PointerWheelChangedEventHandler(IPuppeter puppeter)
    : INeyrdEventHandler<PointerWheelChangedEvent, (double, double, double)>
{
    public Task Handle(PointerWheelChangedEvent @event)
    {
        puppeter.HandleWheel(@event.Payload.Item1, @event.Payload.Item2, @event.Payload.Item3);
        return Task.CompletedTask;
    }
}