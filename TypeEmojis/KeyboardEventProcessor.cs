using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TypeEmojis
{
    public class KeyboardEventProcessor
    {
        private string CapturedCharacters { get; set; }

        private bool IsHandling { get; set; }

        private Dictionary<string, string> EmojiList { get; set; }

        private List<string> ApplicationWhiteList { get; set; }

        public KeyboardEventProcessor(Dictionary<string, string> emojiList, List<string> applicationWhiteList)
        {
            EmojiList = emojiList;
            CapturedCharacters = string.Empty;
            ApplicationWhiteList = applicationWhiteList;
        }

        public void Start()
        {
            KeyboardHooks.KeyPress += OnKeyPress;
        }

        public void Stop()
        {
            KeyboardHooks.KeyPress -= OnKeyPress;
        }
        

        private void OnKeyPress(object sender, KeyPressEventArgs args)
        {
            if (IsHandling) return;

            var process = WindowsApi.GetActiveProcess().ProcessName;
            if (!ApplicationWhiteList.Contains(process))
            {
                CapturedCharacters = string.Empty;
                return;
            }


            IsHandling = true;


            if (CapturedCharacters.Length == 0)
            {
                if (EmojiList.Keys.Any(p => p.StartsWith(args.KeyChar.ToString())))
                {
                    CapturedCharacters += args.KeyChar;
                    args.Handled = true;
                }
            }
            else
            {
                CapturedCharacters += args.KeyChar;

                var possibleResults = EmojiList.Keys
                    .Where(k => k.StartsWith(CapturedCharacters))
                    .ToList();

                if (possibleResults.Any(r => r == CapturedCharacters))
                {
                    SendKeys.SendWait(ExcapeForSendKeys(EmojiList[CapturedCharacters]));
                    args.Handled = true;
                    CapturedCharacters = string.Empty;
                }
                else if (possibleResults.Count == 0)
                {
                    SendKeys.SendWait(ExcapeForSendKeys(CapturedCharacters));

                    args.Handled = true;
                    CapturedCharacters = string.Empty;
                }
                else if (possibleResults.Count > 0)
                {
                    args.Handled = true;
                }
            }
            
            IsHandling = false;
        }

        public static string ExcapeForSendKeys(string text)
        {
            var escapeDictionary = new Dictionary<string, string>
            {
                {"+", "{+}"},
                {"^", "{^}"},
                {"%", "{%}"},
                {"~", "{~}"},
                {"(", "{(}"},
                {")", "{)}"},
                {"]", "{]}"},
                {"[", "{[}"},
                {"{", "{{}"},
                {"}", "{}}"}
            };

            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i].ToString();

                if (escapeDictionary.ContainsKey(character))
                {
                    text = text.Remove(i, 1);
                    text = text.Insert(i, escapeDictionary[character]);
                    i += escapeDictionary[character].Length - 1;
                }
            }
            
            return text;
        }
    }
}
