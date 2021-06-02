using System;
using UnityEngine;

namespace Profighter.Client.UI
{
    public static class DeviceScreenInfo
    {
        private const float DefaultDPI = 200f;

        private static float dpi;

        public static float PixelsToInches(float pixels)
        {
            return pixels / dpi;
        }

        public static float InchesToPixels(float inches)
        {
            return inches * dpi;
        }

        public static float DPI
        {
            get => dpi;
            set
            {
                value = Math.Max(0.00001f, value);
                dpi = value;
            }
        }

        public static void Setup()
        {
            var screenDPI = Screen.dpi;
            dpi = screenDPI > 0 ? screenDPI : DefaultDPI;
        }
    }
}

