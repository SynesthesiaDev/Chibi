using System.Diagnostics;
using System.Numerics;
using SynesthesiaDev.Chibi.Core;
using SynesthesiaDev.Chibi.Core.Enums;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SpectreConsole;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Gdi32;

namespace SynesthesiaDev.Chibi.Example;

internal class Program
{
    /*
     * This is a tiny particle simulation example I wrote with window's built-in software renderer GDI32
     *
     * Left-Click - Attract
     * Right-Click - Push Away
     * Space - Pause/Resume
     * R - Randomize velocities
     * Esc - Dispose/Close
     */

    private const int particle_count = 1500;
    private const int palette_size = 100;

    private static ChibiWindow window = null!;
    private static SafeHDC graphicsDeviceHandle = SafeHDC.Null;
    private static SafeHBITMAP offscreen = SafeHBITMAP.Null;
    private static HGDIOBJ oldBmp = HGDIOBJ.NULL;
    private static Vector2 lastWindowSize = Vector2.Zero;

    private static readonly Particle[] particles = new Particle[particle_count];
    private static readonly SafeHBRUSH background_brush = CreateSolidBrush(new COLORREF(10, 10, 20));
    private static readonly SafeHBRUSH[] velocity_palette = new SafeHBRUSH[palette_size];

    private static long lastFrame = Stopwatch.GetTimestamp();
    private static bool leftDown, rightDown;
    private static bool running = true;

    private class Particle(Vector2 position, Vector2 velocity)
    {
        public Vector2 Position = position;
        public Vector2 Velocity = velocity;
    };

    public static Random Random = new Random();

    private static void Main()
    {
        using var log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.SpectreConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}", minLevel: LogEventLevel.Verbose)
            .CreateLogger();

        Log.Logger = log;

        window = new ChibiWindow();
        window.Title = "Chibi - Particle Example";

        window.OnKeyDown.Subscribe(e =>
        {
            switch (e.Key)
            {
                case VK.VK_ESCAPE:
                    window.Dispose();
                    return;
                case VK.VK_R:
                {
                    for (int i = 0; i < particle_count; i++)
                        particles[i].Velocity = new Vector2(
                            (Random.NextSingle() - 0.5f) * 8f,
                            (Random.NextSingle() - 0.5f) * 8f
                        );
                    break;
                }
                case VK.VK_SPACE:
                    running = !running;
                    break;
            }
        });

        window.OnWindowCreated.Subscribe(e =>
        {
            // Pre-allocate fixed-size color pallet instead of creating a brush every frame
            PaletteGenerator.Generate(in velocity_palette, palette_size);

            for (int i = 0; i < particle_count; i++)
            {
                particles[i] = new Particle(
                    new Vector2(Random.Next((int)e.Window.WindowSize.X), Random.Next((int)e.Window.WindowSize.Y)),
                    new Vector2((Random.NextSingle() - 0.5f) * 4f, (Random.NextSingle() - 0.5f) * 4f)
                );
            }
        });

        window.OnMouseButtonStateChanged.Subscribe(e =>
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    leftDown = e.IsDown;
                    break;
                case MouseButton.Right:
                    rightDown = e.IsDown;
                    break;
            }
        });

        window.OnFrame += OnFrame;

        window.Run(1280, 720, WindowFlags.Resizable | WindowFlags.HighPixelDensity);
    }

    private static void OnFrame()
    {
        if(!window.IsWindowInitialized) return;

        var now = Stopwatch.GetTimestamp();
        var dt = (float)(now - lastFrame) / Stopwatch.Frequency;
        lastFrame = now;
        dt = MathF.Min(dt, 0.05f);

        var hwnd = (HWND)window.WindowHandle;
        var hdc = GetDC(hwnd);

        // Resize & Initialization
        if (window.WindowSize != lastWindowSize)
        {
            if (!offscreen.IsInvalid)
            {
                SelectObject(graphicsDeviceHandle, oldBmp);
                offscreen.Dispose();
            }

            if (graphicsDeviceHandle.IsInvalid)
                graphicsDeviceHandle = CreateCompatibleDC(hdc);

            offscreen = CreateCompatibleBitmap(hdc, (int)window.WindowSize.X, (int)window.WindowSize.Y);
            oldBmp = SelectObject(graphicsDeviceHandle, offscreen);
            lastWindowSize = window.WindowSize;
        }

        // Clear window
        FillRect(graphicsDeviceHandle, new RECT { right = (int)window.WindowSize.X, bottom = (int)window.WindowSize.Y }, background_brush);

        var mouse = window.MousePosition;

        if (!running) return;

        // physics step and draw
        for (int i = 0; i < particle_count; i++)
        {
            var particle = particles[i];
            var direction = mouse - particle.Position;
            var dist = direction.Length();

            if (dist is > 1f and < 250f && (leftDown || rightDown))
            {
                var force = Vector2.Normalize(direction) * (2500f / dist);
                particle.Velocity += (leftDown ? force : -force) * dt;
            }

            particle.Velocity *= MathF.Pow(0.8f, dt);

            var speed = particle.Velocity.Length();

            // cap speed
            if (speed > 10f)
            {
                particle.Velocity = Vector2.Normalize(particle.Velocity) * 10f;
                speed = 10f;
            }

            particle.Position += particle.Velocity * dt * 60f;

            // bounds check
            if (particle.Position.X < 0 || particle.Position.X > window.WindowSize.X) particle.Velocity.X *= -1;
            if (particle.Position.Y < 0 || particle.Position.Y > window.WindowSize.Y) particle.Velocity.Y *= -1;
            particle.Position = Vector2.Clamp(particle.Position, Vector2.Zero, window.WindowSize);

            float speedRatio = speed / 10f;
            int paletteIndex = (int)(speedRatio * (palette_size - 1));

            paletteIndex = Math.Clamp(paletteIndex, 0, palette_size - 1);

            FillRect(graphicsDeviceHandle, new RECT
            {
                left = (int)particle.Position.X - 3, top = (int)particle.Position.Y - 3,
                right = (int)particle.Position.X + 3, bottom = (int)particle.Position.Y + 3
            }, velocity_palette[paletteIndex]);
        }

        // Push to window buffer and release dc
        BitBlt(hdc, 0, 0, (int)window.WindowSize.X, (int)window.WindowSize.Y, graphicsDeviceHandle, 0, 0, RasterOperationMode.SRCCOPY);
        ReleaseDC(hwnd, hdc);
    }
}
