//-----------------------------------------------------------------------
// <copyright file="Assert.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace SunlightUnit
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Assert Class Definition
    /// </summary>
    /// <remarks>
    /// Until VS plugin is ready, I'm using this class as a entry point for QUnit, that way no tests
    /// have to be changed, only this class. (kevinc)
    /// </remarks>
    [ImportedType]
    [ScriptName("QUnit.assert")]
    public class Assert
    {
        /// <summary>
        /// Returns completion function to be called when completing asyn test
        /// </summary>
        /// <param name="n">Number of times async function expected to be called</param>
        /// <returns>Callback for notifing asyncFunction completion.</returns>
        public extern Action Async(int n);

        [IgnoreGenericArguments]
        public extern void DeepEqual<T>(T actual, T expected, string message);

        /// <summary>
        /// Will Check for Equality
        /// </summary>
        /// <param name="expected">expected object</param>
        /// <param name="actual">actual object</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("equal")]
        [IgnoreGenericArguments]
        public extern void Equal<T>(T actual, T expected, string message);

        /// <summary>
        /// Specify the number of expected assertions to guarantee that failed test (no assertions
        /// are run at all) don't slip through. <see link="http://docs.jquery.com/QUnit/expect"/>
        /// </summary>
        /// <param name="assertions">The expected number of assertions in the test.</param>
        [ScriptName("expect")]
        public extern void Expect(int assertions);

        [ScriptAlias("notOk")]
        public extern void IsFalse(bool b, string message);

        /// <summary>
        /// Asserts true
        /// </summary>
        /// <param name="b">boolean</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("ok")]
        public extern void IsTrue(bool b, string message);

        [IgnoreGenericArguments]
        public extern void NotDeepEqual<T>(T actual, T unexpected, string message);

        /// <summary>
        /// Will compare for Not Equal
        /// </summary>
        /// <param name="expected">expected object</param>
        /// <param name="actual">actual object</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("notEqual")]
        [IgnoreGenericArguments]
        public extern void NotEqual<T>(T actual, T expected, string message);

        [IgnoreGenericArguments]
        public extern void NotPropEqual<T>(T actual, T unexpected, string message);

        /// <summary>
        /// Checks that the first two arguments are not strictly equal (!==).
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">The message logged by the check.</param>
        /// <remarks>Prints out both actual and expected values.</remarks>
        [ScriptAlias("notStrictEqual")]
        [IgnoreGenericArguments]
        public extern void NotStrictEqual<T>(T actual, T expected, string message);

        [IgnoreGenericArguments]
        public extern void PropEqual<T>(T actual, T expected, string message);

        public extern void Step(string stepId);

        /// <summary>
        /// Checks that the first two arguments are strictly equal (===).
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">The message logged by the check.</param>
        /// <remarks>Prints out both actual and expected values.</remarks>
        [ScriptAlias("strictEqual")]
        [IgnoreGenericArguments]
        public extern void StrictEqual<T>(T actual, T expected, string message);

        /// <summary>
        /// Checks that the given function raises an exception.
        /// </summary>
        /// <param name="callback">Function that is supposed to raise an exception.</param>
        /// <param name="message">The message logged by the check.</param>
        [ScriptAlias("throws")]
        public extern void Throws(Callback callback);

        [ScriptAlias("throws")]
        public extern void Throws(Callback callback, RegularExpression regex, string message);

        [ScriptAlias("throws")]
        public extern void Throws(Callback callback, Type type, string message);

        [ScriptAlias("throws")]
        public extern void Throws(Callback callback, Func<object, bool> verifyCallback, string message);

        public extern void VerifySteps(string[] stepIds);
    }
}