using System;
using BinaryPatcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class SplitMatchTest
    {
        [TestMethod]
        public async Task TestSplitMatchInTwoBlocks()
        {
            byte[] bytes = new byte[1024 * 2]; // simulate two blocks
            // block 1
            bytes[1022] = 1;
            bytes[1023] = 2;
            // block 2
            bytes[1024] = 3;
            bytes[1025] = 4;

            byte[] matchBytes = new byte[] { 0x1, 0x2, 0x3, 0x4 };

            Binary binary = new Binary(bytes);

            long[] matches = await binary.FindAllBytes(matchBytes);
            Assert.AreEqual(1, matches.Length);
            Assert.AreEqual(1022, matches[0]);
        }
    }
}
