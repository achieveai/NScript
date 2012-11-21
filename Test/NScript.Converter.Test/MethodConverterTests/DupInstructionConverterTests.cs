//-----------------------------------------------------------------------
// <copyright file="DupInstructionConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for DupInstructionConverterTests
    /// </summary>
    [TestFixture]
    public class DupInstructionConverterTests
    {
        private const string TestClassNameStr = @"DupInstructionBlocks";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DupInstructionConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperationDebug.js", TestType.Debug)]
        [Row(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.js", TestType.Retail)]
        [Row(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.js", TestType.All)]
        [Row(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.js", TestType.All)]
        [Row(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.js", TestType.All)]
        [Row(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.js", TestType.All)]
        [Row(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.js", TestType.All)]
        [Row(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.js", TestType.All)]
        [Row(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.js", TestType.All)]
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