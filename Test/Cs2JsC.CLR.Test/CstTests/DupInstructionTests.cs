//-----------------------------------------------------------------------
// <copyright file="DupInstructionBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for DupInstructionBlockTests
    /// </summary>
    [TestFixture]
    public class DupInstructionTests
    {
        private const string TestClassNameStr = "DupInstructionBlocks";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.DupInstructionBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", false, "PostfixPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", false, "PostfixSecondOrderPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperationDebug.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", false, "PostfixPropertyInConditionOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", false, "FunctionCallWithInlineAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", false, "PropertyInlineEquals.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", false, "PostfixPropertyOnPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", false, "LocalOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", false, "PropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", false, "PropertyOnPropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", false, "NullCheckAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", false, "InlineOpAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", false, "FieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", false, "StaticFieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", false, "InlineFieldPrePostfixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", false, "StaticFieldPrePostifixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", false, "RegressionPreIncrementInCompare.xml")]
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
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", false, "PostfixPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", false, "PostfixSecondOrderPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", false, "PostfixPropertyInConditionOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", false, "FunctionCallWithInlineAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", false, "PropertyInlineEquals.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", false, "PostfixPropertyOnPropertyOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", false, "LocalOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", false, "PropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", false, "PropertyOnPropertyOpAssignmentOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", false, "NullCheckAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", false, "InlineOpAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", false, "FieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", false, "StaticFieldPrefix.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PostfixOperation", false, "PostfixOperation.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", false, "InlineFieldPrePostfixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", false, "StaticFieldPrePostifixAssignment.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.xml")]
        [Row(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", false, "RegressionPreIncrementInCompare.xml")]
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

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}