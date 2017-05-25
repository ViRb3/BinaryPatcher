﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BinaryPatcher
{
    public enum ReplaceMode
    {
        FirstMatch,
        LastMatch,
        AllMatches
    }

    public class Binary
    {
        private readonly Stream _binaryStream;

        public Binary(byte[] bytes)
        {
            _binaryStream = new MemoryStream(bytes);
        }

        public Binary(Stream stream)
        {
            _binaryStream = stream;
        }

        public Binary(string file)
        {
            _binaryStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite);
        }

        /// <summary>
        /// Mask wildcard is '?'. Ex.: "xxx??xx".
        /// Mask is matched with the raw bytes, not string characters.
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="encoding"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public async Task<long[]> FindAllStrings(string matchString, string mask, Encoding encoding)
        {
            byte[] matchBytes = encoding.GetBytes(matchString);
            return await DoFindBytes(matchBytes, mask);
        }

        public async Task<long[]> FindAllStrings(string matchString, Encoding encoding)
        {
            return await FindAllStrings(matchString, null, encoding);
        }

        /// <summary>
        /// Mask wildcard is '?'. Ex.: "xxx??xx".
        /// Mask is matched with the raw bytes, not string characters.
        /// </summary>
        /// <param name="matchString"></param>
        /// <param name="newString"></param>
        /// <param name="mask"></param>
        /// <param name="encoding"></param>
        /// <param name="replaceMode"></param>
        /// <returns></returns>
        public async Task<int> ReplaceStrings(string matchString, string newString, string mask, Encoding encoding, ReplaceMode replaceMode)
        {
            long[] matches = await FindAllStrings(matchString, mask, encoding);
            if (matches.Length < 1)
                return 0;

            int replaces = 0;

            switch (replaceMode)
            {
                case ReplaceMode.FirstMatch:
                    await DoReplaceBytes(encoding.GetBytes(newString), matches[0]);
                    break;
                case ReplaceMode.LastMatch:
                    await DoReplaceBytes(encoding.GetBytes(newString), matches[matches.Length - 1]);
                    break;
                case ReplaceMode.AllMatches:
                    foreach (long index in matches)
                        await DoReplaceBytes(encoding.GetBytes(newString), index);
                    replaces = matches.Length;
                    break;
            }

            if (replaceMode == ReplaceMode.FirstMatch || replaceMode == ReplaceMode.LastMatch)
                replaces = 1;

            return replaces;
        }

        public async Task<int> ReplaceStrings(string matchString, string newString, Encoding encoding, ReplaceMode replaceMode)
        {
            return await ReplaceStrings(matchString, newString, null, encoding, replaceMode);
        }

        /// <summary>
        /// Mask wildcard is '?'. Ex.: "xxx??xx".
        /// </summary>
        /// <param name="matchBytes"></param>
        /// <param name="newBytes"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public async Task<long[]> FindAllBytes(byte[] matchBytes, string mask)
        {
            return await DoFindBytes(matchBytes, mask);
        }

        public async Task<long[]> FindAllBytes(byte[] matchBytes)
        {
            return await DoFindBytes(matchBytes, null);
        }

        /// <summary>
        /// Mask wildcard is '?'. Ex.: "xxx??xx".
        /// </summary>
        /// <param name="matchBytes"></param>
        /// <param name="newBytes"></param>
        /// <param name="mask"></param>
        /// <param name="replaceMode"></param>
        /// <returns></returns>
        public async Task<int> ReplaceBytes(byte[] matchBytes, byte[] newBytes, string mask, ReplaceMode replaceMode)
        {
            long[] matches = await DoFindBytes(matchBytes, mask);
            if (matches.Length < 1)
                return 0;

            int replaces = 0;

            switch (replaceMode)
            {
                case ReplaceMode.FirstMatch:
                    await DoReplaceBytes(newBytes, matches[0]);
                    break;
                case ReplaceMode.LastMatch:
                    await DoReplaceBytes(newBytes, matches[matches.Length - 1]);
                    break;
                case ReplaceMode.AllMatches:
                    foreach (long index in matches)
                        await DoReplaceBytes(newBytes, index);
                    replaces = matches.Length;
                    break;
            }

            if (replaceMode == ReplaceMode.FirstMatch || replaceMode == ReplaceMode.LastMatch)
                replaces = 1;

            return replaces;
        }

        private async Task<long[]> DoFindBytes(byte[] matchBytes, string mask = null)
        {
            List<long> matches = new List<long>();

            while (await SeekToByteArray(_binaryStream, matchBytes, mask))
            {
                matches.Add(_binaryStream.Position);
                _binaryStream.Position += matchBytes.Length;
            }

            _binaryStream.Position = 0;

            return matches.ToArray();
        }

        private async Task DoReplaceBytes(byte[] newBytes, long startIndex)
        {
            _binaryStream.Position = startIndex;
            await _binaryStream.WriteAsync(newBytes, 0, newBytes.Length);
            _binaryStream.Position = 0;
        }

        private async Task<bool> SeekToByteArray(Stream stream, byte[] matchBytes, string mask = null)
        {
            if (mask != null && mask.Length != matchBytes.Length)
                throw new ArgumentException("Mask length isn't the same as byte sequence");
            if (matchBytes.Length > stream.Length)
                return false;

            byte[] buffer = new byte[1024];
            int i, matches = 0;
            while ((i = await stream.ReadAsync(buffer, 0, 1024)) >= matchBytes.Length)
            {
                matches = 0;
                for (int u = 0; u < buffer.Length; u++)
                {
                    if (matches == matchBytes.Length)
                    {
                        stream.Seek(-i + u - matchBytes.Length, SeekOrigin.Current);
                        return true; // fileStream.Position - i + u - byteSequence.Length;
                    }

                    if (buffer[u] == matchBytes[matches] || (mask != null && (mask[matches] == '?')))
                    {
                        matches++;
                    }
                    else
                    {
                        u -= matches;
                        matches = 0;
                    }
                }
                stream.Position -= matchBytes.Length - 1;
            }
            return false; // -1;
        }
    }
}