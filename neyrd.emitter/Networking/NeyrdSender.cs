using System.Net;
using System.Net.Sockets;
using neyrd.core;
using neyrd.core.Messages;
using neyrd.core.Models.Messages;

namespace neyrd.emitter.Networking;

internal sealed class NeyrdSender(IPAddress emitterIpAddress) : IDisposable
{
    private readonly Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);

    public async Task<ConnectionTestResult> TestConnectionAsync()
    {
        try
        {
            await _socket.ConnectAsync(emitterIpAddress, NeyrdConfiguration.DefaultListeningPort);

            await Send(HandshakeMessage.ToMessage(emitterIpAddress));
            await Send(TestStartedMessage.ToMessage());

            for (var i = 0; i < 10; i++)
            {
                await Send(TestMessage.ToMessage());
            }
            
            await Send(TestCompletedMessage.ToMessage());
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

    public async Task Send(IMessage message)
    {
        _ = await _socket.SendAsync(message.Payload);
    }

    internal sealed class ConnectionTestResult
    {
        public bool IsSuccessful { get; init; }
        public string? ErrorMessage { get; init; }
        public Exception? Exception { get; init; }
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}