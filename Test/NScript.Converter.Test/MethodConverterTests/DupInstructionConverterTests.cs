//-----------------------------------------------------------------------
// <copyright file="DupInstructionConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

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
        [TestCase(TestClassNameStr, "PostfixOperation", "PostfixOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "PostfixPropertyOperation", "PostfixPropertyOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "PostfixSecondOrderPropertyOperation", "PostfixSecondOrderPropertyOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "PostfixPropertyInConditionOperation", "PostfixPropertyInConditionOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "FunctionCallWithInlineAssignment", "FunctionCallWithInlineAssignment.js", TestType.All)]
        [TestCase(TestClassNameStr, "PropertyInlineEquals", "PropertyInlineEquals.js", TestType.All)]
        [TestCase(TestClassNameStr, "PostfixPropertyOnPropertyOperation", "PostfixPropertyOnPropertyOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "LocalOpAssignmentOperation", "LocalOpAssignmentOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "PropertyOpAssignmentOperation", "PropertyOpAssignmentOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", "PropertyOnPropertyOpAssignmentOperation.js", TestType.All)]
        [TestCase(TestClassNameStr, "NullCheckAssignment", "NullCheckAssignment.js", TestType.All)]
        [TestCase(TestClassNameStr, "InlineOpAssignment", "InlineOpAssignment.js", TestType.All)]
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