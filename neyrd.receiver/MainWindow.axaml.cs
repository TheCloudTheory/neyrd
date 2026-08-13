using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

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
        FrameBitmap ??= new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);

        using var fb = FrameBitmap.Lock();
        Marshal.Copy(bgra, 0, fb.Address, bgra.Length);

        Dispatcher.UIThread.Post(() => ScreenImage.Source = FrameBitmap);
    }
}