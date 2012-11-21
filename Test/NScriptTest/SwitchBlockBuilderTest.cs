using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Lib.AsmDeasm.IlBlocks;

namespace NScriptTest
{
    [TestClass]
    public class SwitchBlockBuilderTest
    {
        private const string TemplateNamespace = "NScriptTest.SwitchBlockBuilderTemplates.";

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
                SwitchBlock.SwitchBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    SwitchBlockBuilderTest.TemplateNamespace + "SimpleSwitchFunction.xml");

            blockInfo.CheckSame(actual);
        }
    }
}
