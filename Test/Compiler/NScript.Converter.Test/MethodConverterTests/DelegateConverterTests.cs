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
        [DataRow(TestClassNameStr, "AddEvent", "AddEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "AddEvent", "AddEvent.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "CallEvent", "CallEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ClearEvent", "ClearEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "InstanceDelegate", "InstanceReferencingDelegate.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "InstanceDelegate", "InstanceReferencingDelegate.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegateMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "InstanceReferencingDelegate", "InstanceReferencingDelegateMcs.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "IntDelegateTaker", "IntDelegateTaker.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegateMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "LocalAndInstanceReferencingDelegate", "LocalAndInstanceReferencingDelegateMcs.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegateMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "LocalReferencingDelegate", "LocalReferencingDelegateMcs.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "RemoveEvent", "RemoveEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "RemoveEvent", "RemoveEvent.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegateMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "StaticReferencingDelegate", "StaticReferencingDelegateMcs.static.js", TestType.All, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                instanceAsStatic);
        }
    }
}
