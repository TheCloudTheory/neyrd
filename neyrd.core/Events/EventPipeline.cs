using System.Collections.Concurrent;

namespace neyrd.core.Events;

public static class EventPipeline
{
    private interface IHandlerWrapper
    {
        Task Handle(object @event);
    }

    private sealed class HandlerWrapper<TEvent, TPayload>(INeyrdEventHandler<TEvent, TPayload> handler) : IHandlerWrapper
        where TEvent : INeyrdEvent<TPayload>
    {
        public Task Handle(object @event) => handler.Handle((TEvent)@event);
    }
    
    private static readonly ConcurrentDictionary<string, IHandlerWrapper[]> Handlers = new();

    /// <summary>
    /// Publishes an event to all registered handlers that are subscribed to the event's type.
    /// Handlers for the event type will have their <c>Handle</c> method invoked.
    /// </summary>
    /// <param name="event">The event to publish. It must implement the <c>INeyrdEvent</c> interface.</param>
    public static void Publish<TEvent, TPayload>(TEvent @event) where TEvent : INeyrdEvent<TPayload>
    {
        if (!Handlers.TryGetValue(TEvent.Type, out var handlers))
        {
            NeyrdLogger.Log($"No handlers registered for event type '{TEvent.Type}'.");
            return;
        }

        foreach (var handler in handlers)
        {
            NeyrdLogger.Log($"Handling event '{TEvent.Type}'.");
            handler.Handle(@event);
        }
    }

    /// <summary>
    /// Registers an event handler for the specified event type.
    /// Once subscribed, the handler will be invoked when an event of the matching type is published.
    /// </summary>
    /// <param name="handler">The event handler to be registered. It must implement the <c>INeyrdEventHandler</c> interface.</param>
    /// <typeparam name="TEvent">The type of event the handler will respond to.</typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public static void Subscribe<TEvent, TPayload>(INeyrdEventHandler<TEvent, TPayload> handler) where TEvent : INeyrdEvent<TPayload>
    {
        var type = TEvent.Type;
        
        Handlers.AddOrUpdate(type,
            _ => [new HandlerWrapper<TEvent, TPayload>(handler)],
            (_, existing) => [.. existing, new HandlerWrapper<TEvent, TPayload>(handler)]);
    }
}