namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, ScriptName("RegExp"), Imported]
    public sealed class RegularExpression
    {
        public RegularExpression(string pattern)
        {
        }

        public RegularExpression(string pattern, string flags)
        {
        }

        public string[] Exec(string s)
        {
            return null;
        }

        public static RegularExpression Parse(string s)
        {
            return null;
        }

        public bool Test(string s)
        {
            return false;
        }

        [IntrinsicProperty]
        public bool Global
        {
            get
            {
                return false;
            }
        }

        [IntrinsicProperty]
        public bool IgnoreCase
        {
            get
            {
                return false;
            }
        }

        [IntrinsicProperty]
        public int LastIndex
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        [IntrinsicProperty]
        public bool Multiline
        {
            get
            {
                return false;
            }
        }

        [IntrinsicProperty]
        public string Pattern
        {
            get
            {
                return null;
            }
        }
    }
}

