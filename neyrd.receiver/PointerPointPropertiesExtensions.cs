using Avalonia.Input;
using MouseButton = neyrd.core.Models.MouseButton;

namespace neyrd.receiver;

public static class PointerPointPropertiesExtensions
{
    /// <summary>
    /// Converts <see cref="PointerPointProperties"/> to a corresponding <see cref="MouseButton"/> value
    /// based on the button press state of the pointer.
    /// </summary>
    /// <param name="properties">The pointer properties that indicate the state of mouse buttons.</param>
    /// <returns>
    /// A <see cref="MouseButton"/> value representing the pressed mouse button. Returns
    /// <see cref="MouseButton.None"/> if no button is pressed.
    /// </returns>
    public static MouseButton ToMouseButton(this PointerPointProperties properties)
    {
        if (properties.IsRightButtonPressed) return MouseButton.Right;
        if (properties.IsLeftButtonPressed) return MouseButton.Left;
        if (properties.IsMiddleButtonPressed) return MouseButton.Middle;
        if (properties.IsXButton2Pressed) return MouseButton.XButton2;
        return properties.IsXButton1Pressed ? MouseButton.XButton1 : MouseButton.None;
    }
}