// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public class WindowMovedEvent : IChibiEvent
{
    public Vector2 Before = Vector2.Zero;
    public Vector2 Now = Vector2.Zero;

    public void Reset()
    {
        Before = Vector2.Zero;
        Now = Vector2.Zero;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"WindowMovedEvent(Before={Before}, Now={Now}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
