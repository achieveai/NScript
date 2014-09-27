namespace System
{
    using System.Runtime.CompilerServices;

    public struct SByte
    {
        [IntrinsicOperator]
        public extern static implicit operator Number(sbyte i);

        [Script("return parseInt(s);")]
        public extern static sbyte Parse(string s);

        [Script("return parseInt(s, radix);")]
        public extern static sbyte Parse(string s, int radix);

        public string Format(string format)
        {
            return this.ToString(10);
        }

        public string LocaleFormat(string format)
        {
            return this.Format(format);
        }

        [Script("return this.toString(radix);")]
        public extern string ToString(int radix);

        public override string ToString()
        {
            return this.ToString(10);
        }

        public override string ToLocaleString()
        {
            return this.ToString();
        }
    }
}

