using neyrd.core.Models;

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
    void HandleClick(double x, double y, MouseButton button);
    void HandleWheel(double deltaLength, double deltaX, double deltaY);
    
    void HandleKeyDown(string key, KeyModifier modifier);
    
    (int width, int height) GetScreenSize();
}