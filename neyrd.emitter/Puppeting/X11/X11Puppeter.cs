using System.Runtime.InteropServices;
using neyrd.core;
using neyrd.core.Models;

namespace neyrd.emitter.Puppeting.X11;

internal sealed partial class X11Puppeter : IPuppeter
{
    [LibraryImport("libX11")]
    private static partial int XWarpPointer(IntPtr display, ulong srcW, ulong destW,
        int srcX, int srcY, uint srcWidth, uint srcHeight, int destX, int destY);

    [LibraryImport("libX11")]
    private static partial IntPtr XOpenDisplay([MarshalAs(UnmanagedType.LPStr)] string? display);
    
    [LibraryImport("libX11")]
    private static partial IntPtr XCloseDisplay(IntPtr display);
    
    [LibraryImport("libX11")]
    private static partial ulong XDefaultRootWindow(IntPtr display);

    [LibraryImport("libX11")]
    private static partial int XFlush(IntPtr display);
    
    [LibraryImport("libX11")]
    private static partial int XDisplayWidth(IntPtr display, int screen);

    [LibraryImport("libX11")]
    private static partial int XDisplayHeight(IntPtr display, int screen);
    
    [LibraryImport("libXtst.so.6")]
    private static partial int XTestFakeButtonEvent(IntPtr display, uint button, [MarshalAs(UnmanagedType.Bool)] bool isPress, ulong delay);
    
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

    public void HandleClick(double x, double y, MouseButton button)
    {
        MovePointer(x, y);
        
        try
        {
            NeyrdLogger.Log($"Display: {_display != IntPtr.Zero}");
            
            _ = XTestFakeButtonEvent(_display, (uint)button, true, 0);
            _ = XTestFakeButtonEvent(_display, (uint)button, false, 5);
            _ = XFlush(_display);
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error when handling pointer click: {ex.Message}");
        }
    }

    public void HandleWheel(double deltaLength, double deltaX, double deltaY)
    {
        // X11 maps scroll to button events: 4=up, 5=down, 6=left, 7=right
        var button = deltaY < 0 ? 4u : 5u;
        if (deltaX != 0) button = deltaX < 0 ? 6u : 7u;

        _ = XTestFakeButtonEvent(_display, button, true, 0);
        _ = XTestFakeButtonEvent(_display, button, false, 0);
        _ = XFlush(_display);
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