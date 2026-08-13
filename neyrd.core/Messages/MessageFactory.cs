using System.Buffers.Binary;
using neyrd.core.Models;

namespace neyrd.core.Messages;

public static class MessageFactory
{
    private const byte Separator = (byte)':';

    public static string Encode(string[] parameters, MessageType type)
    {
        return $"{DateTimeOffset.Now.Ticks}:{(int)type}:{string.Join(":", parameters)}==";
    }
    
    public static byte[] Encode(MessageOrigin.Kind origin, int originalSize, int encodedLength, byte[] data, MessageType type)
    {
        var timestamp = DateTimeOffset.Now.Ticks;

        // layout: [4 length][1 frame-marker][8 timestamp][1 :][4 origin][1 :][4 originalSize][1 :][4 encodedLength][1 :][data]
        var result = new byte[4 + 1 + 8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + data.Length];
        BinaryPrimitives.WriteInt32LittleEndian(result, result.Length);
        result[4] = (byte)MessageType.Frame;
        var span = result.AsSpan(5);

        BinaryPrimitives.WriteInt64LittleEndian(span, timestamp);
        span[8] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[9..], (int)origin);
        span[13] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[14..], originalSize);
        span[18] = Separator;
        BinaryPrimitives.WriteInt32LittleEndian(span[19..], encodedLength);
        span[23] = Separator;
        data.CopyTo(span[24..]);

        return result;
    }
    
    public static string[] Decode(string message)
    {
        return message.Split(':');
    }
    
    public static DecodedFrame Decode(byte[] message)
    {
        // layout: [4 length][1 frame-marker][8 timestamp][1 :][4 origin][1 :][4 originalSize][1 :][4 encodedLength][1 :][data]
        var span = message.AsSpan(5); // skip length+frame-marker
        var timestamp = BinaryPrimitives.ReadInt64LittleEndian(span);
        var origin = (MessageOrigin.Kind)BinaryPrimitives.ReadInt32LittleEndian(span[9..]);
        var originalSize = BinaryPrimitives.ReadInt32LittleEndian(span[14..]);
        var encodedLength = BinaryPrimitives.ReadInt32LittleEndian(span[19..]);
        var data = message[29..];

        return new DecodedFrame(timestamp, origin, originalSize, encodedLength, data);
    }
}