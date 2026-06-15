# 🌸 Chibi

![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/SynesthesiaDev/Chibi/build.yml?branch=main&style=for-the-badge&label=Build&color=33cc33)
![NuGet Version](https://img.shields.io/nuget/v/SynesthesiaDev.Chibi.Core?style=for-the-badge&color=blue&label=Release)

![Target .NET](https://img.shields.io/badge/.NET-10.0-512bd4?style=for-the-badge&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-black?style=for-the-badge)

**Tiny, lightweight, and type-safe managed Windows library for lifecycle management and window creation**

---

### ⚡ Features

- **💨 Extremly fast and lightweight**
    - Made with a single responsibility, spawn a window and manage its lifecycle. From calling `Run()` to the window
      being displayed takes on average 170ms
- **✅ Type safe:**
    - Fully abstracts away raw `User32` P/Invoke calls into type-safe C# abstractions.
- **🎫 Easy event lifecycle:**
    - Zero-allocation event system. Every underlying Windows message (`WM_*`) maps neatly to a dedicated `EventDispatcher`.
- 🛠️ **Developer Comforts**
    - Has a safe cross-thread scheduling, low-memory handling indicators via `WM_COMPACTING`, system theme detection, native taskbar flashing api and a clipboard api.
- 🚫 **No AI Slop**
    - Purely written by passionete single-brain-celled autistic individual

---

### 🚀 Quick Start

Getting a window up and running requires minimal boilerplate!
Here's how to initialize `ChibiWindow` and hook into events

```csharp
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
```

---

### 📦 Installation

Install via the NuGet Package Manager Console:

```shell
Install-Package SynesthesiaDev.Chibi.Core
```

Or via the .NET CLI:

```shell
dotnet add package SynesthesiaDev.Chibi.Core
```


```
⣇⣿⠘⣿⣿⣿⡿⡿⣟⣟⢟⢟⢝⠵⡝⣿⡿⢂⣼⣿⣷⣌⠩⡫⡻⣝⠹⢿⣿⣷
⡆⣿⣆⠱⣝⡵⣝⢅⠙⣿⢕⢕⢕⢕⢝⣥⢒⠅⣿⣿⣿⡿⣳⣌⠪⡪⣡⢑⢝⣇
⡆⣿⣿⣦⠹⣳⣳⣕⢅⠈⢗⢕⢕⢕⢕⢕⢈⢆⠟⠋⠉⠁⠉⠉⠁⠈⠼⢐⢕⢽
⡗⢰⣶⣶⣦⣝⢝⢕⢕⠅⡆⢕⢕⢕⢕⢕⣴⠏⣠⡶⠛⡉⡉⡛⢶⣦⡀⠐⣕⢕
⡝⡄⢻⢟⣿⣿⣷⣕⣕⣅⣿⣔⣕⣵⣵⣿⣿⢠⣿⢠⣮⡈⣌⠨⠅⠹⣷⡀⢱⢕
⡝⡵⠟⠈⢀⣀⣀⡀⠉⢿⣿⣿⣿⣿⣿⣿⣿⣼⣿⢈⡋⠴⢿⡟⣡⡇⣿⡇⡀⢕
⡝⠁⣠⣾⠟⡉⡉⡉⠻⣦⣻⣿⣿⣿⣿⣿⣿⣿⣿⣧⠸⣿⣦⣥⣿⡇⡿⣰⢗⢄
⠁⢰⣿⡏⣴⣌⠈⣌⠡⠈⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣬⣉⣉⣁⣄⢖⢕⢕⢕
⡀⢻⣿⡇⢙⠁⠴⢿⡟⣡⡆⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣵⣵⣿
⡻⣄⣻⣿⣌⠘⢿⣷⣥⣿⠇⣿⣿⣿⣿⣿⣿⠛⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿
⣷⢄⠻⣿⣟⠿⠦⠍⠉⣡⣾⣿⣿⣿⣿⣿⣿⢸⣿⣦⠙⣿⣿⣿⣿⣿⣿⣿⣿⠟
⡕⡑⣑⣈⣻⢗⢟⢞⢝⣻⣿⣿⣿⣿⣿⣿⣿⠸⣿⠿⠃⣿⣿⣿⣿⣿⣿⡿⠁⣠
⡝⡵⡈⢟⢕⢕⢕⢕⣵⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣶⣿⣿⣿⣿⣿⠿⠋⣀⣈⠙
⡝⡵⡕⡀⠑⠳⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠛⢉⡠⡲⡫⡪⡪⡣

ps.. in Chibi.Example is included cool particle sim 
that I wrote using GDI32 as a test
```