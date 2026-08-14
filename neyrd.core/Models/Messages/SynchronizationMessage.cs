using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class SynchronizationMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);
    
    public static SynchronizationMessage ToMessage(long timestamp)
    {
        return new SynchronizationMessage(MessageFactory.Encode([MessageOrigin.Emitter, timestamp.ToString()], MessageType.Synchronization));
    }
}