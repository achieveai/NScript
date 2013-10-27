//-----------------------------------------------------------------------
// <copyright file="GenericTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using System;
    using NUnit.Framework;


    /// <summary>
    /// Definition for GenericTests
    /// </summary>
    [TestFixture]
    public class GenericTests
    {
        private const string TestClassNameStr = "GenericSamples";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.BlockTests.GenericTests";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NewGenericObject(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                GenericTests.ResourceFileNamepace + ".NewGenericObject.xml",
                GenericTests.TestClassNameStr,
                "NewGenericObject",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void NewGenericObject2(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                GenericTests.ResourceFileNamepace + ".NewGenericObject2.xml",
                GenericTests.TestClassNameStr,
                "NewGenericObject2",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GenericMethodCall(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                GenericTests.ResourceFileNamepace + ".GenericMethodCall.xml",
                GenericTests.TestClassNameStr,
                "GenericMethodCall",
                isDebug);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GenericMethodCall2(bool isDebug)
        {
            TestHelpers.TestILBlocks(
                GenericTests.ResourceFileNamepace + ".GenericMethodCall2.xml",
                GenericTests.TestClassNameStr,
                "GenericMethodCall2",
                isDebug);
        }
    }
}
