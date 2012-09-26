namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, ScriptName("RegExp"), Imported]
    public sealed class RegularExpression
    {
        public extern RegularExpression(string pattern);

        public extern RegularExpression(string pattern, string flags);

        public extern string[] Exec(string s);

        public extern static RegularExpression Parse(string s);

        public extern bool Test(string s);

        [IntrinsicProperty]
        public extern bool Global
        {
            get;
        }

        [IntrinsicProperty]
        public extern bool IgnoreCase
        {
            get;
        }

        [IntrinsicProperty]
        public extern int LastIndex
        {
            get;
            set;
        }

        [IntrinsicProperty]
        public extern bool Multiline
        {
            get;
        }

        [IntrinsicProperty]
        public extern string Pattern
        {
            get;
        }
    }
}

