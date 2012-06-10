namespace System
{
    using System.Runtime.CompilerServices;

    [Imported, IgnoreNamespace, ScriptName("String")]
    public struct Char
    {
        public static explicit operator string(char ch)
        {
            return null;
        }
    }
}

