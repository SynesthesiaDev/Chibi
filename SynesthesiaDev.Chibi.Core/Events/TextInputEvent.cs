// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public class TextInputEvent : IChibiEvent
{
    public char Character = char.MinValue;

    public void Reset()
    {
        Character = char.MinValue;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"TextInputEvent(Character={Character}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
