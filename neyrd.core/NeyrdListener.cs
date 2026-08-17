using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using neyrd.core.Events;
using neyrd.core.Messages;
using neyrd.core.Models;
using neyrd.core.Models.Events;

namespace neyrd.core;

public sealed class NeyrdListener(IPAddress ipAddress)
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
        var ep = new IPEndPoint(ipAddress, NeyrdConfiguration.DefaultListeningPort);
        _socket.Bind(ep);
        _socket.Listen(Backlog);

        Socket? current = null;
        while (!ct.IsCancellationRequested)
        {
            var client = await _socket.AcceptAsync(ct);
            current?.Dispose(); // close the previous connection before handling the new one
            current = client;
            _ = Task.Run(() => HandleEmitted(client, ct), ct);
        }
    }

    private async Task HandleEmitted(Socket client, CancellationToken ct)
    {
        var receiveBuffer = new byte[4096];
        var reassembly = new List<byte>();

        int bytesRead;
        while ((bytesRead = await client.ReceiveAsync(receiveBuffer, ct)) > 0)
        {
            reassembly.AddRange(receiveBuffer.AsSpan(0, bytesRead));

            while (reassembly.Count >= 5) // need at least length (4) + marker (1)
            {
                if (reassembly[4] == (byte)MessageType.Frame)
                {
                    if (reassembly.Count < 4) break;
                    var messageLength = BinaryPrimitives.ReadInt32LittleEndian(
                        CollectionsMarshal.AsSpan(reassembly));
                    if (reassembly.Count < messageLength) break;

                    var message = reassembly.GetRange(0, messageLength).ToArray();
                    reassembly.RemoveRange(0, messageLength);
                    EventPipeline.Publish<FrameReceivedEvent, byte[]>(FrameReceivedEvent.From(message));
                }
                else
                {
                    var asText = Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(reassembly));
                    var delimIdx = asText.IndexOf("==", StringComparison.Ordinal);
                    if (delimIdx < 0) break;

                    var message = asText[..delimIdx];
                    var consumed = Encoding.UTF8.GetByteCount(asText[..(delimIdx + 2)]);
                    reassembly.RemoveRange(0, consumed);
                    ProcessReceivedMessages(message);
                }
            }
        }
    }

    private static void ProcessReceivedMessages(string message)
    {
        try
        {
            var segments = message.Split(':');
            var type = segments[1];

            if (MessageTypeComparer.IsEqual(type, MessageType.TestStarted))
            {
                NeyrdLogger.Log($"Test started: {message}");
                EventPipeline.Publish<TestStartedEvent, long>(
                    TestStartedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.Test))
            {
                NeyrdLogger.Log($"Test: {message}");
                EventPipeline.Publish<TestReceivedEvent, long>(
                    TestReceivedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.TestCompleted))
            {
                NeyrdLogger.Log($"Test completed: {message}");
                EventPipeline.Publish<TestCompletedEvent, bool>(
                    TestCompletedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.Handshake))
            {
                NeyrdLogger.Log($"Handshake: {message}");
                EventPipeline.Publish<HandshakeReceivedEvent, IPAddress>(
                    HandshakeReceivedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.Acknowledgement))
            {
                NeyrdLogger.Log($"Acknowledgement: {message}");
                EventPipeline.Publish<AcknowledgementReceivedEvent, long>(
                    AcknowledgementReceivedEvent.From(MessageEnvelope.From(message)));
            }
            
            if (MessageTypeComparer.IsEqual(type, MessageType.Synchronization))
            {
                NeyrdLogger.Log($"Synchronization: {message}");
                EventPipeline.Publish<SynchronizationRequestedEvent, (long, long)>(
                    SynchronizationRequestedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.Pointer))
            {
                NeyrdLogger.Log($"Pointer: {message}");
                EventPipeline.Publish<PointerMovedEvent, (double, double)>(
                    PointerMovedEvent.From(MessageEnvelope.From(message)));
            }

            if (MessageTypeComparer.IsEqual(type, MessageType.Screen))
            {
                NeyrdLogger.Log($"Screen: {message}");
                EventPipeline.Publish<ScreenResolutionEstablishedEvent, (int, int)>(
                    ScreenResolutionEstablishedEvent.From(MessageEnvelope.From(message)));
            }
            
            if (MessageTypeComparer.IsEqual(type, MessageType.PointerPressed))
            {
                NeyrdLogger.Log($"Pointer pressed: {message}");
                EventPipeline.Publish<PointerPressedEvent, (double, double, MouseButton)>(
                    PointerPressedEvent.From(MessageEnvelope.From(message)));
            }
            
            if (MessageTypeComparer.IsEqual(type, MessageType.PointerWheel))
            {
                NeyrdLogger.Log($"Pointer wheel: {message}");
                EventPipeline.Publish<PointerWheelChangedEvent, (double, double, double)>(
                    PointerWheelChangedEvent.From(MessageEnvelope.From(message)));
            }
        }
        catch (Exception ex)
        {
            NeyrdLogger.Log($"Failed to process message '{message}': {ex.Message}");
        }
    }
}