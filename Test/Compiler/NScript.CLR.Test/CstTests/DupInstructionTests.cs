//-----------------------------------------------------------------------
// <copyright file="DupInstructionBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    /// <summary>
    /// Definition for DupInstructionBlockTests
    /// </summary>
    [TestClass]
    public class DupInstructionTests
    {
        private const string TestClassNameStr = "DupInstructionBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.DupInstructionBlocks.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.csast")]
        [DataRow(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.csast")]
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