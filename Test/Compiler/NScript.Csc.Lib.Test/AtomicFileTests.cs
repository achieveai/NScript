namespace NScript.Csc.Lib.Test
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OwaSourceMapper;

    /// <summary>
    /// Generated bundles are replaced through a temp file and a rename, so a reader never
    /// sees a truncated file. These tests pin what happens when a reader holds the target.
    /// </summary>
    [TestClass]
    public class AtomicFileTests
    {
        private string dir;

        [TestInitialize]
        public void Setup()
        {
            this.dir = Path.Combine(Path.GetTempPath(), "nscript-atomic-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.dir);
        }

        [TestCleanup]
        public void Cleanup() => Directory.Delete(this.dir, recursive: true);

        /// <summary>
        /// Regression (M5 D2): a reader without delete sharing (a dev server, a test runner)
        /// held the bundle for a moment and the replace failed after 100 ms, losing the emit.
        /// </summary>
        [TestMethod]
        public void Write_TargetBrieflyHeldWithoutDeleteShare_ReplacesAfterRelease()
        {
            var target = Path.Combine(this.dir, "App.js");
            File.WriteAllText(target, "old");
            var reader = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read);
            var release = Task.Delay(200).ContinueWith(_ => reader.Dispose());

            try
            {
                AtomicFile.Write(target, Encoding.UTF8, w => w.Write("new"));
            }
            finally
            {
                release.Wait();
            }

            Assert.AreEqual("new", File.ReadAllText(target));
            CollectionAssert.AreEqual(Array.Empty<string>(), Directory.GetFiles(this.dir, "*.tmp"));
        }

        /// <summary>
        /// A target held for longer than the retries fails loudly: the error names the file,
        /// the old content stays, and no temp file is left behind.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // waits out the full ~1 s rename retry
        public void Write_TargetHeldThroughRetries_ThrowsNamingFileAndLeavesNoTemp()
        {
            var target = Path.Combine(this.dir, "App.js");
            File.WriteAllText(target, "old");
            using (new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var ex = Assert.ThrowsException<IOException>(() => AtomicFile.Write(target, Encoding.UTF8, w => w.Write("new")));
                StringAssert.Contains(ex.Message, target);
            }

            Assert.AreEqual("old", File.ReadAllText(target));
            CollectionAssert.AreEqual(Array.Empty<string>(), Directory.GetFiles(this.dir, "*.tmp"));
        }
    }
}
