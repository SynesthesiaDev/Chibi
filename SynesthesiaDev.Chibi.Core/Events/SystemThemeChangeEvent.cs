// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;
using SynesthesiaDev.Chibi.Core.Enums;

namespace SynesthesiaDev.Chibi.Core.Events;

public class SystemThemeChangeEvent : IChibiEvent
{
    public SystemTheme Before = SystemTheme.Light;
    public SystemTheme Now = SystemTheme.Light;

    public void Reset()
    {
        Before = SystemTheme.Light;
        Now = SystemTheme.Light;
    }

    public bool IsPooled { get; set; }
    public Action<IPooledObject>? ReturnAction { get; set; }
    public long Timestamp { get; set; }

    public override string ToString()
    {
        return $"SystemThemeChangeEvent(Before={Before}, Now={Now}, IsPooled={IsPooled}, Timestamp={Timestamp})";
    }
}
