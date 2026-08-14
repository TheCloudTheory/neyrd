using System;
using System.Threading.Tasks;
using neyrd.core;
using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.receiver.Handlers;

internal sealed class SynchronizationRequestedEventHandler(MainWindow window) : INeyrdEventHandler<SynchronizationRequestedEvent,
    (long, long)>
{
    public Task Handle(SynchronizationRequestedEvent @event)
    {
        var receiversCurrentClock = DateTimeOffset.Now.Ticks;
        var receiversOldClock = @event.Payload.Item2;
        var emittersClock = @event.Payload.Item1; 
        var offset = emittersClock - (receiversOldClock + receiversCurrentClock) / 2;
        
        NeyrdLogger.Log($"Synchronization request received: {receiversCurrentClock}, {receiversOldClock}, {emittersClock}, {offset}");
        
        window.SetClockOffset(offset);
        return Task.CompletedTask;
    }
}