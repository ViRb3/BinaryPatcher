using System;
using System.Linq;

namespace BinaryPatcher
{
    public static class Utils
    {
        /// <summary>
        /// Converts a string to byte array without encoding.
        /// Ex.: "\xA1\x10" => 0xA1, 0x10
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Byte representation of input string.</returns>
        public static byte[] StringToBytesRaw(string input)
        {
            return input.ToCharArray().Select(s => Convert.ToByte((int)s)).ToArray();
        }
    }
}