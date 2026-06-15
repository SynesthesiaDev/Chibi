// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.InteropServices;
using System.Text;
using Serilog;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;
using static Vanara.PInvoke.User32;

namespace SynesthesiaDev.Chibi.Core;

public class Clipboard
{
    private const int format_unicode_text = 13;
    private const int format_bitmap = 8;

    public uint RegisterFormat(string formatName)
    {
        var id = RegisterClipboardFormat(formatName);
        if (id == 0)
        {
            Log.Error("Failed to register custom clipboard format: {name}", formatName);
        }

        return id;
    }

    public bool SetText(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        var bytes = Encoding.Unicode.GetBytes(text + "\0");
        return SetRawData(format_unicode_text, bytes);
    }

    public string GetText()
    {
        var bytes = GetRawData(format_unicode_text);
        if (bytes == null) return string.Empty;

        string text = Encoding.Unicode.GetString(bytes);
        return text.Trim('\0');
    }

    public bool SetImage(byte[] bytes)
    {
        // header only, meaning no valid data
        if (bytes.Length < 14) return false;

        // clipboard expects stripped out header
        var payload = new byte[bytes.Length - 14];
        Array.Copy(bytes, 14, payload, 0, payload.Length);

        return SetRawData(format_bitmap, payload);
    }

    public byte[]? GetImage()
    {
        var payload = GetRawData(format_bitmap);
        if (payload == null) return null;

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // reconstruct image header
        writer.Write((ushort)0x4D42);
        writer.Write((uint)14 + payload.Length);
        writer.Write((ushort)0);
        writer.Write((ushort)0);
        writer.Write((uint)54);

        writer.Write(payload);
        return stream.ToArray();
    }

    public bool SetRawData(uint formatId, byte[] data)
    {
        if (data.Length == 0) return false;
        if (!OpenClipboard(HWND.NULL)) return false;
        try
        {
            EmptyClipboard();

            var hGlobal = GlobalAlloc(GMEM.GMEM_MOVEABLE, (uint)data.Length);
            if (hGlobal == IntPtr.Zero) return false;

            var pGlobal = GlobalLock(hGlobal);
            if (pGlobal != IntPtr.Zero)
            {
                //let's just shove the raw bytes into unmanaged memory!! very good api design mr microsoft
                Marshal.Copy(data, 0, pGlobal, data.Length);
                GlobalUnlock(hGlobal);

                HANDLE result = SetClipboardData(formatId, (nint)hGlobal);
                if (!result.IsNull)
                {
                    return true;
                }
            }

            GlobalFree(hGlobal);
            return false;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to write data payload to clipboard");
            return false;
        }
        finally
        {
            CloseClipboard();
        }
    }

    public byte[]? GetRawData(uint formatId)
    {
        if (!IsClipboardFormatAvailable(formatId)) return null;
        if (!OpenClipboard(HWND.NULL)) return null;

        try
        {
            HANDLE hClip = GetClipboardData(formatId);
            if (hClip.IsNull) return null;

            var pClip = GlobalLock(hClip.DangerousGetHandle());
            if (pClip == IntPtr.Zero) return null;

            try
            {
                var dataSize = GlobalSize(hClip.DangerousGetHandle());
                var rawBuffer = new byte[dataSize];

                Marshal.Copy(pClip, rawBuffer, 0, rawBuffer.Length);
                return rawBuffer;
            }
            finally
            {
                GlobalUnlock(hClip.DangerousGetHandle());
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get data payload from clipboard");
            return null;
        }
        finally
        {
            CloseClipboard();
        }
    }

    public uint[] GetAvailableFormats()
    {
        if (!OpenClipboard(HWND.NULL)) return Array.Empty<uint>();

        var formatsList = new List<uint>();
        try
        {
            uint currentFormat = EnumClipboardFormats(0);
            while (currentFormat != 0)
            {
                formatsList.Add(currentFormat);
                currentFormat = EnumClipboardFormats(currentFormat); // Get next chain link
            }
        }
        finally
        {
            CloseClipboard();
        }

        return formatsList.ToArray();
    }
}
