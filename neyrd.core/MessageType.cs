namespace neyrd.core;

public enum MessageType
{
    /// <summary>
    /// Represents a test message type used for internal operations or validations.
    /// </summary>
    Test
}

public sealed class MessageTypeComparer
{
    public static bool IsEqual(string rawType, MessageType type)
    {
        return int.Parse(rawType) == (int)type;
    }
}