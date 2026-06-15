// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace SynesthesiaDev.Chibi.Core.Enums;

[Flags]
public enum WindowFlags : ulong
{
    /// <summary>window is in fullscreen mode</summary>
    Fullscreen = 1,

    /// <summary>window usable with OpenGL context</summary>
    OpenGL = 2,

    /// <summary>window is occluded</summary>
    Occluded = 4,

    /// <summary>
    /// window is neither mapped onto the desktop nor shown in the taskbar/dock/window list; <see cref="M:SDL3.SDL.ShowWindow(System.IntPtr)" /> is required for it to become visible
    /// </summary>
    Hidden = 8,

    /// <summary>no window decoration</summary>
    Borderless = 16, // 0x0000000000000010

    /// <summary>window can be resized</summary>
    Resizable = 32, // 0x0000000000000020

    /// <summary>window is minimized</summary>
    Minimized = 64, // 0x0000000000000040

    /// <summary>window is maximized</summary>
    Maximized = 128, // 0x0000000000000080

    /// <summary>window has grabbed mouse input</summary>
    MouseGrabbed = 256, // 0x0000000000000100

    /// <summary>window has input focus</summary>
    InputFocus = 512, // 0x0000000000000200

    /// <summary>window has mouse focus</summary>
    MouseFocus = 1024, // 0x0000000000000400

    /// <summary>window not created by SDL</summary>
    External = 2048, // 0x0000000000000800

    /// <summary>window is modal</summary>
    Modal = 4096, // 0x0000000000001000

    /// <summary>
    /// window uses high pixel density back buffer if possible
    /// </summary>
    HighPixelDensity = 8192, // 0x0000000000002000

    /// <summary>
    /// window has mouse captured (unrelated to <see cref="F:SDL3.SDL.WindowFlags.MouseGrabbed" />)
    /// </summary>
    MouseCapture = 16384, // 0x0000000000004000

    /// <summary>window has relative mode enabled</summary>
    MouseRelativeMode = 32768, // 0x0000000000008000

    /// <summary>window should always be above others</summary>
    AlwaysOnTop = 65536, // 0x0000000000010000

    /// <summary>
    /// window should be treated as a utility window, not showing in the task bar and window list
    /// </summary>
    Utility = 131072, // 0x0000000000020000

    /// <summary>
    /// window should be treated as a tooltip and does not get mouse or keyboard focus, requires a parent window
    /// </summary>
    Tooltip = 262144, // 0x0000000000040000

    /// <summary>
    /// window should be treated as a popup menu, requires a parent window
    /// </summary>
    PopupMenu = 524288, // 0x0000000000080000

    /// <summary>window has grabbed keyboard input</summary>
    KeyboardGrabbed = 1048576, // 0x0000000000100000

    /// <summary>
    /// window is in fill-document mode (Emscripten only), since SDL 3.4.0
    /// </summary>
    WindowFillDocument = 2097152, // 0x0000000000200000

    /// <summary>window usable for Vulkan surface</summary>
    Vulkan = 268435456, // 0x0000000010000000

    /// <summary>window usable for Metal view</summary>
    Metal = 536870912, // 0x0000000020000000

    /// <summary>window with transparent buffer</summary>
    Transparent = 1073741824, // 0x0000000040000000

    /// <summary>window should not be focusable</summary>
    NotFocusable = 2147483648, // 0x0000000080000000
}
