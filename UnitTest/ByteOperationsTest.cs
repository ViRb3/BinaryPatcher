using System;
using BinaryPatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class ByteOperationsTest
    {
        [TestMethod]
        public void TestBinaryConstruction()
        {
            byte[] bytes = new byte[] { 0x1, 0x2, 0x3 };
            Binary binary = new Binary(bytes);

            binary = new Binary(new MemoryStream(bytes));

            FileStream fileStream = File.Create("test.tmp");
            fileStream.Close();
            binary = new Binary("test.tmp");
        }

        [TestMethod]
        public async Task TestByteOperations()
        {
            byte[] bytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,
                                        0x1, 0x2,
                                        0x5, 0x1, 0x3,
                                        0x1, 0x2 };

            byte[] matchBytes = new byte[] { 0x1, 0x2 };
            byte[] newBytes = new byte[] { 0xff, 0xff };
            string mask = "xx";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllBytes(matchBytes, mask);
            Assert.AreEqual(3, matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(9, matches[1]);
            Assert.AreEqual(14, matches[2]);

            int replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.AllMatches);
            Assert.AreEqual(2, replaces);

            matches = await binary.FindAllBytes(newBytes, mask);
            Assert.AreEqual(3, matches.Length);
        }

        [TestMethod]
        public async Task TestByteStrOperations()
        {
            byte[] bytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,
                                        0x1, 0x2,
                                        0x5, 0x1, 0x3,
                                        0x1, 0x2 };

            byte[] matchBytes = Utils.StringToBytesRaw("\x01\x02");
            byte[] newBytes = Utils.StringToBytesRaw("\xff\xff");
            string mask = "xx";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllBytes(matchBytes, mask);
            Assert.AreEqual(3,  matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(9, matches[1]);
            Assert.AreEqual(14, matches[2]);

            int replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.AllMatches);
            Assert.AreEqual(2, replaces);

            matches = await binary.FindAllBytes(newBytes, mask);
            Assert.AreEqual(3, matches.Length);
        }

        [TestMethod]
        public async Task TestBytePatternOperations()
        {
            byte[] bytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,
                                        0x1, 0x2,
                                        0x5, 0x1, 0x3,
                                        0x1, 0x2 };

            byte[] matchBytes = new byte[] { 0x1, 0x2 };
            byte[] newBytes = new byte[] { 0xff, 0xff };
            string mask = "x?";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllBytes(matchBytes, mask);
            Assert.AreEqual(4, matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(9, matches[1]);
            Assert.AreEqual(12, matches[2]);
            Assert.AreEqual(14, matches[3]);

            int replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.AllMatches);
            Assert.AreEqual(3, replaces);

            matches = await binary.FindAllBytes(newBytes, mask);
            Assert.AreEqual(4, matches.Length);
        }

        [TestMethod]
        public async Task TestByteStrPatternOperations()
        {
            byte[] bytes = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,
                                        0x1, 0x2,
                                        0x5, 0x1, 0x3,
                                        0x1, 0x2 };

            byte[] matchBytes = Utils.StringToBytesRaw("\x01\x02");
            byte[] newBytes = Utils.StringToBytesRaw("\xff\xff");
            string mask = "x?";

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllBytes(matchBytes, mask);
            Assert.AreEqual(4, matches.Length);
            Assert.AreEqual(0, matches[0]);
            Assert.AreEqual(9, matches[1]);
            Assert.AreEqual(12, matches[2]);
            Assert.AreEqual(14, matches[3]);

            int replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.FirstMatch);
            Assert.AreEqual(1, replaces);

            replaces = await binary.ReplaceBytes(matchBytes, newBytes, mask, ReplaceMode.AllMatches);
            Assert.AreEqual(3, replaces);

            matches = await binary.FindAllBytes(newBytes, mask);
            Assert.AreEqual(4, matches.Length);
        }
    }
}
