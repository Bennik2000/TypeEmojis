using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypeEmojis
{
    class BlockedAppsReader
    {
        private string Path { get; set; }

        public BlockedAppsReader(string path)
        {
            Path = path;
        }

        public List<string> GetBlockedApps()
        {
            try
            {
                return File.ReadAllLines(Path).Where(l => !l.StartsWith("#")).ToList();
            }
            catch (IOException)
            {
                return new List<string>();
            }
        }
    }
}
