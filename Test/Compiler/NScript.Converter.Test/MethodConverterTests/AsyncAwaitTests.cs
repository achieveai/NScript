namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncAwaitTests
    {
        private const string TestClassNameStr = @"TestAsyncAwait";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.AsyncAwait.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "Test1", "Test1.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test2", "Test2.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test3", "Test3.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test4", "Test4.js", TestType.All)]
        [DataRow(TestClassNameStr, "Test5", "Test5.js", TestType.All)]
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