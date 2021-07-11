namespace System.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, ScriptNamespace("ss")]
    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
        }

        [Conditional("DEBUG")]
        public static void Fail(string message)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceDump(object o)
        {
        }

        [Conditional("DEBUG")]
        public static void TraceDump(object o, string name)
        {
        }

        [Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
        }
    }
}

