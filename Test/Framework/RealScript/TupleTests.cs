using System;
using System.Collections.Generic;

namespace RealScript
{
    public static class TupleTests
    {
        public static void TestTupleDecons()
        {
            var (a, b, c) = FuncReturningTuple();
            var (x, (y, z), w) = FuncReturningTuple();
            var (i, (j, k), l) = (0, (1, 2), "rand");
            var ((p, (q, r)), s) = TestTupleReturn(1, "asdf");
        }

        public static ((int, (string, int)), int) TestTupleReturn(int x, string y)
        {
            return ((x, (y, x)), x);
        }

        public static void TestTempVarCreation()
        {
            var (a, b, c) = (1, 2, 3);
            var x = (a, b, c);
            var (j, k, l) = x;
            var tmp = (x, "asdf");
            (a, b, c) = tmp.Item1;
        }

        public static void TestDiscard(string _)
        {
            // var (a, _) = (1, 2);
            _ = "asdf";
            var (a, _, str) = FuncReturningTuple();
        }

        private static (int, (int, int), string) FuncReturningTuple()
            => (0, (1, 2), "rand");


        // MARK: Execution Tests

        public static void Main()
        {
            Test1();
            Test2();
            Test3();
        }

        private static void Test1()
        {
            Console.WriteLine("Test1");
            var (a, b) = FunctionWithSideEffects1();
            Console.WriteLine(a);
            Console.WriteLine(b);
            (a, b) = (FunctionWithSideEffects2(), 2);
            Console.WriteLine(a);
            Console.WriteLine(b);
            (a, b) = (FunctionWithSideEffects2(), FunctionWithSideEffects2());
            Console.WriteLine(a);
            Console.WriteLine(b);
        }

        private static void Test2()
        {
            Console.WriteLine("Test2");
            var (a, _) = FunctionWithSideEffects1();
            Console.WriteLine(a);
            _ = FunctionWithSideEffects1();
            var (_, _) = (1, FunctionWithSideEffects2());
        }

        private static void Test3()
        {
            Console.WriteLine("Test3");
            var (a, (b, c), d) = (1, FunctionWithSideEffects1(), 2);
            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
            Console.WriteLine(d);

            var mid = ((a, (b, c), d) = (1, (1, 2), 2)).Item2;
            Console.WriteLine(mid.Item1);
            Console.WriteLine(mid.Item2);
        }

        private static (int, int) FunctionWithSideEffects1()
        {
            Console.WriteLine("SideEffect1");
            return (1, 2);
        }

        private static int FunctionWithSideEffects2()
        {
            Console.WriteLine("SideEffect2");
            return 12;
        }
    }
}
