// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using Vanara.PInvoke;

namespace SynesthesiaDev.Chibi.Core.Events;

public class KeyEvent : IChibiEvent
{
    public User32.VK Key = User32.VK.VK_0;
    public bool IsDown = false;

    public void Reset()
    {
        Key = User32.VK.VK_0;
        IsDown = false;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"KeyEvent(IsDown={IsDown}, Key={Key}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
