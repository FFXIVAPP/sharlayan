using FFXIVAPP.Memory.Core;

namespace FFXIVAPP.Memory.Models
{
    public class TargetReadResult
    {
        public TargetReadResult()
        {
            TargetEntity = new TargetEntity();
        }

        public TargetEntity TargetEntity { get; set; }
        public bool TargetsFound { get; set; }
    }
}