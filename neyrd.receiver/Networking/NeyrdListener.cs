using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace neyrd.receiver.Networking;

internal sealed class NeyrdListener
{
    /// <summary>
    /// Specifies the maximum number of pending connections that can be queued
    /// on the socket listening for incoming connection requests.
    /// </summary>
    private const int Backlog = 8;

    /// <summary>
    /// Defines the default port number on which the server will listen for incoming connections.
    /// </summary>
    private const int DefaultListeningPort = 22222;

    private readonly Socket _socket = new(AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp);

    public async Task BeginListeningAsync(CancellationToken ct = default)
    {
        var ep = new IPEndPoint(IPAddress.Loopback, DefaultListeningPort);
        _socket.Bind(ep);
        _socket.Listen(Backlog);

        while (!ct.IsCancellationRequested)
        {
            var client = await _socket.AcceptAsync(ct);
            _ = Task.Run(() => HandleEmitted(client, ct), ct);
        }
    }

    private async Task HandleEmitted(Socket client, CancellationToken ct)
    {
        var buffer = new ArraySegment<byte>(new byte[4096]);
        var result = await client.ReceiveAsync(buffer, ct);
        if (result == 0 || buffer.Array == null)
        {
            return;
        }
        
        var now = DateTimeOffset.Now;
        var decoded = Encoding.UTF8.GetString(buffer.Array!, 0, result);
        var messages = decoded.Split("==");
        foreach (var message in messages)
        {
            var segments = message.Split('|');
            var timestamp = long.Parse(segments[0]);
            var type = segments[1];
            var diff = now.Ticks - timestamp;

            if (type == "0")
            {
                NeyrdLogger.Log($"Received message with timestamp {timestamp} and diff {diff}");
            }
        }
    }
}