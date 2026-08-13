using System;
using System.Threading.Tasks;
using neyrd.core;
using neyrd.core.Events;
using neyrd.core.Messages;
using neyrd.core.Models.Events;
using neyrd.receiver.Decoding;

namespace neyrd.receiver.Handlers;

internal sealed class FrameReceivedHandler(MainWindow window) : INeyrdEventHandler<FrameReceivedEvent, byte[]>
{
    public Task Handle(FrameReceivedEvent @event)
    {
        var decoded = MessageFactory.Decode(@event.Payload);
        NeyrdLogger.Log($"Received frame: {decoded}");

        try
        {
            var frame = DecodingStrategySelector.GetDecoder().Decode(decoded);
            window.UpdateFrame(frame, decoded.Width, decoded.Height);
        
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            NeyrdLogger.Log($"Error when handling frame received event: {ex.Message}");
            return Task.FromException(ex);
        }
    }
}