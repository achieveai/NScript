using Cs2JsC.Lib.AsmDeasm.IlBlocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cs2JsCTest
{
    /// <summary>
    ///This is a test class for RootBlockTest and is intended
    ///to contain all RootBlockTest Unit Tests
    ///</summary>
    [TestClass]
    public class BasicBlockBuilderTest
    {
        private const string TemplateNamespace = "Cs2JsCTest.BasicBlockBuilderTemplates.";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void Create2SimpleIntReturn()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(TestILFunctions.SimpleIntReturnFunction);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            Assert.AreEqual(1, actual.Children.Count);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void StringFormat3ArgCall()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(TestILFunctions.StringFormat3ArgCall);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo = BlockInfo.ReadFromResourceFile(
                BasicBlockBuilderTest.TemplateNamespace + "SimpleFormat3ArgCall.xml");

            blockInfo.CheckSame(actual);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void SimpleConditionalArgument()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(TestILFunctions.SimpleConditionalArgument);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicBlockBuilderTest.TemplateNamespace + "SimpleConditionalArgument.xml");

            blockInfo.CheckSame(actual);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void Complex1ConditionalArgument()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.Complex1ConditionalArgument);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicBlockBuilderTest.TemplateNamespace + "Complex1ConditionalArgument.xml");

            blockInfo.CheckSame(actual);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void Complex2ConditionalArgument()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.Complex2ConditionalArgument);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicBlockBuilderTest.TemplateNamespace + "Complex2ConditionalArgument.xml");

            blockInfo.CheckSame(actual);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void Complex2IfCondition()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.Complex2IfCondition);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicBlockBuilderTest.TemplateNamespace + "Complex2IfCondition.xml");

            blockInfo.CheckSame(actual);

            actual.ProccessThroughPipeline(BasicStatementBuilder.Process);
        }

        /// <summary>
        ///A test for Create2
        ///</summary>
        [TestMethod]
        public void IncrementOnStack()
        {
            var executionBlock = TestILFunctions.GetExecutionBlock(
                TestILFunctions.IncrementOnStack);

            var actual = RootBlock.Create2(
                executionBlock.Arguments,
                executionBlock.Variables,
                executionBlock.Instructions,
                executionBlock.LabelInstructionMap);

            actual.ProccessThroughPipeline(BasicBlockBuilder.Process);

            BlockInfo blockInfo =
                BlockInfo.ReadFromResourceFile(
                    BasicBlockBuilderTest.TemplateNamespace + "IncrementOnStack.xml");

            blockInfo.CheckSame(actual);

            actual.ProccessThroughPipeline(BasicStatementBuilder.Process);
        }
    }
}
