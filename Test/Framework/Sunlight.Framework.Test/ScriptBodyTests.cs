//-----------------------------------------------------------------------
// <copyright file="ScriptBodyTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System.Runtime.CompilerServices;
    using SunlightUnit;

    /// <summary>
    /// [Script] bodies with try/catch, 'for (var ...)', regex literals and multi-property
    /// object literals compile and run (#136).
    /// </summary>
    [TestFixture]
    public class ScriptBodyTests
    {
        [Script(@"try { return @:JSON.parse(text).v; } catch (e) { return -1; }")]
        private static extern int ParseOrDefault(string text);

        [Script(@"try { a = a + 1; } finally { a = a * 10; } return a;")]
        private static extern int TryFinally(int a);

        [Script(@"var n = 0; for (var i = 0; i < a; i = i + 1) { n = n + i; } return n;")]
        private static extern int SumBelow(int a);

        [Script(@"return /^a+$/.test(s);")]
        private static extern bool OnlyAs(string s);

        [Script(@"var o = { x: a, y: a + 1, z: a + 2 }; return o.x + o.y + o.z;")]
        private static extern int SumOfThree(int a);

        [Test]
        public static void TryCatchCatchesTheThrow(Assert assert)
        {
            assert.Equal(ParseOrDefault("{\"v\":7}"), 7, "try body result");
            assert.Equal(ParseOrDefault("not json"), -1, "catch result");
            assert.Equal(TryFinally(1), 20, "finally runs after try");
        }

        [Test]
        public static void ForVarLoops(Assert assert)
        {
            assert.Equal(SumBelow(4), 6, "0+1+2+3");
        }

        [Test]
        public static void RegexLiteralMatches(Assert assert)
        {
            assert.Equal(OnlyAs("aaa"), true, "matches");
            assert.Equal(OnlyAs("ab"), false, "does not match");
        }

        [Test]
        public static void ObjectLiteralKeepsEveryProperty(Assert assert)
        {
            assert.Equal(SumOfThree(1), 6, "1+2+3");
            assert.Equal(Join3(Make3("x", "y", "z")), "x|y|z", "#142: returned literal keeps the middle property");
        }

        [Script(@"return { a: x, b: y, c: z };")]
        private static extern object Make3(string x, string y, string z);

        [Script(@"return o.a + '|' + o.b + '|' + o.c;")]
        private static extern string Join3(object o);
    }
}
