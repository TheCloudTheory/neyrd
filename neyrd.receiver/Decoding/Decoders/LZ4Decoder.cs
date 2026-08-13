using K4os.Compression.LZ4;
using neyrd.core.Models;

namespace neyrd.receiver.Decoding.Decoders;

internal sealed class Lz4Decoder : IDecoder
{
    public byte[] Decode(DecodedFrame frame)
    {
        var source = frame.Data;
        var target = new byte[frame.OriginalSize];
        _ = LZ4Codec.Decode(
            source, 0, source.Length,
            target, 0, target.Length);

        return target;
    }
}