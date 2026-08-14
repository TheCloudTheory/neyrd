namespace neyrd.emitter.Puppeting;

/// <summary>
/// 
/// </summary>
internal interface IPuppeter : IDisposable
{
    string Name { get; }
    bool IsSupported { get; }

    void Initialize();
    void MovePointer(double x, double y);
    (int width, int height) GetScreenSize();
}