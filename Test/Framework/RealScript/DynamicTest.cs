//-----------------------------------------------------------------------
// <copyright file="DynamicTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for DynamicTest
    /// </summary>
    public class DynamicTest
    {
        public static int CheckIndex()
        {
            DynamicTest.GetFoo()["this"] = 10;
            return DynamicTest.GetFoo()["bar"];
        }

        public static int CheckMethod()
        {
            return DynamicTest.GetFoo().met();
        }

        public static int[] GetArray()
        {
            return DynamicTest.GetFoo();
        }

        public static System.Collections.Generic.Dictionary<int, string> GetDictionary()
        {
            return DynamicTest.GetFoo();
        }

        public static dynamic GetFoo()
        {
            // TODO: We still need to test and make sure that
            // references to anonymous types members are type
            // preserved. Can only be validated in LINQ.
            return new { Test = "string" };
        }

        public static System.Collections.Generic.List<int> GetList()
        {
            return DynamicTest.GetFoo();
        }

        public static string GetSomething(dynamic test)
        {
            test.bar = DynamicTest.GetFoo();
            test.iB = test.iA + test.iC;

            return test.foo;
        }
    }
}
