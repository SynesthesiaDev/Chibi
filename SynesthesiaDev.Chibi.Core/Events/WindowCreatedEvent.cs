// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public class WindowCreatedEvent : IChibiEvent
{
    public ChibiWindow Window = null!;
    public IntPtr Handle = IntPtr.Zero;
    public long Timestamp { get; set; }

    public void Reset()
    {
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }

    public override string ToString() => $"WindowCreatedEvent(Window={Window.Title}, Handle={Handle}, IsPooled={IsPooled}, Timestamp={Timestamp})";
}
