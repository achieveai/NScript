namespace System
{
    using System.Runtime.CompilerServices;

    public struct Byte
    {
        public extern string Format(string format);

        public extern string LocaleFormat(string format);

        [IntrinsicOperator]
        public static extern implicit operator Number(byte i);

        public extern string ToString(int radix);
    }
}

