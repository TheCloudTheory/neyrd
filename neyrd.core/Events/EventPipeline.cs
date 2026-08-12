using System.Collections.Concurrent;

namespace neyrd.core.Events;

public sealed class EventPipeline
{
    private static readonly ConcurrentDictionary<string, INeyrdEventHandler[]> Handlers = new();

    /// <summary>
    /// Publishes an event to all registered handlers that are subscribed to the event's type.
    /// Handlers for the event type will have their <c>Handle</c> method invoked.
    /// </summary>
    /// <param name="event">The event to publish. It must implement the <c>INeyrdEvent</c> interface.</param>
    public static void Publish(INeyrdEvent @event)
    {
        if (!Handlers.TryGetValue(@event.Type, out var handlers)) return;
        
        foreach (var handler in handlers)
        {
            handler.Handle(@event);
        }
    }

    /// <summary>
    /// Registers an event handler for the specified event type.
    /// Once subscribed, the handler will be invoked when an event of the matching type is published.
    /// </summary>
    /// <param name="handler">The event handler to be registered. It must implement the <c>INeyrdEventHandler</c> interface.</param>
    /// <typeparam name="TEvent">The type of event the handler will respond to.</typeparam>
    public static void Subscribe<TEvent>(INeyrdEventHandler handler)
    {
        var type = typeof(TEvent).Name;
        if (!Handlers.TryGetValue(type, out var value))
        {
            value = [];
            Handlers[type] = value;
        }
        
        Handlers[type] = [.. value, handler];
    }
}