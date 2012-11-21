//-----------------------------------------------------------------------
// <copyright file="CoreLibTypeTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;

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
        [Row("ObjectType.js",
             TestType.All,
             new[] { "System.Object" })]
        [Row("TypeType.js",
             TestType.All,
             new[] { "System.Type" })]
        [Row("StringType.js",
             TestType.All,
             new[] { "System.String" })]
        [Row("IntType.js",
             TestType.All,
             new[] { "System.Int32" })]
        [Row("ArrayType.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray",
                 "System.ArrayG`1"})]
        public void Test(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                CoreLibTypeTests.TestFilesNSStr + resourceName,
                TestType.All,
                false,
                classNames);
        }

        [Test]
        [Row("ObjectType.js",
             TestType.All,
             new[] { "System.Object" })]
        [Row("TypeType.js",
             TestType.All,
             new[] { "System.Type" })]
        [Row("StringType.js",
             TestType.All,
             new[] { "System.String" })]
        [Row("IntType.js",
             TestType.All,
             new[] { "System.Int32" })]
        [Row("ArrayTypeMcs.js",
             TestType.All,
             new[]{
                 "System.Array",
                 "System.NativeArray",
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