using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class TestMessage(string payload) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(payload);

    public static TestMessage ToMessage()
    {
        return new TestMessage(MessageFactory.Encode(["e", DateTimeOffset.Now.Ticks.ToString()], MessageType.Test));
    }
}