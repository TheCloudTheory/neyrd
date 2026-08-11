namespace neyrd.core;

public enum MessageType
{
    /// <summary>
    /// Represents a test message type used for internal operations or validations.
    /// </summary>
    Test,

    /// <summary>
    /// Represents an acknowledgement message type used to confirm the receipt of a message
    /// or signal the successful execution of an operation.
    /// </summary>
    Acknowledgement,

    /// <summary>
    /// Represents a handshake message type used during the initial connection
    /// phase to establish communication between a sender and a receiver.
    /// </summary>
    Handshake
}

public static class MessageTypeComparer
{
    public static bool IsEqual(string rawType, MessageType type)
    {
        return int.Parse(rawType) == (int)type;
    }
}