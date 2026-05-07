//-----------------------------------------------------------------------
// <copyright file="ArrayWithSpreadsConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Converter.ExpressionsConverter;
    using NScript.JST;
    using NScript.Utils;

    /// <summary>
    /// Structural invariants for <see cref="ArrayWithSpreadsConverter.BuildConcatCall"/>.
    ///
    /// The Phase F1 collection-expression-with-spread lowering relies on wrapping
    /// every output through <c>[].concat(...)</c> — even for a lone spread argument —
    /// because collection-expression semantics require a fresh array. If the wrapper
    /// is ever short-circuited in the lone-spread case, the spread source would alias
    /// into the result and a later mutation on the produced collection would be
    /// observable on the source. These tests pin the structural shape directly so a
    /// future refactor can't quietly drop the wrapper.
    /// </summary>
    [TestClass]
    public class ArrayWithSpreadsConverterTests
    {
        [TestMethod]
        public void BuildConcatCall_SingleSpreadArg_WrapsInEmptyArrayConcat()
        {
            var scope = new IdentifierScope(false);
            var location = new Location("test", 1, 1);
            var spreadArg = new StringLiteralExpression(scope, "fakeSpread", location);

            var result = ArrayWithSpreadsConverter.BuildConcatCall(
                location,
                scope,
                new List<Expression> { spreadArg });

            AssertConcatShape(result, expectedArgCount: 1);
        }

        [TestMethod]
        public void BuildConcatCall_MultipleArgs_WrapsInEmptyArrayConcat()
        {
            var scope = new IdentifierScope(false);
            var location = new Location("test", 1, 1);
            var arg0 = new StringLiteralExpression(scope, "first", location);
            var arg1 = new StringLiteralExpression(scope, "second", location);
            var arg2 = new StringLiteralExpression(scope, "third", location);

            var result = ArrayWithSpreadsConverter.BuildConcatCall(
                location,
                scope,
                new List<Expression> { arg0, arg1, arg2 });

            var call = AssertConcatShape(result, expectedArgCount: 3);
            Assert.AreSame(arg0, call.Arguments[0]);
            Assert.AreSame(arg1, call.Arguments[1]);
            Assert.AreSame(arg2, call.Arguments[2]);
        }

        // Asserts the expression has the shape:
        //   ([]).concat(arg0, arg1, ..., argN-1)
        // i.e. a method call whose target is an IndexExpression whose left operand
        // is an empty InlineNewArrayInitialization and whose right operand is the
        // string literal "concat".
        private static MethodCallExpression AssertConcatShape(Expression result, int expectedArgCount)
        {
            Assert.IsInstanceOfType(result, typeof(MethodCallExpression));
            var call = (MethodCallExpression)result;
            Assert.AreEqual(expectedArgCount, call.Arguments.Count);

            Assert.IsInstanceOfType(call.MethodExpression, typeof(IndexExpression));
            var index = (IndexExpression)call.MethodExpression;

            Assert.IsInstanceOfType(index.LeftExpression, typeof(InlineNewArrayInitialization));
            var receiver = (InlineNewArrayInitialization)index.LeftExpression;
            Assert.AreEqual(0, receiver.Values.Count, "receiver array literal must be empty for fresh-array semantics");

            Assert.IsInstanceOfType(index.RightExpression, typeof(StringLiteralExpression));
            var member = (StringLiteralExpression)index.RightExpression;
            Assert.AreEqual("concat", member.StringLiteral);

            return call;
        }
    }
}
