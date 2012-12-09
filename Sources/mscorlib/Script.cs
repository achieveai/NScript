namespace System
{
    using System.Runtime.CompilerServices;

    [GlobalMethods, Extended, IgnoreNamespace]
    public static class Script
    {
        public static extern void Alert(bool b);
        public static extern void Alert(DateTime d);
        public static extern void Alert(Number n);
        public static extern void Alert(string message);
        public static extern bool Boolean(object o);
        public static extern bool Confirm(string message);
        public static extern object Eval(string s);

        [Script(@"return o === undefined;")]
        public static extern bool IsUndefined(object o);

        // TODO: make literal work
        /*
        public static object Literal(string script, params object[] args)
        {
            return null;
        }
        */

        public static extern string Prompt(string message);
        public static extern string Prompt(string message, string defaultValue);
    }
}

