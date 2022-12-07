using NScript.Csc.Lib.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NScript.Converter.Test.MethodConverterTests
{
    [TestClass]
    public class TestCompilerGeneratedStuff
    {
        private const string TestClassNameStr = "TestCompilerGeneratedStuff";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.CompilerGeneratedStuff.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "get_IntProperty", "GetAutoProperty.js", TestType.All)]
        [DataRow(TestClassNameStr, "set_IntProperty", "SetAutoProperty.js", TestType.All)]
        [DataRow(TestClassNameStr, "add_testEvent", "AddTestEvent.js", TestType.All)]
        [DataRow(TestClassNameStr, "remove_testEvent", "RemoveTestEvent.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestEventCheck", "TestEventAddRemove.js", TestType.All)]
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