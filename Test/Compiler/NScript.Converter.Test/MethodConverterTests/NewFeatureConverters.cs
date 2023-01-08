//-----------------------------------------------------------------------
// <copyright file="NewFeatureConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
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
        [DataRow("AddNum", "AddNum.js", TestType.All, false)]
        [DataRow("AddNum", "AddNum.static.js", TestType.All, true)]
        [DataRow("CallWithDefaultParameter", "CallWithDefaultParameter.js", TestType.All, false)]
        [DataRow("CallWithDefaultParameter", "CallWithDefaultParameter.js", TestType.All, true)]
        [DataRow("DiscardVariable", "DiscardVariable.js", TestType.All, false)]
        [DataRow("DiscardVariable", "DiscardVariable.js", TestType.All, true)]
        [DataRow("NameofField", "NameofField.js", TestType.All, false)]
        [DataRow("NameofField", "NameofField.static.js", TestType.All, true)]
        [DataRow("OutVarParam", "OutVar.js", TestType.All, false)]
        [DataRow("OutVarParam", "OutVar.static.js", TestType.All, true)]
        [DataRow("TestConditionalAccess", "TestConditionalAccess.js", TestType.All, false)]
        [DataRow("TestConditionalAccess", "TestConditionalAccess.static.js", TestType.All, true)]
        [DataRow("TestConditionalAccess2", "TestConditionalAccess2.js", TestType.All, false)]
        [DataRow("TestConditionalAccess2", "TestConditionalAccess2.static.js", TestType.All, true)]
        [DataRow("TestConditionalInvoke", "TestConditionalInvoke.js", TestType.All, false)]
        [DataRow("TestConditionalInvoke", "TestConditionalInvoke.js", TestType.All, true)]
        [DataRow("TestNestedFunction", "TestNestedFunction.js", TestType.All, false)]
        [DataRow("TestNestedFunction", "TestNestedFunction.static.js", TestType.All, true)]
        [DataRow("TestNestedFunctionCrossReferenced", "TestNestedFunctionCrossReferenced.js", TestType.All, false)]
        [DataRow("TestNestedFunctionCrossReferenced", "TestNestedFunctionCrossReferenced.static.js", TestType.All, true)]
        [DataRow("TestNestedFunctionScoped", "TestNestedFunctionScoped.js", TestType.All, false)]
        [DataRow("TestNestedFunctionScoped", "TestNestedFunctionScoped.static.js", TestType.All, true)]
        [DataRow("get_InitProp", "InitProp.js", TestType.All, false)]
        [DataRow("get_InitProp", "InitProp.static.js", TestType.All, true)]
        [DataRow("get_InlineProp", "InlineProp.js", TestType.All, false)]
        [DataRow("get_InlineProp", "InlineProp.static.js", TestType.All, true)]
        public void Test(string methodName, string resourceName, TestType testType, bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                NewFeatureConverters.TestFilesNSStr + resourceName,
                NewFeatureConverters.TestClassNameStr,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }

        public async Task<int> NoTestAsync()
        {
            Assert.AreEqual(1, 1 + 0);
            Assert.AreEqual(10, 9 + await Task.FromResult(1));

            return 1;
        }
    }
}