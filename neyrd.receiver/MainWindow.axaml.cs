using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using neyrd.core;
using neyrd.core.Environment;

namespace neyrd.receiver;

public partial class MainWindow : Window
{
    private WriteableBitmap? FrameBitmap { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        
        var ips = NetworkInfoCollector.NetworkInterfaces;
        IpLabel.Text = ips.Length > 0
            ? $"IP: {string.Join(", ", ips)}"
            : "IP: unavailable";
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
            Placeholder.IsVisible = false;
            
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

            // Force alpha=255 to avoid issue when the emitted frame would have alpha channel
            // to be set to 0 -> causing Avalonia to incorrectly interpret it if <Image>
            // is inside a <Grid> or similar component
            for (var i = 3; i < bgra.Length; i += 4)
            {
                bgra[i] = 255;
            }
            
            using var fb = FrameBitmap.Lock();
            Marshal.Copy(bgra, 0, fb.Address, bgra.Length);
            ScreenImage.InvalidateVisual();
        });
    }

    public void UpdateStats(long decodedTimestamp)
    {
        var now = DateTimeOffset.Now.Ticks;
        var latencyMs = (now - decodedTimestamp + Offset) / 10000;
        
        Dispatcher.UIThread.Post(() => LatencyLabel.Text = $"Latency: {latencyMs} ms");
    }

    public void SetClockOffset(long offset)
    {
        Offset = offset;
    }

    private long Offset { get; set; }
}