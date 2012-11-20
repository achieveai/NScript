using NScript.CLR.Test;
using MbUnit.Framework;

namespace NScript.Converter.Test.MethodConverterTests
{
    [TestFixture]
    public class TestCompilerGeneratedStuff
    {
        private const string TestClassNameStr = "TestCompilerGeneratedStuff";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.CompilerGeneratedStuff.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "get_IntProperty", "GetAutoProperty.js", TestType.All)]
        [Row(TestClassNameStr, "set_IntProperty", "SetAutoProperty.js", TestType.All)]
        [Row(TestClassNameStr, "add_testEvent", "AddTestEvent.js", TestType.All)]
        [Row(TestClassNameStr, "remove_testEvent", "RemoveTestEvent.js", TestType.All)]
        [Row(TestClassNameStr, "TestEventCheck", "TestEventAddRemove.js", TestType.All)]
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