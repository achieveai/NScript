namespace System
{
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss"), Extended]
    public sealed class CultureInfo
    {
        private CultureInfo()
        {
        }

        [PreserveCase, IntrinsicProperty]
        public static CultureInfo CurrentCulture
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public DateFormatInfo DateFormat
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty, PreserveCase]
        public static CultureInfo InvariantCulture
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public string Name
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public NumberFormatInfo NumberFormat
        {
            get
            {
                return null;
            }
        }
    }
}

