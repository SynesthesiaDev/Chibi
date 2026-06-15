// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using SynesthesiaDev.Chibi.Core.Events;

namespace SynesthesiaDev.Chibi.Core;

// auto generated, do not edit manually
public sealed class Pooled
{
    public static readonly FastObjectPool<MouseButtonEvent> MOUSE_BUTTON_EVENT_POOL = new FastObjectPool<MouseButtonEvent>(() => new MouseButtonEvent());
    public static readonly FastObjectPool<WindowCreatedEvent> WINDOW_CREATED_EVENT = new FastObjectPool<WindowCreatedEvent>(() => new WindowCreatedEvent());
    public static readonly FastObjectPool<WindowExitRequestedEvent> WINDOW_EXIT_REQUESTED_EVENT = new FastObjectPool<WindowExitRequestedEvent>(() => new WindowExitRequestedEvent());
    public static readonly FastObjectPool<WindowDestroyedEvent> WINDOW_DESTROYED_EVENT = new FastObjectPool<WindowDestroyedEvent>(() => new WindowDestroyedEvent());
    public static readonly FastObjectPool<WindowResizedEvent> WINDOW_RESIZED_EVENT = new FastObjectPool<WindowResizedEvent>(() => new WindowResizedEvent());
    public static readonly FastObjectPool<WindowMovedEvent> WINDOW_MOVED_EVENT = new FastObjectPool<WindowMovedEvent>(() => new WindowMovedEvent());
    public static readonly FastObjectPool<WindowFocusChangedEvent> WINDOW_FOCUS_CHANGED_EVENT = new FastObjectPool<WindowFocusChangedEvent>(() => new WindowFocusChangedEvent());
    public static readonly FastObjectPool<TextInputEvent> TEXT_INPUT_EVENT = new FastObjectPool<TextInputEvent>(() => new TextInputEvent());
    public static readonly FastObjectPool<KeyEvent> KEY_EVENT = new FastObjectPool<KeyEvent>(() => new KeyEvent());
    public static readonly FastObjectPool<MouseMoveEvent> MOUSE_MOVE_EVENT = new FastObjectPool<MouseMoveEvent>(() => new MouseMoveEvent());
    public static readonly FastObjectPool<MouseWheelEvent> MOUSE_WHEEL_EVENT = new FastObjectPool<MouseWheelEvent>(() => new MouseWheelEvent());
    public static readonly FastObjectPool<WindowStateChangeEvent> WINDOW_STATE_CHANGE_EVENT = new FastObjectPool<WindowStateChangeEvent>(() => new WindowStateChangeEvent());
    public static readonly FastObjectPool<SystemThemeChangeEvent> SYSTEM_THEME_CHANGE_EVENT = new FastObjectPool<SystemThemeChangeEvent>(() => new SystemThemeChangeEvent());
    public static readonly FastObjectPool<RawWindowMessageEvent> RAW_WINDOW_MESSAGE_EVENT = new FastObjectPool<RawWindowMessageEvent>(() => new RawWindowMessageEvent());
    public static readonly FastObjectPool<WindowHoverStateChangeEvent> WINDOW_HOVER_STATE_CHANGE_EVENT = new FastObjectPool<WindowHoverStateChangeEvent>(() => new WindowHoverStateChangeEvent());
    public static readonly FastObjectPool<DeviceMemoryLowEvent> DEVICE_MEMORY_LOW_EVENT = new FastObjectPool<DeviceMemoryLowEvent>(() => new DeviceMemoryLowEvent());
}
