using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class TestCompletedMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static TestCompletedMessage ToMessage()
    {
        return new TestCompletedMessage(
            MessageFactory.Encode([MessageOrigin.Emitter, DateTimeOffset.Now.Ticks.ToString()],
                MessageType.TestStarted));
    }
}