// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldOffsetReader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Attribute-based offset + size lookups for FFXIVClientStructs types. Used by mappers,
//   FFXIVClientStructsDirectProvider, and Reader.GameState to derive every byte offset
//   from the current submodule snapshot rather than hard-coding values that silently
//   drift on patch day. Prefer this over Marshal.OffsetOf / Marshal.SizeOf — FCS uses
//   LayoutKind.Explicit throughout, and many of its types carry managed function
//   pointers (AtkStage, UIModule, RaptureHotbarModule) that Marshal can't marshal.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources.Mappers {
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class FieldOffsetReader {
        /// <summary>
        /// Returns the <see cref="FieldOffsetAttribute"/> value of <paramref name="fieldName"/>
        /// on type <typeparamref name="T"/>. Works for public and internal/private fields, and
        /// for types that <see cref="Marshal.OffsetOf"/> rejects because they carry managed
        /// function pointers (AtkStage, UIModule, RaptureHotbarModule).
        /// </summary>
        /// <exception cref="MissingFieldException">Field not found on the type.</exception>
        /// <exception cref="InvalidOperationException">Field exists but has no FieldOffsetAttribute.</exception>
        public static int OffsetOf<T>(string fieldName) => OffsetOf(typeof(T), fieldName);

        /// <inheritdoc cref="OffsetOf{T}(string)"/>
        public static int OffsetOf(Type type, string fieldName) {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                              ?? throw new MissingFieldException(type.FullName, fieldName);
            FieldOffsetAttribute attr = field.GetCustomAttribute<FieldOffsetAttribute>()
                                        ?? throw new InvalidOperationException($"{type.FullName}.{fieldName} has no FieldOffsetAttribute — struct must use LayoutKind.Explicit.");
            return attr.Value;
        }

        /// <summary>
        /// Returns the size declared on <typeparamref name="T"/> via
        /// <c>[StructLayout(Size = N)]</c>. Equivalent to <see cref="Marshal.SizeOf{T}()"/>
        /// for blittable types, but also works for FCS types that contain managed function
        /// pointers (which <see cref="Marshal"/> can't size).
        /// </summary>
        public static int SizeOf<T>() => SizeOf(typeof(T));

        /// <inheritdoc cref="SizeOf{T}()"/>
        public static int SizeOf(Type type) {
            StructLayoutAttribute attr = type.StructLayoutAttribute;
            if (attr != null && attr.Size > 0) {
                return attr.Size;
            }
            // Fallback for struct types without a declared Size — let Marshal figure it out.
            return Marshal.SizeOf(type);
        }
    }
}
