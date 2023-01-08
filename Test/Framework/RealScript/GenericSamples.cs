//-----------------------------------------------------------------------
// <copyright file="GenericSamples.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class TestGeneric<T>
    {
        public class TestSubClass
        {
            public static T Foo<V>(T bar, T foo)
            {
                return bar;
            }
        }

        public class TestSubClass2<U>
        {
            public static U Foo<V>(T t, U u, V v)
            {
                return u;
            }
        }

        public T Foo(T bar)
        {
            return bar;
        }

        public T Foo(TestGeneric<T> bar)
        {
            return default(T);
        }

        public T Foo<V>(TestGeneric<T> bar, TestGeneric<V> b)
        {
            return default(T);
        }

        public T Foo<V>(TestGeneric<V> bar)
        {
            return default(T);
        }
    }

    public class TestGeneric
    {
        public int Foo(int bar)
        {
            return bar;
        }

        public int GenericTypeMethod(int bar)
        {
            return (new TestGeneric<int>()).Foo(bar);
        }

        public Too Foo<Too, Uoo>(Too bar, Uoo var2)
            where Too : struct
            where Uoo : class
        {
            return bar;
        }

        public T Foo<T, U>(U bar, T foo)
        {
            return TestGeneric<T>.TestSubClass.Foo<bool>(foo, foo);
        }
    }

    /// <summary>
    /// Definition for GenericSamples
    /// </summary>
    public class GenericSamples
    {
        public static List<GenericSamples> NewGenericObject()
        {
            return new List<GenericSamples>();
        }

        public static List<T> NewGenericObject2<T>() where T : GenericSamples
        {
            return new List<T>();
        }

        public int GenericMethodCall(string bar, int foo)
        {
            return TestGeneric<int>.TestSubClass.Foo<bool>(foo, foo);
        }

        public T GenericMethodCall2<T, U>(U bar, T foo)
        {
            return TestGeneric<T>.TestSubClass.Foo<bool>(foo, foo);
        }

        public T GenericMethodCall3<T, U>(U bar, T foo)
        {
            return TestGeneric<U>.TestSubClass2<T>.Foo<bool>(bar, foo, true);
        }

        public T GetDefaultValue<T>() where T : new()
        {
            return default(T);
        }
    }

    public class GenericSamples<T>
    {
        public static List<T> NewGenericObject()
        {
            return new List<T>();
        }
    }

    public class GenericSamplesList : List<GenericSamples>
    {
        public GenericSamplesList(int count)
        {
        }

        public override void InsertAt(GenericSamples elem, int index)
        {
            base.InsertAt(elem, index);
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }
    }

    public class GenericRegressions
    {
        string target;

        public static void GenericStructBoxing(IDictionary<string, int> dict)
        {
            var tmp_ = dict.GetEnumerator();
            try
            {
                while (tmp_.MoveNext())
                {
                    var kvPair = tmp_.Current;
                    Console.WriteLine(kvPair.Key + " -> " + kvPair.Value);
                }
            }
            finally
            {
                if (tmp_ != null)
                {
                    tmp_.Dispose();
                }
            }
        }

        public static T TestGenericInterfacePropertyCall<T>(IEnumerable<T> t)
        {
            var tmp_ = t.GetEnumerator();
            try
            {
                while (tmp_.MoveNext())
                {
                    var i = tmp_.Current;
                    return i;
                }
            }
            finally
            {
                if (tmp_ != null)
                {
                    tmp_.Dispose();
                }
            }

            return default(T);
        }

        public static void TestGenericMethodCalls()
        {
            var tmp = new TestGeneric<int>();
            var tmp1 = new TestGeneric<long>();
            var tmp2 = new TestGeneric<int>();
            tmp.Foo(1);
            tmp.Foo(tmp2);
            tmp.Foo<int>(tmp2);
            tmp.Foo<long>(tmp2, tmp1);
        }

        public static bool IsW3wc(string target)
        {
            return true;
        }

        [IgnoreGenericArguments]
        public void AddEvent<T, U>(
            string name,
            Action<T, U> action,
            bool onCapture = false)
        {
            bool isW3wc = GenericRegressions.IsW3wc(this.target);
        }
    }

    public class MultiArgGeneric<T, U>
        where T : new()
        where U : new()
    {
        public static T t;
        public static U u;

        public MultiArgGeneric() { }

        public string GetValue()
        {
            return t.ToString() + u.ToString();
        }
    }

    public class MultiArgGeneric<T, U, V>
        : MultiArgGeneric<T, U>
        where T : new()
        where U : new()
        where V : new()
    {
        public static V v;

        public MultiArgGeneric() { }

        public string GetValue()
        {
            return base.GetValue() + v.ToString();
        }
    }
}