//-----------------------------------------------------------------------
// <copyright file="ForLoopConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for ForLoopConverterTests
    /// </summary>
    [TestClass]
    public class ForLoopConverterTests
    {
        private const string TestClassNameStr = "ForLoopBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.ForLoopConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "ForLoopBasic", "ForLoopBasic.js", TestType.All)]
        [DataRow(TestClassNameStr, "ForLoopPadded", "ForLoopPadded.js", TestType.All)]
        [DataRow(TestClassNameStr, "ForLoopWithContinue", "ForLoopWithContinueMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "ForLoopWithBreak", "ForLoopWithBreak.js", TestType.All)]
        [DataRow(TestClassNameStr, "ForEachLoop", "ForEachLoop.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType,
                true,
                false);
        }
    }
}
