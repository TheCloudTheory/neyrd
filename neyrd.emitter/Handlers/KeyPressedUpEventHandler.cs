using neyrd.core.Events;
using neyrd.core.Models;
using neyrd.core.Models.Events;
using neyrd.emitter.Puppeting;

namespace neyrd.emitter.Handlers;

internal sealed class KeyPressedUpEventHandler(IPuppeter puppeter) : INeyrdEventHandler<KeyPressedUpEvent, (string, KeyModifier)>
{
    public Task Handle(KeyPressedUpEvent @event)
    {
        puppeter.HandleKeyUp(@event.Payload.Item1, @event.Payload.Item2);
        return Task.CompletedTask;
    }
}