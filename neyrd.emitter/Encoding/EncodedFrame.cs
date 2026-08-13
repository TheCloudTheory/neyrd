namespace neyrd.emitter.Encoding;

/// <summary>
/// Represents a data structure for storing information about an encoded frame,
/// including the original size of the data, the length of the encoded data, and the encoded byte array.
/// </summary>
/// <remarks>
/// This structure is intended for use in scenarios involving data encoding, where efficient storage of
/// the original and encoded data properties is necessary for further processing.
/// </remarks>
/// <param name="OriginalSize">The original size of the data in bytes before encoding.</param>
/// <param name="EncodedLength">The length of the encoded data in bytes.</param>
/// <param name="Data">The byte array containing the encoded data.</param>
internal readonly record struct EncodedFrame(int OriginalSize, int EncodedLength, byte[] Data);