using System;
using UnityEngine;

namespace Profighter.Client.UI
{
    public static class DeviceScreenInfo
    {
        private const float DefaultDPI = 200f;

        private static float unitMultiplier;
        private static float oneOverUnitMultiplier;

        public static float PixelsToInches(float pixels)
        {
            return pixels * oneOverUnitMultiplier;
        }

        public static float InchesToPixels(float units)
        {
            return units * UnitMultiplier;
        }

        public static float UnitMultiplier
        {
            get => unitMultiplier;
            set
            {
                value = Math.Max(0.00001f, value);
                unitMultiplier = value;
                oneOverUnitMultiplier = 1.0f / value;
            }
        }

        public static void Setup()
        {
            var screenDPI = Screen.dpi;
            UnitMultiplier = screenDPI > 0 ? screenDPI : DefaultDPI;
        }
    }
}

