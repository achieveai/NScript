//-----------------------------------------------------------------------
// <copyright file="WhileLoopBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for WhileLoopBlockConverters
    /// </summary>
    [TestFixture]
    public class WhileLoopBlockConverters
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.WhileLoopBlockConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "DoWhileLoop", "DoWhileLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoop", "WhileLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "DoWhilePaddedLoop", "DoWhilePaddedLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhilePaddedLoop", "WhilePaddedLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoopWithBreak", "WhileLoopWithBreak.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinue.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinueRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [TestCase(TestClassNameStr, "DoWhileLoop", "DoWhileLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoop", "WhileLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "DoWhilePaddedLoop", "DoWhilePaddedLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhilePaddedLoop", "WhilePaddedLoop.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoopWithBreak", "WhileLoopWithBreak.js", TestType.All)]
        [TestCase(TestClassNameStr, "WhileLoopWithContinue", "WhileLoopWithContinue.js", TestType.Debug)]
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
