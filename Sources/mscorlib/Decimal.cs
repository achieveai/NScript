namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, ScriptName("Number"), Extended]
    public struct Decimal
    {
        public extern Decimal(double d);

        public extern Decimal(int i);

        public extern Decimal(long n);

        public extern Decimal(float f);

        public extern string Format(string format);

        public extern string LocaleFormat(string format);

        [IntrinsicOperator]
        public extern static decimal operator +(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static decimal operator --(decimal d);

        [IntrinsicOperator]
        public extern static decimal operator /(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static bool operator ==(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static explicit operator double(decimal value);

        [IntrinsicOperator]
        public extern static explicit operator int(decimal value);

        [IntrinsicOperator]
        public extern static explicit operator long(decimal value);

        [IntrinsicOperator]
        public extern static explicit operator float(decimal value);

        [IntrinsicOperator]
        public extern static bool operator >(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static bool operator >=(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static implicit operator Number(decimal i);

        [IntrinsicOperator]
        public extern static implicit operator decimal(double value);

        [IntrinsicOperator]
        public extern static implicit operator decimal(int value);

        [IntrinsicOperator]
        public extern static implicit operator decimal(long value);

        [IntrinsicOperator]
        public extern static implicit operator decimal(float value);

        [IntrinsicOperator]
        public extern static decimal operator ++(decimal d);

        [IntrinsicOperator]
        public extern static bool operator !=(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static bool operator <(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static bool operator <=(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static decimal operator %(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static decimal operator *(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static decimal operator -(decimal d1, decimal d2);

        [IntrinsicOperator]
        public extern static decimal operator -(decimal d);

        [IntrinsicOperator]
        public extern static decimal operator +(decimal d);

        public extern string ToString(int radix);
    }
}

