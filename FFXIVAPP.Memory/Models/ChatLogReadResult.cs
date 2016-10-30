using System.Collections.Generic;
using FFXIVAPP.Memory.Core;

namespace FFXIVAPP.Memory.Models
{
    public class ChatLogReadResult
    {
        public ChatLogReadResult()
        {
            ChatLogEntries = new List<ChatLogEntry>();
        }

        public List<ChatLogEntry> ChatLogEntries { get; set; }
        public int PreviousArrayIndex { get; set; }
        public int PreviousOffset { get; set; }
    }
}