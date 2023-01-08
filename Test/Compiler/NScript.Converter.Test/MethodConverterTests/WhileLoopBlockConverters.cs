//-----------------------------------------------------------------------
// <copyright file="WhileLoopBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for WhileLoopBlockConverters
    /// </summary>
    [TestClass]
    public class WhileLoopBlockConverters
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.WhileLoopBlockConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "DoWhileLoop", "DoWhileLoop.js", TestType.All)]
        [DataRow(TestClassNameStr, "DoWhilePaddedLoop", "DoWhilePaddedLoop.js", TestType.All)]
        [DataRow(TestClassNameStr, "WhileLoop", "WhileLoop.js", TestType.All)]
        [DataRow(TestClassNameStr, "WhileLoopWithBreak", "WhileLoopWithBreak.js", TestType.All)]
        [DataRow(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinue.js", TestType.Debug)]
        [DataRow(TestClassNameStr, "WhilePaddedLoop", "WhilePaddedLoop.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                false);
        }
    }
}
