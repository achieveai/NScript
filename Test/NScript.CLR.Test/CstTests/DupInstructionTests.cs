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
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOperation", true,  "PostfixPropertyOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixSecondOrderPropertyOperation", true,  "PostfixSecondOrderPropertyOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyInConditionOperation", true,  "PostfixPropertyInConditionOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "FunctionCallWithInlineAssignment", true,  "FunctionCallWithInlineAssignment.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyInlineEquals", true,  "PropertyInlineEquals.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixPropertyOnPropertyOperation", true,  "PostfixPropertyOnPropertyOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "LocalOpAssignmentOperation", true,  "LocalOpAssignmentOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOpAssignmentOperation", true,  "PropertyOpAssignmentOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PropertyOnPropertyOpAssignmentOperation", true,  "PropertyOnPropertyOpAssignmentOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "NullCheckAssignment", true,  "NullCheckAssignment.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "InlineOpAssignment", true,  "InlineOpAssignment.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixFieldIncrement", true,  "FieldPrefix.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrefixStaticFieldIncrement", true,  "StaticFieldPrefix.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PostfixOperation", true,  "PostfixOperation.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixFieldIncrementAssignment", true,  "InlineFieldPrePostfixAssignment.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "PrePostfixStaticFieldIncrementAssignment", true,  "StaticFieldPrePostifixAssignment.csast")]
        [TestCase(DupInstructionTests.TestClassNameStr, "RegressionPreIncrementInCompare", true,  "RegressionPreIncrementInCompare.csast")]
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