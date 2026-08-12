namespace neyrd.core.Events;

public interface INeyrdEventHandler<TEvent, in TPayload> where TEvent : INeyrdEvent<TPayload>
{
    Task Handle(TEvent @event);
}