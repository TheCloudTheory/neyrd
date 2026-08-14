using neyrd.core.Events;
using neyrd.core.Messages;

namespace neyrd.core.Models.Events;

public sealed class ScreenResolutionEstablishedEvent(int width, int height) : INeyrdEvent<(int, int)>
{
    public static string Type => "ScreenResolutionEstablished";
    public (int, int) Payload { get; }  = (width, height);
    
    public static ScreenResolutionEstablishedEvent From(MessageEnvelope message)
    {
        var width = int.Parse(message.Segments[0]);
        var height = int.Parse(message.Segments[1]);
        
        return new ScreenResolutionEstablishedEvent(width, height);
    }
}