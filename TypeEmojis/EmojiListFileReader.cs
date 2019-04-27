using System;
using System.Collections.Generic;
using System.IO;

namespace TypeEmojis
{
    public class EmojiListFileReader
    {
        private string CommentString = "#";
        private string KeyValueSeparator = "\t=";

        public Dictionary<string, string> KeyValueDictionary { get; private set; }

        public EmojiListFileReader(string path)
        {
            try
            {
                LoadLines(File.ReadAllLines(path));
            }
            catch (IOException e)
            {
                // ignore
            }
        }

        public EmojiListFileReader()
        {
            KeyValueDictionary = new Dictionary<string, string>();
        }

        public void LoadLines(IEnumerable<string> fileLines)
        {
            KeyValueDictionary = new Dictionary<string, string>();

            foreach (var line in fileLines)
            {
                if(line.StartsWith(CommentString)) continue;

                var separatorIndex = line.IndexOf(KeyValueSeparator, StringComparison.Ordinal);

                if (separatorIndex < 0) continue;

                var key = line.Substring(0, separatorIndex).Trim();
                var value = line.Substring(separatorIndex + KeyValueSeparator.Length).Trim();

                if(string.IsNullOrWhiteSpace(key)) continue;
                if(string.IsNullOrWhiteSpace(value)) continue;

                if (!KeyValueDictionary.ContainsKey(key))
                {
                    KeyValueDictionary.Add(key, value);
                }
                else
                {
                    KeyValueDictionary[key] = value;
                }
            }
        }
    }
}
