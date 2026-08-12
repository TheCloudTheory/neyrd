using System.Net;
using System.Net.Sockets;
using System.Text;
using neyrd.core.Events;
using neyrd.core.Messages;
using neyrd.core.Models.Events;

namespace neyrd.core;

public sealed class NeyrdListener(string ip)
{
    /// <summary>
    /// Specifies the maximum number of pending connections that can be queued
    /// on the socket listening for incoming connection requests.
    /// </summary>
    private const int Backlog = 8;

    private readonly Socket _socket = new(AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp);

    public async Task BeginListeningAsync(CancellationToken ct = default)
    {
        var ep = new IPEndPoint(IPAddress.Parse(ip), NeyrdConfiguration.DefaultListeningPort);
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
        int result;
        while ((result = await client.ReceiveAsync(buffer, ct)) > 0)
        {
            if (buffer.Array == null) break;
            
            var decoded = Encoding.UTF8.GetString(buffer.Array!, 0, result);
            var messages = decoded.Split("==").Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
        
            NeyrdLogger.Log($"Received {messages.Length} messages");
        
            foreach (var message in messages)
            {
                NeyrdLogger.Log($"Processing message: {message}");
            
                var segments = message.Split(':');
                var type = segments[1];
                
                if (MessageTypeComparer.IsEqual(type, MessageType.TestStarted))
                {
                    NeyrdLogger.Log($"Test started: {message}");
                    EventPipeline.Publish<TestStartedEvent, long>(TestStartedEvent.From(MessageEnvelope.From(message)));
                }

                if (MessageTypeComparer.IsEqual(type, MessageType.Test))
                {
                    NeyrdLogger.Log($"Test: {message}");
                }
                
                if (MessageTypeComparer.IsEqual(type, MessageType.TestCompleted))
                {
                    NeyrdLogger.Log($"Test completed: {message}");
                }
            
                if(MessageTypeComparer.IsEqual(type, MessageType.Handshake))
                {
                    NeyrdLogger.Log($"Handshake: {message}");
                    EventPipeline.Publish<HandshakeReceivedEvent, IPAddress>(HandshakeReceivedEvent.From(MessageEnvelope.From(message)));
                }
            }
        }
    }
}