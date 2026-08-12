using System.Text;

namespace neyrd.core.Messages;

public static class MessageFactory
{
    public static byte[] CreateMessageMessage(MessageType type, string message)
    {
        return Encoding.UTF8.GetBytes($"{DateTimeOffset.Now.Ticks}|{(int)type}|{message}==");
    }
    
    public static string Encode(string[] parameters, MessageType type)
    {
        return $"{DateTimeOffset.Now.Ticks}:{(int)type}:{string.Join(":", parameters)}==";
    }
}