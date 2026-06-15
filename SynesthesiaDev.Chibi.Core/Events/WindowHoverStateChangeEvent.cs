// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public class WindowHoverStateChangeEvent : IChibiEvent
{
    public bool Before;
    public bool Now;

    public void Reset()
    {
        Before = false;
        Now = false;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"WindowHOverStateChangeEvent(Before={Before}, Now={Now}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
