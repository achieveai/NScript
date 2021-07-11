//-----------------------------------------------------------------------
// <copyright file="DupInstructionConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for DupInstructionConverterTests
    /// </summary>
    [TestClass]
    public class DupInstructionConverterTests
    {
        private const string TestClassNameStr = @"DupInstructionBlocks";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DupInstructionConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.js", TestType.All)]
        [DataRow(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.js", TestType.All)]
        [DataRow(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.js", TestType.All)]
        [DataRow(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.js", TestType.All)]
        [DataRow(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.js", TestType.All)]
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