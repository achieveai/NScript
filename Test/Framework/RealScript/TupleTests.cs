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

        private static (int, (int, int), string) FuncReturningTuple()
            => (0, (1, 2), "rand");
    }
}
