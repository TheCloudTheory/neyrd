using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.core.Models.Messages;
using neyrd.emitter.Networking;
using Spectre.Console;

namespace neyrd.emitter.Handlers;

internal sealed class AcknowledgementReceivedEventHandler(NeyrdSender sender) : INeyrdEventHandler<AcknowledgementReceivedEvent, long>
{
    public Task Handle(AcknowledgementReceivedEvent @event)
    {
        AnsiConsole.WriteLine("Synchronizing emitter and receiver...");
        return sender.Send(SynchronizationMessage.ToMessage(@event.Payload));
    }
}