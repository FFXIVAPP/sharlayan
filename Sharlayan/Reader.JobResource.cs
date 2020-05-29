using System;
using Sharlayan.Models.ReadResults;

namespace Sharlayan {
    public static partial class Reader {
        public static bool CanGetJobResources() {
            return Scanner.Instance.Locations.ContainsKey(Signatures.JobResourceKey);
        }

        public static JobResourceResult GetJobResources() {
            if (!CanGetJobResources() || !MemoryHandler.Instance.IsAttached)
                return JobResourceResult.Empty;
            var manager = Scanner.Instance.Locations[Signatures.JobResourceKey].GetAddress();
            if (manager == IntPtr.Zero)
                return JobResourceResult.Empty;
            manager = new IntPtr(MemoryHandler.Instance.GetPlatformUInt(manager));
            if (manager == IntPtr.Zero)
                return JobResourceResult.Empty;
            var jobStruct = MemoryHandler.Instance.GetStructure<JobResourceResult.JobResourceStruct>(manager);
            return new JobResourceResult(jobStruct);
        }
    }
}