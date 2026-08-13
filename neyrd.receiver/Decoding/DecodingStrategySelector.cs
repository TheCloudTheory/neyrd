using neyrd.receiver.Decoding.Decoders;

namespace neyrd.receiver.Decoding;

internal sealed class DecodingStrategySelector
{
    public static IDecoder[] Decoders { get; } =
    [
        new Lz4Decoder()
    ];
    
    public static IDecoder GetDecoder()
    {
        return Decoders[0];
    }
}