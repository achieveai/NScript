//-----------------------------------------------------------------------
// <copyright file="NamedArgumentTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;

    public class NamedArgLog
    {
        public string Text = "";

        public string Add(string s)
        {
            Text += s;
            return s;
        }
    }

    public static class NamedArgExtensions
    {
        public static NamedArgLog Then(this NamedArgLog log, string a, string b)
        {
            log.Add(a + b + ";");
            return log;
        }

        public static NamedArgLog Finish(this NamedArgLog log, Func<string> content, string name)
        {
            log.Add(name + "=" + content() + ";");
            return log;
        }
    }

    /// <summary>
    /// Named arguments out of parameter order are copied into temps. A call
    /// nested in another argument, or in the receiver, must not overwrite
    /// the outer call's temps.
    /// </summary>
    [TestFixture]
    public class NamedArgumentTests
    {
        private static string Content() => "c";

        private static NamedArgLog Make(string a, string b, NamedArgLog log)
        {
            log.Add("make" + a + b + ";");
            return log;
        }

        private static string Join(string x, string a, string b) => x + "|" + a + b;

        [Test]
        public static void FluentChainKeepsOuterArguments(Assert assert)
        {
            var log = new NamedArgLog();

            // The shape that broke: each link reorders its arguments, and
            // the last link takes a Func.
            log.Then(b: log.Add("1"), a: log.Add("2"))
                .Then(b: log.Add("3"), a: log.Add("4"))
                .Finish(name: log.Add("n"), content: Content);

            assert.Equal(log.Text, "1221;3443;nn=c;", "each call gets its own arguments, in source order");
        }

        [Test]
        public static void NestedArgumentKeepsOuterArguments(Assert assert)
        {
            var log = new NamedArgLog();
            var r = Join(Join("x", b: log.Add("1"), a: log.Add("2")), b: log.Add("3"), a: log.Add("4"));

            assert.Equal(r, "x|21|43", "inner and outer calls keep their own values");
            assert.Equal(log.Text, "1234", "arguments run in source order");
        }

        [Test]
        public static void NestedReceiverKeepsOuterArguments(Assert assert)
        {
            var log = new NamedArgLog();
            Make(b: log.Add("1"), a: log.Add("2"), log: log).Then(b: log.Add("3"), a: log.Add("4"));

            assert.Equal(log.Text, "12make21;3443;", "receiver runs first and keeps the outer values");
        }
    }
}
