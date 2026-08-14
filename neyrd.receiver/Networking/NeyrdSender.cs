using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using neyrd.core;
using neyrd.core.Messages;

namespace neyrd.receiver.Networking;

public sealed class NeyrdSender : IDisposable
{
    private readonly Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);
    
    private readonly Channel<IMessage> _channel = Channel.CreateUnbounded<IMessage>();

    private bool _isConnected;
    private Task? _consumer;

    public void Connect(IPAddress emitterIpAddress)
    {
        if (_isConnected)
        {
            return;
        }

        _socket.Connect(emitterIpAddress, NeyrdConfiguration.DefaultListeningPort);
        _isConnected = true;
        _consumer = ConsumeAsync();
    }

    public async Task Send(IMessage message)
    {
        try
        {
            if (!_isConnected)
            {
                throw new InvalidOperationException();    
            }
            
            await _channel.Writer.WriteAsync(message);
        }
        catch (Exception ex)
        {
            NeyrdLogger.Log($"Error sending message: {ex.Message}");
        }
    }
    
    private async Task ConsumeAsync()
    {
        await foreach (var message in _channel.Reader.ReadAllAsync())
        {
            try
            {
                _ = await _socket.SendAsync(message.Payload);
            }
            catch (Exception ex)
            {
                NeyrdLogger.Log($"Error sending message: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}