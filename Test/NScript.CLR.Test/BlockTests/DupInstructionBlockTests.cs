//-----------------------------------------------------------------------
// <copyright file="DupInstructionBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.BlockTests
{
    using NUnit.Framework;

    /// <summary>
    /// Definition for DupInstructionBlockTests
    /// </summary>
    [TestFixture]
    public class DupInstructionBlockTests
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.DupInstructionTemplates.";
        private const string ClassName = @"DupInstructionBlocks";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPostfixOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PostfixOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PostfixOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPostfixPropertyOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PostfixPropertyOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PostfixPropertyOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPostfixSecondOrderPropertyOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PostfixSecondOrderPropertyOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PostfixSecondOrderPropertyOperation",
                isDebug);
        }

        [Test]
        [TestCase(false, "PostfixPropertyInConditionOperation.xml")]
        [TestCase(true, "PostfixPropertyInConditionOperationDebug.xml")]
        public void TestPostfixPropertyInConditionOperation(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + fileName,
                DupInstructionBlockTests.ClassName,
                "PostfixPropertyInConditionOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPropertyInlineEquals(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PropertyInlineEquals.xml",
                DupInstructionBlockTests.ClassName,
                "PropertyInlineEquals",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestFunctionCallWithInlineAssignment(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "FunctionCallWithInlineAssignment.xml",
                DupInstructionBlockTests.ClassName,
                "FunctionCallWithInlineAssignment",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPostfixPropertyOnPropertyOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PostfixPropertyOnPropertyOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PostfixPropertyOnPropertyOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLocalOpAssignmentOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "LocalOpAssignmentOperation.xml",
                DupInstructionBlockTests.ClassName,
                "LocalOpAssignmentOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPropertyOpAssignmentOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PropertyOpAssignmentOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PropertyOpAssignmentOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPropertyOnPropertyOpAssignmentOperation(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "PropertyOnPropertyOpAssignmentOperation.xml",
                DupInstructionBlockTests.ClassName,
                "PropertyOnPropertyOpAssignmentOperation",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestNullCheckAssignment(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "NullCheckAssignment.xml",
                DupInstructionBlockTests.ClassName,
                "NullCheckAssignment",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestInlineOpAssignment(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                DupInstructionBlockTests.TemplateNamespace + "InlineOpAssignment.xml",
                DupInstructionBlockTests.ClassName,
                "InlineOpAssignment",
                isDebug);
        }
    }
}