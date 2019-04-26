using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EmojiKeyboard
{
    public class KeyboardEventProcessor
    {
        private string CapturedCharacters { get; set; }

        private bool IsHandling { get; set; }

        private Dictionary<string, string> EmojiLookup { get; set; }


        public KeyboardEventProcessor()
        {
            EmojiLookup = new Dictionary<string, string>
            {
                {":)", "😊"},
                {":(", "😕"},
                {":P", "😛"},
                {"<3", "❤️"},
                {":?", "🤔"},
                {"O:)", "😇"},
                {":*", "😘"},
                {"*.*", "😍"},
                {"O.O", "😱"},
                {":,)", "😅"}
            };

            CapturedCharacters = string.Empty;
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
            IsHandling = true;

            if (CapturedCharacters.Length == 0)
            {
                if (EmojiLookup.Keys.Any(p => p.StartsWith(args.KeyChar.ToString())))
                {
                    CapturedCharacters += args.KeyChar;
                    args.Handled = true;
                }
            }
            else
            {
                CapturedCharacters += args.KeyChar;

                var possibleResults = EmojiLookup.Keys
                    .Where(k => k.StartsWith(CapturedCharacters))
                    .ToList();

                if (possibleResults.Any(r => r == CapturedCharacters))
                {
                    SendKeys.SendWait(ExcapeForSendKeys(EmojiLookup[CapturedCharacters]));
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
