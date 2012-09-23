//-----------------------------------------------------------------------
// <copyright file="GenericSamples.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections.Generic;

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

    public static class GenericRegressions
    {
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
    }
}