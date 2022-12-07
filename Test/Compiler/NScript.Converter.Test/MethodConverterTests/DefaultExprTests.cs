namespace NScript.Converter.Test.MethodConverterTests
{
    // using NScript.CLR.Test;
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultExprTests
    {
        private const string TestClassNameStr = @"TestDefaultExpr";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DefaultExpressions.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "TestReferenceType", "TestReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestGenericReferenceType", "TestGenericReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestBuiltinValueTypes", "TestBuiltinValueTypes.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestValueType", "TestValueType.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestGenericValueType", "TestGenericValueType.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}