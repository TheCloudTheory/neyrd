using Avalonia.Controls;
using neyrd.receiver.Networking;

namespace neyrd.receiver;

public partial class MainWindow : Window
{
    private NeyrdListener? _neyrdListener;
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeReceiver();
    }

    private void InitializeReceiver()
    {
        _neyrdListener = new NeyrdListener();
        _neyrdListener.BeginListening();
    }
}