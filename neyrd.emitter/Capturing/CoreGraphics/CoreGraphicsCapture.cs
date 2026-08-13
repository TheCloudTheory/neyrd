using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Capturing.CoreGraphics;

internal sealed partial class CoreGraphicsCapture : ICaptureAdapter
{
    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGMainDisplayID();

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGDisplayCreateImage(IntPtr display);
    
    public string Name => "CoreGraphics";
    public bool IsSupported => IsCoreGraphicsAvailable();

    private bool IsCoreGraphicsAvailable()
    {
        try
        {
            var display = CGMainDisplayID();
            return display != IntPtr.Zero;
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error loading CoreGraphics: {ex.Message}");
            return false;
        }
    }
}