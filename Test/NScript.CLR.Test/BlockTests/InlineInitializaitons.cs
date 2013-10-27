//-----------------------------------------------------------------------
// <copyright file="InlineInitializaitons.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.BlockTests
{
    using NScript.CLR.Decompiler.Blocks;
    using NUnit.Framework;

    /// <summary>
    /// Definition for InlineInitializaitons
    /// </summary>
    [TestFixture]
    public class InlineInitializaitons
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.InlineInitialization.";
        private const string ClassName = "InlineComplexStatements";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestInlineArrayInitialization(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                InlineInitializaitons.TemplateNamespace + "InlineArrayInitialization.xml",
                InlineInitializaitons.ClassName,
                "ReturnInlineObjectArray",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                IfElseBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestVarArgs(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                InlineInitializaitons.TemplateNamespace + "VarArgs.xml",
                InlineInitializaitons.ClassName,
                "TestVarArgs",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                IfElseBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestReturnInlineSettersElement(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                InlineInitializaitons.TemplateNamespace + "ReturnInlineSettersElement.xml",
                InlineInitializaitons.ClassName,
                "ReturnInlineSettersElement",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                InlinePropertyInitializerBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                IfElseBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestCallMethodWithInlineSettersElement(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                InlineInitializaitons.TemplateNamespace + "CallMethodWithInlineSettersElement.xml",
                InlineInitializaitons.ClassName,
                "CallMethodWithInlineSettersElement",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                InlinePropertyInitializerBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                IfElseBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestInlineConstArrayInitialization(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                InlineInitializaitons.TemplateNamespace + "ReturnInlineConstIntArray.xml",
                InlineInitializaitons.ClassName,
                "ReturnInlineConstIntArray",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                InlineObjectArrayCreateBlockBuilder.Process,
                InlinePropertyInitializerBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues,
                IfElseBlockBuilder.Process);
        }
    }
}
