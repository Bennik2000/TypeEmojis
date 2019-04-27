using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TypeEmojis.Test
{
    [TestClass]
    public class EmojiListFileReaderTest
    {

        [TestMethod]
        public void Test_LoadList_CommentLineNotLoaded()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                "#:(\t= 😕",
            };

            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.IsTrue(loaded.Keys.All(k => k != ":("));
        }

        [TestMethod]
        public void Test_LoadList_ListCorrectLoaded()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                ":(\t= 😕",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.IsTrue(loaded.ContainsKey(":)"));
            Assert.AreEqual("😊", loaded[":)"]);
            Assert.IsTrue(loaded.ContainsKey(":("));
            Assert.AreEqual("😕", loaded[":("]);
        }

        [TestMethod]
        public void Test_LoadList_EmptyLineIgnored()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                "",
                ":(\t= 😕",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.AreEqual(2, loaded.Count);
        }

        [TestMethod]
        public void Test_LoadList_InvalidLineIgnored()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                "InvalidLine",
                ":(\t= 😕",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.AreEqual(2, loaded.Count);
        }

        [TestMethod]
        public void Test_LoadList_LineIgnoredWhenNoValueGiven()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                "InvalidLine\t= ",
                ":(\t= 😕",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.AreEqual(2, loaded.Count);
        }

        [TestMethod]
        public void Test_LoadList_LineIgnoredWhenNoKeyGiven()
        {
            var file = new List<string>()
            {
                ":)\t= 😊",
                "\t= Value",
                ":(\t= 😕",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.AreEqual(2, loaded.Count);
        }

        [TestMethod]
        public void Test_LoadList_WhenDuplicateKeyLastOccuranceIsLoaded()
        {
            var file = new List<string>()
            {
                ":)\t= :D",
                ":)\t= 😊",
            };


            var fileReader = new EmojiListFileReader();
            fileReader.LoadLines(file);
            var loaded = fileReader.KeyValueDictionary;

            Assert.AreEqual("😊", loaded[":)"]);
        }
    }
}
