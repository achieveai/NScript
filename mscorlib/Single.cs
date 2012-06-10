namespace System
{
    using System.Runtime.CompilerServices;

    public struct Single
    {
        [Script("return this.toString(radix);")]
        public extern string ToString(int radix);

        [Script("return parseFloat(s);")]
        public extern static float Parse(string s);

        public string Format(string format)
        {
            return this.ToPrecision();
        }

        public string LocaleFormat(string format)
        {
            return this.Format(format);
        }

        [Script("return i;")]
        public extern static implicit operator Number(float i);

        [Script("return this.toExponential();")]
        public extern string ToExponential();

        [Script("return this.toExponential(fractionDigits);")]
        public extern string ToExponential(int fractionDigits);

        [Script("return this.toFixed();")]
        public extern string ToFixed();

        [Script("return this.toFixed(fractionDigits);")]
        public extern string ToFixed(int fractionDigits);

        [Script("return this.toPrecision();")]
        public extern string ToPrecision();

        [Script("return this.toPrecision(precision);")]
        public extern string ToPrecision(int precision);

        [Script("return this.toString();")]
        public extern override string ToString();

        public override string ToLocaleString()
        {
            return this.ToString();
        }
    }
}