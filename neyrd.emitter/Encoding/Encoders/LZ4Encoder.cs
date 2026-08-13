using K4os.Compression.LZ4;

namespace neyrd.emitter.Encoding.Encoders;

internal sealed class Lz4Encoder : IEncoder
{
    public EncodedFrame Encode(byte[] data)
    {
        var target = new byte[LZ4Codec.MaximumOutputSize(data.Length)];
        var encodedLength = LZ4Codec.Encode(
            data, 0, data.Length,
            target, 0, target.Length);
        
        return new EncodedFrame(data.Length, encodedLength, target[..encodedLength]);
    }
}