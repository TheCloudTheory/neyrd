using System.Net;
using System.Net.Sockets;
using System.Text;
using neyrd.core;

namespace neyrd.emitter.Networking;

internal sealed class NeyrdListener(string emitterIpAddress)
{
    /// <summary>
    /// Specifies the maximum number of pending connections that can be queued
    /// on the socket listening for incoming connection requests.
    /// </summary>
    private const int Backlog = 1;

    private readonly Socket _socket = new(AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp);

    public async Task BeginListeningAsync(CancellationToken ct = default)
    {
        var ep = new IPEndPoint(IPAddress.Parse(emitterIpAddress), NeyrdConfiguration.DefaultListeningPort);
        _socket.Bind(ep);
        _socket.Listen(Backlog);

        while (!ct.IsCancellationRequested)
        {
            var client = await _socket.AcceptAsync(ct);
            _ = Task.Run(() => HandleReceived(client, ct), ct);
        }
    }
    
    private async Task HandleReceived(Socket client, CancellationToken ct)
    {
        var buffer = new ArraySegment<byte>(new byte[4096]);
        var result = await client.ReceiveAsync(buffer, ct);
        if (result == 0 || buffer.Array == null)
        {
            return;
        }

        var decoded = Encoding.UTF8.GetString(buffer.Array!, 0, result);
        var messages = decoded.Split("==");
        foreach (var message in messages)
        {
            var segments = message.Split('|');
            var type = segments[1];

            if (MessageTypeComparer.IsEqual(type, MessageType.Acknowledgement))
            {
                NeyrdLogger.Log($"Acknowledged: {message}");
            }
        }
    }
}