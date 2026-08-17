using System.Runtime.InteropServices;

namespace neyrd.emitter.Capturing.ScreenCaptureKit;

internal sealed partial class ScreenCaptureKitCapture : ICaptureAdapter
{
    private delegate void FrameCallback(IntPtr data, int width, int height);

    [LibraryImport("libneyrd_sckit")]
    private static partial void neyrd_start_capture(FrameCallback callback);

    [LibraryImport("libneyrd_sckit")]
    private static partial void neyrd_stop_capture();

    private FrameData _latest;
    private readonly Lock _lock = new();

    /// <summary>
    /// A reference to the delegate method used for processing the frames captured during the screen capture process.
    /// This delegate is passed to the underlying native implementation to handle frame data whenever new frames are available.
    /// It ensures that the managed callback remains alive for the lifetime of the capture session.
    /// </summary>
    private FrameCallback? _callbackRef;

    public string Name => "ScreenCaptureKit";
    public bool IsSupported => OperatingSystem.IsMacOSVersionAtLeast(12, 3);

    public void StartStream()
    {
        _callbackRef = OnFrame;
        neyrd_start_capture(_callbackRef);
    }

    public void StopStream() => neyrd_stop_capture();
    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public FrameData CaptureFrame() { lock (_lock) return _latest; }

    private void OnFrame(IntPtr data, int width, int height)
    {
        var bytes = new byte[width * height * 4]; // BGRA
        Marshal.Copy(data, bytes, 0, bytes.Length);
        
        lock (_lock)
        {
            _latest = new FrameData((uint)width, (uint)height, bytes);
        }
    }
}