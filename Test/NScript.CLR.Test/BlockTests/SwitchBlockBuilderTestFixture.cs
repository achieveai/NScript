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
    public class SwitchBlockBuilderTestFixture
    {
        private const string TemplateNamespace = @"NScript.CLR.Test.BlockTests.SwitchBlockTemplates.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(true, "SimpleIntSwitchDebug.xml")]
        [Row(false, "SimpleIntSwitchRelease.xml")]
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
        [Row(true, "SwitchOnlyFunctionDebug.xml")]
        [Row(false, "SwitchOnlyFunctionRelease.xml")]
        public void SwitchOnlyFunction(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "SwitchOnlyFunction",
                isDebug);
        }

        [Test]
        [Row(true,  "SwitchWithReturnsOnlyDebug.xml")]
        [Row(false, "SwitchWithReturnsOnlyRelease.xml")]
        public void SwitchWithReturnsOnlyRelease(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "SwitchWithReturnsOnly",
                isDebug);
        }

        [Test]
        [Row(true,  "RegressionContinousSwitchValuesDebug.xml")]
        [Row(false, "RegressionContinousSwitchValues.xml")]
        public void RegressionContinousSwitchValues(bool isDebug, string fileName)
        {
            TestHelpers.TestILBlocks(
                SwitchBlockBuilderTestFixture.TemplateNamespace + fileName,
                "SwitchTest",
                "RegressionContinousSwitchValues",
                isDebug);
        }

        [Test]
        [Row(true,  "RegressionContinousSwitch2Debug.xml")]
        [Row(false, "RegressionContinousSwitch2.xml")]
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
