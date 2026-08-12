using System.Net;
using System.Threading.Tasks;
using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.core.Models.Messages;
using neyrd.receiver.Networking;

namespace neyrd.receiver.Handlers;

internal sealed class HandshakeReceivedEventHandler(NeyrdSender sender) : INeyrdEventHandler<HandshakeReceivedEvent, IPAddress>
{
    public Task Handle(HandshakeReceivedEvent @event)
    {
        sender.Connect(@event.Payload);
        return sender.Send(HandshakeConfirmedMessage.ToMessage());
    }
}