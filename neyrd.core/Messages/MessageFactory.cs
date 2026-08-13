using System.Buffers.Binary;

namespace neyrd.core.Messages;

public static class MessageFactory
{
    private const byte Separator = (byte)':';
    private static readonly byte[] Eom = [.. "=="u8];

    public static string Encode(string[] parameters, MessageType type)
    {
        return $"{DateTimeOffset.Now.Ticks}:{(int)type}:{string.Join(":", parameters)}==";
    }
    
    public static byte[] Encode(MessageOrigin.Kind origin, int originalSize, int encodedLength, byte[] data, MessageType type)
    {
        var timestamp = DateTimeOffset.Now.Ticks;

        // layout: [8 timestamp][1 :][4 type][1 :][4 origin][1 :][4 originalSize][1 :][4 encodedLength][1 :][data][==]
        var result = new byte[8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + data.Length + Eom.Length];
        var span = result.AsSpan();

        BinaryPrimitives.WriteInt64LittleEndian(span, timestamp);
        span[8] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[9..], (int)type);
        span[13] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[14..], (int)origin);
        span[18] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[19..], originalSize);
        span[23] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[24..], encodedLength);
        span[28] = Separator;
        data.CopyTo(span[29..]);
        Eom.CopyTo(span[(29 + data.Length)..]);

        return result;
    }
    
    public static string[] Decode(string message)
    {
        return message.Split(':');
    }
}