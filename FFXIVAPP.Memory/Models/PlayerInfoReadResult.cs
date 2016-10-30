using FFXIVAPP.Memory.Core;

namespace FFXIVAPP.Memory.Models
{
    public class PlayerInfoReadResult
    {
        public PlayerInfoReadResult()
        {
            PlayerEntity = new PlayerEntity();
        }

        public PlayerEntity PlayerEntity { get; set; }
    }
}