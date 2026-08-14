using System.Text;
using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class ScreenResolutionMessage(string message) : IMessage
{
    public byte[] Payload { get; } = Encoding.UTF8.GetBytes(message);

    public static ScreenResolutionMessage ToMessage(int width, int height)
    {
        return new ScreenResolutionMessage(MessageFactory.Encode([MessageOrigin.Emitter, width.ToString(), height.ToString()], MessageType.Screen));
    }
}