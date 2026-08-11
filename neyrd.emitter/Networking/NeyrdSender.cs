using System.Net;
using System.Net.Sockets;
using neyrd.core;

namespace neyrd.emitter.Networking;

internal sealed class NeyrdSender(string emitterIpAddress)
{
    private readonly Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);

    public async Task<ConnectionTestResult> TestConnectionAsync()
    {
        try
        {
            await _socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), NeyrdConfiguration.DefaultListeningPort);
            
            await SendHandshakeMessageAsync();
            
            for(var i = 0; i < 10; i++)
            {
                await SendTestMessageAsync();
            }
        }
        catch (InvalidOperationException ex)
        {
            return new ConnectionTestResult
            {
                ErrorMessage =
                    "The connection could not be established. Listener is already listening to another emitter.",
                Exception = ex
            };
        }
        catch (SocketException ex)
        {
            return new ConnectionTestResult
            {
                ErrorMessage = "The connection could not be established.",
                Exception = ex
            };
        }
        
        return new ConnectionTestResult
        {
            IsSuccessful = true
        };
    }

    private async Task SendHandshakeMessageAsync()
    {
        var buffer = new ArraySegment<byte>([.. MessageFactory.CreateHandshakeMessage(MessageType.Handshake, $"eip:{emitterIpAddress}")]);
        _ = await _socket.SendAsync(buffer);
    }

    private async Task SendTestMessageAsync()
    {
        var buffer = new ArraySegment<byte>([.. MessageFactory.CreateHandshakeMessage(MessageType.Test, "neyrd test message==")]);
        _ = await _socket.SendAsync(buffer);
    }

    internal sealed class ConnectionTestResult
    {
        public bool IsSuccessful { get; init; }
        public string? ErrorMessage { get; init; }
        public Exception? Exception { get; init; }
    }
}