namespace neyrd.core.Messages;

public static class MessageOrigin
{
    public const string Emitter = "e";
    public static string Receiver => "r";
    
    public enum Kind
    {
        Emitter,
        Receiver
    }
}