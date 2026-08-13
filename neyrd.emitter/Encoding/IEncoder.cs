namespace neyrd.emitter.Encoding;

internal interface IEncoder
{
    EncodedFrame Encode(byte[] data);
}