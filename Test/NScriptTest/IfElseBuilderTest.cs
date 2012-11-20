using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Lib.AsmDeasm.IlBlocks;


namespace NScriptTest
{
    [TestClass]
    public class IfElseBuilderTest
    {
        private const string TemplateNamespace = "NScriptTest.IfElseBuilderTemplates.";

        [TestMethod]
        public void SimpleIfTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleIfTest);

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
                ForLoopBuilder.Process,
                IfElseBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    IfElseBuilderTest.TemplateNamespace + "SimpleIfTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void SimpleIfElseTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleIfElseTest);

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
                ForLoopBuilder.Process,
                IfElseBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    IfElseBuilderTest.TemplateNamespace + "SimpleIfElseTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void SimpleIfElseIfTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.SimpleIfElseIfTest);

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
                ForLoopBuilder.Process,
                IfElseBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    IfElseBuilderTest.TemplateNamespace + "SimpleIfElseIfTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void NestedIfElseTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.NestedIfElseTest);

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
                ForLoopBuilder.Process,
                IfElseBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    IfElseBuilderTest.TemplateNamespace + "NestedIfElseTest.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void WhileLoopContinueToIfTest()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.WhileLoopContinueToIfTest);

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
                ForLoopBuilder.Process,
                IfElseBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    IfElseBuilderTest.TemplateNamespace + "WhileLoopContinueToIfTest.xml");

            blockInfo.CheckSame(actual);
        }
    }
}
