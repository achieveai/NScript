//-----------------------------------------------------------------------
// <copyright file="TestInitializers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for TestInitializers
    /// </summary>
    public static class TestInitializer
    {
        public static int si;
        public static string sstr;
        public static EnumInitTest se;
        public static ClassInitTest sc;
        public static StructInitTest ss;

        static void Main()
        {
            {
                Console.WriteLine("Testing static initialization...");
                Console.WriteLine(si.ToString());
                Console.WriteLine(sstr == null ? "null" : sstr);
                Console.WriteLine(((int)se).ToString());
                Console.WriteLine(sc == null ? "null" : sc.ToString());
                Console.WriteLine(ss.ToString());
            }

            {
                Console.WriteLine("Testing struct initialization...");
                var s = new StructInitTest();
                Console.WriteLine(s.ToString());
            }

            {
                Console.WriteLine("Testing struct constructor...");
                var s = new StructInitTest(7.3, 12.11);
                Console.WriteLine(s.ToString());
            }

            {
                Console.WriteLine("Testing class initialization...");
                var c = new ClassInitTest();
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing class constructor...");
                var c = new ClassInitTest(7, "test");
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing derived initialization...");
                var c = new DerivedClassInitTest();
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing derived constructor...");
                var c = new DerivedClassInitTest(7, "test", 12);
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing generic initialization over int...");
                var c = new GenericClassInitTest<int>();
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing generic constructor over int...");
                var c = new GenericClassInitTest<int>(7);
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing generic initialization over string...");
                var c = new GenericClassInitTest<string>();
                Console.WriteLine(c.ToString());
            }

            {
                Console.WriteLine("Testing generic constructor over string...");
                var c = new GenericClassInitTest<string>("test");
                Console.WriteLine(c.ToString());
            }

            /*
            {
                Console.WriteLine("Testing construction of generic parameter with int...");
                var c = new GenericConstructorTest<int>();
                Console.WriteLine(c.ToString());
                Console.WriteLine(c.GenericBox().ToString());
            }

            {
                Console.WriteLine("Testing construction of generic parameter with class...");
                var c = new GenericConstructorTest<Class>();
                Console.WriteLine(c.ToString());
                Console.WriteLine(c.GenericBox().ToString());
            }
             */
        }
    }

    public enum EnumInitTest
    {
        One,
        Two,
        Three
    }

    public struct StructInitTest
    {
        public double x;
        public double y;

        public StructInitTest(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "Struct() { x = " + x.ToString() + ", y = " + y.ToString() + " }";
        }
    }

    public class ClassInitTest
    {
        public StructInitTest s;
        public int i;
        public string str;
        public object o;
        public char c;
        public EnumInitTest e;

        public ClassInitTest()
        {
        }

        public ClassInitTest(int i, string str)
        {
            this.i = i;
            this.str = str;
        }

        public override string ToString()
        {
            return "Class() { s = "
                + s.ToString()
                + ", i = "
                + i.ToString()
                + ", str = "
                + (str == null ? "null" : str)
                + ", o = "
                + (o == null ? "null" : o.ToString())
                + ", c = "
                + ((int)c).ToString()
                + ", e = "
                + ((int)e).ToString() + " }";
        }
    }

    public struct GenericStructInitTest<T>
    {
        public T t;

        public GenericStructInitTest(T t)
        {
            this.t = t;
        }

        public override string ToString()
        {
            return "GenericStruct() { t = " + (((object)t == null) ? "default" : t.ToString()) + " }";
        }
    }

    public class GenericClassInitTest<T>
    {
        public GenericStructInitTest<T> s;

        public GenericClassInitTest()
        {
        }

        public GenericClassInitTest(T t)
        {
            s = new GenericStructInitTest<T>(t); 
        }

        public override string ToString()
        {
            return "GenericClass() { s = " + s.ToString() + " }";
        }
    }

    public class DerivedClassInitTest : ClassInitTest
    {
        public long l;

        public DerivedClassInitTest()
        {
        }

        public DerivedClassInitTest(int i, string str, long l)
            : base(i, str)
        {
            this.l = l;
        }

        public override string ToString()
        {
            return "DerivedClass() { l = " + l.ToString() + ", base = " + base.ToString() + " }";
        }
    }

    /*
    public class GenericConstructorTest<T> where T : new()
    {
        public T t;

        public GenericConstructorTest()
        {
            t = new T();
        }

        public object GenericBox()
        {
            return t;
        }

        public override string ToString()
        {
            return "GenericConstructorTest() { t = " + (((object)t == null) ? "default" : t.ToString()) + " }";
        }
    }
     */
}
