//-----------------------------------------------------------------------
// <copyright file="TestRunner.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SunlightUnit
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TestRunner
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    [GlobalMethods]
    public static class TestRunner
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message logged by the check.</param>
        [ScriptAlias("QUnit.log")]
        public extern static void Log(string message);

        /// <summary>
        /// Stops the test running to wait for async tests to complete.
        /// </summary>
        /// <param name="timeout">How long to wait for a call to ResumeOnAsyncCompleted, in milliseconds.</param>
        /// <remarks>
        /// Call Start to continue running the tests.
        /// <see link="http://docs.jquery.com/QUnit/stop"/>
        /// </remarks>
        [ScriptAlias("QUnit.stop")]
        public extern static void WaitForAsyncCompletion(int timeout);

        /// <summary>
        /// Resumes the test runner after an async test has completed.
        /// <see link="http://docs.jquery.com/QUnit/start"/>
        /// </summary>
        [ScriptAlias("QUnit.start")]
        public extern static void ResumeOnAsyncCompleted();
    }
}
