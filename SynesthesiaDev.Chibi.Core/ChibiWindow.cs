using System.Collections.Concurrent;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using SynesthesiaDev.Chibi.Core.Extensions;
using Serilog;
using Synesthesia.Utils.Events;
using Synesthesia.Utils.Extensions;
using Synesthesia.Utils.Profiler;
using SynesthesiaDev.Chibi.Core.Enums;
using SynesthesiaDev.Chibi.Core.Events;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Gdi32;

namespace SynesthesiaDev.Chibi.Core;

//⣇⣿⠘⣿⣿⣿⡿⡿⣟⣟⢟⢟⢝⠵⡝⣿⡿⢂⣼⣿⣷⣌⠩⡫⡻⣝⠹⢿⣿⣷
//⡆⣿⣆⠱⣝⡵⣝⢅⠙⣿⢕⢕⢕⢕⢝⣥⢒⠅⣿⣿⣿⡿⣳⣌⠪⡪⣡⢑⢝⣇
//⡆⣿⣿⣦⠹⣳⣳⣕⢅⠈⢗⢕⢕⢕⢕⢕⢈⢆⠟⠋⠉⠁⠉⠉⠁⠈⠼⢐⢕⢽
//⡗⢰⣶⣶⣦⣝⢝⢕⢕⠅⡆⢕⢕⢕⢕⢕⣴⠏⣠⡶⠛⡉⡉⡛⢶⣦⡀⠐⣕⢕
//⡝⡄⢻⢟⣿⣿⣷⣕⣕⣅⣿⣔⣕⣵⣵⣿⣿⢠⣿⢠⣮⡈⣌⠨⠅⠹⣷⡀⢱⢕
//⡝⡵⠟⠈⢀⣀⣀⡀⠉⢿⣿⣿⣿⣿⣿⣿⣿⣼⣿⢈⡋⠴⢿⡟⣡⡇⣿⡇⡀⢕
//⡝⠁⣠⣾⠟⡉⡉⡉⠻⣦⣻⣿⣿⣿⣿⣿⣿⣿⣿⣧⠸⣿⣦⣥⣿⡇⡿⣰⢗⢄
//⠁⢰⣿⡏⣴⣌⠈⣌⠡⠈⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣬⣉⣉⣁⣄⢖⢕⢕⢕
//⡀⢻⣿⡇⢙⠁⠴⢿⡟⣡⡆⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣵⣵⣿
//⡻⣄⣻⣿⣌⠘⢿⣷⣥⣿⠇⣿⣿⣿⣿⣿⣿⠛⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
//⣷⢄⠻⣿⣟⠿⠦⠍⠉⣡⣾⣿⣿⣿⣿⣿⣿⢸⣿⣦⠙⣿⣿⣿⣿⣿⣿⣿⣿⠟
//⡕⡑⣑⣈⣻⢗⢟⢞⢝⣻⣿⣿⣿⣿⣿⣿⣿⠸⣿⠿⠃⣿⣿⣿⣿⣿⣿⡿⠁⣠
//⡝⡵⡈⢟⢕⢕⢕⢕⣵⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣶⣿⣿⣿⣿⣿⠿⠋⣀⣈⠙
//⡝⡵⡕⡀⠑⠳⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⢉⡠⡲⡫⡪⡪⡣

// hi..

public class ChibiWindow : IDisposable
{
    private const string class_name = "chibi_window_class";
    private const string system_theme_registry_path = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string system_theme_registry_key = "AppsUseLightTheme";

    //keeping as class variables so gc don't go eat it
    private WindowProc? wndProcDelegate;
    private HWND hwnd;
    private HINSTANCE hInstance;

    private readonly ConcurrentQueue<Action> commandQueue = new ConcurrentQueue<Action>();
    private bool isMouseInsideWindow;

    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Raw User32 window handle
    /// </summary>
    public IntPtr WindowHandle => hwnd.DangerousGetHandle();

    public bool IsWindowInitialized => !hwnd.IsNull;

    public WindowState State { get; private set; } = WindowState.Normal;

    public Vector2 MousePosition { get; private set; } = Vector2.Zero;

    /// <summary>
    /// Position of the top-right corner of the window
    /// </summary>
    public Vector2 WindowPosition { get; private set; } = Vector2.Zero;

    public bool WindowFocused { get; private set; }

    public SystemTheme SystemTheme { get; private set; } = LookupSystemTheme();

