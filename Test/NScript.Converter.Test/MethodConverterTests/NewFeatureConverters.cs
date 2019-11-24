//-----------------------------------------------------------------------
// <copyright file="NewFeatureConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;
    using System.Threading.Tasks;


    /// <summary>
    /// Definition for NewFeatureConverters
    /// </summary>
    [TestFixture]
    public class NewFeatureConverters
    {
        private const string TestClassNameStr = "NewLanguageFeatures";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.NewLanguageFeatures.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase("get_InlineProp", "InlineProp.js", TestType.All)]
        [TestCase("AddNum", "AddNum.js", TestType.All)]
        [TestCase("NameofField", "NameofField.js", TestType.All)]
        [TestCase("get_InitProp", "InitProp.js", TestType.All)]
        [TestCase("OutVarParam", "OutVar.js", TestType.All)]
        [TestCase("TestConditionalAccess", "TestConditionalAccess.js", TestType.All)]
        [TestCase("TestConditionalAccess2", "TestConditionalAccess2.js", TestType.All)]
        [TestCase("TestConditionalInvoke", "TestConditionalInvoke.js", TestType.All)]
        [TestCase("TestNestedFunction", "TestNestedFunction.js", TestType.All)]
        [TestCase("TestNestedFunctionScoped", "TestNestedFunctionScoped.js", TestType.All)]
        public void Test(string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                NewFeatureConverters.TestFilesNSStr + resourceName,
                NewFeatureConverters.TestClassNameStr,
                methodName,
                testType,
                true);
        }

        public async Task<int> NoTestAsync()
        {
            Assert.AreEqual(1, 1 + 0);
            Assert.AreEqual(10, 9 + await Task.FromResult(1));

            return 1;
        }
    }
}
