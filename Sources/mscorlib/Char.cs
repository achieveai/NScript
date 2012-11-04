namespace System
{
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace, ScriptName("String")]
    public struct Char
    {
        public static explicit operator string(char ch)
        {
            return null;
        }
    }
}

