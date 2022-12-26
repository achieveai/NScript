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
        [DataRow(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.static.js", TestType.All, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                instanceAsStatic);
        }
    }
}