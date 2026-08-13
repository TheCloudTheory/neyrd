namespace neyrd.core.Events;

public interface INeyrdEvent
{
    static abstract string Type { get; }
}

public interface INeyrdEvent<out TPayload> : INeyrdEvent where TPayload : allows ref struct
{
    TPayload? Payload { get; }
}