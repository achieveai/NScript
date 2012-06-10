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
    /// <remarks>Until VS plugin is ready, I'm using this class as a entry point for QUnit, that way no tests
    /// have to be changed, only this class.  (kevinc)</remarks>
    [Imported]
    [IgnoreNamespace]
    [GlobalMethods]
    public static class Assert
    {
        /// <summary>
        /// Specify the number of expected assertions to guarantee that failed test (no assertions are run at all) don't slip through.
        /// <see link="http://docs.jquery.com/QUnit/expect"/>
        /// </summary>
        /// <param name="assertions">The expected number of assertions in the test.</param>
        [ScriptAlias("QUnit.expect")]
        public extern static void Expect(int assertions);

        /// <summary>
        /// Asserts true
        /// </summary>
        /// <param name="b">boolean</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("QUnit.ok")]
        public extern static void IsTrue(bool b, string message);

        /// <summary>
        /// Will compare for Not Equal
        /// </summary>
        /// <param name="expected">expected object</param>
        /// <param name="actual">actual object</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("QUnit.notEqual")]
        [IgnoreGenericArguments]
        public extern static void NotEqual<T>(T expected, T actual, string message);

        /// <summary>
        /// Will Check for Equality
        /// </summary>
        /// <param name="expected">expected object</param>
        /// <param name="actual">actual object</param>
        /// <param name="message">message to be logged</param>
        [ScriptAlias("QUnit.equal")]
        [IgnoreGenericArguments]
        public extern static void Equal<T>(T expected, T actual, string message);

        /// <summary>
        /// Checks that the first two arguments are strictly equal (===).
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">The message logged by the check.</param>
        /// <remarks>
        /// Prints out both actual and expected values.
        /// </remarks>
        [ScriptAlias("QUnit.strictEqual")]
        [IgnoreGenericArguments]
        public extern static void StrictEqual<T>(T expected, T actual, string message);

        /// <summary>
        /// Checks that the first two arguments are not strictly equal (!==).
        /// </summary>
        /// <param name="actual">The actual value.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">The message logged by the check.</param>
        /// <remarks>
        /// Prints out both actual and expected values.
        /// </remarks>
        [ScriptAlias("QUnit.notStrictEqual")]
        [IgnoreGenericArguments]
        public extern static void NotStrictEqual<T>(T expected, T actual, string message);

        /// <summary>
        /// Checks that the given function raises an exception.
        /// </summary>
        /// <param name="callback">Function that is supposed to raise an exception.</param>
        /// <param name="message">The message logged by the check.</param>
        [ScriptAlias("QUnit.raises")]
        public extern static void Raises(Callback callback, string message);
    }
}
