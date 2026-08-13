using System.Buffers.Binary;
using neyrd.core.Models;

namespace neyrd.core.Messages;

public static class MessageFactory
{
    public static string Encode(string[] parameters, MessageType type)
    {
        return $"{DateTimeOffset.Now.Ticks}:{(int)type}:{string.Join(":", parameters)}==";
    }
    
    public static byte[] Encode(MessageOrigin.Kind origin, int originalSize, int encodedLength, byte[] data, int width, int height)
    {
        var timestamp = DateTimeOffset.Now.Ticks;

        // layout: [4 length][1 frame-marker][8 timestamp][4 origin][4 originalSize][4 encodedLength][data][4 width][4 height]
        var result = new byte[4 + 1 + 8 + 4 + 4 + 4 + data.Length + 4 + 4];
        BinaryPrimitives.WriteInt32LittleEndian(result, result.Length);
        result[4] = (byte)MessageType.Frame;
        var span = result.AsSpan(5);

        BinaryPrimitives.WriteInt64LittleEndian(span, timestamp);
        BinaryPrimitives.WriteInt32LittleEndian(span[8..], (int)origin);
        BinaryPrimitives.WriteInt32LittleEndian(span[12..], originalSize);
        BinaryPrimitives.WriteInt32LittleEndian(span[16..], encodedLength);
        data.CopyTo(span[20..]);
        BinaryPrimitives.WriteInt32LittleEndian(span[(20 + data.Length)..], width);
        BinaryPrimitives.WriteInt32LittleEndian(span[(24 + data.Length)..], height);

        return result;
    }
    
    public static string[] Decode(string message)
    {
        return message.Split(':');
    }
    
    public static DecodedFrame Decode(byte[] message)
    {
        // layout: [4 length][1 frame-marker][8 timestamp][4 origin][4 originalSize][4 encodedLength][data][4 width][4 height]
        var span = message.AsSpan(5);
        var timestamp = BinaryPrimitives.ReadInt64LittleEndian(span);
        var origin = (MessageOrigin.Kind)BinaryPrimitives.ReadInt32LittleEndian(span[8..]);
        var originalSize = BinaryPrimitives.ReadInt32LittleEndian(span[12..]);
        var encodedLength = BinaryPrimitives.ReadInt32LittleEndian(span[16..]);
        var data = message[25..(25 + encodedLength)];
        var width = BinaryPrimitives.ReadInt32LittleEndian(span[(20 + encodedLength)..]);
        var height = BinaryPrimitives.ReadInt32LittleEndian(span[(20 + encodedLength + 4)..]);

        return new DecodedFrame(timestamp, origin, originalSize, encodedLength, data, width, height);
    }
}