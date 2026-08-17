using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Capturing.CoreGraphics;

internal sealed partial class CoreGraphicsCapture : ICaptureAdapter
{
    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGMainDisplayID();

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGDisplayCreateImage(IntPtr display);
    
    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGImageGetDataProvider(IntPtr image);

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGDataProviderCopyData(IntPtr provider);

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial nint CGImageGetWidth(IntPtr image);

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial nint CGImageGetHeight(IntPtr image);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static partial IntPtr CFDataGetBytePtr(IntPtr cfData);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static partial nint CFDataGetLength(IntPtr cfData);

    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static partial void CFRelease(IntPtr cf);
    
    public string Name => "CoreGraphics";
    public bool IsSupported => IsCoreGraphicsAvailable();
    
    public FrameData CaptureFrame()
    {
        var display = CGMainDisplayID();
        var image = CGDisplayCreateImage(display);
        
        var width = (uint)CGImageGetWidth(image);
        var height = (uint)CGImageGetHeight(image);

        var provider = CGImageGetDataProvider(image);
        var cfData = CGDataProviderCopyData(provider);

        var ptr = CFDataGetBytePtr(cfData);
        var length = (int)CFDataGetLength(cfData);

        // copy before releasing — format is BGRA, width*height*4 bytes
        var bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);

        CFRelease(cfData);
        CFRelease(image);

        return new FrameData(width, height, bytes);
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

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

    public void Dispose()
    {
    }
}