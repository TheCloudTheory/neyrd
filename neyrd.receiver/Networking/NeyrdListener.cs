using System.Net;
using System.Net.Sockets;

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

    public void BeginListening()
    {
        var ep = new IPEndPoint(IPAddress.Loopback, DefaultListeningPort);
        
        _socket.Bind(ep);
        _socket.Listen(Backlog);
    }
}