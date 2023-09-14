//-----------------------------------------------------------------------
// <copyright file="TestGeneric.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections.Generic;

    internal static class TestGenerics
    {
        public static void Main()
        {
            TestGenericBasic();
            TestImplicitImplementationOfInterface();
            TestImplicitAndExplicitInterfaces();
            TestConstructorAndFieldMutation();
            TestMiniLists();
            TestMiniDictionary();
            TestStaticFieldsOnDistinctTypeInstances();
            TestGenericMethodsIntrestingPermutation();
            TestRecursiveTypes();
            TestPropertyGetForGenerics();
        }

        private static void TestGenericBasic()
            {
                Console.WriteLine("Testing first- vs higher-kinded and mono- vs polymorphic-method combinations...");
                var l = new System.Collections.Generic.List<string>();
                l.Add("abc");
                Console.WriteLine(new FirstOrderClassPolymorphicMethod().Test(l));
                Console.WriteLine(new HigherKindedClassMonomorphicMethod<string>().Test(l));
                var dict = new StringDictionary<int>();
                dict.Add("def", 3);
                Console.WriteLine(new HigherKindedClassPolymorphicMethod<string>().Test(dict));
            }

        private static void TestImplicitImplementationOfInterface()
        {
            Console.WriteLine("Testing implicit implementations of interface methods on different instantiations of the same interface ...");
            TestSameInterfaceTwice<string>("test");
            TestSameInterfaceTwice<int>(1);
        }

        private static void TestImplicitAndExplicitInterfaces()
            {
                Console.WriteLine("Testing implicit and explicit implementations of interface methods on the same interface...");
                var x = new ImplicitAndExplicit<string>();
                x.Test1(new Wrapper<string>("hij"));
                ITwoTests<Wrapper<string>> y = x;
                y.Test2(new Wrapper<string>("klm"));
            }

        private static void TestConstructorAndFieldMutation()
        {
            Console.WriteLine("Testing construction and field mutation...");
            var p = new Person("pqr");
            Console.WriteLine(p.ToString());

            var b1 = new MyBox<Person, string>(new Person("opq"));
            b1.u = "b1";
            var b2 = new MyBox<int, string>(1);
            b2.u = "b2";

            Console.WriteLine(b1.ToString());
            Console.WriteLine(b2.ToString());

            b1.t = new Person("rst");
            b1.u = "uvw";
            b2.t = 3;

            Console.WriteLine(b1.ToString());
            Console.WriteLine(b2.ToString());
        }

        private static void TestMiniLists()
        {
            Console.WriteLine("Testing mini generic lists of object...");
            var list = new MyList<object>();
            list.Add(1);
            list.Add("2");
            list.Add(new Person("pqr"));
            foreach (var obj in list)
                Console.WriteLine(obj.ToString());
        }

        private static void TestMiniDictionary()
        {
            Console.WriteLine("Testing mini generic dictionary of string to int...");
            var dict = new MyDictionary<string, int>();
            dict.Add("one", 1);
            dict.Add("two", 2);
            dict.Add("three", 3);
            foreach (var kvPair in dict)
                Console.WriteLine(kvPair.Key + " -> " + kvPair.Value);
        }

        private static void TestStaticFieldsOnDistinctTypeInstances()
        {
            Console.WriteLine("Testing static fields on distinct type instances...");
            MyList<int>.testVal = 1;
            MyList<string>.testVal = 2;

            Console.WriteLine(MyList<int>.testVal.ToString());
            Console.WriteLine(MyList<string>.testVal.ToString());
        }

        private static void TestGenericMethodsIntrestingPermutation()
        {
            Console.WriteLine("Testing generic methods with interesting permutation of type arguments.."); ;
            var b = new MyBox<Person, string>(new Person("opq"));
            b.u = "second";
            TestGenericMethod<Person, string, int>(b, 5, null);
            TestGenericMethod<Person, string, string>(b, "xyz", null);
            TestGenericMethod<Person, string, Person>(b, new Person("efg"), null);
        }

        private static void TestRecursiveTypes()
        {
            Console.WriteLine("Testing recursive types...");
            var a = new TestGenericA<TestGenericB>();
            var b = new TestGenericB();
            Console.WriteLine(a.M(b));
            Console.WriteLine(b.M(a));
        }

        private static void TestPropertyGetForGenerics()
        {
            var w = new Wrapper<Person>(new Person("pname"));
            var person = w.Property;
            Console.WriteLine(person.Name);
        }

        private static void TestSameInterfaceTwice<T>(T x)
        {
            var adder = (IHigherKindedAndPolymorphic<T>)new SameInterfaceTwice();
            adder.Test<object>(x, null);
        }

        private static void TestGenericMethod<TestGenericB, C, TestGenericA>(MyBox<TestGenericB, C> b, TestGenericA val, TestGenericB dummyVal)
        {
            Console.WriteLine(b.ConcatFirstAndPrint<TestGenericA>(val, dummyVal));
        }
    }

    interface IHigherKindedAndPolymorphic<T>
    {
        void Test<U>(T x, U y);
    }

    internal class SameInterfaceTwice : IHigherKindedAndPolymorphic<int>, IHigherKindedAndPolymorphic<string>
    {
        public void Test<U>(string x, U y)
        {
            Console.WriteLine("IHigherKindedAndPolymorphic<string>::Add");
        }

        public void Test<U>(int x, U y)
        {
            Console.WriteLine("IHigherKindedAndPolymorphic<int>::Add");
        }

        public override string ToString()
        {
            return "";
        }
    }

    internal class MyBox<T, U>
    {
        public T t;
        public U u;

        public MyBox(T val)
        {
            t = val;
        }

        override public string ToString()
        {
            return "MyBox(t=" + t + ", u=" + u + ")";
        }

        public string ConcatFirstAndPrint<TestGenericA>(TestGenericA a, T dummy)
        {
            var b1 = new MyBox<TestGenericA, int>(a);
            var b2 = new MyBox<int, MyBox<int, int>>(3);
            var b3 = new MyBox<int, MyBox<TestGenericA, TestGenericA>>(4);
            return "t=" + t + ", a=" + a.ToString() + ", Second=" + Second;
        }

        public string ConcatFirstAndPrint<TestGenericA, TestGenericB>(TestGenericA a)
        {
            return "...never called...";
        }

        virtual public U Second { get { return u; } }
    }

    internal class Person : MyBox<int, int>
    {
        private string name;

        public Person(string name)
            : base(5)
        {
            this.name = name;
        }

        override public string ToString()
        {
            return "Person(t=" + t + ", u=" + u + ", name=" + name + ")";
        }

        override public int Second { get { return 10; } }

        public string Name => name;
    }

    public interface ITwoTests<T>
    {
        void Test1(T test);

        void Test2(T test);
    }

    public class Wrapper<T>
    {
        public T Value;

        public Wrapper(T value)
        {
            Value = value;
            Property = value;
        }

        public T Property { get; }

        public override string ToString()
        {
            return "Wrapper(Value=" + Value + ")";
        }
    }

    public class ImplicitAndExplicit<T> : ITwoTests<Wrapper<T>>
    {
        public void Test1(Wrapper<T> test)
        {
            Console.WriteLine("ImplicitAndExplicit::Test(" + test + ")");
        }

        void ITwoTests<Wrapper<T>>.Test2(Wrapper<T> test)
        {
            Console.WriteLine("ImplicitAndExplicit::Test2(" + test + ")");
        }
    }

    internal class MyList<T, V> { }

    internal class MyList<T> : IEnumerable<T>
    {
        static public int testVal;

        private int count = 0;
        private int capacity = 10;
        private T[] array;

        public MyList()
        {
            array = new T[capacity];
        }

        public int Count { get { return count; } }

        public void Add(T val)
        {
            if (count < capacity)
            {
                array[count] = val;
                count++;
            }
            else
            {
                var temp = array;
                capacity = capacity * 2;
                array = new T[capacity];
                for (var i = 0; i < temp.Length; i++)
                    array[i] = temp[i];
                Add(val);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                if (index < count)
                    return array[index];
                else
                    throw new Exception("index");
            }
        }

        private class Enumerator : IEnumerator<T>
        {
            MyList<T> list;
            int index = -1;

            public Enumerator(MyList<T> list)
            {
                this.list = list;
            }

            public T Current
            {
                get { return this.list[index]; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return ++this.index < this.list.count;
            }

            public void Reset()
            {
                this.index = -1;
            }

            public void Dispose()
            {
            }
        }
    }

    internal class MyKeyValuePair<K, V>
    {
        public K key;
        public V value;

        public MyKeyValuePair(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        public K Key { get { return key; } }

        public V Value { get { return value; } }
    }

    internal class MyDictionary<K, V> : IEnumerable<MyKeyValuePair<K, V>>
    {
        private MyList<MyKeyValuePair<K, V>> list;

        public MyDictionary()
        {
            list = new MyList<MyKeyValuePair<K, V>>();
        }

        public IEnumerator<MyKeyValuePair<K, V>> GetEnumerator()
        {
            return new Enumerator(list.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(K key, V value)
        {
            list.Add(new MyKeyValuePair<K, V>(key, value));
        }

        private class Enumerator : IEnumerator<MyKeyValuePair<K, V>>
        {
            IEnumerator<MyKeyValuePair<K, V>> inner;

            public Enumerator(IEnumerator<MyKeyValuePair<K, V>> inner)
            {
                this.inner = inner;
            }

            public MyKeyValuePair<K, V> Current
            {
                get { return this.inner.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return this.inner.MoveNext();
            }

            public void Reset()
            {
                this.inner.Reset();
            }

            public void Dispose()
            {
                this.inner.Dispose();
            }
        }
    }

    public interface IX<T>
    {
        int M(T t);
    }

    public class TestGenericA<T> : List<TestGenericA<T>>, IX<TestGenericB>
    {
        private static TestGenericB theB;

        static TestGenericA() { theB = new TestGenericB(); }

        public int M(TestGenericB b) { return 1; }
    }

    public class TestGenericB : IX<TestGenericA<TestGenericB>>
    {
        private static TestGenericA<TestGenericB> theA;

        static TestGenericB() { theA = new TestGenericA<TestGenericB>(); }

        public int M(TestGenericA<TestGenericB> a) { return 2; }
    }

    /// <summary>
    /// Definition for TestGeneric
    /// </summary>
    public class FirstOrderClassPolymorphicMethod
    {
        public T Test<T>(IEnumerable<T> t)
        {
            foreach (var i in t)
                return i;
            return default(T);
        }
    }

    public class HigherKindedClassMonomorphicMethod<T>
    {
        public T Test(IEnumerable<T> t)
        {
            foreach (var i in t)
                return i;
            return default(T);
        }
    }

    public class HigherKindedClassPolymorphicMethod<T>
    {
        public T Test<V>(IEnumerable<KeyValuePair<T, V>> t)
        {
            foreach (var i in t)
                return i.Key;
            return default(T);
        }
    }
}