//-----------------------------------------------------------------------
// <copyright file="YeildReturn.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for YeildReturn
    /// </summary>
    //// [TestFixture]
    public class YieldReturn
    {
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.YieldReturn.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        //// [Test]
        //// [TestCase(TestType.Retail, "")]
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