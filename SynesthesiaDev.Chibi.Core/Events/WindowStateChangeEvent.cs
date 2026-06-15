// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using SynesthesiaDev.Chibi.Core.Enums;

namespace SynesthesiaDev.Chibi.Core.Events;

public class WindowStateChangeEvent : IChibiEvent
{
    public WindowState Before = WindowState.Normal;
    public WindowState Now = WindowState.Normal;

    public void Reset()
    {
        Before = WindowState.Normal;
        Now = WindowState.Normal;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"WindowStateChangeEvent(Before={Before}, Now={Now}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
