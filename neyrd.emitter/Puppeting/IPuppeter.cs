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
    void HandleClick(double x, double y);
    void HandleWheel(double deltaLength, double deltaX, double deltaY);
    (int width, int height) GetScreenSize();
}