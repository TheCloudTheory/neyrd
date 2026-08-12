namespace neyrd.core.Messages;

public interface IMessage
{
    byte[] Payload { get; }
}