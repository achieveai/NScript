//-----------------------------------------------------------------------
// <copyright file="CoreLibTypeTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for CoreLibTypeTests
    /// </summary>
    [TestFixture]
    public class CoreLibTypeTests
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.CoreLibTests.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase("ObjectType.js",
             TestType.All,
             new[] { "System.Object" })]
        [TestCase("TypeType.js",
             TestType.All,
             new[] { "System.Type" })]
        [TestCase("StringType.js",
             TestType.All,
             new[] { "System.String" })]
        [TestCase("IntType.js",
             TestType.All,
             new[] { "System.Int32" })]
        [TestCase("ArrayType.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray`1",
                 "System.ArrayG`1"})]
        public void Test(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                CoreLibTypeTests.TestFilesNSStr + resourceName,
                TestType.All,
                false,
                classNames);
        }

        /// <summary>
        /// Tests mcs.
        /// </summary>
        /// <param name="resourceName"> Name of the resource. </param>
        /// <param name="testType">     Type of the test. </param>
        /// <param name="classNames">   List of names of the class. </param>
        [Test]
        [TestCase("ObjectType.js",
             TestType.All,
             new[] { "System.Object" })]
        [TestCase("TypeType.js",
             TestType.All,
             new[] { "System.Type" })]
        [TestCase("StringType.js",
             TestType.All,
             new[] { "System.String" })]
        [TestCase("IntType.js",
             TestType.All,
             new[] { "System.Int32" })]
        [TestCase("ListType.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray`1",
                 "System.Collections.Generic.List`1"})]
        [TestCase("ArrayTypeMcs.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray`1",
                 "System.ArrayG`1"})]
        public void TestMcs(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                CoreLibTypeTests.TestFilesNSStr + resourceName,
                TestType.All,
                true,
                classNames);
        }
    }
}