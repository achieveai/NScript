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
        [DataRow(TestType.Debug, "FactorialCalculator", "Main")]
        [DataRow(TestType.Debug, "GetMin", "RunTest")]
        [DataRow(TestType.Debug, "QuickSort", "RunTest")]
        [DataRow(TestType.Debug, "TestArithmetics", "Main")]
        [DataRow(TestType.Debug, "TestControlFlow", "Main")]
        [DataRow(TestType.Debug, "TestInitializer", "Main")]
        [DataRow(TestType.Debug, "TestDelegates", "Main")]
        [DataRow(TestType.Debug, "TestGenerics", "Main")]
        [DataRow(TestType.Debug, "NullableTests", "Main")]
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