    /// <summary>
    /// Title of the window, defaults to name of the entry assembly
    /// </summary>
    public string Title
    {
        get;
        set
        {
            if (value == field) return;

            // we want to assign but don't do anything until the window is initialized
            field = value;

            if (!IsWindowInitialized) return;
            commandQueue.Enqueue(() => SetWindowText(hwnd, value));
        }
    } = Assembly.GetEntryAssembly()!.FullName!;

    public bool WindowVisible
    {
        get;
        set
        {
            if (field == value) return;
            field = value;

            if (!IsWindowInitialized) return;
            Schedule(() => ShowWindow(hwnd, field ? ShowWindowCommand.SW_SHOW : ShowWindowCommand.SW_HIDE));
        }
    } = true;

    public Vector2 WindowSize { get; private set; } = Vector2.Zero;

    public bool IsWindowHovered { get; private set; }

    public Clipboard Clipboard { get; } = new Clipboard();

    public Vector2 ScreenResolution
    {
        get
        {
            var width = GetSystemMetrics(SystemMetric.SM_CXSCREEN);
            var height = GetSystemMetrics(SystemMetric.SM_CYSCREEN);
            return new Vector2(width, height);
        }
    }

    public bool MouseCaptured
    {
        get;
        set
        {
            if (field == value) return;
            if (!IsWindowInitialized) return;

            field = value;
            Schedule(() =>
            {
                if (value) SetCapture(hwnd);
                else ReleaseCapture();
            });
        }
    } = false;

    #region Event Dispatchers

    public readonly EventDispatcher<WindowCreatedEvent> OnWindowCreated = new EventDispatcher<WindowCreatedEvent>();
    public readonly EventDispatcher<WindowDestroyedEvent> OnWindowDestroyed = new EventDispatcher<WindowDestroyedEvent>();
    public readonly EventDispatcher<WindowResizedEvent> OnWindowResized = new EventDispatcher<WindowResizedEvent>();
    public readonly EventDispatcher<WindowMovedEvent> OnWindowMoved = new EventDispatcher<WindowMovedEvent>();
    public readonly EventDispatcher<WindowFocusChangedEvent> OnWindowFocusChanged = new EventDispatcher<WindowFocusChangedEvent>();
    public readonly EventDispatcher<TextInputEvent> OnTextInput = new EventDispatcher<TextInputEvent>();
    public readonly EventDispatcher<KeyEvent> OnKeyDown = new EventDispatcher<KeyEvent>();
    public readonly EventDispatcher<KeyEvent> OnKeyUp = new EventDispatcher<KeyEvent>();
    public readonly EventDispatcher<MouseButtonEvent> OnMouseButtonStateChanged = new EventDispatcher<MouseButtonEvent>();
    public readonly EventDispatcher<MouseMoveEvent> OnMouseMove = new EventDispatcher<MouseMoveEvent>();
    public readonly EventDispatcher<MouseWheelEvent> OnMouseWheel = new EventDispatcher<MouseWheelEvent>();
    public readonly EventDispatcher<WindowStateChangeEvent> OnWindowStateChanged = new EventDispatcher<WindowStateChangeEvent>();
    public readonly EventDispatcher<SystemThemeChangeEvent> OnSystemThemeChanged = new EventDispatcher<SystemThemeChangeEvent>();
    public readonly EventDispatcher<WindowHoverStateChangeEvent> OnWindowHoverStateChange = new EventDispatcher<WindowHoverStateChangeEvent>();

    /// <summary>
    /// When the user clicks X or a program like WM sends a kill command to the window. Use this to give the user a confirmation or do final cleanup.
    /// If you register a subscriber to this event dispatcher, the window will no longer automatically close, and you will need to call <see cref="Dispose"/> on your own
    /// </summary>
    public readonly EventDispatcher<WindowExitRequestedEvent> OnExitRequested = new EventDispatcher<WindowExitRequestedEvent>();

    /// <summary>
    /// A global event multiplexer. Fires for every single event that passes through this window.
    /// </summary>
    public readonly EventDispatcher<IChibiEvent> OnEvent = new EventDispatcher<IChibiEvent>();

    /// <summary>
    /// Hook into raw Win32 messages (HWND, msg, wParam, lParam).
    /// Use with caution; modifying or blocking certain messages can break window behavior.
    /// </summary>
    public readonly EventDispatcher<RawWindowMessageEvent> OnRawWindowMessage = new EventDispatcher<RawWindowMessageEvent>();

