namespace NScript.CLR.Test.BlockTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NScript.CLR.Decompiler.Blocks;
    using NUnit.Framework;

    [TestFixture]
    public class BasicStatementBuilderTestFixture
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.BasicStatementTemplates.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        public void Complex2IfConditionBlockDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                true,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex2IfConditionBlockDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process);
        }

        [Test]
        public void Complex2IfConditionBlockRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                false,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex2IfConditionBlockRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process);
        }

        [Test]
        public void Complex2IfConditionStatementDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                true,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex2IfConditionStatementDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);
        }

        [Test]
        public void Complex2IfConditionStatementRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex2IfCondition",
                false,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex2IfConditionStatementRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);
        }

        [Test]
        public void Complex3IfConditionStatementDebug()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex3IfCondition",
                true,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex3IfConditionStatementDebug.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);
        }

        [Test]
        public void Complex3IfConditionStatementRelease()
        {
            TestHelpers.RunBlockInfoTest(
                "BasicBlockTestFunctions",
                "Complex3IfCondition",
                false,
                BasicStatementBuilderTestFixture.TemplateNamespace + "Complex3IfConditionStatementRelease.xml",
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);
        }
    }
}
