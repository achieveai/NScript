using NScript.CLR.Test;
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
        [DataRow(TestClassNameStr, "TestEventCheck", "TestEventAddRemove.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestEventCheck", "TestEventAddRemove.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "add_testEvent", "AddTestEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "add_testEvent", "AddTestEvent.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "get_IntProperty", "GetAutoProperty.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "get_IntProperty", "GetAutoProperty.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "remove_testEvent", "RemoveTestEvent.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "remove_testEvent", "RemoveTestEvent.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "set_IntProperty", "SetAutoProperty.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "set_IntProperty", "SetAutoProperty.static.js", TestType.All, true)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType, bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                instanceAsStatic);
        }
    }
}