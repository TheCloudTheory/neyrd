using K4os.Compression.LZ4;

namespace neyrd.emitter.Encoding.Encoders;

internal sealed class LZ4Encoder : IEncoder
{
    public EncodedFrame Encode(byte[] data)
    {
        var source = new byte[1000];
        var target = new byte[LZ4Codec.MaximumOutputSize(source.Length)];
        var encodedLength = LZ4Codec.Encode(
            source, 0, source.Length,
            target, 0, target.Length);
        
        return new EncodedFrame(data.Length, encodedLength, target);
    }
}