// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using SynesthesiaDev.Chibi.Core;
using SynesthesiaDev.Chibi.Core.Enums;
using Vanara.PInvoke;

namespace SynesthesiaDev.Chibi.Example;

public class Start
{
    public static void StartMain()
    {
        // Create window class
        var window = new ChibiWindow
        {
            Title = "Hello! 🌸"
        };

        // Hook into the easy event ecosystem
        window.OnWindowCreated.Subscribe(ev =>
        {
            Console.WriteLine($"Window successfully spawned with handle: {ev.Handle}");
        });

        window.OnKeyDown.Subscribe(ev =>
        {
            Console.WriteLine($"Key Pressed: {ev.Key}");
            if (ev.Key == User32.VK.VK_ESCAPE)
            {
                window.Dispose();
            }
        });

        window.OnMouseMove.Subscribe(ev =>
        {
            Console.WriteLine($"Mouse Moved: {ev.Now.X}, {ev.Now.Y}");
        });

        // Bind your custom loop frame logic
        window.OnFrame = () =>
        {
            // Your rendering logic here!
            // If left null, Chibi automatically optimizes CPU cycles via MsgWaitForMultipleObjectsEx

            // If you are making more complex program or a game, I recommend not using this and using your own render loop/thread instead
        };

        // Run the window (this blocks the execution thread)
        window.Run(1280, 720, WindowFlags.Resizable | WindowFlags.HighPixelDensity);
    }
}
