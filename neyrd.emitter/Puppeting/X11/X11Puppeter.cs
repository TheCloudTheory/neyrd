using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Puppeting.X11;

internal sealed class X11Puppeter : IPuppeter
{
    [DllImport("libX11")]
    static extern int XWarpPointer(IntPtr display, ulong srcW, ulong destW,
        int srcX, int srcY, uint srcWidth, uint srcHeight, int destX, int destY);

    [DllImport("libX11")]
    static extern IntPtr XOpenDisplay(string? display);
    
    [DllImport("libX11")]
    static extern IntPtr XCloseDisplay(IntPtr display);
    
    [DllImport("libX11")]
    static extern ulong XDefaultRootWindow(IntPtr display);

    [DllImport("libX11")]
    static extern int XFlush(IntPtr display);

    public string Name => "X11";
    
    public bool IsSupported => IsX11Supported();
    
    public void MovePointer(double x, double y)
    {
        var display = XOpenDisplay(null);
        
        var root = XDefaultRootWindow(display);
        _ = XWarpPointer(display, 0, root, 0, 0, 0, 0, (int)x, (int)y);
        _ = XFlush(display);
    }

    private bool IsX11Supported()
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