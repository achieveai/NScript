namespace NScript.Csc.Lib.Test
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The fixtures' own setup. Each test guards a fix for a test run that failed for a reason
    /// other than the code under test.
    /// </summary>
    [TestClass]
    public class TestInfrastructureTests
    {
        /// <summary>
        /// B2: the fixture DLLs went to fixed names in %TEMP%, so two test runs at once
        /// overwrote and locked each other's files (IOException).
        /// </summary>
        [TestMethod]
        public void FixtureDirectory_IsThisProcesssOwn_NotTheSharedTempFolder()
        {
            var dir = TestResources.FixtureDirectory;
            Assert.IsTrue(Directory.Exists(dir), dir);
            Assert.AreEqual(
                Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(Path.GetDirectoryName(dir)).TrimEnd(Path.DirectorySeparatorChar));
            StringAssert.StartsWith(Path.GetFileName(dir), "nscript-csclib-" + Environment.ProcessId + "-");
        }

        /// <summary>
        /// B8: the loader skipped framework DLLs missing from lib/Release, so the await tests
        /// ran without the framework and failed with wrong JS instead of a reason.
        /// </summary>
        [TestMethod]
        public void FrameworkPaths_AnyMissing_ThrowsNamingEachAndTheBuild()
        {
            var dir = Path.Combine(Path.GetTempPath(), "nscript-lib-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            var names = new[] { "System.Web.dll", "System.Web.Html.dll", "Sunlight.Framework.dll" };
            try
            {
                File.WriteAllText(Path.Combine(dir, names[0]), string.Empty);
                var ex = Assert.ThrowsException<FileNotFoundException>(() => TestAssemblyLoader.GetFrameworkPaths(dir, names));
                StringAssert.Contains(ex.Message, Path.Combine(dir, names[1]));
                StringAssert.Contains(ex.Message, Path.Combine(dir, names[2]));
                StringAssert.Contains(ex.Message, "dotnet build NScript_Full.sln -c Release");

                File.WriteAllText(Path.Combine(dir, names[1]), string.Empty);
                File.WriteAllText(Path.Combine(dir, names[2]), string.Empty);
                CollectionAssert.AreEqual(
                    Array.ConvertAll(names, name => Path.Combine(dir, name)),
                    TestAssemblyLoader.GetFrameworkPaths(dir, names),
                    "In load order.");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
