namespace neyrd.core.Models;

public enum MouseButton
{
    None = 0,
    Left = 1,
    Right = 2,
    Middle = 3
}

public static class MouseButtonExtensions
{
    public static MouseButton ToButton(bool isLeftButtonClick, bool isMiddleButtonClicked, bool isRightButtonClicked)
    {
        if (isLeftButtonClick) return MouseButton.Left;
        if (isMiddleButtonClicked) return MouseButton.Middle;
        return isRightButtonClicked ? MouseButton.Right : throw new InvalidOperationException();
    }

    public static MouseButton ToButton(int avaloniaMouseButton)
    {
        var button = (MouseButton)avaloniaMouseButton;
        return button;
    }
}