using System;
using Sharlayan.Models.ReadResults;

namespace Sharlayan {
    public static partial class Reader {
        public static bool CanGetJobResources() {
            return Scanner.Instance.Locations.ContainsKey(Signatures.JobResourceKey);
        }

        public static JobResourceResult GetJobResources() {
            if (!CanGetJobResources() || !MemoryHandler.Instance.IsAttached)
                return new JobResourceResult(null);

            var resourcePtr = Scanner.Instance.Locations[Signatures.JobResourceKey].GetAddress();
            if (resourcePtr == IntPtr.Zero)
                return new JobResourceResult(null);

            var resource = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(resourcePtr));
            if (resource == IntPtr.Zero)
                return new JobResourceResult(null);

            var bytes = MemoryHandler.Instance.GetByteArray(resource, MemoryHandler.Instance.Structures.JobResources.SourceSize);
            return new JobResourceResult(bytes);
        }
    }
}