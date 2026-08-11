using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using neyrd.receiver.Networking;

namespace neyrd.receiver;

public partial class App : Application
{
    private readonly CancellationTokenSource _cts = new();
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
        InitializeReceiver();
    }
    
    private void InitializeReceiver()
    {
        _neyrdListener = new NeyrdListener();
        _ = _neyrdListener.BeginListeningAsync(_cts.Token);
    }
}