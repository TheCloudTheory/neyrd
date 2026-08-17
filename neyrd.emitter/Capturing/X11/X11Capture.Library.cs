using System.Runtime.InteropServices;

namespace neyrd.emitter.Capturing.X11;

internal sealed partial class X11Capture
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
    private static partial int XDefaultScreen(IntPtr display);

    [LibraryImport("libX11.so.6")]
    private static partial int XDefaultDepth(IntPtr display, int screen);

    [LibraryImport("libX11.so.6")]
    private static partial int XSync(IntPtr display, int discard);
    
    [LibraryImport("libX11.so.6")]
    private static partial int XDisplayHeight(IntPtr display, int screen);

    [StructLayout(LayoutKind.Sequential)]
    internal struct XShmSegmentInfo
    {
        public ulong shmseg; // ShmSeg (X resource ID)
        public int shmid;
        public IntPtr shmaddr;
        public int readOnly;
    }

    private const int IPC_PRIVATE = 0;
    private const int IPC_RMID = 0;
    private const int IPC_CREAT = 0x200;
    private const int ZPixmap = 2;
}