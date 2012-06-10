using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cs2JsC.Lib.AsmDeasm.IlBlocks;

namespace Cs2JsCTest
{
    [TestClass]
    public class BasicStatementBuilderTest
    {
        private const string TemplateNamespace = "Cs2JsCTest.BasicStatementBuilderTemplates.";

        [TestMethod]
        public void Complex2IfConditionStatementBuilder()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.Complex2IfCondition);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                ConditionalStatementBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicStatementBuilderTest.TemplateNamespace + "Complex2IfConditionStatementBuilder.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void Complex2IfConditionBlockBuilder()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.Complex2IfCondition);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicStatementBuilderTest.TemplateNamespace + "Complex2IfConditionBlockBuilder.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void ComplexIfConditoinOptimized1StatementBuilder()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.ComplexIfConditionOptimized1);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicStatementBuilderTest.TemplateNamespace + "ComplexIfConditoinOptimized1StatementBuilder.xml");

            blockInfo.CheckSame(actual);
        }

        [TestMethod]
        public void ComplexIfConditoinOptimized2StatementBuilder()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.ComplexIfConditionOptimized2);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicStatementBuilderTest.TemplateNamespace + "ComplexIfConditoinOptimized2StatementBuilder.xml");

            blockInfo.CheckSame(actual);
        }
    }
}
