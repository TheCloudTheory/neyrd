using System.Runtime.InteropServices;
using neyrd.core;

namespace neyrd.emitter.Capturing.X11;

internal sealed partial class X11Capture : ICaptureAdapter
{
    public string Name => "X11";
    public bool IsSupported => IsX11Available();

    private IntPtr _display;
    private int _screen;
    private uint _size;
    private byte[]? _pixels;
    private uint _width;
    private uint _height;

    public FrameData CaptureFrame()
    {
        var root = XDefaultRootWindow(_display);
        
        var shmInfo = new XShmSegmentInfo
        {
            shmid = shmget(IPC_PRIVATE, (int)_size, IPC_CREAT | 0x1FF)
        };

        if (shmInfo.shmid == -1)
        {
            ReleaseResources(_display, shmInfo, null);
            throw new InvalidOperationException("shmget failed");
        }

        shmInfo.shmaddr = shmat(shmInfo.shmid, IntPtr.Zero, 0);
        if (shmInfo.shmaddr == new IntPtr(-1))
        {
            ReleaseResources(_display, shmInfo, null);
            throw new InvalidOperationException("shmat failed");
        }

        var depth = (uint)XDefaultDepth(_display, _screen);
        var image = XShmCreateImage(_display, IntPtr.Zero, depth, ZPixmap,
            shmInfo.shmaddr, ref shmInfo, (uint)_width, (uint)_height);

        if (image == IntPtr.Zero)
        {
            ReleaseResources(_display, shmInfo, image);
            throw new InvalidOperationException("XShmCreateImage failed");
        }

        shmInfo.readOnly = 0;
        XShmAttach(_display, ref shmInfo);
        _ = XSync(_display, 0);
        _ = XShmGetImage(_display, root, image, 0, 0, ~0UL);
        
        Marshal.Copy(shmInfo.shmaddr, _pixels!, 0, (int)_size);

        ReleaseResources(_display, shmInfo, image);

        return new FrameData(_width, _height, _pixels!);
    }

    public void Initialize()
    {
        _display = XOpenDisplay(null);
        _screen = XDefaultScreen(_display);
        _width = (uint)XDisplayWidth(_display, _screen);
        _height = (uint)XDisplayHeight(_display, _screen);
        _size = _width * _height * 4;
        _pixels = new byte[_size];
    }

    /// <summary>
    /// Releases the resources associated with the X11 shared memory segment, image, and display.
    /// </summary>
    /// <param name="dpy">The pointer to the X11 display connection.</param>
    /// <param name="shmInfo">The X11 shared memory segment information structure.</param>
    /// <param name="image">The pointer to the X11 image resource to be destroyed.</param>
    private static void ReleaseResources(IntPtr dpy, XShmSegmentInfo shmInfo, IntPtr? image)
    {
        if (shmInfo.shmaddr != IntPtr.Zero && shmInfo.shmaddr != new IntPtr(-1))
        {
            XShmDetach(dpy, ref shmInfo);
        }
        
        if (image != null)
        {
            _ = XDestroyImage(image.Value);
        }

        if (shmInfo.shmaddr != IntPtr.Zero && shmInfo.shmaddr != new IntPtr(-1))
        {
            _ = shmdt(shmInfo.shmaddr);
        }

        if (shmInfo.shmid != -1)
        {
            _ = shmctl(shmInfo.shmid, IPC_RMID, IntPtr.Zero);
        }
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

    public void Dispose()
    {
        _ = XCloseDisplay(_display);
    }
}