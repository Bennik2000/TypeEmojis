using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace EmojiKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            KeyboardHooks.KeyPress += KeyboardHooksOnKeyPress;
            
            emojiLookup.Add(":)", "😊");
            emojiLookup.Add(":(", "😕");
            emojiLookup.Add(":P", "😛");
            emojiLookup.Add("<3", "❤️");
            emojiLookup.Add(":?", "🤔");
            emojiLookup.Add("O:)", "😇");
            emojiLookup.Add(":*", "😘");
            emojiLookup.Add("*.*", "😍");
            emojiLookup.Add("O.O", "😱");
            emojiLookup.Add(":,)", "😅");
        }

        private string capturedCharacters = string.Empty;

        private bool isHandling;

        private Dictionary<string, string> emojiLookup = new Dictionary<string, string>();
        
        private void KeyboardHooksOnKeyPress(object sender, KeyPressEventArgs args)
        {
            if(isHandling) return;

            isHandling = true;

            if (capturedCharacters.Length == 0)
            {
                capturedCharacters = string.Empty;
                if (emojiLookup.Any(p => p.Key.StartsWith(args.KeyChar.ToString())))
                {
                    capturedCharacters += args.KeyChar;
                    args.Handled = true;
                }
            }

            else
            {
                capturedCharacters += args.KeyChar;

                var possibleResults = emojiLookup.Where(k => k.Key.StartsWith(capturedCharacters)).Select(k => k.Key).ToList();

                var exactMatch = possibleResults.FirstOrDefault(r => r == capturedCharacters);

                if (exactMatch != null)
                {
                    SendKeys.SendWait(ExcapeForSendKeys(emojiLookup[exactMatch]));
                    args.Handled = true;
                    capturedCharacters = string.Empty;
                }
                else if(possibleResults.Count == 0)
                {
                    SendKeys.SendWait(ExcapeForSendKeys(capturedCharacters));

                    args.Handled = true;
                    capturedCharacters = string.Empty;
                }
                else if (possibleResults.Count > 0)
                {
                    args.Handled = true;
                }
            }

            isHandling = false;
        }

        private string ExcapeForSendKeys(string text)
        {
            //text = text.Replace("}", "{}}");
            //text = text.Replace("{", "{{}");

            text = text.Replace("+", "{+}");
            text = text.Replace("^", "{^}");
            text = text.Replace("%", "{%}");
            text = text.Replace("~", "{~}");
            text = text.Replace("(", "{(}");
            text = text.Replace(")", "{)}");
            text = text.Replace("]", "{]}");
            text = text.Replace("[", "{[}");

            return text;
        }
    }
}
