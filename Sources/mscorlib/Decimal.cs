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

        public extern static decimal operator +(decimal d1, decimal d2);

        public extern static decimal operator --(decimal d);

        public extern static decimal operator /(decimal d1, decimal d2);

        public extern static bool operator ==(decimal d1, decimal d2);

        public extern static explicit operator double(decimal value);

        public extern static explicit operator int(decimal value);

        public extern static explicit operator long(decimal value);

        public extern static explicit operator float(decimal value);

        public extern static bool operator >(decimal d1, decimal d2);

        public extern static bool operator >=(decimal d1, decimal d2);

        public extern static implicit operator Number(decimal i);

        public extern static implicit operator decimal(double value);

        public extern static implicit operator decimal(int value);

        public extern static implicit operator decimal(long value);

        public extern static implicit operator decimal(float value);

        public extern static decimal operator ++(decimal d);

        public extern static bool operator !=(decimal d1, decimal d2);

        public extern static bool operator <(decimal d1, decimal d2);

        public extern static bool operator <=(decimal d1, decimal d2);

        public extern static decimal operator %(decimal d1, decimal d2);

        public extern static decimal operator *(decimal d1, decimal d2);

        public extern static decimal operator -(decimal d1, decimal d2);

        public extern static decimal operator -(decimal d);

        public extern static decimal operator +(decimal d);

        public extern string ToString(int radix);
    }
}

