using neyrd.core.Models;

namespace neyrd.receiver.Decoding;

internal interface IDecoder
{
    byte[] Decode(DecodedFrame frame);
}