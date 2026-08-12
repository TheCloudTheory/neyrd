using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using neyrd.core;
using neyrd.core.Messages;

namespace neyrd.receiver.Networking;

internal sealed class NeyrdSender : IDisposable
{
    private readonly Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);

    private bool _isConnected;
    
    public void Connect(IPAddress emitterIpAddress)
    {
        if (_isConnected)
        {
            return;
        }

        _socket.Connect(emitterIpAddress, NeyrdConfiguration.DefaultListeningPort);
        _isConnected = true;
    }

    public async Task Send(IMessage message)
    {
        _ = await _socket.SendAsync(message.Payload);
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}