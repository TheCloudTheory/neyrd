using System.Text;

namespace neyrd.core;

public sealed class MessageFactory
{
    public static byte[] CreateHandshakeMessage(MessageType type, string message)
    {
        return Encoding.UTF8.GetBytes($"{DateTimeOffset.Now.Ticks}|{(int)type}|{message}==");
    }
}