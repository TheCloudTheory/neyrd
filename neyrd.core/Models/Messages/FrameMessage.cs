using neyrd.core.Messages;

namespace neyrd.core.Models.Messages;

public sealed class FrameMessage(byte[] data) : IMessage
{
    public byte[] Payload { get; } = data;
    
    public static FrameMessage ToMessage(int originalSize, int encodedLength, byte[] data)
    {
        return new FrameMessage(MessageFactory.Encode(MessageOrigin.Kind.Emitter, originalSize, encodedLength, data, MessageType.Frame));
    }
}