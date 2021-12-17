//-----------------------------------------------------------------------
// <copyright file="NullableTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for NullableTests
    /// </summary>
    public static class NullableTests
    {
        private static void Main()
        {
            NullableTests.TestNonNull();
            NullableTests.TestNull();
            NullableTests.TestLiftingNonNullArgs();
            NullableTests.TestLiftingNullArgs();
            NullableTests.TestNullCoalescing();
        }

        private static void TestNonNull()
        {
            Console.WriteLine("Testing non-null");
            var n = (int?)42;
            LogNullable(n);
        }

        private static void TestNull()
        {
            Console.WriteLine("Testing null");
            var n = default(int?);
            LogNullable(n);
        }

        private static void TestLiftingNonNullArgs()
        {
            Console.WriteLine("Testing lifting with non-null args");
            var n1 = (int?)1;
            var n2 = (int?)2;
            LogNullable(n1 + n2);
        }

        private static void TestLiftingNullArgs()
        {
            Console.WriteLine("Testing lifting with null args");
            int? n1 = (int?)1;
            int? n2 = null;
            LogNullable(n1 + n2);
        }

        private static void TestNullCoalescing()
        {
            // int
            Console.WriteLine("Testing Null Coalescing operator");
            int? i = 0;
            Console.WriteLine("i = " + (i ?? 1).ToString());
            i = null;
            Console.WriteLine("i = " + (i ?? 1).ToString());

            // bool
            bool? b = false;
            Console.WriteLine("b = " + (b ?? true).ToString());
            b = null;
            Console.WriteLine("b = " + (b ?? true).ToString());

            // string
            string? s = "";
            Console.WriteLine("s.Length = " + (s ?? "Empty").Length);
            s = null;
            Console.WriteLine("s.Length = " + (s ?? "Empty").Length);
        }

        private static void LogNullable<T>(Nullable<T> nt) where T : struct
        {
            if (nt.HasValue)
                Console.WriteLine("Nullable has value " + nt.Value.ToString());
            else
                Console.WriteLine("Nullable has no value");
            if (nt == null)
                Console.WriteLine("Nullable equals null");
            var o = (object)nt;
            if (o == null)
                Console.WriteLine("Nullable casts to null object");
            else
                Console.WriteLine("Nullable casts to object " + o.ToString());
            var nt2 = (Nullable<T>)o;
            if (nt.HasValue)
                Console.WriteLine("Recast nullable has value " + nt.Value.ToString());
            else
                Console.WriteLine("Recast nullable has no value");
            try
            {
                var t1 = (T)nt;
                Console.WriteLine("Nullable casts to underlying value " + t1.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Nullable fails casting to underlying value");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            var t2 = nt ?? default(T);
            Console.WriteLine("Value or default is " + t2.ToString());
        }
    }
}