//-----------------------------------------------------------------------
// <copyright file="TestSetup.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SunlightUnit
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    [JsonType]
    public class TestEnvironment
    {
        [ScriptName("before")]
        public extern Action Setup
        { get; set; }

        [ScriptName("before")]
        public extern Func<Promise> SetupAsync
        { get; set; }

        [ScriptName("beforeEach")]
        public extern Action EachTestSetup
        { get; set; }

        [ScriptName("beforeEach")]
        public extern Func<Promise> EachTestSetupAsync
        { get; set; }

        [ScriptName("afterEach")]
        public extern Action EachTestTeardown
        { get; set; }

        [ScriptName("afterEach")]
        public extern Func<Promise> EachTestTeardownAsync
        { get; set; }

        [ScriptName("after")]
        public extern Action Teardown
        { get; set; }

        [ScriptName("after")]
        public extern Func<Promise> TeardownAsync
        { get; set; }
    }

    /// <summary>
    /// Class used to setup test cases and suites
    /// </summary>
    [ImportedType]
    [ScriptName("QUnit")]
    [IgnoreNamespace]
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
        public extern static void Module(string name, TestEnvironment testEnvironment);

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
        public extern static void Test(string testName, Action<Assert> testCase);

        public extern static void Test(string testName, Func<Assert, Promise> testCase);

        public extern static void Only(string testName, Action<Assert> testCase);

        public extern static void Only(string testName, Func<Assert, Promise> testCase);

        public extern static void Todo(string testName, Action<Assert> todoTestCase);

        public extern static void Todo(string testName, Func<Assert, Promise> todoTestCase);

        public extern static void Skip(string testName, Action<Assert> skipTestCase);

        public extern static void Skip(string testName, Func<Assert, Promise> skipTestCase);

        public extern static void Begin(Action<Dictionary> callback);

        public extern static void Log(Action<Dictionary> callback);

        public extern static void ModuleDone(Action<Dictionary> callback);

        public extern static void ModuleStart(Action<Dictionary> callback);

        public extern static void TestDone(Action<Dictionary> callback);

        public extern static void TestStart(Action<Dictionary> callback);
    }
}
