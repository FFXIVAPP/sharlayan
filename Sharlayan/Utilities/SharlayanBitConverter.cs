// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharlayanBitConverter.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SharlayanBitConverter.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;
    using System.Linq;

    internal static class SharlayanBitConverter {
        public static bool TryToBoolean(byte[] value, int index) {
            if (value.Length < 1 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToBoolean(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static char TryToChar(byte[] value, int index) {
            if (value.Length < 2 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToChar(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static double TryToDouble(byte[] value, int index) {
            if (value.Length < 8 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToDouble(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static long TryToDoubleToInt64Bits(double value) {
            try {
                return BitConverter.DoubleToInt64Bits(value);
            }
            catch (Exception) {
                return default;
            }
        }

        public static short TryToInt16(byte[] value, int index) {
            if (value.Length < 2 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToInt16(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static int TryToInt32(byte[] value, int index) {
            if (value.Length < 4 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToInt32(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static long TryToInt64(byte[] value, int index) {
            if (value.Length < 8 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToInt64(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static double TryToInt64BitsToDouble(long value) {
            try {
                return BitConverter.Int64BitsToDouble(value);
            }
            catch (Exception) {
                return default;
            }
        }

        public static float TryToSingle(byte[] value, int index) {
            if (value.Length < 4 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToSingle(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static string TryToString(byte[] value, int index) {
            if (!value.Any() || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToString(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static ushort TryToUInt16(byte[] value, int index) {
            if (value.Length < 2 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToUInt16(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static uint TryToUInt32(byte[] value, int index) {
            if (value.Length < 4 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToUInt32(value, index);
            }
            catch (Exception) {
                return default;
            }
        }

        public static ulong TryToUInt64(byte[] value, int index) {
            if (value.Length < 8 || index >= value.Length) {
                return default;
            }

            try {
                return BitConverter.ToUInt64(value, index);
            }
            catch (Exception) {
                return default;
            }
        }
    }
}