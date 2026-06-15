// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using SynesthesiaDev.Chibi.Core.Enums;

namespace SynesthesiaDev.Chibi.Core.Events;

public class MouseButtonEvent : IChibiEvent
{
    public MouseButton Button = MouseButton.Left;
    public bool IsDown;

    public long Timestamp { get; set; }

    public void Reset()
    {
        Button = MouseButton.Left;
        IsDown = false;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }

    public override string ToString() => $"MouseButtonEvent(IsDown={IsDown}, Button={Button}, IsPooled={IsPooled}, Timestamp={Timestamp})";
}
