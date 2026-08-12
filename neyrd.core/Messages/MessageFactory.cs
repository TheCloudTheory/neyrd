namespace neyrd.core.Messages;

public static class MessageFactory
{
    public static string Encode(string[] parameters, MessageType type)
    {
        return $"{DateTimeOffset.Now.Ticks}:{(int)type}:{string.Join(":", parameters)}==";
    }
    
    public static string[] Decode(string message)
    {
        return message.Split(':');
    }
}