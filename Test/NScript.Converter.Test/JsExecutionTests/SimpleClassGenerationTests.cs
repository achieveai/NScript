//-----------------------------------------------------------------------
// <copyright file="SimpleClassGenerationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System;
    using NScript.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for SimpleClassGenerationTests
    /// </summary>
    [TestFixture]
    public class SimpleClassGenerationTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        // [Test]
        // [Row(TestType.All, "FactorialCalculator", "Main")]
        // [Row(TestType.All, "GetMin", "RunTest")]
        // [Row(TestType.All, "QuickSort", "RunTest")]
        // [Row(TestType.All, "TestArithmetics", "Main")]
        // [Row(TestType.Retail, "TestControlFlow", "Main")]
        // [Row(TestType.All, "TestInitializer", "Main")]
        // [Row(TestType.All, "TestDelegates", "Main")]
        // [Row(TestType.Retail, "TestGenerics", "Main")]
        public void Test(
                TestType testType,
                string className,
                string entryPointMethod)
        {
            if ((testType & TestType.Debug) == TestType.Debug)
            {
                JsExecutionHelper.ExecuteScript(
                    className,
                    true,
                    Tuple.Create(className, entryPointMethod));
            }

            if ((testType & TestType.Retail) == TestType.Retail)
            {
                JsExecutionHelper.ExecuteScript(
                    className,
                    false,
                    Tuple.Create(className, entryPointMethod));
            }
        }

        [Test]
        [Row(TestType.All, "FactorialCalculator", "Main")]
        [Row(TestType.All, "GetMin", "RunTest")]
        [Row(TestType.All, "QuickSort", "RunTest")]
        [Row(TestType.All, "TestArithmetics", "Main")]
        [Row(TestType.All, "TestControlFlow", "Main")]
        [Row(TestType.All, "TestInitializer", "Main")]
        [Row(TestType.All, "TestDelegates", "Main")]
        [Row(TestType.All, "TestGenerics", "Main")]
        [Row(TestType.All, "NullableTests", "Main")]
        public void TestMcs(
                TestType testType,
                string className,
                string entryPointMethod)
        {
            if ((testType & TestType.Debug) == TestType.Debug)
            {
                JsExecutionHelper.ExecuteScript(
                    className,
                    true,
                    Tuple.Create(className, entryPointMethod),
                    true);
            }

            if ((testType & TestType.Retail) == TestType.Retail)
            {
                JsExecutionHelper.ExecuteScript(
                    className,
                    false,
                    Tuple.Create(className, entryPointMethod),
                    true);
            }
        }
    }
}