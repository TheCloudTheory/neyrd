namespace neyrd.emitter.Capturing;

internal interface ICaptureAdapter
{
    string Name { get; }
    bool IsSupported { get; }

    /// <summary>
    /// Captures a single frame of screen data and returns it as a <see cref="FrameData"/> object.
    /// </summary>
    /// <returns>
    /// A <see cref="FrameData"/> object containing the width, height, and raw pixel data
    /// of the captured frame.
    /// </returns>
    FrameData CaptureFrame();

    /// <summary>
    /// Initiates the screen capture process to begin streaming frames.
    /// </summary>
    /// <remarks>
    /// This method configures and starts the underlying capture mechanism,
    /// allowing frames to be continuously captured until the stream is manually stopped.
    /// </remarks>
    void StartStream()
    {
    }

    /// <summary>
    /// Stops the ongoing screen capture stream initiated by the <see cref="StartStream"/> method.
    /// </summary>
    /// <remarks>
    /// This method halts the underlying capture mechanism, ensuring that no further frames are captured
    /// until the stream is started again. It should be called when the capture pipeline is no longer
    /// required or when capturing needs to be paused.
    /// </remarks>
    void StopStream()
    {
    }
}