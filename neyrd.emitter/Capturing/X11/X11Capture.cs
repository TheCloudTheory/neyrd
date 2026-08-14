using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Capturing.X11;

internal sealed partial class X11Capture : ICaptureAdapter
{
    [LibraryImport("libX11.so.6", StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr XOpenDisplay(string? display);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XCloseDisplay(IntPtr display);

    [LibraryImport("libXext.so.6")]
    private static partial IntPtr XShmCreateImage(IntPtr display, IntPtr visual,
        uint depth, int format, IntPtr data, ref XShmSegmentInfo shminfo,
        uint width, uint height);

    [LibraryImport("libXext.so.6")]
    private static partial int XShmAttach(IntPtr display, ref XShmSegmentInfo shminfo);

    [LibraryImport("libXext.so.6")]
    private static partial int XShmDetach(IntPtr display, ref XShmSegmentInfo shminfo);

    [LibraryImport("libXext.so.6")]
    private static partial int XShmGetImage(IntPtr display, IntPtr drawable,
        IntPtr image, int x, int y, ulong plane_mask);

    [LibraryImport("libX11.so.6")]
    private static partial int XDestroyImage(IntPtr image);

    [LibraryImport("libc.so.6")]
    private static partial int shmget(int key, nint size, int shmflg);

    [LibraryImport("libc.so.6")]
    private static partial IntPtr shmat(int shmid, IntPtr shmaddr, int shmflg);

    [LibraryImport("libc.so.6")]
    private static partial int shmdt(IntPtr shmaddr);

    [LibraryImport("libc.so.6")]
    private static partial int shmctl(int shmid, int cmd, IntPtr buf);
    
    [LibraryImport("libX11.so.6")]
    private static partial IntPtr XDefaultRootWindow(IntPtr display);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XDisplayWidth(IntPtr display, int screen);

    [LibraryImport("libX11.so.6")]
    private static partial int XDisplayHeight(IntPtr display, int screen);

    [LibraryImport("libX11.so.6")]
    private static partial int XDefaultScreen(IntPtr display);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XDefaultDepth(IntPtr display, int screen);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XSync(IntPtr display, int discard);
    
    [StructLayout(LayoutKind.Sequential)]
    internal struct XShmSegmentInfo
    {
        public ulong shmseg;   // ShmSeg (X resource ID)
        public int   shmid;
        public IntPtr shmaddr;
        public int   readOnly;
    }

    private const int IPC_PRIVATE = 0;
    private const int IPC_RMID    = 0;
    private const int IPC_CREAT   = 0x200;
    private const int ZPixmap     = 2;

    public string Name => "X11";
    public bool IsSupported => IsX11Available();
    
    public FrameData CaptureFrame()
    {
        var dpy  = XOpenDisplay(null);
        var root = XDefaultRootWindow(dpy);

        var screen = XDefaultScreen(dpy);
        var w = XDisplayWidth(dpy, screen);
        var h = XDisplayHeight(dpy, screen);
        var size = w * h * 4;

        var shminfo = new XShmSegmentInfo
        {
            shmid = shmget(IPC_PRIVATE, size, IPC_CREAT | 0x1FF)
        };
        
        if (shminfo.shmid == -1) throw new InvalidOperationException("shmget failed");

        shminfo.shmaddr = shmat(shminfo.shmid, IntPtr.Zero, 0);
        if (shminfo.shmaddr == new IntPtr(-1)) throw new InvalidOperationException("shmat failed");
        
        var depth = (uint)XDefaultDepth(dpy, screen);
        var image = XShmCreateImage(dpy, IntPtr.Zero, depth, ZPixmap,
        shminfo.shmaddr, ref shminfo, (uint)w, (uint)h);  // pass shmaddr directly

        if (image == IntPtr.Zero) throw new InvalidOperationException("XShmCreateImage failed");

        shminfo.readOnly = 0;
        XShmAttach(dpy, ref shminfo);
        XSync(dpy, 0);
        _ = XShmGetImage(dpy, root, image, 0, 0, ~0UL);

        var pixels = new byte[size];
        Marshal.Copy(shminfo.shmaddr, pixels, 0, size);

        // Cleanup
        XShmDetach(dpy, ref shminfo);
        _ = XDestroyImage(image);
        _ = shmdt(shminfo.shmaddr);
        _ = shmctl(shminfo.shmid, IPC_RMID, IntPtr.Zero);
        _ = XCloseDisplay(dpy);

        return new FrameData(w, h, pixels);
    }

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