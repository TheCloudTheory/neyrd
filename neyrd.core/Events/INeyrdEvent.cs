namespace neyrd.core.Events;

public interface INeyrdEvent
{
    string Type { get; }
    object Payload { get; }
}