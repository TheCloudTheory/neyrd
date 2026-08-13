using neyrd.core.Events;

namespace neyrd.core.Models.Events;

public sealed class FrameReceivedEvent(byte[] message) : INeyrdEvent<byte[]>
{
    public static string Type => "FrameReceived";
    public byte[] Payload { get; set; } = message;
    
    public static FrameReceivedEvent From(byte[] message) => new FrameReceivedEvent(message);
}