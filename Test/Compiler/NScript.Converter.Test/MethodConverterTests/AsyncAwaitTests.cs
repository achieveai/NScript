namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncAwaitTests
    {
        private const string TestClassNameStr = @"TestAsyncAwait";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DefaultExpressions.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "Test1", "TestReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test2", "TestReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test3", "TestReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test4", "TestReferenceType.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test5", "TestReferenceType.js", TestType.All)]
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