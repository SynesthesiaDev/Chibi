// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Extensions;
using SynesthesiaDev.Chibi.Core.Enums;
using static Vanara.PInvoke.User32;

namespace SynesthesiaDev.Chibi.Core;

public static class ChibiUtils
{
    public static long TimestampNow => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public static (WindowStyles Style, WindowStylesEx StyleEx) MapFlagsToWin32Styles(WindowFlags flags)
    {
        WindowStyles style = WindowStyles.WS_OVERLAPPED;
        if (!flags.HasFlagFast(WindowFlags.Hidden))
        {
            style |= WindowStyles.WS_VISIBLE;
        }

        WindowStylesEx styleEx = 0;

        if (flags.HasFlagFast(WindowFlags.Borderless) || flags.HasFlagFast(WindowFlags.Fullscreen))
        {
            style |= WindowStyles.WS_POPUP;
        }
        else
        {
            style |= WindowStyles.WS_CAPTION | WindowStyles.WS_SYSMENU | WindowStyles.WS_MINIMIZEBOX;

            if (flags.HasFlagFast(WindowFlags.Maximized))
            {
                style |= WindowStyles.WS_MAXIMIZEBOX;
            }
        }

        if (flags.HasFlagFast(WindowFlags.Resizable) && !flags.HasFlagFast(WindowFlags.Fullscreen))
        {
            style |= WindowStyles.WS_THICKFRAME | WindowStyles.WS_MAXIMIZEBOX;
        }

        if (flags.HasFlagFast(WindowFlags.Minimized))
        {
            style |= WindowStyles.WS_MINIMIZE;
        }
        else if (flags.HasFlagFast(WindowFlags.Maximized))
        {
            style |= WindowStyles.WS_MAXIMIZE;
        }

        if (flags.HasFlagFast(WindowFlags.AlwaysOnTop))
        {
            styleEx |= WindowStylesEx.WS_EX_TOPMOST;
        }

        if (flags.HasFlagFast(WindowFlags.Utility) || flags.HasFlagFast(WindowFlags.Tooltip))
        {
            styleEx |= WindowStylesEx.WS_EX_TOOLWINDOW;
        }

        if (flags.HasFlagFast(WindowFlags.NotFocusable))
        {
            styleEx |= WindowStylesEx.WS_EX_NOACTIVATE;
        }

        if (flags.HasFlagFast(WindowFlags.Transparent))
        {
            styleEx |= WindowStylesEx.WS_EX_LAYERED;
        }

        return (style, styleEx);
    }
}
