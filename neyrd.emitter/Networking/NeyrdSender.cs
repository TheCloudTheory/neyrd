using System.Net;
using System.Net.Sockets;
using neyrd.core;
using neyrd.core.Messages;
using neyrd.core.Models.Messages;

namespace neyrd.emitter.Networking;

internal sealed class NeyrdSender(IPAddress emitterIpAddress, IPAddress receiverIpAddress) : IDisposable
{
    private Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);

    /// <summary>
    /// Tests the connection to the listener by sending a series of messages and ensures the communication is successful.
    /// </summary>
    /// <param name="attempt">
    /// The current attempt number for establishing the connection. Defaults to 1.
    /// The method retries up to 10 times if the connection fails.
    /// </param>
    /// <returns>
    /// A <see cref="ConnectionTestResult"/> object encapsulating the results of the connection test.
    /// Contains information about the success or failure of the operation, including any error messages or exceptions.
    /// </returns>
    public async Task<ConnectionTestResult> TestConnectionAsync(int attempt = 1)
    {
        try
        {
            await _socket.ConnectAsync(receiverIpAddress, NeyrdConfiguration.DefaultListeningPort);

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
            if (attempt == 10)
            {
                return new ConnectionTestResult
                {
                    ErrorMessage = "The connection could not be established.",
                    Exception = ex
                };
            }

            attempt++;
            var nextDelay = 1000 * attempt;
            NeyrdLogger.Log($"Attempt {attempt}/10 to connect. Failed: {ex.Message}. Retrying after {nextDelay}ms.");

            await Task.Delay(nextDelay);
            await TestConnectionAsync(attempt);
        }

        return new ConnectionTestResult
        {
            IsSuccessful = true
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<bool> Send(IMessage message)
    {
        try
        {
            _ = await _socket.SendAsync(message.Payload);
            return true;
        }
        catch (SocketException ex)
        {
            NeyrdLogger.Log($"Failed to  send message: {ex.Message}. Error code: {ex.SocketErrorCode}");

            // If receiver has been closed / crashed, just gracefully stop, clear
            // the state and attempt reconnection
            if (ex.SocketErrorCode is SocketError.Shutdown or SocketError.ConnectionReset)
            {
                _socket.Dispose();
            }
            
            return false;
        }
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

    public async Task Reconnect(int width, int height)
    {
        try
        {
            _socket = new Socket(SocketType.Stream,
                ProtocolType.Tcp);
            await _socket.ConnectAsync(receiverIpAddress, NeyrdConfiguration.DefaultListeningPort);
            
            await Send(HandshakeMessage.ToMessage(emitterIpAddress));
            await Send(ScreenResolutionMessage.ToMessage(width, height));
        }
        catch (SocketException ex)
        {
            NeyrdLogger.Log($"Failed to connect to receiver. Error code: {ex.SocketErrorCode}");
        }
    }
}