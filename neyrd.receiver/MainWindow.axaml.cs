using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using neyrd.core;
using neyrd.core.Environment;
using neyrd.core.Models;
using neyrd.core.Models.Messages;
using neyrd.receiver.Networking;

namespace neyrd.receiver;

public partial class MainWindow : Window
{
    private readonly NeyrdSender _sender;

    private long _pointerMovedControlTimestamp;
    private WriteableBitmap? FrameBitmap { get; set; }
    
    public MainWindow(NeyrdSender sender)
    {
        _sender = sender;
        
        InitializeComponent();
        
        var ips = NetworkInfoCollector.NetworkInterfaces;
        IpLabel.Text = ips.Length > 0
            ? $"IP: {string.Join(", ", ips)}"
            : "IP: unavailable";
        
        LatencyLabel.Text = $"Latency: N/A";
        VersionLabel.Text = $"Version: {ThisAssembly.AssemblyInformationalVersion}";

        PointerMoved += OnPointerMoved;
        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerWheelChanged += OnPointerWheelChanged;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Conceptually, there's no reason to fire this event more than 30 times
        // per second - this is why we throttle it here. We deliberately skip thread
        // safety here as this is supposed to be triggered by a single UI thread
        var now = DateTimeOffset.Now.Ticks;
        const double threshold = 1000d / 30;
        if (now - _pointerMovedControlTimestamp < threshold) return;
        
        // The desired position needs to be downscaled / upscaled depending
        // on the size of the window and the actual emitted resolution
        var position = e.GetPosition(ScreenImage);
        var mousePosition = GetMousePosition(position);
        
        _ = _sender.Send(PointerMovedMessage.ToMessage(mousePosition.x, mousePosition.y));
        
        _pointerMovedControlTimestamp = now;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        
        var position = e.GetPosition(ScreenImage);
        var mousePosition = GetMousePosition(position);
        
        var button = e.Properties.ToMouseButton();
        
        _ = _sender.Send(PointerPressedMessage.ToMessage(mousePosition.x, mousePosition.y, button));
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        
        var button = MouseButtonExtensions.ToButton((int)e.InitialPressMouseButton);
        
        _ = _sender.Send(PointerReleasedMessage.ToMessage(button));
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _ = _sender.Send(PointerWheelChangedMessage.ToMessage(e.Delta.Length, e.Delta.X, e.Delta.Y));
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        
        _ = _sender.Send(KeyDownMessage.ToMessage(e.Key.ToString(), (int)e.KeyModifiers));
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        
        _ = _sender.Send(KeyUpMessage.ToMessage(e.Key.ToString(), (int)e.KeyModifiers));
    }

    private (double x, double y) GetMousePosition(Point position)
    {
        var scaleX = EmittedScreenWidth / ScreenImage.Bounds.Width;
        var scaleY = EmittedScreenHeight / ScreenImage.Bounds.Height;
        
        return (position.X * scaleX, position.Y * scaleY);
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

    public void SetEmittersScreenResolution(int width, int height)
    {
        EmittedScreenWidth = width;
        EmittedScreenHeight = height;
    }

    public int EmittedScreenHeight { get; set; }
    public int EmittedScreenWidth { get; set; }
}