using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.Lib.MetaData;

namespace NScript.Lib.ILParser
{
    static class AttributeParser
    {
        private static KeyValuePair<MethodSignature, byte[]>? GetAttribute(
            IList<string> lines,
            ref int lineIndex)
        {
            //.custom instance void [mscorlib]System.AttributeUsageAttribute::.ctor(valuetype [mscorlib]System.AttributeTargets) = ( 01 00 04 00 00 00 02 00 54 02 09 49 6E 68 65 72   // ........T..Inher
            //                                                                                                                       69 74 65 64 01 54 02 0D 41 6C 6C 6F 77 4D 75 6C   // ited.T..AllowMul
            //                                                                                                                       74 69 70 6C 65 00 )                               // tiple.

            if (lineIndex >= lines.Count)
            {
                return null;
            }

            StringFragment fragment = ParseUtils.GetNextWord(lines[lineIndex], 0);

            if (fragment != ".custom")
            {
                return null;
            }

            fragment = new StringFragment(
                fragment.ParentString,
                fragment.StartIndex + fragment.Length + 1,
                lines[lineIndex].IndexOf(')') - (fragment.Length + fragment.StartIndex));

            var signature = ParseUtils.ParseMethodSignature(fragment);

            // This will get us to = part of Attribute.
            //
            fragment = ParseUtils.GetNextWord(fragment);

            // This will get us to ( for data part of Attribute
            //
            fragment = ParseUtils.GetNextWord(fragment);

            fragment = ParseUtils.GetNextWord(fragment);

            bool readData = false;
            List<byte> data = new List<byte>();

            while (lineIndex < lines.Count && !readData)
            {
                if (fragment == (StringFragment)null)
                {
                    fragment = ParseUtils.GetNextWord(lines[lineIndex], 0);
                }

                while (!readData && fragment != (StringFragment)null)
                {
                    if (fragment == ")")
                    {
                        readData = true;
                    }
                    else if (fragment == "//")
                    {
                        fragment = null;
                    }
                    else
                    {
                        data.Add(
                            (byte)int.Parse(
                                (String)fragment,
                                System.Globalization.NumberStyles.AllowHexSpecifier));

                        fragment = ParseUtils.GetNextWord(fragment);
                    }
                }

                lineIndex++;
            }
                
            return new KeyValuePair<MethodSignature,byte[]>(
                signature,
                data.ToArray());
        }

        public static int GetAttributesLastLineIndex(
            IList<string> lines,
            int startIndex)
        {
            for (;
                startIndex < lines.Count && AttributeParser.IsAttributeLine(lines[startIndex]);
                startIndex++)
            {
                var word = ParseUtils.GetNextWord(lines[startIndex], 0);

                if (StringFragment.IsNull(word) || word.Length == 0)
                {
                    continue;
                }
            }
            return startIndex;
        }

        public static bool IsAttributeLine(string line)
        {
            var word = ParseUtils.GetNextWord(line, 0);
            return word == ".custom";
        }

        public static List<AttributeInfo> GetAttributes(
            IList<string> lines,
            int lineIndex)
        {
            List<AttributeInfo> returnValue = new List<AttributeInfo>();

            if (lines.Count == 0)
            {
                return returnValue;
            }

            for (var attribute = GetAttribute(lines, ref lineIndex);
                attribute.HasValue;
                attribute = GetAttribute(lines, ref lineIndex))
            {
                if (lineIndex < lines.Count)
                {
                    var word = ParseUtils.GetNextWord(lines[lineIndex], 0);

                    if (StringFragment.IsNull(word) || word.Length == 0 || word.StartsWith("//"))
                    {
                        continue;
                    }
                }

                returnValue.Add(
                    new AttributeInfo(
                        attribute.Value.Key,
                        attribute.Value.Value));
            }
            return returnValue;
        }
    }
}
