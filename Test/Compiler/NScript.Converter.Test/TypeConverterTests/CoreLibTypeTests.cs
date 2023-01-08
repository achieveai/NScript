//-----------------------------------------------------------------------
// <copyright file="CoreLibTypeTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for CoreLibTypeTests
    /// </summary>
    [TestClass]
    public class CoreLibTypeTests
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.CoreLibTests.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        /// <summary>
        /// Tests mcs.
        /// </summary>
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="testType">     Type of the test. </param>
        /// <param name="classNames">   List of names of the class. </param>
        [DataTestMethod]
        [DataRow("ObjectType.js",
             TestType.All,
             new[] { "System.Object" })]
        [DataRow("TypeType.js",
             TestType.All,
             new[] { "System.Type" })]
        [DataRow("StringType.js",
             TestType.All,
             new[] { "System.String" })]
        [DataRow("IntType.js",
             TestType.All,
             new[] { "System.Int32" })]
        [DataRow("ListType.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray`1",
                 "System.Collections.Generic.List`1"})]
        [DataRow("ArrayTypeMcs.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.ArrayImpl",
                 "System.NativeArray`1",
                 "System.ArrayG`1"})]
        public void TestMcs(string resourceName, TestType testType, string[] classNames)
        {
            TypeConverterHelper.RunTest(
                CoreLibTypeTests.TestFilesNSStr + resourceName,
                TestType.All,
                true,
                false,
                classNames);
        }
    }
}
