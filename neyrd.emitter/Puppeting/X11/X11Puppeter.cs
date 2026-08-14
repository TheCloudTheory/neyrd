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
    
    [DllImport("libX11")]
    static extern int XDisplayWidth(IntPtr display, int screen);

    [DllImport("libX11")]
    static extern int XDisplayHeight(IntPtr display, int screen);
    
    [DllImport("libXtst")]
    static extern int XTestFakeButtonEvent(IntPtr display, uint button, bool isPress, ulong delay);
    
    private IntPtr _display;
    private ulong _root;

    public string Name => "X11";
    
    public bool IsSupported => IsX11Supported();
    
    public void Initialize()
    {
        _display = XOpenDisplay(null);
        _root = XDefaultRootWindow(_display);
    }
    
    public void MovePointer(double x, double y)
    {
        _ = XWarpPointer(_display, 0, _root, 0, 0, 0, 0, (int)x, (int)y);
        _ = XFlush(_display);
    }

    public void HandleClick(double x, double y)
    {
        MovePointer(x, y);
        
        try
        {
            NeyrdLogger.Log($"Display: {_display != IntPtr.Zero}");
            
            // Note 1 - left button, 2 - middle, 3 - right
            _ = XTestFakeButtonEvent(_display, 1, true, 0);
            _ = XTestFakeButtonEvent(_display, 1, false, 0);
            _ = XFlush(_display);
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error when handling pointer click: {ex.Message}");
        }
    }

    public (int width, int height) GetScreenSize()
    {
        var width = XDisplayWidth(_display, 0);
        var height = XDisplayHeight(_display, 0);
        
        return (width, height);
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
    
    public void Dispose() => XCloseDisplay(_display);
}