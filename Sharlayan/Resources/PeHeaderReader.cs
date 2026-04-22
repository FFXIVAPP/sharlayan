// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeHeaderReader.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2022 Ryan Wilson <syndicated.life@gmail.com> (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reads the ImageBase field from a PE file's optional header, used to translate
//   absolute addresses from FFXIVClientStructs/ida/data.yml into offsets relative
//   to the running process's loaded module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Resources {
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection.PortableExecutable;

    internal static class PeHeaderReader {
        internal static ulong GetImageBase(Process process) {
            if (process == null) {
                throw new ArgumentNullException(nameof(process));
            }

            ProcessModule mainModule = process.MainModule
                                       ?? throw new InvalidOperationException("Process has no MainModule; cannot read PE header.");

            return GetImageBase(mainModule.FileName);
        }

        internal static ulong GetImageBase(string exeFilePath) {
            if (string.IsNullOrWhiteSpace(exeFilePath)) {
                throw new ArgumentException("exeFilePath must be a non-empty path.", nameof(exeFilePath));
            }

            if (!File.Exists(exeFilePath)) {
                throw new FileNotFoundException("PE file not found.", exeFilePath);
            }

            using FileStream stream = File.OpenRead(exeFilePath);
            using PEReader reader = new PEReader(stream);

            PEHeader header = reader.PEHeaders.PEHeader
                              ?? throw new InvalidDataException($"File is not a valid PE image: {exeFilePath}");

            return header.ImageBase;
        }
    }
}
