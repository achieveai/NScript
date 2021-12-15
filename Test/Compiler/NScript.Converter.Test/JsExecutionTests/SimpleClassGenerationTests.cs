//-----------------------------------------------------------------------
// <copyright file="SimpleClassGenerationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System;
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for SimpleClassGenerationTests
    /// </summary>
    [TestClass]
    public class SimpleClassGenerationTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }


        [TestCategory("CI")]
        [DataTestMethod]
        [TestMethod]
        [DataRow(TestType.Debug, "FactorialCalculator", "Main")]
        [DataRow(TestType.Debug, "GetMin", "RunTest")]
        [DataRow(TestType.Debug, "QuickSort", "RunTest")]
        [DataRow(TestType.Debug, "TestArithmetics", "Main")]
        [DataRow(TestType.Debug, "TestControlFlow", "Main")]
        [DataRow(TestType.Debug, "TestInitializer", "Main")]
        [DataRow(TestType.Debug, "TestDelegates", "Main")]
        [DataRow(TestType.Debug, "TestGenerics", "Main")]
        [DataRow(TestType.Debug, "NullableTests", "Main")]
        [DataRow(TestType.Debug, "TestAsyncAwait", "Main", true)]
        public void TestMcs(
            TestType testType,
            string className,
            string entryPointMethod,
            bool isAsyncEntryPoint = false)
        {
            if ((testType & TestType.Debug) == TestType.Debug)
            {
                if (isAsyncEntryPoint)
                {
                    JsExecutionHelper.ExecuteAsyncScript(
                        className,
                        true,
                        Tuple.Create(className, entryPointMethod),
                        true).Wait();
                }
                else
                {
                    JsExecutionHelper.ExecuteScript(
                        className,
                        true,
                        Tuple.Create(className, entryPointMethod),
                        true);
                }
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