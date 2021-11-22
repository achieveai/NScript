//-----------------------------------------------------------------------
// <copyright file="Blob.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    public class BlobCreateOptions
    {
        public extern string Type { get; set; }
    }

    /// <summary>
    /// Definition for Blob
    /// </summary>
    [IgnoreNamespace, ScriptName("Blob")]
    public class Blob
    {
        public extern Blob(NativeArray array, BlobCreateOptions options = null);

        public extern Blob(Blob blob, BlobCreateOptions options = null);

        public extern Blob(string blob, BlobCreateOptions options = null);

        public extern bool IsClosed { get; set; }

        public extern int Size { get; set; }

        public extern string Type { get; set; }

        public extern Action Close { get; }

        [ScriptName("slice")]
        public extern bool? HasSlice { get; }

        public extern Blob Slice(int start);

        public extern Blob Slice(int start, int end);

        public extern Blob Slice(int start, int end, string ContentType);

        public static Blob FromBase64String(string base64Data, string contentType = "")
        {

            return new Blob(
                (NativeArray)(object)Blob.ArrayFromBase64String(base64Data),
                new BlobCreateOptions{ Type= contentType });
        }

        public static Uint8Array ArrayFromBase64String(string base64Data)
        {
            var byteCharacters = AtoB(base64Data);

            var bytes = new Uint8Array(byteCharacters.Length);
            for (int offset = 0, i = 0 ; offset < byteCharacters.Length; ++i, ++offset) {
                bytes[i] = (byte)byteCharacters[offset];
            }

            return bytes;
        }

        private static readonly RegularExpression base64CleanerRegex
            = new RegularExpression("[^A-Za-z0-9\\+\\/]", "g");

        public static NativeArray<byte> Base64DecToArr (string sBase64)
        {
            var sB64Enc = sBase64.Replace(base64CleanerRegex, "");
            var nInLen = sB64Enc.Length;
            var nOutLen = nInLen * 3 + 1 >> 2;
            var taBytes = new Uint8Array(nOutLen);

            for (int nMod3 = 0, nMod4 = 0, nUint24 = 0, nOutIdx = 0, nInIdx = 0;
                nInIdx < nInLen;
                nInIdx++)
            {
                nMod4 = nInIdx & 3;
                nUint24 |= B64ToUint6((char)(sB64Enc.CharCodeAt(nInIdx) << 6 * (3 - nMod4)));
                if (nMod4 == 3 || nInLen - nInIdx == 1)
                {
                    for (nMod3 = 0; nMod3 < 3 && nOutIdx < nOutLen; nMod3++, nOutIdx++)
                    {
                        taBytes[nOutIdx] = (byte)(nUint24 >> (16 >> nMod3 & 24) & 255);
                    }

                    nUint24 = 0;
                }
            }

            return taBytes;
        }

        public static string Base64EncArr (NativeArray<byte> aBytes)
        {
            var nMod3 = 2;
            var sB64Enc = "";
            var nUint24 = 0ul;
            for (int nLen = aBytes.Length, nIdx = 0;
                nIdx < nLen;
                nIdx++)
            {
                nMod3 = nIdx % 3;
                nUint24 |= ((ulong)aBytes[nIdx]) << (16 >> nMod3 & 24);
                if (nMod3 == 2 || aBytes.Length - nIdx == 1)
                {
                    sB64Enc += String.FromCharCode(
                        Uint6ToB64((byte)(nUint24 >> 18 & 63)),
                        Uint6ToB64((byte)(nUint24 >> 12 & 63)),
                        Uint6ToB64((byte)(nUint24 >> 6 & 63)),
                        Uint6ToB64((byte)(nUint24 & 63)));
                    nUint24 = 0;
                }
            }

            return sB64Enc.Substring(0, sB64Enc.Length - 2 + nMod3)
                + (nMod3 == 2 ? "" : nMod3 == 1 ? "=" : "==");
        }

        public static string ToBase64String(NativeArray<byte> array)
        {
            int iStr = 0, jStr = 0, i1, i2, i3, word;
            NativeArray<string> strParts = new NativeArray<string>((array.Length * 4) / 3);

            for (; iStr < array.Length - 3; iStr += 3)
            {
                i1 = array[iStr];
                i2 = array[iStr + 1];
                i3 = array[iStr + 2];

                word = (i1 << 16) + (i2 << 8) + i3;

                strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 6) & 0x3f);
                strParts[jStr++] = b64Code.At(word & 0x3f);
            }

            if (iStr < array.Length)
            {
                word = array[iStr] << 16;

                if (iStr < array.Length - 2)
                { word += array[iStr + 1] << 8; }

                if (iStr < array.Length - 1)
                { word += array[iStr + 2]; }

                strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
                strParts[jStr++] = array.Length % 3 == 2 ? b64Code.At((word >> 6) & 0x3f) : "=";
                strParts[jStr++] = "=";
            }

            var rv = strParts.Join(string.Empty);
            /*
            var rv2 = Base64EncArr(array);

            if (rv != rv2)
            {
                throw new Exception("Error matching base64 string");
            }
            */

            return rv;
        }

        [ScriptAlias("atob")]
        private static extern Func<string, string> browserAtoB { get; }

        [ScriptAlias("btoa")]
        private static extern Func<string, string> browserBtoA { get; }

        private const string b64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        private static string AtoB(string str)
        {
            if (!object.IsNullOrUndefined(browserAtoB))
            {
                return browserAtoB(str);
            }

            NativeArray<string> strParts = new NativeArray<string>((str.Length * 3) / 4);
            for (int iStr = 0, jStr = 0; iStr < str.Length; iStr+=4)
            {
                int i1 = b64Code.IndexOf(str.CharCodeAt(iStr));
                int i2 = b64Code.IndexOf(str.CharCodeAt(iStr + 1));
                int i3 = b64Code.IndexOf(str.CharCodeAt(iStr + 2));
                int i4 = b64Code.IndexOf(str.CharCodeAt(iStr + 3));

                int bits = (i1 << 18) + (i2 << 12) + (i3 << 6) + i4;
                strParts[jStr++] = String.FromCharCode((char)((bits >> 16) & 0xff));
                strParts[jStr++] = String.FromCharCode((char)((bits >> 8) & 0xff));
                strParts[jStr++] = String.FromCharCode((char)(bits & 0xff));
            }

            return strParts.Join(string.Empty);
        }

        private static string BtoA(string str)
        {
            if (!object.IsNullOrUndefined(browserBtoA))
            { return browserBtoA(str); }

            int iStr = 0, jStr = 0, i1, i2, i3, word;
            NativeArray<string> strParts = new NativeArray<string>((str.Length * 4) / 3);

            for (; iStr < str.Length - 3; iStr += 3)
            {
                i1 = str.CharCodeAt(iStr);
                i2 = str.CharCodeAt(iStr + 1);
                i3 = str.CharCodeAt(iStr + 2);

                word = (i1 << 16) + (i2 << 8) + i3;

                strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 6) & 0x3f);
                strParts[jStr++] = b64Code.At(word & 0x3f);
            }

            if (iStr < str.Length)
            {
                word = 0;
                word = str.CharCodeAt(iStr) << 16;

                if (iStr < str.Length - 2)
                {
                    word += str.CharCodeAt(iStr + 1) << 8;
                }

                if (iStr < str.Length - 1)
                {
                    word += str.CharCodeAt(iStr + 2);
                }

                strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
                strParts[jStr++] = str.Length % 3 == 2 ? b64Code.At((word >> 6) & 0x3f) : "=";
                strParts[jStr++] = "=";
            }

            return strParts.Join(string.Empty);
        }

        private static char Uint6ToB64 (byte nUint6)
        {
            return (char)(nUint6 < 26
                ? nUint6 + 65
                : nUint6 < 52
                ? nUint6 + 71
                : nUint6 < 62
                ? nUint6 - 4
                : nUint6 == 62
                ? 43
                : nUint6 == 63
                ? 47
                : 65);
        }
        private static byte B64ToUint6 (char nChr)
        {

            return (byte)(nChr > 64 && nChr < 91
                ? nChr - 65
                : nChr > 96 && nChr < 123
                ? nChr - 71
                : nChr > 47 && nChr < 58
                ? nChr + 4
                : nChr == 43
                ? 62
                : nChr == 47
                ? 63
                : 0);

        }
    }
}