    /// <summary>
    /// Triggered when the OS reports low memory. Ideal for clearing custom texture or particle caches.
    /// </summary>
    public readonly EventDispatcher<DeviceMemoryLowEvent> OnDeviceLowMemory = new EventDispatcher<DeviceMemoryLowEvent>();

    /// <summary>
    /// Called every frame, ideal for if you don't really care about framerate or when rendering happens,
    /// but for more complex programs and games, I would recommend using your own render thread logic
    /// </summary>
    public Action? OnFrame { get; set; }

    #endregion


    #region Window Creation

    /// <summary>
    ///  Creates a window and runs the event loop. This blocks the thread
    /// </summary>
    /// <param name="width">Width of the window</param>
    /// <param name="height">Height of the window</param>
    /// <param name="flags">Window creation flags the window will be created with</param>
    /// <exception cref="ObjectDisposedException">Thrown if this window is already disposed</exception>
    /// <exception cref="InvalidOperationException">Thrown if window creation fails</exception>
    public void Run(int width, int height, WindowFlags flags)
    {
        var profiler = Timings.RentAndPush();

        Log.Verbose("Chibi Initialized");
        Log.Verbose("- Version:         {version}", Assembly.GetExecutingAssembly().GetName().Version!.ToString());

        var displayDevice = new DISPLAY_DEVICE { cb = (uint)Marshal.SizeOf<DISPLAY_DEVICE>() };
        if (EnumDisplayDevices(null, 0, ref displayDevice, 0))
            Log.Verbose("- Video Driver:    {videoDriver}", displayDevice.DeviceString);

        WindowSize = new Vector2(width, height);

        ObjectDisposedException.ThrowIf(IsDisposed, "Window already disposed");

        if (!OperatingSystem.IsWindows()) return;

        wndProcDelegate = customWndProc;
        hInstance = Kernel32.GetModuleHandle();

        var windowClass = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf<WNDCLASSEX>(),
            style = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW,
            lpfnWndProc = wndProcDelegate,
            hInstance = hInstance,
            hCursor = LoadCursor(HINSTANCE.NULL, IDC_ARROW),
            // hbrBackground = GetStockObject(StockObjectType.WHITE_BRUSH),
            lpszClassName = class_name
        };

        if (RegisterClassEx(windowClass).IsInvalid)
            throw new InvalidOperationException("Failed to register chibi window class.");

        var (win32Style, win32StyleEx) = ChibiUtils.MapFlagsToWin32Styles(flags);

        int styledWindowWidth = width;
        int styledWindowHeight = height;

        if (flags.HasFlagFast(WindowFlags.Hidden))
        {
            WindowVisible = false;
        }

        if (flags.HasFlagFast(WindowFlags.Borderless) && !flags.HasFlagFast(WindowFlags.Fullscreen))
        {
            var rect = new RECT { left = 0, top = 0, right = width, bottom = height };
            AdjustWindowRectEx(ref rect, WindowStyles.WS_OVERLAPPEDWINDOW, false, 0);

            styledWindowWidth = rect.Right - rect.Left;
            styledWindowHeight = rect.Bottom - rect.Top;
        }
        else if (flags.HasFlagFast(WindowFlags.Fullscreen))
        {
            styledWindowWidth = (int)ScreenResolution.X;
            styledWindowHeight = (int)ScreenResolution.Y;
        }

        hwnd = CreateWindowEx(
            win32StyleEx,
            class_name,
            Title,
            win32Style,
            flags.HasFlagFast(WindowFlags.Fullscreen) ? 0 : 100,
            flags.HasFlagFast(WindowFlags.Fullscreen) ? 0 : 100,
            styledWindowWidth, styledWindowHeight,
            HWND.NULL, HMENU.NULL, hInstance, IntPtr.Zero
        );

        if (hwnd.IsNull)
            throw new InvalidOperationException("Failed to create chibi native window handle.");

        if (flags.HasFlagFast(WindowFlags.MouseGrabbed) || flags.HasFlagFast(WindowFlags.MouseCapture))
        {
            SetCapture(hwnd);
            isMouseInsideWindow = true;
        }

        var windowCreatedEvent = Pooled.WINDOW_CREATED_EVENT.Rent();
        windowCreatedEvent.Handle = WindowHandle;
        windowCreatedEvent.Window = this;

