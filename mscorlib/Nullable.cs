//-----------------------------------------------------------------------
// <copyright file="Nullable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Runtime.CompilerServices;

    // Summary:
    //     Supports a value type that can be assigned null like a reference type. This
    //     class cannot be inherited.
    public static class Nullable
    {
        // Summary:
        //     Compares the relative values of two System.Nullable<T> objects.
        //
        // Parameters:
        //   n1:
        //     A System.Nullable<T> object.
        //
        //   n2:
        //     A System.Nullable<T> object.
        //
        // Type parameters:
        //   T:
        //     The underlying value type of the n1 and n2 parameters.
        //
        // Returns:
        //     An integer that indicates the relative values of the n1 and n2 parameters.Return
        //     ValueDescriptionLess than zeroThe System.Nullable<T>.HasValue property for
        //     n1 is false, and the System.Nullable<T>.HasValue property for n2 is true.-or-The
        //     System.Nullable<T>.HasValue properties for n1 and n2 are true, and the value
        //     of the System.Nullable<T>.Value property for n1 is less than the value of
        //     the System.Nullable<T>.Value property for n2.ZeroThe System.Nullable<T>.HasValue
        //     properties for n1 and n2 are false.-or-The System.Nullable<T>.HasValue properties
        //     for n1 and n2 are true, and the value of the System.Nullable<T>.Value property
        //     for n1 is equal to the value of the System.Nullable<T>.Value property for
        //     n2.Greater than zeroThe System.Nullable<T>.HasValue property for n1 is true,
        //     and the System.Nullable<T>.HasValue property for n2 is false.-or-The System.Nullable<T>.HasValue
        //     properties for n1 and n2 are true, and the value of the System.Nullable<T>.Value
        //     property for n1 is greater than the value of the System.Nullable<T>.Value
        //     property for n2.
        public static extern int Compare<T>(T? n1, T? n2) where T : struct;

        //
        // Summary:
        //     Indicates whether two specified System.Nullable<T> objects are equal.
        //
        // Parameters:
        //   n1:
        //     A System.Nullable<T> object.
        //
        //   n2:
        //     A System.Nullable<T> object.
        //
        // Type parameters:
        //   T:
        //     The underlying value type of the n1 and n2 parameters.
        //
        // Returns:
        //     true if the n1 parameter is equal to the n2 parameter; otherwise, false.
        //     The return value depends on the System.Nullable<T>.HasValue and System.Nullable<T>.Value
        //     properties of the two parameters that are compared.Return ValueDescriptiontrueThe
        //     System.Nullable<T>.HasValue properties for n1 and n2 are false. -or-The System.Nullable<T>.HasValue
        //     properties for n1 and n2 are true, and the System.Nullable<T>.Value properties
        //     of the parameters are equal.falseThe System.Nullable<T>.HasValue property
        //     is true for one parameter and false for the other parameter.-or-The System.Nullable<T>.HasValue
        //     properties for n1 and n2 are true, and the System.Nullable<T>.Value properties
        //     of the parameters are unequal.
        public static extern bool Equals<T>(T? n1, T? n2) where T : struct;
    }

    public struct Nullable<T> where T : struct
    {
        [Script(@" return value; ")]
        public extern Nullable(T value);

        public bool HasValue
        {
            get { return this.GetHasValue(); }
        }

        public T Value
        {
            get
            {
                if (this.GetHasValue())
                {
                    return this.GetValue();
                }

                throw new Exception("Nullable object must have a value");
            }
        }

        public bool Equals(object other)
        {
            if (other == null)
                return !this.HasValue;
            if (!(other is Nullable<T>))
                return false;

            return Equals((Nullable<T>)other);
        }

        public bool Equals(Nullable<T> other)
        {
            if (other.HasValue != this.HasValue)
                return false;

            if (this.HasValue == false)
                return true;

            return object.Equals(other.GetValue(), this.GetValue());
        }

        public T GetValueOrDefault()
        {
            return this.HasValue ? this.GetValue() : default(T);
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return this.HasValue ? this.GetValue() : defaultValue;
        }

        public override string ToString()
        {
            if (this.HasValue)
                return this.GetValue().ToString();
            else
                return String.Empty;
        }

        [Script(@" return value; ")]
        public extern static implicit operator Nullable<T>(T value);

        public static explicit operator T(Nullable<T> value)
        {
            return value.Value;
        }

        [Script(@" return this; ")]
        private extern T GetValue();

        [Script(@" return this !== null; ")]
        private extern bool GetHasValue();

        [Script(@"return o === null ? null : @{T}.@{[mscorlib]System.Type::Box([mscorlib]System.Object)}(o);")]
        private extern static object Box(T? o);

        private static T? Unbox(object o)
        {
            return o == null ? (T?)null : (T)o;
        }
    }
}