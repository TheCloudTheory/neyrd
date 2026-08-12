using System.Net;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using neyrd.core;
using neyrd.core.Events;
using neyrd.core.Models.Events;
using neyrd.receiver.Handlers;
using neyrd.receiver.Networking;

namespace neyrd.receiver;

public partial class App : Application
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
            desktop.MainWindow = new MainWindow();
            desktop.Exit += (_, _) => _cts.Cancel();
        }

        base.OnFrameworkInitializationCompleted();
        
        InitializeEventHandlers();
        InitializeReceiver();
    }

    private void InitializeEventHandlers()
    {
        EventPipeline.Subscribe(new HandshakeReceivedEventHandler(_sender));
    }

    private void InitializeReceiver()
    {
        _neyrdListener = new NeyrdListener(IPAddress.Loopback.ToString());
        _ = _neyrdListener.BeginListeningAsync(_cts.Token);
    }
}