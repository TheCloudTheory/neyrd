using System.Threading.Tasks;
using neyrd.core;
using neyrd.core.Events;
using neyrd.core.Messages;
using neyrd.core.Models.Events;
using neyrd.receiver.Decoding;

namespace neyrd.receiver.Handlers;

internal sealed class FrameReceivedHandler : INeyrdEventHandler<FrameReceivedEvent, byte[]>
{
    public Task Handle(FrameReceivedEvent @event)
    {
        var decoded = MessageFactory.Decode(@event.Payload);
        NeyrdLogger.Log($"Received frame: {decoded}");

        var frame = DecodingStrategySelector.GetDecoder().Decode(decoded);
        
        return Task.CompletedTask;
    }
}