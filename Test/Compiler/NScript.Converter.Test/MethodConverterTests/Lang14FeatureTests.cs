using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Csc.Lib.Test;

namespace NScript.Converter.Test.MethodConverterTests
{
    [TestClass]
    public class Lang14FeatureTests
    {
        private const string TestClassNameStr = @"Lang14Features";
        private const string ExtensionsClassNameStr = @"Lang14Extensions";
        private const string FieldHolderClassNameStr = @"Lang14FieldHolder";
        private const string WidgetClassNameStr = @"Lang14Widget";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.Lang14Features.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "ExtensionProperty", "ExtensionProperty.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ExtensionMethod", "ExtensionMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ExtensionStaticMethod", "ExtensionStaticMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ExtensionSetter", "ExtensionSetter.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ExtensionGeneric", "ExtensionGeneric.js", TestType.All, false)]
        [DataRow(ExtensionsClassNameStr, "get_Twice", "ImplGetter.js", TestType.All, false)]
        [DataRow(ExtensionsClassNameStr, "set_Half", "ImplSetter.js", TestType.All, false)]
        [DataRow(ExtensionsClassNameStr, "Get", "ImplGeneric.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CompoundInstance", "CompoundInstance.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "IncrementInstance", "IncrementInstance.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CompoundStaticFallback", "CompoundStaticFallback.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "IncrementStatic", "IncrementStatic.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CompoundExtension", "CompoundExtension.js", TestType.All, false)]
        [DataRow(FieldHolderClassNameStr, "get_Doubled", "FieldGetter.js", TestType.All, false)]
        [DataRow(FieldHolderClassNameStr, "set_Doubled", "FieldSetter.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NullConditionalAssignment", "NullConditionalAssignment.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NameofUnboundGeneric", "NameofUnboundGeneric.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "LambdaOutModifier", "LambdaOutModifier.js", TestType.All, false)]
        [DataRow(WidgetClassNameStr, ".ctor", "PartialConstructor.js", TestType.All, false)]
        [DataRow(WidgetClassNameStr, "add_Changed", "PartialEvent.js", TestType.All, false)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }
    }
}
