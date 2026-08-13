using neyrd.core.Messages;

namespace neyrd.core.Models;

public readonly record struct DecodedFrame(
    long Timestamp,
    MessageOrigin.Kind Origin,
    int OriginalSize,
    int EncodedLength,
    byte[] Data,
    int Width,
    int Height
);