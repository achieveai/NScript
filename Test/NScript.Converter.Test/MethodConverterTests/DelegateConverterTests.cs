//-----------------------------------------------------------------------
// <copyright file="DelegateConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for DelegateConverterTests
    /// </summary>
    [TestFixture]
    public class DelegateConverterTests
    {
        private const string TestClassNameStr = "DelegateBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.DelegateConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegateMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegateMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegateMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegateMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}
