namespace NScript.Csc.Lib.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Service;

    /// <summary>
    /// Watch mode learns what to watch from <see cref="CompileInputs.FromArguments"/>. A file
    /// missing from it is never rebuilt on save.
    /// </summary>
    [TestClass]
    public class CompileInputsTests
    {
        /// <summary>
        /// Regression (M5 D1): embedded resources (/resource:, how skins, css and xwml reach
        /// the compiler) were dropped because Roslyn keeps no file name for them, so skin
        /// edits never triggered a watch batch.
        /// </summary>
        [TestMethod]
        public void FromArguments_EmbeddedResource_IsAnInputWithItsHash()
        {
            var dir = Path.Combine(Path.GetTempPath(), "nscript-inputs-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Skins"));
            try
            {
                File.WriteAllText(Path.Combine(dir, "A.cs"), "class A {}");
                File.WriteAllText(Path.Combine(dir, "Skins", "Shell.skin.cshtml"), "<div>hi</div>");
                File.WriteAllText(Path.Combine(dir, "Linked.css"), ".a {}");
                var args = CSharpCommandLineParser.Default.Parse(
                    new[]
                    {
                        "/target:library",
                        "/out:obj/App.dll",
                        @"/resource:Skins\Shell.skin.cshtml,App.Skins.Shell.skin.cshtml",
                        "/linkresource:Linked.css,App.Linked.css",
                        "A.cs",
                    },
                    dir,
                    sdkDirectory: null);

                var inputs = CompileInputs.FromArguments(args, dir);

                var skin = Path.Combine(dir, "Skins", "Shell.skin.cshtml");
                var linked = Path.Combine(dir, "Linked.css");
                CollectionAssert.AreEquivalent(new[] { skin, linked }, inputs.Resources.ToArray());
                Assert.AreEqual(CompileInputs.HashFile(skin), inputs.Hashes[skin]);
                Assert.AreEqual(Path.Combine(dir, "obj", "App.dll"), inputs.Output);
                CollectionAssert.AreEqual(new[] { Path.Combine(dir, "A.cs") }, inputs.Sources.ToArray());
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
