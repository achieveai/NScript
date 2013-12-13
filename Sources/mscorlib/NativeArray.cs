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
    public sealed class NativeArray
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
            for (i = this.length; i >= startIndex && i >= 0; --i)
            {
              if (this[i] === value)
                return i;
            }

            return -1;
            ")]
        public extern int IndexOf<T>(T value, int startIndex);

        [IgnoreGenericArguments]
        [Script(@"
            var i;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            for(i = this.length-1; i >= index; i--)
                this[i+1] = this[i];
            this[index] = value;
            ")]
        public extern void InsertAt<T>(int index, T value);

        [IgnoreGenericArguments]
        [Script(@"
            var i,len;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            len = values.length;
            for(i = this.length-1; i >= index; index--)
                this[i+len] = this[i];
            for(i = len -1; --i;)
                this[index+i] = values[i];
            ")]
        public extern void InsertRangeAt(int index, NativeArray values);

        [Script(@"
            var i;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            var len = this.length - 1;
            for(i = index; i < len; i++)
                this[i] = this[i + 1];
            this.pop();
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
    public sealed class NativeArray<T>
    {
        public extern NativeArray(int length);

        [IntrinsicProperty]
        public extern int Length
        {
            get;
            set;
        }

        public T[] GetArray()
        {
            return (T[])(object)new ArrayG<T>(this);
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

        [Script(@"return this.pop();")]
        public extern T Pop();

        [Script(@"return this.push(value);")]
        public extern int Push(T value);

        [Script(@"return this.shift();")]
        public extern T Shift();

        [Script(@"return this.unshift(value);")]
        public extern void Unshift(T value);

        public extern void Reverse();

        public extern NativeArray<T> Slice(int start, int end);

        [Script(@"
            var newArray = elements.@{[mscorlib]System.ArrayG`1::innerArray}.slice(0);
            newArray.unshift(howMany);
            newArray.unshift(index);
            return this.splice.apply(this, newArray);
            ")]
        public extern NativeArray<T> Splice(int index, int howMany, params T[] elements);

        [Script(@"
            var i;
            startIndex = startIndex < 0 ? 0 : startIndex;
            for (i = this.length; i >= startIndex && i >= 0; --i)
            {
              if (this[i] === value)
                return i;
            }

            return -1;
            ")]
        public extern int IndexOf(T value, int startIndex);

        [Script(@"
            var i;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            for(i = this.length-1; i >= index; i--)
                this[i+1] = this[i];
            this[index] = value;
            ")]
        public extern void InsertAt(int index, T value);

        [Script(@"
            var i,len;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            len = values.length;
            for(i = this.length-1; i >= index; index--)
                this[i+len] = this[i];
            for(i = len -1; --i;)
                this[index+i] = values[i];
            ")]
        public extern void InsertRangeAt(int index, NativeArray<T> values);

        [Script(@"
            var i;
            if (index < 0 || index > this.length)
                throw new @{[mscorlib]System.Exception}(""Index out of range"");
            var len = this.length - 1;
            for(i = index; i < len; i++)
                this[i] = this[i + 1];
            this.pop();
            ")]
        public extern void RemoveAt(int index);

        public extern T this[int i]
        { get; set; }

        public extern void Sort();

        public extern void Sort(Func<T, T, int> sortFunction);

        public static implicit operator NativeArray<T>(T[] n)
        {
            return ((ArrayG<T>)(object)n).InnerArray;
        }

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
}