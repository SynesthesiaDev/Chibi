// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Vanara.PInvoke;

namespace SynesthesiaDev.Chibi.Example;

public static class PaletteGenerator
{
    public static void Generate(in Gdi32.SafeHBRUSH[] array, int paletteSize)
    {
        for (int i = 0; i < paletteSize; i++)
        {
            float t = i / (float)(paletteSize - 1);
            byte grayValue = 80;
            float redThreshold = 0f;

            float rFloat = grayValue;
            float gFloat = grayValue;
            float bFloat = grayValue;

            if (t > redThreshold)
            {
                float redT = (t - redThreshold) / (1f - redThreshold);

                rFloat = grayValue + ((255 - grayValue) * redT);
                gFloat = grayValue - (grayValue * redT);
                bFloat = grayValue - (grayValue * redT);
            }

            byte r = (byte)rFloat;
            byte g = (byte)gFloat;
            byte b = (byte)bFloat;

            array[i] = Gdi32.CreateSolidBrush(new COLORREF(r, g, b));
        }
    }
}
