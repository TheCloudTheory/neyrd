using System.Net;
using System.Net.Sockets;
using System.Text;

namespace neyrd.emitter;

internal sealed class ConnectionManager
{
    /// <summary>
    /// Defines the default port number on which the server will listen for incoming connections.
    /// </summary>
    private const int DefaultReceiverPort = 22222;
    
    private readonly Socket _socket = new(SocketType.Stream,
        ProtocolType.Tcp);

    public async Task<ConnectionTestResult> TestConnectionAsync()
    {
        try
        {
            await _socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), DefaultReceiverPort);
            
            var buffer = new ArraySegment<byte>([.. Encoding.UTF8.GetBytes($"{DateTimeOffset.Now.Ticks}|neyrd test message")]);
            await _socket.SendAsync(buffer);
        }
        catch (InvalidOperationException ex)
        {
            return new ConnectionTestResult()
            {
                ErrorMessage =
                    "The connection could not be established. Listener is already listening to another emitter.",
                Exception = ex
            };
        }
        catch (SocketException ex)
        {
            return new ConnectionTestResult()
            {
                ErrorMessage = "The connection could not be established.",
                Exception = ex
            };
        }
        
        return new ConnectionTestResult()
        {
            IsSuccessful = true
        };
    }

    internal sealed class ConnectionTestResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
    }
}