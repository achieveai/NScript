//-----------------------------------------------------------------------
// <copyright file="ForLoopConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for ForLoopConverterTests
    /// </summary>
    [TestFixture]
    public class ForLoopConverterTests
    {
        private const string TestClassNameStr = "ForLoopBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.ForLoopConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "ForLoopBasic", "ForLoopBasic.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopPadded", "ForLoopPadded.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopWithContinue", "ForLoopWithContinue.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreak.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreakRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "ForEachLoop", "ForEachLoop.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType);
        }

        [Test]
        [TestCase(TestClassNameStr, "ForLoopBasic", "ForLoopBasic.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopPadded", "ForLoopPadded.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopWithContinue", "ForLoopWithContinueMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreak.js", TestType.All)]
        [TestCase(TestClassNameStr, "ForEachLoop", "ForEachLoop.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType,
                true);
        }
    }
}
