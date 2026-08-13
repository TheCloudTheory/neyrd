using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using neyrd.core;

namespace neyrd.receiver;

public partial class MainWindow : Window
{
    private WriteableBitmap? FrameBitmap { get; set; }

    public MainWindow()
    {
        InitializeComponent();
    }

    public void UpdateFrame(byte[] bgra, int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            NeyrdLogger.Log("Invalid frame size");
            return;
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            // scaling > 1 on HiDPI/Retina; bitmap must reflect physical pixel density
            var scaling = RenderScaling;
            var dpi = 96 * scaling;

            if (FrameBitmap == null || FrameBitmap.PixelSize.Width != width || FrameBitmap.PixelSize.Height != height)
            {
                FrameBitmap?.Dispose();
                FrameBitmap = new WriteableBitmap(
                    new PixelSize(width, height),
                    new Vector(dpi, dpi),
                    PixelFormat.Bgra8888,
                    AlphaFormat.Opaque);
                ScreenImage.Source = FrameBitmap;
            }

            using var fb = FrameBitmap.Lock();
            Marshal.Copy(bgra, 0, fb.Address, bgra.Length);

            ScreenImage.InvalidateVisual();
        });
    }
}