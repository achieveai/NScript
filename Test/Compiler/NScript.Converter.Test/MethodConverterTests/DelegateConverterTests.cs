//-----------------------------------------------------------------------
// <copyright file="DelegateConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for DelegateConverterTests
    /// </summary>
    [TestClass]
    public class DelegateConverterTests
    {
        private const string TestClassNameStr = "DelegateBlocks";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.DelegateConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegateMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegateMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegateMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegateMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.js", TestType.All)]
        [DataRow(TestClassNameStr, "AddEvent", "AddEvent.js", TestType.All)]
        [DataRow(TestClassNameStr, "RemoveEvent", "RemoveEvent.js", TestType.All)]
        [DataRow(TestClassNameStr, "CallEvent", "CallEvent.js", TestType.All)]
        [DataRow(TestClassNameStr, "ClearEvent", "ClearEvent.js", TestType.All)]
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
