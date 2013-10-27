//-----------------------------------------------------------------------
// <copyright file="DupInstructionBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Definition for DupInstructionBlockTests
    /// </summary>
    [TestFixture]
    public class DupInstructionTests
    {
        private const string TestClassNameStr = "DupInstructionBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.DupInstructionBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", false, "PostfixPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", false, "PostfixSecondOrderPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperationDebug.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", false, "PostfixPropertyInConditionOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", false, "FunctionCallWithInlineAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", false, "PropertyInlineEquals.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", false, "PostfixPropertyOnPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", false, "LocalOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", false, "PropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", false, "PropertyOnPropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", false, "NullCheckAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", false, "InlineOpAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", false, "FieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", false, "StaticFieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", false, "InlineFieldPrePostfixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", false, "StaticFieldPrePostifixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", false, "RegressionPreIncrementInCompare.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                DupInstructionTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", false, "PostfixPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", false, "PostfixSecondOrderPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", false, "PostfixPropertyInConditionOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", false, "FunctionCallWithInlineAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", false, "PropertyInlineEquals.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", false, "PostfixPropertyOnPropertyOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", false, "LocalOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", false, "PropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", false, "PropertyOnPropertyOpAssignmentOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", false, "NullCheckAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", false, "InlineOpAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", false, "FieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", false, "StaticFieldPrefix.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", false, "InlineFieldPrePostfixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", false, "StaticFieldPrePostifixAssignment.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.xml")]
        [TestCase(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", false, "RegressionPreIncrementInCompare.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                DupInstructionTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                "InlineOpAssignment",
                true).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}