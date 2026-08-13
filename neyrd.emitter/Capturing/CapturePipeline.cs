using neyrd.core.Models.Messages;
using neyrd.emitter.Encoding;
using neyrd.emitter.Networking;

namespace neyrd.emitter.Capturing;

internal sealed class CapturePipeline(ICaptureAdapter adapter, NeyrdSender sender, CancellationToken cancellationToken)
{
    private static readonly Queue<FrameData> Frames = new();
    private Thread? _encodingThread;
    
    public async Task Begin()
    {
        if (!adapter.IsSupported)
        {
            throw new InvalidOperationException("The capture adapter is not supported.");
        }
        
        // Start a background thread responsible for encoding frames
        _encodingThread = new Thread(EncodeFrames);
        _encodingThread.Start();
        
        // Begin capturing by capping up to 30 FPS
        const int desiredFps = 30;
        const double desiredInterval = 1000d / desiredFps;

        while (!cancellationToken.IsCancellationRequested)
        {
            Frames.Enqueue(adapter.CaptureFrame());
            await Task.Delay((int)desiredInterval);
        }
    }
    
    private void EncodeFrames()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if(Frames.Count == 0) continue;

            var frame = Frames.Dequeue();
            var encoded = EncodingStrategySelector.GetEncoder().Encode(frame.Data);
        
            _ = sender.Send(FrameMessage.ToMessage(encoded.OriginalSize, encoded.EncodedLength, encoded.Data, frame.Width, frame.Height));
        }
    }
}