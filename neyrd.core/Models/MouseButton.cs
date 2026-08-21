namespace neyrd.core.Models;

public enum MouseButton
{
    None = 0,
    Left = 1,
    Right = 2,
    Middle = 3,
    XButton1 = 4,
    XButton2 = 5
}

public static class MouseButtonExtensions
{
    public static MouseButton ToButton(int avaloniaMouseButton)
    {
        var button = (MouseButton)avaloniaMouseButton;
        return button;
    }
}