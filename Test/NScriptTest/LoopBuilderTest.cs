using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Lib.AsmDeasm.IlBlocks;

namespace NScriptTest
{
    [TestClass]
    public class LoopBuilderTest
    {
        private const string TemplateNamespace = "NScriptTest.LoopBuilderTemplates.";

        [TestMethod]
        public void SimpleDoWhileLoopTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleDoWhileLoop);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                DoWhileLoopBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    LoopBuilderTest.TemplateNamespace + "SimpleDoWhileLoop.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void SimpleWhileLoopTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleWhileLoop);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                DoWhileLoopBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    LoopBuilderTest.TemplateNamespace + "SimpleWhileLoop.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void SimpleForLoopTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleForLoop);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForLoopBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    LoopBuilderTest.TemplateNamespace + "SimpleForLoop.xml");

            blockInfo.CheckSame(actual);
        }

        //[TestMethod]
        public void NestedWhileDoWhileLoop1Test()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.NestedWhileDoWhileLoop1);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForLoopBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    LoopBuilderTest.TemplateNamespace + "NestedWhileDoWhileLoop1.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void LastDoWhileBlockTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.LastDoWhileBlock);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                DoWhileLoopBuilder.Process,
                ForLoopBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    LoopBuilderTest.TemplateNamespace + "LastDoWhileBlock.xml");

            blockInfo.CheckSame(actual);
        }
    }
}
