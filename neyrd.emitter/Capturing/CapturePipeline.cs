using neyrd.core;
using neyrd.core.Models.Messages;
using neyrd.emitter.Encoding;
using neyrd.emitter.Networking;
using Spectre.Console;

namespace neyrd.emitter.Capturing;

internal sealed class CapturePipeline(ICaptureAdapter adapter, NeyrdSender sender, CancellationToken cancellationToken)
{
    private static readonly Queue<FrameData> Frames = new();
    private static readonly Queue<EncodedFrame> EncodedFrames = new();
    
    private Thread? _encodingThread;
    
    public async Task Begin()
    {
        if (!adapter.IsSupported)
        {
            throw new InvalidOperationException("The capture adapter is not supported.");
        }
        
        adapter.StartStream();
        
        // Start a background thread responsible for encoding frames
        _encodingThread = new Thread(EncodeFrames)
        {
            Priority = ThreadPriority.AboveNormal
        };
        _encodingThread.Start();
        
        // Begin capturing by capping up to 30 FPS
        const int desiredFps = 30;
        const double desiredInterval = 1000d / desiredFps;

        while (!cancellationToken.IsCancellationRequested)
        {
            var capturedFrame = adapter.CaptureFrame();
            Frames.Enqueue(capturedFrame);

            if (EncodedFrames.TryDequeue(out var frame))
            {
                var success = await sender.Send(FrameMessage.ToMessage(frame.OriginalSize, frame.EncodedLength,
                    frame.Data, (int)capturedFrame.Width, (int)capturedFrame.Height));
                if (!success)
                {
                    AnsiConsole.WriteLine($"Failed to send frame. Attempting to reconnect...");
                    
                    await Task.Delay(5000);
                    await sender.Reconnect((int)capturedFrame.Width, (int)capturedFrame.Height);
                }
            }

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
        
            EncodedFrames.Enqueue(encoded);
        }
        
        adapter.StopStream();
    }
}