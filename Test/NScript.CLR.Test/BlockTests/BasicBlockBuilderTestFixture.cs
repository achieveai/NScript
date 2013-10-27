namespace NScript.CLR.Test.BlockTests
{
    using NScript.CLR.Decompiler.Blocks;
    using NUnit.Framework;

    [TestFixture]
    public class BasicBlockBuilderTestFixture
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.BasicBlockTemplates.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        public void SimpleIntReturnFunctionDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "ReturnInt",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleReturnFunction.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void SimpleIntReturnFunctionRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "ReturnInt",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleReturnFunction.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void SimpleFormat2ArgCallRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Call2ArgFormat",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleFormat3ArgCall.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void SimpleFormat2ArgCallDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Call2ArgFormat",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleFormat3ArgCall.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void SimpleConditionalArgumentDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "SimpleConditionalArgument",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void SimpleConditionalArgumentRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "SimpleConditionalArgument",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "SimpleConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex1ConditionalArgumentDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex1ConditionalArgument",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex1ConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex1ConditionalArgumentRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex1ConditionalArgument",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex1ConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex2ConditionalArgumentDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2ConditionalArgument",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex2ConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex2ConditionalArgumentRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2ConditionalArgument",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex2ConditionalArgument.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex2IfConditionDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                true,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex2IfCondition.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        public void Complex2IfConditionRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                false,
                BasicBlockBuilderTestFixture.TemplateNamespace + "Complex2IfConditionRelease.xml",
                BasicBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FuncIncrement(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                BasicBlockBuilderTestFixture.TemplateNamespace + "FuncIncrement.xml",
                "BasicBlockTestFunctions",
                "FuncIncrement",
                isDebug,
                BasicBlockBuilder.Process);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FuncPostIncrementWithProperty(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                BasicBlockBuilderTestFixture.TemplateNamespace + "FuncPostIncrementWithProperty.xml",
                "BasicBlockTestFunctions",
                "FuncPostIncrementWithProperty",
                isDebug,
                BasicBlockBuilder.Process);
        }
    }
}
