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

        [IntrinsicOperator]
        public extern static implicit operator byte(Number n);

        [IntrinsicOperator]
        public extern static implicit operator sbyte(Number n);

        [IntrinsicOperator]
        public extern static implicit operator char(Number n);

        [IntrinsicOperator]
        public extern static implicit operator short(Number n);

        [IntrinsicOperator]
        public extern static implicit operator ushort(Number n);

        [IntrinsicOperator]
        public extern static implicit operator int(Number n);

        [IntrinsicOperator]
        public extern static implicit operator uint(Number n);

        [IntrinsicOperator]
        public extern static implicit operator long(Number n);

        [IntrinsicOperator]
        public extern static implicit operator ulong(Number n);

        [IntrinsicOperator]
        public extern static implicit operator float(Number n);

        [IntrinsicOperator]
        public extern static implicit operator double(Number n);

        [IntrinsicOperator]
        public extern static implicit operator Number(byte i);

        [IntrinsicOperator]
        public extern static implicit operator Number(sbyte i);

        [IntrinsicOperator]
        public extern static implicit operator Number(char i);

        [IntrinsicOperator]
        public extern static implicit operator Number(short i);

        [IntrinsicOperator]
        public extern static implicit operator Number(ushort i);

        [IntrinsicOperator]
        public extern static implicit operator Number(int n);

        [IntrinsicOperator]
        public extern static implicit operator Number(uint n);

        [IntrinsicOperator]
        public extern static implicit operator Number(long n);

        [IntrinsicOperator]
        public extern static implicit operator Number(ulong n);

        [IntrinsicOperator]
        public extern static implicit operator Number(float n);

        [IntrinsicOperator]
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

