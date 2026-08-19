namespace neyrd.core.Messages;

public enum MessageType
{
    /// <summary>
    /// Represents a test message type used for internal operations or validations.
    /// </summary>
    Test,

    /// <summary>
    /// Indicates that a test has started, typically used to signal the initiation of a testing process.
    /// </summary>
    TestStarted,

    /// <summary>
    /// Indicates that a test has been completed. This message type is typically used
    /// to signify the conclusion of a test process or operation.
    /// </summary>
    TestCompleted,

    /// <summary>
    /// Represents an acknowledgement message type used to confirm the receipt of a message
    /// or signal the successful execution of an operation.
    /// </summary>
    Acknowledgement,

    /// <summary>
    /// Represents a handshake message type used during the initial connection
    /// phase to establish communication between a sender and a receiver.
    /// </summary>
    Handshake,

    /// <summary>
    /// Represents a frame message type, typically used for transmitting
    /// encoded data frames with associated metadata, such as size or length.
    /// </summary>
    Frame,

    /// <summary>
    /// Represents a synchronization message type used to ensure consistency or alignment between systems during communication.
    /// </summary>
    Synchronization,

    /// <summary>
    /// 
    /// </summary>
    Pointer,

    /// <summary>
    /// 
    /// </summary>
    Screen,

    /// <summary>
    /// 
    /// </summary>
    PointerPressed,
    PointerWheel,
    KeyDown
}

public static class MessageTypeComparer
{
    public static bool IsEqual(string rawType, MessageType type)
    {
        return int.Parse(rawType) == (int)type;
    }
}