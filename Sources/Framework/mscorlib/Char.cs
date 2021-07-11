namespace System
{
    using System.Runtime.CompilerServices;

    public struct Char
    {
        public static explicit operator string(char ch)
        {
            return string.FromCharCode(ch) ;
        }

        public override string ToLocaleString()
        {
            return string.FromCharCode(this);
        }

        public override string ToString()
        {
            return string.FromCharCode(this);
        }
    }
}

