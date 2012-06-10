//-----------------------------------------------------------------------
// <copyright file="TestSetup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SunlightUnit
{
    using System;

    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Class used to setup test cases and suites
    /// </summary>
    [Imported]
    [GlobalMethods]
    public static class TestSetup
    {
        /// <summary>
        /// Indicates the beginning of a new test module.
        /// </summary>
        /// <param name="name">The name of the test module.</param>
        /// <remarks>
        /// Call this method from the static constructor of test classes.
        /// <see link="(http://docs.jquery.com/QUnit/module#namelifecycle)"/>
        /// </remarks>
        public extern static void Module(string name);

        /// <summary>
        /// Registers an async test case.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <param name="testCase">The test case.</param>
        /// <remarks>
        /// Call this method from the static constructor of test classes.
        /// <see link="(http://docs.jquery.com/QUnit/asyncTest#nameexpectedtest)"/>
        /// </remarks>
        public extern static void AsyncTest(string testName, Action testCase);

        /// <summary>
        /// Registers an async test case.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <param name="expectedAsserts">How many assertions are expected to run.</param>
        /// <param name="testCase">The test case.</param>
        /// <remarks>
        /// Call this method from the static constructor of test classes.
        /// <see link="http://docs.jquery.com/QUnit/asyncTest#nameexpectedtest"/>
        /// </remarks>
        public extern static void AsyncTest(string testName, int expectedAsserts, Action testCase);

        /// <summary>
        /// Registers a test case.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <param name="expectedAsserts">How many assertions are expected to run.</param>
        /// <param name="testCase">The test case.</param>
        /// <remarks>
        /// Call this method from the static constructor of test classes.
        /// <see link="http://docs.jquery.com/QUnit/test#nameexpectedtest"/>
        /// </remarks>
        public extern static void Test(string testName, int expectedAsserts, Action testCase);
    }
}
