//-----------------------------------------------------------------------
// <copyright file="TestControlFlow.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for TestControlFlow
    /// </summary>
    public static class TestControlFlow
    {
        public static void Main()
        {
            TestIfThen(3);
            TestIfThen(7);
            TestIfThenElse(3);
            TestIfThenElse(7);
            TestSwitch(3);
            TestSwitch(5);
            TestSwitch(7);
            TestFor(3);
            TestWhile(3);
            TestDoWhile(3);
            TestRefs(3, 7);
            TestTernaryPure(3, 7);
            TestTernaryPure(7, 3);
            TestTernarySideEffects(3, 7);
            TestTernarySideEffects(7, 3);
            TestIs(new B());
            TestIs(new C());
            TestIs(new D());
        }

        static void TestIfThen(int arg)
        {
            Console.WriteLine("Testing if-then...");
            if (arg > 5)
            {
                Console.WriteLine("then");
            }
            Console.WriteLine("after");
        }


        static void TestIfThenElse(int arg)
        {
            Console.WriteLine("Testing if-then-else...");
            if (arg > 5)
            {
                Console.WriteLine("then");
            }
            else
            {
                Console.WriteLine("else");
            }
            Console.WriteLine("after");
        }


        static void TestSwitch(int arg)
        {
            Console.WriteLine("Testing switch...");
            switch (arg)
            {
                case 2:
                    Console.WriteLine("2");
                    break;
                case 3:
                    Console.WriteLine("3");
                    return;
                case 4:
                case 5:
                    Console.WriteLine("4 or 5");
                    break;
                default:
                    Console.WriteLine("default");
                    break;
            }
            Console.WriteLine("after");
        }

        static void TestFor(int arg)
        {
            Console.WriteLine("Testing for...");
            for (var i = 0; i < arg; i++)
                Console.WriteLine(i);
            Console.WriteLine("after");
        }

        static void TestWhile(int arg)
        {
            Console.WriteLine("Testing while...");
            var i = 0;
            while (i < arg)
            {
                Console.WriteLine(i++);
            }
            Console.WriteLine("after");
        }

        static void TestDoWhile(int arg)
        {
            Console.WriteLine("Testing do-while...");
            var i = 0;
            do
            {
                Console.WriteLine(i++);
            } while (i < arg);
            Console.WriteLine("after");
        }

        static void TestRefs(int a, int b)
        {
            Console.WriteLine("Testing refs...");

            Console.WriteLine("a = " + a.ToString());
            Console.WriteLine("b = " + b.ToString());

            Swap(ref a, ref b);

            Console.WriteLine("a = " + a);
            Console.WriteLine("b = " + b);
        }

        public static void Swap(ref int a, ref int b)
        {
            var t = a;
            a = b;
            b = t;
        }

        static void TestTernaryPure(int x, int y)
        {
            Console.WriteLine("Testing ternary ?: operator without side-effects...");
            Console.WriteLine("cond1: " + (x > 7 ? ">" : "<=") +
                ", cond2: " + (y > 3 ? ">" : "<=") +
                ", cond3: " + (x > 3 ? (y > 7 ? "> >" : "> <=") : (y > 7 ? "<= >" : "<= <=")));
        }

        static void TestTernarySideEffects(int x, int y)
        {
            Console.WriteLine("Testing ternary ?: operator with side-effects...");
            Console.WriteLine("x = " + x + ", y = " + y);
            Console.WriteLine("cond1: " + (x++ > 7 ? ">" : "<=") +
                ", cond2: " + (y-- > 3 ? ">" : "<=") +
                ", cond3: " + (++x > 3 ? (--y > 7 ? "> >" : "> <=") : (--y > 7 ? "<= >" : "<= <=")));
            Console.WriteLine("x = " + x + ", y = " + y);
        }

        static void TestIs(A a)
        {
            Console.WriteLine("Testing is in conditionals...");
            if (!(a is B) && !(a is C))
                Console.WriteLine("is a D");
            else
                Console.WriteLine("is not a D");
            if (a is B || a is C)
                Console.WriteLine("is not a D");
            else
                Console.WriteLine("is a D");
        }
    }

    public class A {}
    public class B : A {}
    public class C : A {}
    public class D : A {}
}
