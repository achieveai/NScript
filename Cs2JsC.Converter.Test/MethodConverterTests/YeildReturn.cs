//-----------------------------------------------------------------------
// <copyright file="YeildReturn.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for YeildReturn
    /// </summary>
    //// [TestFixture]
    public class YieldReturn
    {
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.YieldReturn.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        //// [Test]
        //// [Row(TestType.Retail, "")]
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