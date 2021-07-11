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

        [ScriptAlias("atob")]
        private static extern Func<string, string> browserAtoB { get; }

        [ScriptAlias("btoa")]
        private static extern Func<string, string> browserBtoA { get; }

        private const string b64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        private static string AtoB(string str)
        {
            if (!object.IsNullOrUndefined(browserAtoB))
            { return browserAtoB(str); }

            NativeArray<string> strParts = new NativeArray<string>((str.Length * 3) / 4);
            for (int iStr = 0, jStr = 0; iStr < str.Length; iStr+=4)
            {
                int i1 = b64Code.IndexOf(str.CharCodeAt(iStr));
                int i2 = b64Code.IndexOf(str.CharCodeAt(iStr + 1));
                int i3 = b64Code.IndexOf(str.CharCodeAt(iStr + 2));
                int i4 = b64Code.IndexOf(str.CharCodeAt(iStr + 3));

                int bits = i1 << 18 + i2 << 12 + i3 << 6 + i4;
                strParts[jStr++] = String.FromCharCode((char)((bits >> 16) & 0xff));
                strParts[jStr++] = String.FromCharCode((char)((bits >> 8) & 0xff));
                strParts[jStr++] = String.FromCharCode((char)(bits & 0xff));
            }

            return strParts.Join();
        }

        private static string BtoA(string str)
        {
            if (!object.IsNullOrUndefined(browserBtoA))
            { return browserBtoA(str); }

            int iStr = 0, jStr = 0, i1, i2, i3, word;
            NativeArray<string> strParts = new NativeArray<string>((str.Length * 4) / 3);

            for (; iStr < str.Length; iStr += 3)
            {
                i1 = str.CharCodeAt(iStr);
                i2 = str.CharCodeAt(iStr + 1);
                i3 = str.CharCodeAt(iStr + 2);

                word = i1 << 16 + i2 << 8 + i3;

                strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
                strParts[jStr++] = b64Code.At((word >> 6) & 0x3f);
                strParts[jStr++] = b64Code.At(word & 0x3f);
            }

            word = 0;
            if (iStr < str.Length + 3)
            { word = str.CharCodeAt(iStr) << 16; }

            if (iStr < str.Length + 2)
            { word += str.CharCodeAt(iStr + 1) << 8; }

            if (iStr < str.Length + 1)
            { word += str.CharCodeAt(iStr + 2); }

            strParts[jStr++] = b64Code.At((word >> 18) & 0x3f);
            strParts[jStr++] = b64Code.At((word >> 12) & 0x3f);
            strParts[jStr++] = b64Code.At((word >> 6) & 0x3f);
            strParts[jStr++] = b64Code.At(word & 0x3f);

            return strParts.Join();
        }
    }
}
