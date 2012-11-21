//-----------------------------------------------------------------------
// <copyright file="DelegateConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;

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
        [Row(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegateMcs.js", TestType.All)]
        [Row(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegateMcs.js", TestType.All)]
        [Row(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegateMcs.js", TestType.All)]
        [Row(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegateMcs.js", TestType.All)]
        [Row(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.js", TestType.All)]
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
