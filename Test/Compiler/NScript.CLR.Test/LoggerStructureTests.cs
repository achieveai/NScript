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
        public void Logger_GetIsoTimestampHasScriptAttribute()
        {
            // After WI-11, Logger dispatches through ILogSink instances rather than a
            // private Emit method. The only remaining [Script]-bodied helper is the
            // ISO timestamp bridge, which must stay private and carry the attribute
            // so Stage 2 emits the inline JS body.
            var loggerType = GetLoggerType();
            var tsMethod = loggerType.Methods.FirstOrDefault(m => m.Name == "GetIsoTimestamp");

            Assert.IsNotNull(tsMethod, "Logger should have private GetIsoTimestamp method");
            Assert.IsTrue(tsMethod.IsPrivate, "GetIsoTimestamp should be private");

            var scriptAttr = tsMethod.CustomAttributes
                .FirstOrDefault(a => a.AttributeType.Name == "ScriptAttribute");
            Assert.IsNotNull(scriptAttr, "GetIsoTimestamp should have [Script] attribute for JS body");
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
            // Back-compat guarantee: every level must retain the single-string
            // overload so pre-WI-11 call sites keep compiling untouched. (The
            // WI-11 refactor also adds (string, string[]) overloads — those are
            // verified separately by Logger_LogLevelMethodsHavePropertiesOverload.)
            var loggerType = GetLoggerType();

            foreach (var name in new[] { "Debug", "Info", "Warn", "Error" })
            {
                var singleStringOverload = loggerType.Methods
                    .FirstOrDefault(m => m.Name == name
                        && m.Parameters.Count == 1
                        && m.Parameters[0].ParameterType.Name == "String");

                Assert.IsNotNull(
                    singleStringOverload,
                    $"Logger.{name}(string) back-compat overload should exist");
            }
        }

        [TestMethod]
        public void Logger_LogLevelMethodsHavePropertiesOverload()
        {
            // WI-11 adds a (string, string[]) overload on each level so callers
            // can attach structured key/value pairs without boxing. string[] is
            // used (rather than object) to survive NScript minification.
            var loggerType = GetLoggerType();

            foreach (var name in new[] { "Debug", "Info", "Warn", "Error" })
            {
                var overload = loggerType.Methods
                    .FirstOrDefault(m => m.Name == name
                        && m.Parameters.Count == 2
                        && m.Parameters[0].ParameterType.Name == "String"
                        && m.Parameters[1].ParameterType.Name == "String[]");

                Assert.IsNotNull(
                    overload,
                    $"Logger.{name}(string, string[]) properties overload should exist");
            }
        }
    }
}
