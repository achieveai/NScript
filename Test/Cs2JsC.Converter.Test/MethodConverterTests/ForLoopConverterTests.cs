//-----------------------------------------------------------------------
// <copyright file="ForLoopConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for ForLoopConverterTests
    /// </summary>
    [TestFixture]
    public class ForLoopConverterTests
    {
        private const string TestClassNameStr = "ForLoopBlocks";
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.ForLoopConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "ForLoopBasic", "ForLoopBasic.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopPadded", "ForLoopPadded.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopWithContinue", "ForLoopWithContinue.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreak.js", TestType.Debug)]
        [Row(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreakRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "ForEachLoop", "ForEachLoop.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "ForLoopBasic", "ForLoopBasic.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopPadded", "ForLoopPadded.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopWithContinue", "ForLoopWithContinueMcs.js", TestType.All)]
        [Row(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreak.js", TestType.All)]
        [Row(TestClassNameStr, "ForEachLoop", "ForEachLoop.js", TestType.All)]
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
