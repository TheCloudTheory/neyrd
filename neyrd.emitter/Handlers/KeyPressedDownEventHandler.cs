using neyrd.core.Events;
using neyrd.core.Models;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class KeyPressedDownEventHandler(IPuppeter puppeter) : INeyrdEventHandler<KeyPressedDownEvent, (string, KeyModifier)>
{
    public Task Handle(KeyPressedDownEvent @event)
    {
        puppeter.HandleKeyDown(@event.Payload.Item1, @event.Payload.Item2);
        return Task.CompletedTask;
    }
}