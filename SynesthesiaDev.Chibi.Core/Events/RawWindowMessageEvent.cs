// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using Vanara.PInvoke;

namespace SynesthesiaDev.Chibi.Core.Events;

public class RawWindowMessageEvent : IChibiEvent
{
    public User32.WindowMessage WindowMessage;
    public HWND EventHwnd;
    public uint Msg;
    public IntPtr WParam;
    public IntPtr LParam;

    public void Reset()
    {
        WindowMessage = 0;
        EventHwnd = HWND.NULL;
        Msg = 0;
        WParam = IntPtr.Zero;
        LParam = IntPtr.Zero;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"RawWindowMessageEvent(WindowMessage={WindowMessage}, EventHwnd={EventHwnd}, Msg={Msg}, WParam={WParam}, LParam={LParam}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
