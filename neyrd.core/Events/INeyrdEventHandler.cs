namespace neyrd.core.Events;

public interface INeyrdEventHandler
{
    void Handle(INeyrdEvent @event);
}