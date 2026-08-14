using System;
using System.Threading.Tasks;
using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.receiver.Handlers;

internal sealed class SynchronizationRequestedEventHandler(MainWindow window) : INeyrdEventHandler<SynchronizationRequestedEvent,
    (long, long)>
{
    public Task Handle(SynchronizationRequestedEvent @event)
    {
        var t3 = DateTimeOffset.Now.Ticks;
        var t1 = @event.Payload.Item2;
        var tEmitter = @event.Payload.Item1; // emitter's clock at echo time
        var offset = tEmitter - (t1 + t3) / 2;
        
        window.SetClockOffset(offset);
        return Task.CompletedTask;
    }
}