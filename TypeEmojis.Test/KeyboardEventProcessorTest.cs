using System;
using EmojiKeyboard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TypeEmojis.Test
{
    [TestClass]
    public class KeyboardEventProcessorTest
    {
        [TestMethod]
        public void Test_ExcapeForSendKeys_CorrectEscape()
        {
            var input = "H+e^l%l~o(w)o]r[ld";
            var escaped = KeyboardEventProcessor.ExcapeForSendKeys(input);

            Assert.AreEqual("H{+}e{^}l{%}l{~}o{(}w{)}o{]}r{[}ld", escaped);
        }

        [TestMethod]
        public void Test_ExcapeForSendKeys_With_Braces_CorrectEscape()
        {
            var input = "H{e^l}l~o(w)o]r[ld";
            var escaped = KeyboardEventProcessor.ExcapeForSendKeys(input);

            Assert.AreEqual("H{{}e{^}l{}}l{~}o{(}w{)}o{]}r{[}ld", escaped);
        }
    }
}
