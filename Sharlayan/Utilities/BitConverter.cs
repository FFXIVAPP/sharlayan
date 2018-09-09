﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitConverter.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   BitConverter.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Utilities {
    using System;

    internal static class BitConverter {
        public static bool TryToBoolean(byte[] value, int index) {
            try {
                return System.BitConverter.ToBoolean(value, index);
            }
            catch (Exception) {
                return default(bool);
            }
        }

        public static char TryToChar(byte[] value, int index) {
            try {
                return System.BitConverter.ToChar(value, index);
            }
            catch (Exception) {
                return default(char);
            }
        }

        public static double TryToDouble(byte[] value, int index) {
            try {
                return System.BitConverter.ToDouble(value, index);
            }
            catch (Exception) {
                return default(double);
            }
        }

        public static long TryToDoubleToInt64Bits(double value) {
            try {
                return System.BitConverter.DoubleToInt64Bits(value);
            }
            catch (Exception) {
                return default(long);
            }
        }

        public static short TryToInt16(byte[] value, int index) {
            try {
                return System.BitConverter.ToInt16(value, index);
            }
            catch (Exception) {
                return default(short);
            }
        }

        public static int TryToInt32(byte[] value, int index) {
            try {
                return System.BitConverter.ToInt32(value, index);
            }
            catch (Exception) {
                return default(int);
            }
        }

        public static long TryToInt64(byte[] value, int index) {
            try {
                return System.BitConverter.ToInt64(value, index);
            }
            catch (Exception) {
                return default(long);
            }
        }

        public static double TryToInt64BitsToDouble(long value) {
            try {
                return System.BitConverter.Int64BitsToDouble(value);
            }
            catch (Exception) {
                return default(double);
            }
        }

        public static float TryToSingle(byte[] value, int index) {
            try {
                return System.BitConverter.ToSingle(value, index);
            }
            catch (Exception) {
                return default(float);
            }
        }

        public static string TryToString(byte[] value, int index) {
            try {
                return System.BitConverter.ToString(value, index);
            }
            catch (Exception) {
                return default(string);
            }
        }

        public static ushort TryToUInt16(byte[] value, int index) {
            try {
                return System.BitConverter.ToUInt16(value, index);
            }
            catch (Exception) {
                return default(ushort);
            }
        }

        public static uint TryToUInt32(byte[] value, int index) {
            try {
                return System.BitConverter.ToUInt32(value, index);
            }
            catch (Exception) {
                return default(uint);
            }
        }

        public static ulong TryToUInt64(byte[] value, int index) {
            try {
                return System.BitConverter.ToUInt64(value, index);
            }
            catch (Exception) {
                return default(ulong);
            }
        }
    }
}