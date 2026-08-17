namespace neyrd.core.Models;

public enum MouseButton
{
    Left = 1,
    Middle = 2,
    Right = 3
}

public static class MouseButtonExtensions
{
    public static MouseButton ToButton(bool isLeftButtonClick, bool isMiddleButtonClicked, bool isRightButtonClicked)
    {
        if (isLeftButtonClick) return MouseButton.Left;
        if (isMiddleButtonClicked) return MouseButton.Middle;
        return isRightButtonClicked ? MouseButton.Right : throw new InvalidOperationException();
    }
}