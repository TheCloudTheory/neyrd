using System.Runtime.InteropServices;

namespace neyrd.emitter.Puppeting.X11;

internal sealed partial class X11Puppeter
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
    
    [LibraryImport("libX11")]
    private static partial ulong XStringToKeysym([MarshalAs(UnmanagedType.LPStr)] string name);

    [LibraryImport("libX11")]
    private static partial uint XKeysymToKeycode(IntPtr display, ulong keysym);

    [LibraryImport("libXtst.so.6")]
    private static partial int XTestFakeKeyEvent(IntPtr display, uint keycode, [MarshalAs(UnmanagedType.Bool)] bool isPress, ulong delay);
}