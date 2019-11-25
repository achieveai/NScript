//-----------------------------------------------------------------------
// <copyright file="SimpleClassGenerationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System;
    using NScript.CLR.Test;
    using NUnit.Framework;

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
        // [TestCase(TestType.All, "FactorialCalculator", "Main")]
        // [TestCase(TestType.All, "GetMin", "RunTest")]
        // [TestCase(TestType.All, "QuickSort", "RunTest")]
        // [TestCase(TestType.All, "TestArithmetics", "Main")]
        // [TestCase(TestType.Retail, "TestControlFlow", "Main")]
        // [TestCase(TestType.All, "TestInitializer", "Main")]
        // [TestCase(TestType.All, "TestDelegates", "Main")]
        // [TestCase(TestType.Retail, "TestGenerics", "Main")]
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

        [TestCase(TestType.Debug, "FactorialCalculator", "Main")]
        [TestCase(TestType.Debug, "GetMin", "RunTest")]
        [TestCase(TestType.Debug, "QuickSort", "RunTest")]
        [TestCase(TestType.Debug, "TestArithmetics", "Main")]
        [TestCase(TestType.Debug, "TestControlFlow", "Main")]
        [TestCase(TestType.Debug, "TestInitializer", "Main")]
        [TestCase(TestType.Debug, "TestDelegates", "Main")]
        [TestCase(TestType.Debug, "TestGenerics", "Main")]
        [TestCase(TestType.Debug, "NullableTests", "Main")]
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