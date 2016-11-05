using System.Collections.Concurrent;
using System.Linq;

namespace FFXIVAPP.Memory.Core.Enums
{
    public class Enumeration
    {
        private ConcurrentDictionary<byte, string> _byte = new ConcurrentDictionary<byte, string>();
        private ConcurrentDictionary<string, byte> _string = new ConcurrentDictionary<string, byte>();

        public string this[byte key] => _byte[key];
        public byte this[string key] => _string[key];

        public void Load(ConcurrentDictionary<string, byte> dictionary)
        {
            _string = dictionary;
            _byte = new ConcurrentDictionary<byte, string>(dictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));
        }
    }
}