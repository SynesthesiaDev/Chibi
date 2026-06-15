// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public class MouseWheelEvent : IChibiEvent
{
    public float Delta;

    public bool IsDown => Delta < 0f;
    public bool IsUp => Delta > 0f;

    public void Reset()
    {
        Delta = 0f;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }


    public override string ToString()
    {
        return $"MouseWheelEvent(Delta={Delta}, IsDown={IsDown}, IsUp={IsUp}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
