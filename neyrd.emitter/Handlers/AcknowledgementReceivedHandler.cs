using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.core.Models.Messages;
using neyrd.emitter.Networking;

namespace neyrd.emitter.Handlers;

internal sealed class AcknowledgementReceivedHandler(NeyrdSender sender) : INeyrdEventHandler<AcknowledgementReceivedEvent, long>
{
    public Task Handle(AcknowledgementReceivedEvent @event)
    {
        return sender.Send(SynchronizationMessage.ToMessage(@event.Payload));
    }
}