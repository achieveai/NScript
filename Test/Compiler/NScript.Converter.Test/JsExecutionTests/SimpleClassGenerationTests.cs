//-----------------------------------------------------------------------
// <copyright file="SimpleClassGenerationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System;
    // using NScript.CLR.Test;
    using NScript.Csc.Lib.Test;
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
        [DataRow(TestType.Debug, "FactorialCalculator", "Main", false, false, false)]
        [DataRow(TestType.Debug, "FactorialCalculator", "Main", false, false, true)]
        [DataRow(TestType.Debug, "ForEachLoopTests", "Main", false, false, false)]
        [DataRow(TestType.Debug, "GetMin", "RunTest", false, false, false)]
        [DataRow(TestType.Debug, "GetMin", "RunTest", false, false, true)]
        [DataRow(TestType.Debug, "QuickSort", "RunTest", false, false, false)]
        [DataRow(TestType.Debug, "QuickSort", "RunTest", false, false, true)]
        [DataRow(TestType.Debug, "TestArithmetics", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TestArithmetics", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TestControlFlow", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TestControlFlow", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TestInitializer", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TestInitializer", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TestDelegates", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TestDelegates", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TestGenerics", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TestGenerics", "Main", false, false, true)]
        [DataRow(TestType.Debug, "NullableTests", "Main", false, false, false)]
        [DataRow(TestType.Debug, "NullableTests", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TupleTests", "Main", false, false, false)]
        [DataRow(TestType.Debug, "TupleTests", "Main", false, false, true)]
        [DataRow(TestType.Debug, "YieldReturnTests", "Main", false, false, false)]
        [DataRow(TestType.Debug, "YieldReturnTests", "Main", false, false, true)]
        [DataRow(TestType.Debug, "TestStdlib", "Main", true, false, false)]
        [DataRow(TestType.Debug, "TestStdlib", "Main", true, false, true)]
        [DataRow(TestType.Debug, "TestAsyncAwait", "Main", false, true, false)]
        [DataRow(TestType.Debug, "TestAsyncAwait", "Main", false, true, true)]
        public void TestMcs(
            TestType testType,
            string className,
            string entryPointMethod,
            bool isDisabled = false,
            bool isAsyncEntryPoint = false,
            bool instanceAsStatic = false)
        {
            if (isDisabled)
            { return; }

            if ((testType & TestType.Debug) == TestType.Debug)
            {
                if (isAsyncEntryPoint)
                {
                    JsExecutionHelper.ExecuteAsyncScript(
                        className,
                        true,
                        Tuple.Create(className, entryPointMethod),
                        true,
                        instanceAsStatic).Wait();
                }
                else
                {
                    JsExecutionHelper.ExecuteScript(
                        className,
                        true,
                        Tuple.Create(className, entryPointMethod),
                        true,
                        instanceAsStatic);
                }
            }

            if ((testType & TestType.Retail) == TestType.Retail)
            {
                JsExecutionHelper.ExecuteScript(
                    className,
                    false,
                    Tuple.Create(className, entryPointMethod),
                    true,
                    instanceAsStatic);
            }
        }
    }
}