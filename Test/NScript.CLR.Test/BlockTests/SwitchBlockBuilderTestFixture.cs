
namespace NScript.CLR.Test.BlockTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NScript.CLR.Decompiler.Blocks;
    using NUnit.Framework;

    [TestFixture]
    public class SwitchBlockBuilderTestFixture
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.SwitchBlockTemplates.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(true, "SimpleIntSwitchDebug.xml")]
        [TestCase(false, "SimpleIntSwitchRelease.xml")]
        public void SimpleIntSwitch(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "SimpleIntSwitch",
                isDebug,
                BasicBlockBuilder.Process,
                BasicStatementBuilder.Process,
                SwitchBlock.SwitchBlockBuilder.Process,
                ConditionalStatementBuilder.Process);
        }

        [Test]
        [TestCase(true, "SwitchOnlyFunctionDebug.xml")]
        [TestCase(false, "SwitchOnlyFunctionRelease.xml")]
        public void SwitchOnlyFunction(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "SwitchOnlyFunction",
                isDebug);
        }

        [Test]
        [TestCase(true,  "SwitchWithReturnsOnlyDebug.xml")]
        [TestCase(false, "SwitchWithReturnsOnlyRelease.xml")]
        public void SwitchWithReturnsOnlyRelease(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "SwitchWithReturnsOnly",
                isDebug);
        }

        [Test]
        [TestCase(true,  "RegressionContinousSwitchValuesDebug.xml")]
        [TestCase(false, "RegressionContinousSwitchValues.xml")]
        public void RegressionContinousSwitchValues(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "RegressionContinousSwitchValues",
                isDebug);
        }

        [Test]
        [TestCase(true,  "RegressionContinousSwitch2Debug.xml")]
        [TestCase(false, "RegressionContinousSwitch2.xml")]
        // [Timeout(20000)]
        public void RegressionRegressionContinousSwitch2(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "RegressionContinousSwitch2",
                isDebug);
        }
    }
}
