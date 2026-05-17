//-----------------------------------------------------------------------
// <copyright file="ExtendedBaseTypes.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Regression coverage for issue #79: prior to the fix the compiler emitted
    /// <c>ptyp_ = new Error()</c> for any user type whose base type was an
    /// <c>[Extended]</c> facade (here, <c>System.Exception</c> mapping to native
    /// <c>Error</c>). That seed-construction ran the native constructor during
    /// module initialization, which fails before the prototype chain is wired.
    /// The fix switches the seed to <c>Object.create(Error.prototype)</c>.
    ///
    /// This V8-execution fixture forces the new code path by constructing a
    /// subclass of <see cref="Exception"/> from <c>Main()</c>. Without the fix,
    /// the bundle fails to load and <c>Main()</c> never runs.
    /// </summary>
    public class ExtendedBaseExceptionExecutionTests
    {
        public static void Main()
        {
            ExtendedBaseException ex = new ExtendedBaseException("boom");
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Custom exception whose base type is the <c>[Extended]</c>
    /// <see cref="Exception"/> facade. Declaring this type is what exercises
    /// the <c>GetBasePrototypeExpression</c> Extended-base branch.
    /// </summary>
    public class ExtendedBaseException : Exception
    {
        public ExtendedBaseException(string message)
            : base(message)
        {
        }
    }
}
