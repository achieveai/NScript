//-----------------------------------------------------------------------
// <copyright file="NewFeatureConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    /// <summary>
    /// Definition for NewFeatureConverters
    /// </summary>
    [TestClass]
    public class NewFeatureConverters
    {
        private const string TestClassNameStr = "NewLanguageFeatures";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.NewLanguageFeatures.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow("get_InlineProp", "InlineProp.js", TestType.All)]
        [DataRow("AddNum", "AddNum.js", TestType.All)]
        [DataRow("NameofField", "NameofField.js", TestType.All)]
        [DataRow("get_InitProp", "InitProp.js", TestType.All)]
        [DataRow("OutVarParam", "OutVar.js", TestType.All)]
        [DataRow("TestConditionalAccess", "TestConditionalAccess.js", TestType.All)]
        [DataRow("TestConditionalAccess2", "TestConditionalAccess2.js", TestType.All)]
        [DataRow("TestConditionalInvoke", "TestConditionalInvoke.js", TestType.All)]
        [DataRow("TestNestedFunction", "TestNestedFunction.js", TestType.All)]
        [DataRow("TestNestedFunctionScoped", "TestNestedFunctionScoped.js", TestType.All)]
        [DataRow("CallWithDefaultParameter", "CallWithDefaultParameter.js", TestType.All)]
        [DataRow("DiscardVariable", "DiscardVariable.js", TestType.All)]
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