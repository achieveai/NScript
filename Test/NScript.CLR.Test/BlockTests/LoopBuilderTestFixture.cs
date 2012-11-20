using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using NScript.CLR.Decompiler.Blocks;

namespace NScript.CLR.Test.BlockTests
{
    [TestFixture]
    public class LoopBuilderTestFixture
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.LoopBuilderTemplates.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        public void SimpleWhileLoopDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleWhileLoopDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForBlockBuilder.Process);
        }

        [Test]
        public void SimpleWhileLoopRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleWhileLoopRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForBlockBuilder.Process);
        }

        [Test]
        public void SimpleDoWhileLoopDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleDoWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleDoWhileLoopDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForBlockBuilder.Process);
        }

        [Test]
        public void SimpleDoWhileLoopRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleDoWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleDoWhileLoopRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForBlockBuilder.Process);
        }

        [Test]
        public void SimpleForLoopDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleForLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleForLoopDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void SimpleForLoopRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "SimpleForLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "SimpleForLoopRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void OnlyForLoopDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "OnlyForLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "OnlyForLoopDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void OnlyForLoopRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "OnlyForLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "OnlyForLoopRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void OnlyWhileLoopDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "OnlyWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "OnlyWhileLoopDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void OnlyWhileLoopRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "OnlyWhileLoop",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "OnlyWhileLoopRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void NestedWhileDoWhileDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "NestedWhileDoWhile",
                true,
                LoopBuilderTestFixture.TemplateNamespace + "NestedWhileDoWhileDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }

        [Test]
        public void NestedWhileDoWhileRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "LoopTests",
                "NestedWhileDoWhile",
                false,
                LoopBuilderTestFixture.TemplateNamespace + "NestedWhileDoWhileRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process,
                DoWhileLoopBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForBlockBuilder.Process,
                JumpBlockBuilder.ProcessContinues);
        }
    }
}
