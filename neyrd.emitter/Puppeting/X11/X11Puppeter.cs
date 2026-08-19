using neyrd.core;
using neyrd.core.Models;

namespace neyrd.emitter.Puppeting.X11;

internal sealed partial class X11Puppeter : IPuppeter
{
    private IntPtr _display;
    private ulong _root;

    private static readonly Dictionary<string, string> KeyToX11 = new()
    {
        ["Enter"] = "Return",
        ["Return"] = "Return",
        ["Space"] = "space",
        ["Back"] = "BackSpace",
        ["Tab"] = "Tab",
        ["Escape"] = "Escape",
        ["Delete"] = "Delete",
        ["Insert"] = "Insert",
        ["Home"] = "Home",
        ["End"] = "End",
        ["PageUp"] = "Prior",
        ["PageDown"] = "Next",
        ["Left"] = "Left",
        ["Right"] = "Right",
        ["Up"] = "Up",
        ["Down"] = "Down",
        ["F1"] = "F1",
        ["F2"] = "F2",
        ["F3"] = "F3",
        ["F4"] = "F4",
        ["F5"] = "F5",
        ["F6"] = "F6",
        ["F7"] = "F7",
        ["F8"] = "F8",
        ["F9"] = "F9",
        ["F10"] = "F10",
        ["F11"] = "F11",
        ["F12"] = "F12"
    };

    public string Name => "X11";

    public bool IsSupported => IsX11Supported();

    public void Initialize()
    {
        _display = XOpenDisplay(null);
        _root = XDefaultRootWindow(_display);
    }

    public void MovePointer(double x, double y)
    {
        _ = XWarpPointer(_display, 0, _root, 0, 0, 0, 0, (int)x, (int)y);
        _ = XFlush(_display);
    }

    public void HandleClick(double x, double y, MouseButton button)
    {
        MovePointer(x, y);

        try
        {
            NeyrdLogger.Log($"Display: {_display != IntPtr.Zero}");

            _ = XTestFakeButtonEvent(_display, (uint)button, true, 0);
            _ = XFlush(_display);
            Thread.Sleep(50);
            _ = XTestFakeButtonEvent(_display, (uint)button, false, 0);
            _ = XFlush(_display);
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error when handling pointer click: {ex.Message}");
        }
    }

    public void HandleWheel(double deltaLength, double deltaX, double deltaY)
    {
        // X11 maps scroll to button events: 4=up, 5=down, 6=left, 7=right
        var button = deltaY < 0 ? 4u : 5u;
        if (deltaX != 0) button = deltaX < 0 ? 6u : 7u;

        _ = XTestFakeButtonEvent(_display, button, true, 0);
        _ = XTestFakeButtonEvent(_display, button, false, 0);
        _ = XFlush(_display);
    }

    public void HandleKeyDown(string key, KeyModifier modifier)
    {
        var keysym = XStringToKeysym(ToX11KeysymName(key));
        var keycode = XKeysymToKeycode(_display, keysym);

        var activeModifiers = Enum.GetValues<KeyModifier>()
            .Where(m => m != KeyModifier.None && modifier.HasFlag(m))
            .Select(m => XKeysymToKeycode(_display, XStringToKeysym(ModifierToX11String(m))))
            .ToList();

        foreach (var modKeycode in activeModifiers)
        {
            _ = XTestFakeKeyEvent(_display, modKeycode, true, 0);
        }

        _ = XTestFakeKeyEvent(_display, keycode, true, 0);
        _ = XTestFakeKeyEvent(_display, keycode, false, 0);

        foreach (var modKeycode in activeModifiers)
        {
            _ = XTestFakeKeyEvent(_display, modKeycode, false, 0);
        }

        _ = XFlush(_display);
    }

    private static string ModifierToX11String(KeyModifier modifier) => modifier switch
    {
        KeyModifier.Alt => "Alt_L",
        KeyModifier.Control => "Control_L",
        KeyModifier.Shift => "Shift_L",
        KeyModifier.Meta => "Super_L",
        KeyModifier.None => "",
        _ => throw new ArgumentOutOfRangeException(nameof(modifier))
    };

    private static string ToX11KeysymName(string avaloniaKey)
    {
        if (KeyToX11.TryGetValue(avaloniaKey, out var x11))
        {
            return x11;
        }

        return avaloniaKey.Length == 1 ? avaloniaKey.ToLowerInvariant() : avaloniaKey;
    }

    public (int width, int height) GetScreenSize()
    {
        var width = XDisplayWidth(_display, 0);
        var height = XDisplayHeight(_display, 0);

        return (width, height);
    }

    private bool IsX11Supported()
    {
        try
        {
            var dpy = XOpenDisplay(null);
            if (dpy == IntPtr.Zero) return false;
            _ = XCloseDisplay(dpy);
            return true;
        }
        catch (DllNotFoundException ex)
        {
            NeyrdLogger.Log($"Error loading libX11: {ex.Message}");
            return false;
        }
    }

    public void Dispose() => XCloseDisplay(_display);
}