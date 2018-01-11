//-----------------------------------------------------------------------
// <copyright file="NativeArray.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for NativeArray
    /// </summary>
    [ScriptName("Array"), IgnoreNamespace]
    public class NativeArray
    {
        public extern NativeArray(int length);

        [IntrinsicProperty]
        public extern int Length
        {
            get;
            set;
        }

        public extern object this[int i]
        { get; set; }

        public T[] GetArray<T>()
        {
            return (T[])(object)new ArrayG<T>(this.ToSpecific<T>());
        }

        [IgnoreGenericArguments]
        [Script("return array ? array.@{[mscorlib]System.ArrayG`1::innerArray} : null;")]
        public static extern NativeArray GetNativeArray<T>(T[] array);

        [IgnoreGenericArguments]
        [Script("return array ? array.@{[mscorlib]System.Collections.Generic.List`1::nativeArray} : null;")]
        public static extern NativeArray GetNativeArray<T>(List<T> array);

        [Script(@"
            return this.@{[mscorlib]System.NativeArray::ConcatImpl()}.apply(
                this,
                arrays.@{[mscorlib]System.Collections.Generic.List`1::nativeArray});")]
        public extern NativeArray Concat(params NativeArray[] arrays);

        [Script(@"return this.concat(array);")]
        public extern NativeArray Concat(NativeArray array);

        [Script(@"return this.concat(array, array2);")]
        public extern NativeArray Concat(NativeArray array, NativeArray array2);

        [Script(@"return this.concat(array, array2, array3);")]
        public extern NativeArray Concat(NativeArray array, NativeArray array2, NativeArray array3);

        public extern string Join();

        public extern string Join(string seperator);

        [IgnoreGenericArguments]
        [Script(@"return this;")]
        public extern NativeArray<T> ToSpecific<T>();

        [IgnoreGenericArguments]
        [Script(@"return this.pop();")]
        public extern T Pop<T>();

        [IgnoreGenericArguments]
        [Script(@"return this.push(value);")]
        public extern int Push<T>(T value);

        [IgnoreGenericArguments]
        [Script(@"return this.shift();")]
        public extern T Shift<T>();

        [IgnoreGenericArguments]
        [Script(@"return this.unshift(value);")]
        public extern void Unshift<T>(T value);

        public extern void Reverse();

        public extern NativeArray Slice(int start, int end);

        [IgnoreGenericArguments]
        [Script(@"
            var newArray = elements.@{[mscorlib]System.ArrayG`1::innerArray}.slice(0);
            newArray.unshift(howMany);
            newArray.unshift(index);
            return this.splice.apply(this, newArray);
            ")]
        public extern NativeArray Splice<T>(int index, int howMany, params T[] elements);

        [IgnoreGenericArguments]
        [Script(@"
            var i;
            startIndex = startIndex < 0 ? 0 : startIndex;
            return this.indexOf(value, startIndex);
            ")]
        public extern int IndexOf<T>(T value, int startIndex);

        [IgnoreGenericArguments]
        [Script(@"
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            this.splice(index, 0, value);
            ")]
        public extern void InsertAt<T>(int index, T value);

        [IgnoreGenericArguments]
        [Script(@"
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            values.unshift(0);
            values.unshift(index);
            this.splice.apply(this, values);
            values.shift();
            values.shift();
            ")]
        public extern void InsertRangeAt(int index, NativeArray values);

        [Script(@"
            if (index < 0 || index >= this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            this.splice(index, 1);
            ")]
        public extern void RemoveAt(int index);

        [IgnoreGenericArguments]
        [Script(@"this[index] = value;")]
        public extern void SetAt<T>(int index, T value);

        [IgnoreGenericArguments]
        [Script(@"return this[index];")]
        public extern T GetFrom<T>(int index);

        public extern void Sort();

        [IgnoreGenericArguments]
        public extern void Sort<T>(Func<T, T, int> sortFunction);

        [ScriptName("shift")]
        private extern void ShiftImpl();

        [ScriptName("unshift")]
        private extern void UnshiftImpl();

        [ScriptName("push")]
        private extern void PushImpl();

        [ScriptName("pop")]
        private extern void PopImpl();

        [ScriptName("concat")]
        private extern void ConcatImpl();

        [ScriptName("splice")]
        private extern void SpliceImpl();
    }

    /// <summary>
    /// Definition for NativeArray
    /// </summary>
    [ScriptName("Array"), IgnoreNamespace, IgnoreGenericArguments]
    public class NativeArray<T>
    {
        public extern NativeArray(int length);

        [IntrinsicProperty]
        public extern int Length
        {
            get;
            set;
        }

        [Script("return array ? array.@{[mscorlib]System.ArrayG`1::innerArray} : null;")]
        public static extern NativeArray<T> GetNativeArray(T[] array);

        [Script("return array ? array.@{[mscorlib]System.Collections.Generic.List`1::nativeArray} : null;")]
        public static extern NativeArray<T> GetNativeArray(List<T> array);

        [Script(@"
            return this.@{[mscorlib]System.NativeArray::ConcatImpl()}.apply(
                this,
                arrays.@{[mscorlib]System.Collections.Generic.List`1::nativeArray});")]
        public extern NativeArray<T> Concat(params NativeArray<T>[] arrays);

        [Script(@"return this.concat(array);")]
        public extern NativeArray<T> Concat(NativeArray<T> array);

        [Script(@"return this.concat(array, array2);")]
        public extern NativeArray<T> Concat(NativeArray<T> array, NativeArray<T> array2);

        [Script(@"return this.concat(array, array2, array3);")]
        public extern NativeArray<T> Concat(NativeArray<T> array, NativeArray<T> array2, NativeArray<T> array3);

        public extern string Join();

        public extern string Join(string seperator);

        public extern T Pop();

        public extern int Push(T value);

        public extern T Shift();

        public extern void Unshift(T value);

        public extern void Reverse();

        public extern NativeArray<T> Slice(int start, int end);

        [Script(@"
            var newArray = elements.@{[mscorlib]System.ArrayG`1::innerArray};
            newArray.unshift(howMany);
            newArray.unshift(index);
            var rv = this.splice.apply(this, newArray);
            newArray.shift();
            newArray.shift();
            return rv;
            ")]
        public extern NativeArray<T> Splice(int index, int howMany, params T[] elements);

        [Script(@"
            var i;
            startIndex = startIndex < 0 ? 0 : startIndex;
            return this.indexOf(value, startIndex);
            ")]
        public extern int IndexOf(T value, int startIndex);

        [Script(@"
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            this.splice(index, 0, value);
            ")]
        public extern void InsertAt(int index, T value);

        [Script(@"
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            values.unshift(0);
            values.unshift(index);
            this.splice.apply(this, values);
            values.shift();
            values.shift();
            ")]
        public extern void InsertRangeAt(int index, NativeArray<T> values);

        [Script(@"
            var i;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            this.splice(index, 1);
            ")]
        public extern void RemoveAt(int index);

        public extern T this[int i]
        { get; set; }

        public extern void Sort();

        public extern void Sort(Func<T, T, int> sortFunction);

        public static implicit operator NativeArray<T>(T[] n)
        { return n == null ? null : ((ArrayG<T>)(object)n).InnerArray; }

        [ScriptName("shift")]
        private extern void ShiftImpl();

        [ScriptName("unshift")]
        private extern void UnshiftImpl();

        [ScriptName("push")]
        private extern void PushImpl();

        [ScriptName("pop")]
        private extern void PopImpl();

        [ScriptName("concat")]
        private extern void ConcatImpl();

        [ScriptName("splice")]
        private extern void SpliceImpl();
    }

    [ScriptName("Uint8ClampedArray"), IgnoreNamespace, ImportedType]
    public class Uint8ClampedArray : NativeArray<byte>
    {
        public extern Uint8ClampedArray();

        public extern Uint8ClampedArray(int size);

        public extern Uint8ClampedArray(NativeArray arr);

        public extern Uint8ClampedArray(NativeArray<byte> arr);
    }

    [ScriptName("Uint8Array"), IgnoreNamespace, ImportedType]
    public class Uint8Array : NativeArray<byte>
    {
        public extern Uint8Array();

        public extern Uint8Array(int size);

        public extern Uint8Array(NativeArray arr);

        public extern Uint8Array(NativeArray<byte> arr);
    }

    [ScriptName("Int8Array"), IgnoreNamespace, ImportedType]
    public class Int8Array : NativeArray<sbyte>
    {
        public extern Int8Array();

        public extern Int8Array(int size);

        public extern Int8Array(NativeArray arr);

        public extern Int8Array(NativeArray<sbyte> arr);
    }

    [ScriptName("Int16Array"), IgnoreNamespace, ImportedType]
    public class Int16Array : NativeArray<short>
    {
        public extern Int16Array();

        public extern Int16Array(int size);

        public extern Int16Array(NativeArray arr);

        public extern Int16Array(NativeArray<short> arr);
    }

    [ScriptName("UInt16Array"), IgnoreNamespace, ImportedType]
    public class Uint16Array : NativeArray<ushort>
    {
        public extern Uint16Array();

        public extern Uint16Array(int size);

        public extern Uint16Array(NativeArray arr);

        public extern Uint16Array(NativeArray<ushort> arr);
    }

    [ScriptName("Int32Array"), IgnoreNamespace, ImportedType]
    public class Int32Array : NativeArray<int>
    {
        public extern Int32Array();

        public extern Int32Array(int size);

        public extern Int32Array(NativeArray arr);

        public extern Int32Array(NativeArray<int> arr);
    }

    [ScriptName("Uint32Array"), IgnoreNamespace, ImportedType]
    public class Uint32Array : NativeArray<uint>
    {
        public extern Uint32Array();

        public extern Uint32Array(int size);

        public extern Uint32Array(NativeArray arr);

        public extern Uint32Array(NativeArray<uint> arr);
    }

    [ScriptName("Float32Array"), IgnoreNamespace, ImportedType]
    public class Float32Array : NativeArray<float>
    {
        public extern Float32Array();

        public extern Float32Array(int size);

        public extern Float32Array(NativeArray arr);

        public extern Float32Array(NativeArray<float> arr);
    }

    [ScriptName("Float64Array"), IgnoreNamespace, ImportedType]
    public class Float64Array : NativeArray<double>
    {
        public extern Float64Array();

        public extern Float64Array(int size);

        public extern Float64Array(NativeArray arr);

        public extern Float64Array(NativeArray<double> arr);
    }

    public static class NativeArrayExtensions
    {
        public static T[] GetArray<T>(this NativeArray<T> n)
        { return (T[])(object)new ArrayG<T>(n); }
    }
}