//-----------------------------------------------------------------------
// <copyright file="TestDelegates.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    public delegate void Test();
    public delegate int TestInt();
    public delegate int TestIntInt(int i);
    public delegate string TestString();
    public delegate string TestStringString(string s);
    public delegate void TestMixed(string a, bool b, int c);

    /// <summary>
    /// Definition for TestDelegates
    /// </summary>
    public static class TestDelegates
    {
        static Test privN;
        private static TestMixed tester;

        static void Prepare(Test n)
        {
            privN = n;
        }

        static void Call()
        {
            privN();
        }

        public static void Main()
        {
            TestDelegates.TestVoidToVoid();
            TestDelegates.TestVoidToInt();
            TestDelegates.TestIntToInt();
            TestDelegates.TestVoidToString();
            TestDelegates.TestStringToString();
            TestDelegates.TestStringToStringStatic();
            TestDelegates.TestSimpleDelegateHash();
            TestDelegates.TestCombinedDelegates();
            TestDelegates.TestVirtualDelegates();
            TestDelegates.TestDelegatesWithCapturedVariables();
            TestDelegates.TestDelegatesWithCapturedReferencedVariable();
            TestDelegates.TestDelegatesWithCapturedValueVariable();
            TestDelegates.TestEvents();
            TestDelegates.TestHigherKindedDelegates();
        }

        private static void TestVoidToVoid()
        {
            Console.WriteLine("Testing Void -> Void...");
            var a = new TestDelegateA() { i = 3 };
            var d = new Test(a.One);
            d();
        }

        private static void TestVoidToInt()
        {
            Console.WriteLine("Testing Void -> Int...");
            var a = new TestDelegateA() { i = 3 };
            var d = new TestInt(a.OneInt);
            Console.WriteLine(d());
        }

        private static void TestIntToInt()
        {
            Console.WriteLine("Testing Int -> Int...");
            var a = new TestDelegateA() { i = 3 };
            var d = new TestIntInt(a.OneIntInt);
            Console.WriteLine(d(5));
        }

        private static void TestVoidToString()
            {
                Console.WriteLine("Testing Void -> String...");
                var a = new TestDelegateA() { i = 3 };
                var d = new TestString(a.OneString);
                Console.WriteLine(d());
            }

        private static void TestStringToString()
            {
                Console.WriteLine("Testing String -> String...");
                var a = new TestDelegateA() { i = 3 };
                var d = new TestStringString(a.OneStringString);
                Console.WriteLine(d("5"));
            }

        private static void TestStringToStringStatic()
            {
                Console.WriteLine("Testing String -> String static...");
                var d = new TestStringString(TestDelegateA.StaticStringString);
                Console.WriteLine(d("2"));
            }

        private static void TestSimpleDelegateHash()
            {
                Console.WriteLine("Testing simple delegate hash...");
                var a1 = new TestDelegateA() { i = 3 };
                var a2 = new TestDelegateA() { i = 5 };
                var d1 = new TestStringString(a1.OneStringString);
                var d2 = new TestStringString(a2.OneStringString);
                var d3 = new TestString(a1.OneString);
                // Remember: Hash code for simple delegates is based on delegate type alone
                // Console.WriteLine(d1.GetHashCode() == d1.GetHashCode());
                // Console.WriteLine(d1.GetHashCode() == d2.GetHashCode());
                // Console.WriteLine(d1.GetHashCode() != d3.GetHashCode());
            }

        private static void TestCombinedDelegates()
            {
                Console.WriteLine("Testing combined delegates...");
                var a1 = new TestDelegateA() { i = 3 };
                var a2 = new TestDelegateA() { i = 5 };
                var d1 = new TestStringString(a1.OneStringString);
                var d2 = new TestStringString(a2.OneStringString);
                var d3 = new TestStringString(a1.TwoStringString);
                var d4 = new TestStringString(a1.ThreeStringString);
                var d5 = (TestStringString)Delegate.Combine(d1, d2);
                var d6 = (TestStringString)Delegate.Combine(d3, d5);
                var d7 = (TestStringString)Delegate.Combine(d6, d4);
                Console.WriteLine(d7("7"));

                // Console.WriteLine("Testing combined delegate hash...");
                // Console.WriteLine(d7.GetHashCode() == d7.GetHashCode());
                // Console.WriteLine(d7.GetHashCode() != d6.GetHashCode());

                Console.WriteLine("Testing removing delegates...");
                var d8 = (TestStringString)Delegate.Remove(d7, d5);
                Console.WriteLine(d8("8"));
                var d9 = (TestStringString)Delegate.Remove(d7, d2);
                Console.WriteLine(d9("9"));
                var d10 = (TestStringString)Delegate.Combine(d3, d2);
                var d11 = (TestStringString)Delegate.Remove(d7, d10);
                Console.WriteLine(d11("11"));
            }

        private static void TestVirtualDelegates()
            {
                Console.WriteLine("Testing virtual delegate...");
                var a = new TestDelegateA() { i = 3 };
                var d1 = new TestString(a.Virtual);
                Console.WriteLine(d1());
                var b = (TestDelegateA)new TestDelegateB() { i = 7 };
                var d2 = new TestString(b.Virtual);
                Console.WriteLine(d2());
            }

        private static void TestDelegatesWithCapturedVariables()
            {
                Console.WriteLine("Testing delegate with captured variable...");
                var outer = 7;
                FromTo(1, 3, delegate(int i) { Console.WriteLine(outer); Console.WriteLine(i); return outer * 4; });
                FromTo(1, 3, i => { Console.WriteLine(outer); Console.WriteLine(i); return outer * 4; });
            }

        private static void TestDelegatesWithCapturedReferencedVariable()
            {
                Console.WriteLine("Testing delegate with captured reference variable...");
                for (var i = 1; i <= 3; i++)
                {
                    Prepare(delegate { Console.WriteLine(i); });
                }
                Call();
            }

        private static void TestDelegatesWithCapturedValueVariable()
            {
                Console.WriteLine("Testing delegate with captured value variable...");
                for (var i = 1; i <= 3; i++)
                {
                    var j = i;
                    Prepare(delegate { Console.WriteLine(j); });
                }
                Call();
            }

        private static void TestEvents()
            {
                Console.WriteLine("Testing event registering, triggering and unregestering...");
                SomethingHappened += delegate { Console.WriteLine("Something happened."); };
                if (SomethingHappened != null)
                    SomethingHappened();
                SomethingHappened += MoreHappened;
                if (SomethingHappened != null)
                    SomethingHappened();
                SomethingHappened -= MoreHappened;
                if (SomethingHappened != null)
                    SomethingHappened();
            }

        private static void TestHigherKindedDelegates()
        {
            Console.WriteLine("Testing delegates of higher-kinded type over polymorphic methods...");
            var polyint = new Poly<int>(3);
            var polystr = new Poly<string>("four");

            StringToString f = polyint.M<string>;
            Console.WriteLine(f("six"));
            IntToString g = polyint.M<int>;
            Console.WriteLine(g(7));
            StringToString h = polystr.M<string>;
            Console.WriteLine(h("eight"));
            IntToString i = polystr.M<int>;
            Console.WriteLine(i(9));
        }

        static void FromTo(int from, int to, TestIntInt d)
        {
            for (var i = from; i <= to; i++)
                Console.WriteLine(d(i));
        }

        static event Test SomethingHappened;

        static void MoreHappened()
        {
            Console.WriteLine("More happened");
        }
    }

    delegate string StringToString(string s);
    delegate string IntToString(int i);

    public class TestDelegateA
    {
        public int i;

        protected void Log(string s)
        {
            Console.WriteLine(s + " invoked");
            Console.WriteLine("i: " + i.ToString());
        }

        public static string StaticStringString(string s)
        {
            Console.WriteLine("StaticStringString invoked");
            return "1" + s;
        }

        public void One()
        {
            Log("One");
        }

        public int OneInt()
        {
            Log("OneInt");
            return 1 + i;
        }

        public int OneIntInt(int j)
        {
            Log("OneIntInt");
            return j + i;
        }


        public string OneString()
        {
            Log("OneString");
            return "1" + i.ToString();
        }

        public string OneStringString(string s)
        {
            Log("OneStringString");
            return s + i.ToString();
        }

        public string TwoStringString(string s)
        {
            Log("TwoStringString");
            return s + i.ToString();
        }

        public string ThreeStringString(string s)
        {
            Log("ThreeStringString");
            return s + i.ToString();
        }

        public virtual string Virtual()
        {
            Log("A.Virtual");
            return "A" + i.ToString();
        }
    }

    public class TestDelegateB : TestDelegateA
    {
        public override string Virtual()
        {
            Log("B.Virtual");
            return "B" + i.ToString();
        }
    }

    public class Poly<TestDelegateA>
    {
        TestDelegateA a;

        public Poly(TestDelegateA a)
        {
            this.a = a;
        }

        public string M<T>(T b)
        {
            return "a = " + a + ", b = " + b;
        }
    }
}
