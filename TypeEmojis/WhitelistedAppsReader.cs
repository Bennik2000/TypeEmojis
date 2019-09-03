using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypeEmojis
{
    class WhitelistedAppsReader
    {
        private string Path { get; set; }

        public WhitelistedAppsReader(string path)
        {
            Path = path;
        }

        public List<string> GetWhitelistedApps()
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
