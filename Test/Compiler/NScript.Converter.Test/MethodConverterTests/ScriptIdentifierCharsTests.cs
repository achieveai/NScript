namespace NScript.Converter.Test.MethodConverterTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// ScriptIdentifierChars is generated from JavaScript.g3. Every BMP character must be an
    /// identifier character exactly when the grammar's UnicodeLetter, UnicodeDigit or
    /// UnicodeConnectorPunctuation fragment (or '$' / '_') lists it.
    /// </summary>
    [TestClass]
    public class ScriptIdentifierCharsTests
    {
        [TestMethod]
        public void Contains_MatchesTheGrammarForEveryBmpCharacter()
        {
            var grammar = File.ReadAllText(FindGrammar());
            var allowed = new bool[0x10000];
            allowed['$'] = true;
            allowed['_'] = true;
            foreach (var name in new[] { "UnicodeLetter", "UnicodeDigit", "UnicodeConnectorPunctuation" })
            {
                var fragment = Regex.Match(grammar, @"fragment\s+" + name + @"\b[^:]*:(.*?)\n\s*;", RegexOptions.Singleline);
                Assert.IsTrue(fragment.Success, "fragment " + name + " is in JavaScript.g3");
                var body = Regex.Replace(fragment.Groups[1].Value, "//[^\n]*", string.Empty);
                foreach (Match range in Regex.Matches(body, @"'\\u([0-9A-Fa-f]{4})'(?:\s*\.\.\s*'\\u([0-9A-Fa-f]{4})')?"))
                {
                    var lo = Convert.ToInt32(range.Groups[1].Value, 16);
                    var hi = range.Groups[2].Success ? Convert.ToInt32(range.Groups[2].Value, 16) : lo;
                    for (var ch = lo; ch <= hi; ch++)
                    {
                        allowed[ch] = true;
                    }
                }
            }

            var mismatches = new List<string>();
            for (var ch = 0; ch < 0x10000 && mismatches.Count < 10; ch++)
            {
                if (ScriptIdentifierChars.Contains((char)ch) != allowed[ch])
                {
                    mismatches.Add(string.Format("U+{0:X4} grammar={1}", ch, allowed[ch]));
                }
            }

            Assert.AreEqual(string.Empty, string.Join(", ", mismatches));
        }

        private static string FindGrammar()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                var path = Path.Combine(dir.FullName, "Sources", "Compiler", "NScript.JSParser", "JavaScript.g3");
                if (File.Exists(path))
                {
                    return path;
                }
            }

            Assert.Fail("JavaScript.g3 not found above " + AppContext.BaseDirectory);
            return null;
        }
    }
}
