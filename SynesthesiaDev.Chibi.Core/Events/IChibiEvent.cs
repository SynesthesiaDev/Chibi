// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Synesthesia.Utils.Pooling;

namespace SynesthesiaDev.Chibi.Core.Events;

public interface IChibiEvent : IPooledObject
{
    long Timestamp { get; set; }
}
