//-----------------------------------------------------------------------
// <copyright file="YeildReturn.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for YeildReturn
    /// </summary>
    //// [TestClass]
    public class YieldReturn
    {
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.YieldReturn.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        //// [DataTestMethod]
        //// [DataRow(TestType.Retail, "")]
        public void NestedForYieldReturn(TestType testType, string fileName)
        {
            ConverterTestHelpers.RunTest(
                YieldReturn.TestFilesNSStr + fileName,
                "YieldReturnTests.<GetEnumeratorNestedFor>d__5",
                "MoveNext",
                testType);
        }
    }
}