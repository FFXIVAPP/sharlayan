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

    // F84: bounds checks are exact (`index < 0 || index + width > Length`), so negative
    // offsets (-1 = unmapped-field sentinel) and reads too close to the buffer end
    // return default without ever throwing — these helpers run per-field, per-actor,
    // per-poll, and the previous `index >= Length`-only check pushed every near-end or
    // negative read through an exception.
    internal static class SharlayanBitConverter {
        public static bool TryToBoolean(byte[] value, int index) {
            if (index < 0 || index + 1 > value.Length) {
                return default;
            }

            return BitConverter.ToBoolean(value, index);
        }

        public static char TryToChar(byte[] value, int index) {
            if (index < 0 || index + 2 > value.Length) {
                return default;
            }

            return BitConverter.ToChar(value, index);
        }

        public static double TryToDouble(byte[] value, int index) {
            if (index < 0 || index + 8 > value.Length) {
                return default;
            }

            return BitConverter.ToDouble(value, index);
        }

        public static long TryToDoubleToInt64Bits(double value) {
            return BitConverter.DoubleToInt64Bits(value);
        }

        public static short TryToInt16(byte[] value, int index) {
            if (index < 0 || index + 2 > value.Length) {
                return default;
            }

            return BitConverter.ToInt16(value, index);
        }

        public static int TryToInt32(byte[] value, int index) {
            if (index < 0 || index + 4 > value.Length) {
                return default;
            }

            return BitConverter.ToInt32(value, index);
        }

        public static long TryToInt64(byte[] value, int index) {
            if (index < 0 || index + 8 > value.Length) {
                return default;
            }

            return BitConverter.ToInt64(value, index);
        }

        public static double TryToInt64BitsToDouble(long value) {
            return BitConverter.Int64BitsToDouble(value);
        }

        public static float TryToSingle(byte[] value, int index) {
            if (index < 0 || index + 4 > value.Length) {
                return default;
            }

            return BitConverter.ToSingle(value, index);
        }

        public static string TryToString(byte[] value, int index) {
            // Note: BitConverter.ToString returns a hyphenated hex dump ("1A-2B-FF"),
            // not decoded text — use MemoryHandler.GetStringFromBytes for text.
            if (index < 0 || index >= value.Length) {
                return default;
            }

            return BitConverter.ToString(value, index);
        }

        public static ushort TryToUInt16(byte[] value, int index) {
            if (index < 0 || index + 2 > value.Length) {
                return default;
            }

            return BitConverter.ToUInt16(value, index);
        }

        public static uint TryToUInt32(byte[] value, int index) {
            if (index < 0 || index + 4 > value.Length) {
                return default;
            }

            return BitConverter.ToUInt32(value, index);
        }

        public static ulong TryToUInt64(byte[] value, int index) {
            if (index < 0 || index + 8 > value.Length) {
                return default;
            }

            return BitConverter.ToUInt64(value, index);
        }
    }
}
