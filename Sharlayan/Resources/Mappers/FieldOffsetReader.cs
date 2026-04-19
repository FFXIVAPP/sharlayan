// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldOffsetReader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Helpers for mappers that translate FFXIVClientStructs struct field offsets into the
//   int-offset fields on Sharlayan's Models/Structures classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class FieldOffsetReader {
        /// <summary>
        /// Returns the <see cref="FieldOffsetAttribute"/> value of <paramref name="fieldName"/>
        /// on type <typeparamref name="T"/>. Works for both public fields (where
        /// <see cref="Marshal.OffsetOf"/> would also work) and internal / private fields
        /// (e.g. FFXIVClientStructs' <c>_name</c> FixedSizeArray backing fields, which
        /// <see cref="Marshal.OffsetOf"/> cannot reach across assembly boundaries).
        /// Prefer <see cref="Marshal.OffsetOf{T}(string)"/> directly when the field is
        /// public — this method exists for the internal-field case.
        /// </summary>
        /// <exception cref="MissingFieldException">Field not found on the type.</exception>
        /// <exception cref="InvalidOperationException">Field exists but has no FieldOffsetAttribute.</exception>
        public static int OffsetOf<T>(string fieldName) where T : unmanaged {
            FieldInfo field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                              ?? throw new MissingFieldException(typeof(T).FullName, fieldName);
            FieldOffsetAttribute attr = field.GetCustomAttribute<FieldOffsetAttribute>()
                                        ?? throw new InvalidOperationException($"{typeof(T).FullName}.{fieldName} has no FieldOffsetAttribute — struct must use LayoutKind.Explicit.");
            return attr.Value;
        }
    }
}
