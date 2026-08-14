namespace neyrd.emitter.Puppeting;

/// <summary>
/// 
/// </summary>
internal interface IPuppeter
{
    string Name { get; }
    bool IsSupported { get; }

    void MovePointer(double x, double y);
}