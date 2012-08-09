using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;

namespace Cs2JsCTest
{
    [TestClass]
    public class JumpBlockBuilderTest
    {
        private const string TemplateNamespace = "Cs2JsCTest.JumpBlockBuilderTemplates.";

        [TestMethod]
        public void WhileLoopBreakTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.WhileLoopBreakTest);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "WhileLoopBreakTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void WhileLoopContinueTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.WhileLoopContinueTest);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "WhileLoopContinueTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void ForLoopBreakTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.ForLoopBreakTest);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "ForLoopBreakTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void ForLoopContinueTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.ForLoopContinueTest);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "ForLoopContinueTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void ForLoopContinueWoLineNumTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.ForLoopContinueWoLineNumTest);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "ForLoopContinueTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void SimpleSwitchFunctionTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleSwitchFunction);

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
                IfElseBlockBuilder.Process,
                JumpBlockBuilder.ProcessBreaks,
                ForLoopBuilder.Process,
                JumpBlockBuilder.ProcessContinues);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    JumpBlockBuilderTest.TemplateNamespace + "SimpleSwitchFunction.xml");

            blockInfo.CheckSame(actual);
        }
    }
}
