// Sharlayan ~ BitConverter.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace Sharlayan.Helpers
{
    internal static class BitConverter
    {
        public static bool TryToBoolean(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToBoolean(value, index);
            }
            catch (Exception)
            {
                return default(bool);
            }
        }

        public static char TryToChar(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToChar(value, index);
            }
            catch (Exception)
            {
                return default(char);
            }
        }

        public static string TryToString(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToString(value, index);
            }
            catch (Exception)
            {
                return default(string);
            }
        }

        public static float TryToSingle(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToSingle(value, index);
            }
            catch (Exception)
            {
                return default(float);
            }
        }

        public static double TryToDouble(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToDouble(value, index);
            }
            catch (Exception)
            {
                return default(double);
            }
        }

        public static long TryToDoubleToInt64Bits(double value)
        {
            try
            {
                return System.BitConverter.DoubleToInt64Bits(value);
            }
            catch (Exception)
            {
                return default(long);
            }
        }

        public static double TryToInt64BitsToDouble(long value)
        {
            try
            {
                return System.BitConverter.Int64BitsToDouble(value);
            }
            catch (Exception)
            {
                return default(double);
            }
        }

        public static short TryToInt16(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt16(value, index);
            }
            catch (Exception)
            {
                return default(short);
            }
        }

        public static int TryToInt32(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt32(value, index);
            }
            catch (Exception)
            {
                return default(int);
            }
        }

        public static long TryToInt64(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToInt64(value, index);
            }
            catch (Exception)
            {
                return default(long);
            }
        }

        public static ushort TryToUInt16(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt16(value, index);
            }
            catch (Exception)
            {
                return default(ushort);
            }
        }

        public static uint TryToUInt32(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt32(value, index);
            }
            catch (Exception)
            {
                return default(uint);
            }
        }

        public static ulong TryToUInt64(byte[] value, int index)
        {
            try
            {
                return System.BitConverter.ToUInt64(value, index);
            }
            catch (Exception)
            {
                return default(ulong);
            }
        }
    }
}
