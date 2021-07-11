//-----------------------------------------------------------------------
// <copyright file="ConstructorTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for ConstructorTests
    /// </summary>
    public class ConstructorTests : ConstructorTestsBase
    {
        int i = 0;
        int j = ComputeInt();
        string str = "test";
        string str2 = ComputeString();

        public ConstructorTests() { }

        public ConstructorTests(int i1, string s1)
        {
            this.str = s1;
            this.i = i1;
        }

        public ConstructorTests(int i1, int bi1)
            : base(bi1)
        {
            this.i = i1;
        }

        public ConstructorTests(int i1, int j1, string s1)
            : this(i1, s1)
        {
            this.j = j1;
        }

        private static int ComputeInt()
        {
            return 10;
        }

        private static string ComputeString()
        {
            return "test" + ComputeInt().ToString();
        }
    }

    public class ConstructorTestsBase
    {
        int bi = 10;
        int bj = ComputeInt2();

        public ConstructorTestsBase()
        {}

        public ConstructorTestsBase(int i)
        {
            this.bi = i;
        }

        private static int ComputeInt2()
        {
            return 11;
        }
    }
}
