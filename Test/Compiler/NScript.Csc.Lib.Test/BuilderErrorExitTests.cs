namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;

    /// <summary>
    /// F-002: a converter error (for example an unresolved Razor event handler or an
    /// unsupported binding expression that the generator turns into a diagnostic) must
    /// make the compiler fail — <see cref="Builder.Execute"/> returns false and does
    /// NOT publish a partial JavaScript bundle, so a direct CLI caller sees a non-zero
    /// exit code (see <c>NScriptCompiler.Compile</c>). Before the fix Execute always
    /// returned true and wrote the bundle regardless of <c>ConverterContext.Errors</c>,
    /// and <c>NScriptCompiler.Compile</c> discarded the result and returned 0.
    /// </summary>
    [TestClass]
    public class BuilderErrorExitTests
    {
        /// <summary>
        /// Minimal plugin that optionally records a converter error during Initialize —
        /// the same sink the Razor plugin uses when it swallows a generation diagnostic.
        /// </summary>
        private sealed class ErrorInjectingPlugin : IRuntimeConverterPlugin
        {
            private readonly bool injectError;

            public ErrorInjectingPlugin(bool injectError) => this.injectError = injectError;

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
            {
                if (this.injectError)
                {
                    runtimeScopeManager.Context.AddError(
                        null, "F-002 test: injected converter error", false);
                }
            }

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1() => null;

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript() => null;
        }

        private static (string main, string[] refs) EnsureAssemblies()
        {
            // Builds mscorlib.dll / system.core.dll / microsoft.csharp.dll / realScript.dll
            // into the temp directory (idempotent across tests).
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            return (
                Path.Combine(temp, "realScript.dll"),
                new[]
                {
                    Path.Combine(temp, "mscorlib.dll"),
                    Path.Combine(temp, "system.core.dll"),
                    Path.Combine(temp, "microsoft.csharp.dll"),
                });
        }

        private static bool Run(string outJs, bool injectError)
        {
            var (main, refs) = EnsureAssemblies();
            if (File.Exists(outJs))
            {
                File.Delete(outJs);
            }

            var builder = new Builder(
                outJs,
                1,
                main,
                refs,
                new IConverterPlugin[] { new ErrorInjectingPlugin(injectError) },
                (minify: false, uglify: false, optimize: false));

            return builder.Execute();
        }

        [TestMethod]
        public void ConverterError_SkipsOutputAndReportsFailure()
        {
            var outJs = Path.Combine(
                Path.GetTempPath(), "f002_error_" + Guid.NewGuid().ToString("N") + ".js");

            bool succeeded = Run(outJs, injectError: true);

            Assert.IsFalse(
                succeeded, "Execute must return false when the converter reported an error.");
            Assert.IsFalse(
                File.Exists(outJs),
                "No JavaScript bundle must be published when conversion failed. File: " + outJs);
        }

        [TestMethod]
        public void CleanConversion_WritesOutputAndReportsSuccess()
        {
            var outJs = Path.Combine(
                Path.GetTempPath(), "f002_ok_" + Guid.NewGuid().ToString("N") + ".js");

            bool succeeded = Run(outJs, injectError: false);

            try
            {
                Assert.IsTrue(
                    succeeded, "Execute must return true for an error-free compilation.");
                Assert.IsTrue(
                    File.Exists(outJs),
                    "A JavaScript bundle must be published for an error-free compilation. File: " + outJs);
            }
            finally
            {
                if (File.Exists(outJs))
                {
                    File.Delete(outJs);
                }
            }
        }
    }
}
