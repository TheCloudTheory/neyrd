using neyrd.emitter.Encoding.Encoders;

namespace neyrd.emitter.Encoding;

internal sealed class EncodingStrategySelector
{
    private static readonly IEncoder[] Encoders =
    [
        new LZ4Encoder()
    ];
    
    public static IEncoder GetEncoder()
    {
        return Encoders[0];
    }
}