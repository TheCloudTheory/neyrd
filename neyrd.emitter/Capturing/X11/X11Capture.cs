using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Capturing.X11;

internal sealed partial class X11Capture : ICaptureAdapter
{
    [LibraryImport("libX11.so.6", StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr XOpenDisplay(string? display);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XCloseDisplay(IntPtr display);

    public string Name => "X11";
    public bool IsSupported => IsX11Available();

    private static bool IsX11Available()
    {
        try
        {
            var dpy = XOpenDisplay(null);
            if (dpy == IntPtr.Zero) return false;
            _ = XCloseDisplay(dpy);
            return true;
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error loading libX11: {ex.Message}");
            return false;
        }
    }
}