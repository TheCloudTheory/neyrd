using System.Net;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using neyrd.core;
using neyrd.core.Benchmark.Handlers;
using neyrd.core.Events;
using neyrd.receiver.Handlers;
using neyrd.receiver.Networking;

namespace neyrd.receiver;

public class App : Application
{
    private readonly CancellationTokenSource _cts = new();
    private readonly NeyrdSender _sender = new();
    
    private NeyrdListener? _neyrdListener;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(_sender);
            desktop.Exit += (_, _) => _cts.Cancel();
        }

        base.OnFrameworkInitializationCompleted();
        
        InitializeEventHandlers();
        InitializeReceiver();
    }

    private void InitializeEventHandlers()
    {
        var app = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        if (app.MainWindow is not MainWindow window)
        {
            return;
        }
        
        EventPipeline.Subscribe(new HandshakeReceivedEventHandler(_sender));
        EventPipeline.Subscribe(new TestStartedEventHandler());
        EventPipeline.Subscribe(new TestReceivedHandler());
        EventPipeline.Subscribe(new TestCompletedHandler());
        EventPipeline.Subscribe(new FrameReceivedEventHandler(window));
        EventPipeline.Subscribe(new SynchronizationRequestedEventHandler(window));
    }

    private void InitializeReceiver()
    {
        _neyrdListener = new NeyrdListener(IPAddress.Any);
        _ = _neyrdListener.BeginListeningAsync(_cts.Token);
    }
}