using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryPatcher;
using System.Threading.Tasks;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class StringOperationsTest
    {
        [TestMethod]
        public async Task TestStringOperations()
        {
            string testString = "This Thing is a throughout TesT";
            byte[] bytes = Encoding.ASCII.GetBytes(testString);

            string matchString = "T";
            string newString = "t";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllStrings(matchString, Encoding.ASCII);
            Assert.AreEqual(4, matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(5, matches[1]);
            Assert.AreEqual(27, matches[2]);
            Assert.AreEqual(30, matches[3]);

            int replaces = await binary.ReplaceStrings(matchString, newString, Encoding.ASCII, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceStrings(matchString, newString, Encoding.ASCII, ReplaceMode.AllMatches);
            Assert.AreEqual(3, replaces);

            matches = await binary.FindAllStrings(newString, Encoding.ASCII);
            Assert.AreEqual(6, matches.Length);
        }

        [TestMethod]
        public async Task TestStringPatternOperations()
        {
            string testString = "This Thing is a throughout TesT";
            byte[] bytes = Encoding.ASCII.GetBytes(testString);

            string matchString = "Th";
            string newString = "th";
            string mask = "?x";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllStrings(matchString, mask, Encoding.ASCII);
            Assert.AreEqual(4, matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(5, matches[1]);
            Assert.AreEqual(16, matches[2]);
            Assert.AreEqual(21, matches[3]);

            int replaces = await binary.ReplaceStrings(matchString, newString, mask, Encoding.ASCII, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceStrings(matchString, newString, mask, Encoding.ASCII, ReplaceMode.AllMatches);
            Assert.AreEqual(4, replaces);

            matches = await binary.FindAllStrings(newString, Encoding.ASCII);
            Assert.AreEqual(4, matches.Length);
        }
    }
}
