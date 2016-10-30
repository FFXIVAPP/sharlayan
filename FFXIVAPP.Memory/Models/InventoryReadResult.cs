using System.Collections.Generic;
using FFXIVAPP.Memory.Core;

namespace FFXIVAPP.Memory.Models
{
    public class InventoryReadResult
    {
        public InventoryReadResult()
        {
            InventoryEntities = new List<InventoryEntity>();
        }

        public List<InventoryEntity> InventoryEntities { get; set; }
    }
}