        OnWindowCreated.DispatchEvent(windowCreatedEvent, OnEvent);

        var result = profiler.PopAndReturn();
        Log.Verbose("Created {window} in {time}ms", this, result);
        while (!IsDisposed)
        {
            while (commandQueue.TryDequeue(out var action))
            {
                action.Invoke();
            }

            while (PeekMessage(out MSG msg, HWND.NULL, 0, 0, PM.PM_REMOVE))
            {
                if (IsDisposed)
                    break;
                TranslateMessage(msg);
                DispatchMessage(msg);
            }

            if (OnFrame != null)
                OnFrame.Invoke();
            else
                // let's not explode the cpu and block until we get something to do
                MsgWaitForMultipleObjectsEx(0, [], 1, QS.QS_ALLINPUT, MWMO.MWMO_INPUTAVAILABLE);
        }
    }

    #endregion

    #region Utils

    /// <summary>
    /// This flashes the program icon in the taskbar
    /// </summary>
    /// <param name="untilFocused">Makes the flash repeat until the user focuses the window</param>
    public void Flash(bool untilFocused = true)
    {
        if (!IsWindowInitialized) return;

        Schedule(() =>
        {
            var fInfo = new FLASHWINFO
            {
                cbSize = (uint)Marshal.SizeOf<FLASHWINFO>(),
                hwnd = hwnd,
                dwFlags = untilFocused ? FLASHW.FLASHW_ALL | FLASHW.FLASHW_TIMERNOFG : FLASHW.FLASHW_ALL,
                uCount = uint.MaxValue,
                dwTimeout = 0
            };
            FlashWindowEx(in fInfo);
        });
    }

    /// <summary>
    /// Cancels ongoing taskbar flash
    /// </summary>
    public void CancelFlash()
    {
        if (!IsWindowInitialized) return;

        Schedule(() =>
        {
            var fInfo = new FLASHWINFO
            {
                cbSize = (uint)Marshal.SizeOf<FLASHWINFO>(),
                hwnd = hwnd,
                dwFlags = FLASHW.FLASHW_STOP
            };

            FlashWindowEx(in fInfo);
        });
    }

    /// <summary>
    /// Thread-safely schedules actions. Use for modifying the window using raw invokes.
    /// Setting <see cref="Title"/>, <see cref="WindowVisible"/>, or other properties automatically schedules them
    /// so no need to schedule them yourself
    /// </summary>
    /// <param name="action">Action to schedule</param>
    public void Schedule(Action action) => commandQueue.Enqueue(action);

    #endregion

    #region Window Event Handling

    private IntPtr customWndProc(HWND eventHwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        var windowMessage = (WindowMessage)msg;

        var rawMessageEvent = Pooled.RAW_WINDOW_MESSAGE_EVENT.Rent();
        rawMessageEvent.WindowMessage = windowMessage;
        rawMessageEvent.EventHwnd = eventHwnd;
        rawMessageEvent.Msg = msg;
        rawMessageEvent.WParam = wParam;
        rawMessageEvent.LParam = lParam;

        OnRawWindowMessage.DispatchEvent(rawMessageEvent, null);

        switch (windowMessage)
        {
            case WindowMessage.WM_ERASEBKGND:
                return 1;

            case WindowMessage.WM_DESTROY:
                PostQuitMessage();

                var destroyEvent = Pooled.WINDOW_DESTROYED_EVENT.Rent();
                OnWindowDestroyed.DispatchEvent(destroyEvent, OnEvent);

                Dispose();
                return IntPtr.Zero;

            case WindowMessage.WM_KEYDOWN:
                var keyDown = (VK)wParam.ToInt32();

                var keyDownEvent = Pooled.KEY_EVENT.Rent();
                keyDownEvent.Key = keyDown;
                keyDownEvent.IsDown = true;

                OnKeyDown.DispatchEvent(keyDownEvent, OnEvent);
                break;

            case WindowMessage.WM_KEYUP:
                var keyUp = (VK)wParam.ToInt32();

                var keyUpEvent = Pooled.KEY_EVENT.Rent();
                keyUpEvent.Key = keyUp;
                keyUpEvent.IsDown = false;

                OnKeyUp.DispatchEvent(keyUpEvent, OnEvent);
                break;

            case WindowMessage.WM_SIZE:
                var state = (WindowState)wParam.ToInt32();

                if (State != state)
                {
                    var stateEvent = Pooled.WINDOW_STATE_CHANGE_EVENT.Rent();
                    stateEvent.Before = State;
                    stateEvent.Now = state;

                    State = state;

                    OnWindowStateChanged.DispatchEvent(stateEvent, OnEvent);
                }

                var size = new Vector2(Macros.LOWORD(lParam), Macros.HIWORD(lParam));

                var resizeEvent = Pooled.WINDOW_RESIZED_EVENT.Rent();
                resizeEvent.Before = WindowSize;
                resizeEvent.Now = size;

                WindowSize = size;

                OnWindowResized.DispatchEvent(resizeEvent, OnEvent);
                OnFrame?.Invoke();

                return IntPtr.Zero;

            case WindowMessage.WM_CLOSE:
                var exitRequestedEvent = Pooled.WINDOW_EXIT_REQUESTED_EVENT.Rent();
                exitRequestedEvent.Handle = WindowHandle;
                exitRequestedEvent.Window = this;


                // If exit requested has subscribers, we return Zero to say that we want to manually handle exiting
                if (OnExitRequested.HasSubscribers)
                {
                    OnExitRequested.DispatchEvent(exitRequestedEvent, OnEvent);
                    return IntPtr.Zero;
                }

                break;

            case WindowMessage.WM_MOUSEMOVE:
                var position = new Vector2(Macros.GET_X_LPARAM(lParam), Macros.GET_Y_LPARAM(lParam));

                var mouseEvent = Pooled.MOUSE_MOVE_EVENT.Rent();
                mouseEvent.Before = MousePosition;
                mouseEvent.Now = position;

                MousePosition = position;
                OnMouseMove.DispatchEvent(mouseEvent, OnEvent);

                if (!isMouseInsideWindow)
                {
                    var tme = new TRACKMOUSEEVENT
                    {
                        cbSize = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>(),
                        dwFlags = TME.TME_LEAVE,
                        hwndTrack = hwnd
                    };

                    if (TrackMouseEvent(ref tme))
                    {
                        isMouseInsideWindow = true;

                        var windowHoverStateEvent = Pooled.WINDOW_HOVER_STATE_CHANGE_EVENT.Rent();
                        windowHoverStateEvent.Before = IsWindowHovered;
                        windowHoverStateEvent.Now = true;

                        OnWindowHoverStateChange.DispatchEvent(windowHoverStateEvent, OnEvent);
                        IsWindowHovered = true;
                    }
                }

                break;

            case WindowMessage.WM_THEMECHANGED:

                var newTheme = LookupSystemTheme();
                var themeEvent = Pooled.SYSTEM_THEME_CHANGE_EVENT.Rent();
                themeEvent.Before = SystemTheme;
                themeEvent.Now = newTheme;

                SystemTheme = newTheme;

                OnSystemThemeChanged.DispatchEvent(themeEvent, OnEvent);
                break;

            case WindowMessage.WM_MOUSEWHEEL:
                var delta = (short)Macros.HIWORD(wParam);
                var normalized = delta / 120f;

                var wheelEvent = Pooled.MOUSE_WHEEL_EVENT.Rent();
                wheelEvent.Delta = normalized;

                OnMouseWheel.DispatchEvent(wheelEvent, OnEvent);
                break;

            case WindowMessage.WM_MOVE:
                var windowPos = new Vector2(Macros.LOWORD(lParam), Macros.HIWORD(lParam));

                var moveEvent = Pooled.WINDOW_MOVED_EVENT.Rent();
                moveEvent.Before = WindowPosition;
                moveEvent.Now = windowPos;

                WindowPosition = windowPos;

                OnWindowMoved.DispatchEvent(moveEvent, OnEvent);
                OnFrame?.Invoke();

                return IntPtr.Zero;

            case WindowMessage.WM_ACTIVATE:
                var active = (WM_ACTIVATE_WPARAM)Macros.LOWORD(wParam);
                var isFocused = active != WM_ACTIVATE_WPARAM.WA_INACTIVE;

                var focusedEvent = Pooled.WINDOW_FOCUS_CHANGED_EVENT.Rent();
                focusedEvent.Before = WindowFocused;
                focusedEvent.Now = isFocused;

                WindowFocused = isFocused;

                OnWindowFocusChanged.DispatchEvent(focusedEvent, OnEvent);
                break;

            case WindowMessage.WM_MOUSELEAVE:
                isMouseInsideWindow = false;

                var hoverEvent = Pooled.WINDOW_HOVER_STATE_CHANGE_EVENT.Rent();
                hoverEvent.Before = IsWindowHovered;
                hoverEvent.Now = isMouseInsideWindow;

                IsWindowHovered = false;
                OnWindowHoverStateChange.DispatchEvent(hoverEvent, OnEvent);
                break;

            case WindowMessage.WM_COMPACTING:
                //out of ram?? in this economy?? well shit...
                var outOfRamEvent = Pooled.DEVICE_MEMORY_LOW_EVENT.Rent();
                OnDeviceLowMemory.DispatchEvent(outOfRamEvent, OnEvent);
                break;

            case WindowMessage.WM_CHAR:
                var typedChar = (char)wParam.ToInt32();

                if (!char.IsControl(typedChar))
                {
                    var textEvent = Pooled.TEXT_INPUT_EVENT.Rent();
                    textEvent.Character = typedChar;

                    OnTextInput.DispatchEvent(textEvent, OnEvent);
                }

                break;

            // Left mouse
            case WindowMessage.WM_LBUTTONDOWN:
                dispatchMouseClickEvent(MouseButton.Left, true);
                break;

            case WindowMessage.WM_LBUTTONUP:
                dispatchMouseClickEvent(MouseButton.Left, false);
                break;

            // Right Mouse
            case WindowMessage.WM_RBUTTONDOWN:
                dispatchMouseClickEvent(MouseButton.Right, true);
                break;

            case WindowMessage.WM_RBUTTONUP:
                dispatchMouseClickEvent(MouseButton.Right, false);
                break;

            // Middle Mouse
            case WindowMessage.WM_MBUTTONDOWN:
                dispatchMouseClickEvent(MouseButton.Middle, true);
                break;

            case WindowMessage.WM_MBUTTONUP:
                dispatchMouseClickEvent(MouseButton.Middle, false);
                break;

            // Custom Mouse Buttons
            case WindowMessage.WM_XBUTTONDOWN:
                var xButtonDown = Macros.HIWORD(wParam);
                var downButton = (MouseButton)(xButtonDown == 1 ? 4 : 5);

                dispatchMouseClickEvent(downButton, true);
                break;

            case WindowMessage.WM_XBUTTONUP:
                var xButtonUp = Macros.HIWORD(wParam);
                var upButton = (MouseButton)(xButtonUp == 1 ? 4 : 5);
                dispatchMouseClickEvent(upButton, false);
                break;
        }

        return DefWindowProc(eventHwnd, msg, wParam, lParam);
    }

    private void dispatchMouseClickEvent(MouseButton button, bool isDown)
    {
        var mouseButtonEvent = Pooled.MOUSE_BUTTON_EVENT_POOL.Rent();
        mouseButtonEvent.Button = button;
        mouseButtonEvent.IsDown = isDown;
        mouseButtonEvent.Timestamp = ChibiUtils.TimestampNow;

        OnMouseButtonStateChanged.DispatchEvent(mouseButtonEvent, OnEvent);
    }

    #endregion


    /// <summary>
    /// Performs a registry lookup of the system theme.
    /// Returns <see cref="SystemTheme.Light"/> if registry lookup fails
    /// </summary>
    public static SystemTheme LookupSystemTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(system_theme_registry_path);
            if (key?.GetValue(system_theme_registry_key) is int value)
            {
                // 0 - off (dark)
                // 1 - on (light)
                return value == 0 ? SystemTheme.Dark : SystemTheme.Light;
            }
        }
        catch (Exception exception)
        {
            Log.Warning("Failed to lookup system theme in registry, defaulting to Light");
            Log.Warning("{ex}", exception);
        }

        return SystemTheme.Light;
    }

    public void Dispose()
    {
        if (IsDisposed) return;
        IsDisposed = true;

        Log.Verbose("Disposing {window}", this);
        if (!hwnd.IsNull)
        {
            DestroyWindow(hwnd);
            hwnd = HWND.NULL;
        }

        if (hInstance != HINSTANCE.NULL)
        {
            UnregisterClass(class_name, hInstance);
            hInstance = HINSTANCE.NULL;
        }

        wndProcDelegate = null;
    }

    ~ChibiWindow()
    {
        Dispose();
    }

    public override string ToString()
    {
        return $"ChibiWindow(Title={Title}), Handle={WindowHandle}, Size={WindowSize}, IsDisposed={IsDisposed})";
    }
}



