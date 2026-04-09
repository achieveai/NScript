namespace NScript.CLR.Test
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for ClrKnownReferences.WrapPromiseMethod resolution.
    /// Verifies that the compiler can locate CallContext.WrapPromise
    /// in Sunlight.Framework for wrapping external async awaits.
    /// </summary>
    [TestClass]
    public class WrapPromiseMethodTests
    {
        private static string GetRepoRoot()
        {
            // Test DLLs are at Test\Compiler\bin\net8.0\ — go up 4 levels
            string dir = Path.GetDirectoryName(typeof(WrapPromiseMethodTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        [TestMethod]
        public void WrapPromiseMethod_WithFrameworkLoaded_FindsMethod()
        {
            string root = GetRepoRoot();
            string mscorlibPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "mscorlib.dll");
            string frameworkPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "Sunlight.Framework.dll");

            Assert.IsTrue(File.Exists(mscorlibPath), $"mscorlib not found at {mscorlibPath}");
            Assert.IsTrue(File.Exists(frameworkPath), $"Framework not found at {frameworkPath}");

            var context = new ClrContext();
            context.LoadAssembly(mscorlibPath);
            context.LoadAssembly(frameworkPath);

            var knownRefs = new ClrKnownReferences(context);
            var method = knownRefs.WrapPromiseMethod;

            Assert.IsNotNull(method, "WrapPromiseMethod should resolve when Sunlight.Framework is loaded");
            Assert.AreEqual("WrapPromise", method.Name);
            Assert.AreEqual("CallContext", method.DeclaringType.Name);
            Assert.AreEqual("Sunlight.Framework", method.DeclaringType.Namespace);
        }

        [TestMethod]
        public void WrapPromiseMethod_WithoutFramework_ReturnsNull()
        {
            string root = GetRepoRoot();
            string mscorlibPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "mscorlib.dll");

            Assert.IsTrue(File.Exists(mscorlibPath), $"mscorlib not found at {mscorlibPath}");

            var context = new ClrContext();
            context.LoadAssembly(mscorlibPath);

            var knownRefs = new ClrKnownReferences(context);
            var method = knownRefs.WrapPromiseMethod;

            Assert.IsNull(method, "WrapPromiseMethod should return null when Sunlight.Framework is not loaded");
        }

        [TestMethod]
        public void WrapPromiseMethod_CachesResult()
        {
            string root = GetRepoRoot();
            string mscorlibPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "mscorlib.dll");
            string frameworkPath = Path.Combine(root, "NScriptToolSet", "lib", "Release", "Sunlight.Framework.dll");

            var context = new ClrContext();
            context.LoadAssembly(mscorlibPath);
            context.LoadAssembly(frameworkPath);

            var knownRefs = new ClrKnownReferences(context);
            var first = knownRefs.WrapPromiseMethod;
            var second = knownRefs.WrapPromiseMethod;

            Assert.IsNotNull(first);
            Assert.AreSame(first, second, "WrapPromiseMethod should return the same cached reference on subsequent calls");
        }
    }
}
