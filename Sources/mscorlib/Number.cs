namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class Number
    {
        [PreserveCase]
        public const int MAX_VALUE = 0;
        [PreserveCase]
        public const int MIN_VALUE = 0;
        [PreserveCase]
        public const int NaN = 0;
        [PreserveCase]
        public const int NEGATIVE_INFINITY = 0;
        [PreserveCase]
        public const int POSITIVE_INFINITY = 0;

        public extern Number();

        public extern string Format(string format);

        public extern static bool IsFinite(Number n);

        public extern static bool IsNaN(Number n);

        public extern string LocaleFormat(string format);

        public extern static implicit operator int(Number n);

        public extern static implicit operator long(Number n);

        public extern static implicit operator float(Number n);

        public extern static implicit operator double(Number n);

        public extern static implicit operator Number(int n);

        public extern static implicit operator Number(long n);

        public extern static implicit operator Number(float n);

        public extern static implicit operator Number(double n);

        [ScriptAlias("parseInt")]
        public extern static Number ParseInt(string s);

        [ScriptAlias("parseFloat")]
        public extern static Number ParseFloat(string s);

        public extern string ToExponential();

        public extern string ToExponential(int fractionDigits);

        public extern string ToFixed();

        public extern string ToFixed(int fractionDigits);

        public extern string ToPrecision();

        public extern string ToPrecision(int precision);

        public extern string ToString(int radix);

        public extern override string ToString();
    }
}

