namespace neyrd.emitter.Capturing;

internal interface ICaptureAdapter
{
    string Name { get; }
    bool IsSupported { get; }
    
    ReadOnlySpan<byte> CaptureFrame();
}