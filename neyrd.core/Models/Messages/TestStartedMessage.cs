using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class TestStartedMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static TestStartedMessage ToMessage()
    {
        return new TestStartedMessage(MessageFactory.Encode([MessageOrigin.Emitter, DateTimeOffset.Now.Ticks.ToString()], MessageType.TestStarted));
    }
}