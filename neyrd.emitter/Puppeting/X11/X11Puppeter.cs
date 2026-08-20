using neyrd.core;
using neyrd.core.Models;

namespace neyrd.emitter.Puppeting.X11;

internal sealed partial class X11Puppeter : IPuppeter
{
    private IntPtr _display;
    private ulong _root;
    private bool _altGrActive;

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
        ["F12"] = "F12",
        ["OemQuestion"] = "slash",
        ["OemPeriod"] = "period",
        ["OemComma"] = "comma",
        ["OemMinus"] = "minus",
        ["OemPlus"] = "equal",
        ["OemTilde"] = "grave",
        ["OemSemicolon"] = "semicolon",
        ["OemQuotes"] = "apostrophe",
        ["OemBackslash"] = "backslash",
        ["OemOpenBrackets"] = "bracketleft",
        ["OemCloseBrackets"] = "bracketright",
        ["D0"] = "0", ["D1"] = "1", ["D2"] = "2", ["D3"] = "3", ["D4"] = "4",
        ["D5"] = "5", ["D6"] = "6", ["D7"] = "7", ["D8"] = "8", ["D9"] = "9",
        ["NumPad0"] = "KP_0", ["NumPad1"] = "KP_1", ["NumPad2"] = "KP_2",
        ["NumPad3"] = "KP_3", ["NumPad4"] = "KP_4", ["NumPad5"] = "KP_5",
        ["NumPad6"] = "KP_6", ["NumPad7"] = "KP_7", ["NumPad8"] = "KP_8",
        ["NumPad9"] = "KP_9",
        ["Multiply"] = "KP_Multiply", ["Add"] = "KP_Add",
        ["Subtract"] = "KP_Subtract", ["Divide"] = "KP_Divide",
        ["Decimal"] = "KP_Decimal",
        ["Oem3"]       = "grave",
        ["LeftAlt"]    = "Alt_L",
        ["RightAlt"]   = "ISO_Level3_Shift",
        ["LeftCtrl"]   = "Control_L",
        ["RightCtrl"]  = "Control_R",
        ["LWin"]       = "Super_L",
        ["RWin"]       = "Super_R",
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
        _altGrActive = key switch
        {
            "RightAlt" => true,
            "LeftAlt" => false,
            _ => _altGrActive
        };

        var keysymName = ToX11KeysymName(key);
        var keysym = XStringToKeysym(keysymName);
        if (keysym == 0)
        {
            NeyrdLogger.Log($"No X11 keysym for key: {key}");
            return;
        }

        var keycode = XKeysymToKeycode(_display, keysym);

        var activeModifiers = Enum.GetValues<KeyModifier>()
            .Where(m => m != KeyModifier.None && modifier.HasFlag(m))
            .Select(m => XKeysymToKeycode(_display, XStringToKeysym(ModifierToX11String(m))))
            .Where(kc => kc != 0)
            .ToList();

        foreach (var modKeycode in activeModifiers)
        {
            _ = XTestFakeKeyEvent(_display, modKeycode, true, 0);
        }

        if (keycode != 0)
        {
            _ = XTestFakeKeyEvent(_display, keycode, true, 0);
            _ = XTestFakeKeyEvent(_display, keycode, false, 0);
        }
        else
        {
            NeyrdLogger.Log($"No X11 keycode for key: {key}");
        }

        foreach (var modKeycode in activeModifiers)
        {
            _ = XTestFakeKeyEvent(_display, modKeycode, false, 0);
        }

        _ = XFlush(_display);
        
        if (modifier.HasFlag(KeyModifier.Alt))
        {
            _altGrActive = false;
        }
    }

    private string ModifierToX11String(KeyModifier modifier) => modifier switch
    {
        KeyModifier.Alt => _altGrActive ? "ISO_Level3_Shift" : "Alt_L",
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