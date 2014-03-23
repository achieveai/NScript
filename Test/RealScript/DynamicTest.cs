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
        public static string GetSomething(dynamic test)
        {
            test.bar = DynamicTest.GetFoo();
            test.iB = test.iA + test.iC;

            return test.foo;
        }

        public static dynamic GetFoo()
        {
            return new {Test = "string" };
        }

        public static int CheckIndex()
        {
            DynamicTest.GetFoo()["this"] = 10;
            return DynamicTest.GetFoo()["bar"];
        }

        public static int CheckMethod()
        {
            return DynamicTest.GetFoo().met();
        }
    }
}
