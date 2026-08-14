using System.Threading.Tasks;
using neyrd.core.Events;
using neyrd.core.Models.Events;

namespace neyrd.receiver.Handlers;

internal sealed class ScreenResolutionEstablishedEventHandler(MainWindow window) : INeyrdEventHandler<ScreenResolutionEstablishedEvent, (int, int)>
{
    public Task Handle(ScreenResolutionEstablishedEvent @event)
    {
        window.SetEmittersScreenResolution(@event.Payload.Item1,  @event.Payload.Item2);
        return Task.CompletedTask;
    }
}