//-----------------------------------------------------------------------
// <copyright file="StringConformanceTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using SunlightUnit;

    /// <summary>
    /// mscorlib string methods must match .NET (#129, #130).
    /// </summary>
    [TestFixture]
    public class StringConformanceTests
    {
        [Test]
        public static void TrimEndTrimsOnlyTheEnd(Assert assert)
        {
            assert.Equal("  a  ".TrimEnd(), "  a", "TrimEnd trims the end");
            assert.Equal("  a  ".TrimStart(), "a  ", "TrimStart trims the start");
            assert.Equal("  a  ".Trim(), "a", "Trim trims both");
        }

        [Test]
        public static void ReplaceReplacesEveryMatch(Assert assert)
        {
            assert.Equal("a-b-c".Replace("-", "+"), "a+b+c", "Replace replaces every match");
            assert.Equal("a.b".Replace(".", "$&"), "a$&b", "replacement text is literal");
            assert.Equal("a-b".Replace("-", null), "ab", "null replacement removes");
            assert.Equal("a-b-c".ReplaceFirst("-", "+"), "a+b-c", "ReplaceFirst still replaces one");
            assert.Equal("a.b.c".ReplaceFirst(".", "$&"), "a$&b.c", "ReplaceFirst replacement is literal");
            assert.Equal("abc".ReplaceFirst("x", "y"), "abc", "ReplaceFirst with no match");
            assert.Throws(() => "abc".Replace("", "x"), (object e) => true, "empty oldText throws, as in .NET");
        }
    }
}
