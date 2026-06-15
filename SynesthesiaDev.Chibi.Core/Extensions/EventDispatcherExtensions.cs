// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.


using Synesthesia.Utils.Events;
using SynesthesiaDev.Chibi.Core.Events;

namespace SynesthesiaDev.Chibi.Core.Extensions;

public static class EventDispatcherExtensions
{
    // Dispatches the event, assigns its timestamp and then at the end of its lifecycle returns it to its fast object pool
    public static void DispatchEvent<T>(this EventDispatcher<T> dispatcher, T value, EventDispatcher<IChibiEvent>? global) where T : IChibiEvent
    {
        try
        {
            value.Timestamp = ChibiUtils.TimestampNow;
            dispatcher.Dispatch(value);
            global?.Dispatch(value);
        }
        finally
        {
            value.ReturnAction?.Invoke(value);
        }
    }
}
