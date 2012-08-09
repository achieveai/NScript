//-----------------------------------------------------------------------
// <copyright file="WhileLoopBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for WhileLoopBlockConverters
    /// </summary>
    [TestFixture]
    public class WhileLoopBlockConverters
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.WhileLoopBlockConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "DoWhileLoop", "DoWhileLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoop", "WhileLoop.js", TestType.All)]
        [Row(TestClassNameStr, "DoWhilePaddedLoop", "DoWhilePaddedLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhilePaddedLoop", "WhilePaddedLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoopWithBreak", "WhileLoopWithBreak.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinue.js", TestType.Debug)]
        [Row(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinueRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "DoWhileLoop", "DoWhileLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoop", "WhileLoop.js", TestType.All)]
        [Row(TestClassNameStr, "DoWhilePaddedLoop", "DoWhilePaddedLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhilePaddedLoop", "WhilePaddedLoop.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoopWithBreak", "WhileLoopWithBreak.js", TestType.All)]
        [Row(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinue.js", TestType.Debug)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}
