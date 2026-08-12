using System.Collections.Generic;
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

        var tasks = new List<Task>
        {
            sender.Send(HandshakeConfirmedMessage.ToMessage()),
            sender.Send(TestStartedMessage.ToMessage())
        };

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(sender.Send(TestMessage.ToMessage()));
        }
            
        tasks.Add(sender.Send(TestCompletedMessage.ToMessage()));
        
        return Task.WhenAll(tasks);
    }
}