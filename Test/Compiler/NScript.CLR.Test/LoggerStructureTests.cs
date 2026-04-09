namespace NScript.CLR.Test
{
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;

    /// <summary>
    /// Verifies the Logger class structure in the compiled Sunlight.Framework DLL.
    /// Since Logger uses [Script] for JS emission, we verify the type metadata
    /// (methods, fields, attributes) rather than runtime behavior.
    /// </summary>
    [TestClass]
    public class LoggerStructureTests
    {
        private static string GetRepoRoot()
        {
            string dir = Path.GetDirectoryName(typeof(LoggerStructureTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        private static TypeDefinition GetLoggerType()
        {
            string root = GetRepoRoot();
            string frameworkPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "Sunlight.Framework.dll");

            Assert.IsTrue(File.Exists(frameworkPath), $"Framework DLL not found at {frameworkPath}");

            var module = ModuleDefinition.ReadModule(frameworkPath);
            var loggerType = module.GetType("Sunlight.Framework.Logger");
            Assert.IsNotNull(loggerType, "Logger type should exist in Sunlight.Framework");
            return loggerType;
        }

        [TestMethod]
        public void Logger_HasExpectedLogLevelMethods()
        {
            var loggerType = GetLoggerType();
            var methodNames = loggerType.Methods.Select(m => m.Name).ToList();

            Assert.IsTrue(methodNames.Contains("Debug"), "Logger should have Debug method");
            Assert.IsTrue(methodNames.Contains("Info"), "Logger should have Info method");
            Assert.IsTrue(methodNames.Contains("Warn"), "Logger should have Warn method");
            Assert.IsTrue(methodNames.Contains("Error"), "Logger should have Error method");
        }

        [TestMethod]
        public void Logger_EmitMethodHasScriptAttribute()
        {
            var loggerType = GetLoggerType();
            var emitMethod = loggerType.Methods.FirstOrDefault(m => m.Name == "Emit");

            Assert.IsNotNull(emitMethod, "Logger should have private Emit method");
            Assert.IsTrue(emitMethod.IsPrivate, "Emit should be private");

            var scriptAttr = emitMethod.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.Name == "ScriptAttribute");
            Assert.IsNotNull(scriptAttr, "Emit method should have [Script] attribute for JS body");
        }

        [TestMethod]
        public void Logger_MinLevelPropertyExists()
        {
            var loggerType = GetLoggerType();

            var getter = loggerType.Methods.FirstOrDefault(m => m.Name == "get_MinLevel");
            var setter = loggerType.Methods.FirstOrDefault(m => m.Name == "set_MinLevel");

            Assert.IsNotNull(getter, "Logger should have MinLevel getter");
            Assert.IsNotNull(setter, "Logger should have MinLevel setter");
        }

        [TestMethod]
        public void Logger_LogLevelMethodsAcceptSingleStringParameter()
        {
            var loggerType = GetLoggerType();

            foreach (var name in new[] { "Debug", "Info", "Warn", "Error" })
            {
                var method = loggerType.Methods.FirstOrDefault(m => m.Name == name);
                Assert.IsNotNull(method, $"Logger should have {name} method");
                Assert.AreEqual(1, method.Parameters.Count,
                    $"Logger.{name} should accept exactly 1 parameter");
                Assert.AreEqual("String", method.Parameters[0].ParameterType.Name,
                    $"Logger.{name} parameter should be a string");
            }
        }
    }
}
