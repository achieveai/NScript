using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Contract 11 (M5): <see cref="JSWriter.Write(string, string, bool, string, string, string)"/>
    /// replaces the bundle and its map through a temp file and a rename, so a reader opening
    /// the bundle sees the old file or the new one, never a truncated one; the bytes on disk
    /// equal a write to a fresh path.
    /// </summary>
    [TestClass]
    public class JSWriterAtomicWriteTests
    {
        private string dir;

        [TestInitialize]
        public void Setup()
        {
            this.dir = Path.Combine(Path.GetTempPath(), "nscript-jswriter-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this.dir);
        }

        [TestCleanup]
        public void Cleanup() => Directory.Delete(this.dir, recursive: true);

        private static void WriteBundle(string path)
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new NScript.Utils.Location("Program.cs", 3, 1, 3, 2));
            writer.WriteIdentifier("answer");
            writer.LeaveLocation();
            writer.Write(path, sourceRoot: null, emitLegacyAshxHandler: false);
        }

        [TestMethod]
        public void Write_OverExistingBundle_ReplacesByRename_DiskEqualsFreshWrite()
        {
            var fresh = Path.Combine(this.dir, "fresh", "App.js");
            Directory.CreateDirectory(Path.GetDirectoryName(fresh));
            WriteBundle(fresh);

            var target = Path.Combine(this.dir, "App.js");
            var map = Path.ChangeExtension(target, ".map");
            var stale = new string('s', 4096);
            var oldCreation = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            foreach (var file in new[] { target, map })
            {
                File.WriteAllText(file, stale);
                File.SetCreationTimeUtc(file, oldCreation);
            }

            WriteBundle(target);

            // A rename puts the temp file (created now) in place; an in-place write would
            // have truncated and rewritten the old file, keeping its creation time.
            Assert.AreNotEqual(oldCreation, File.GetCreationTimeUtc(target), "bundle written in place");
            Assert.AreNotEqual(oldCreation, File.GetCreationTimeUtc(map), "map written in place");
            CollectionAssert.AreEqual(File.ReadAllBytes(fresh), File.ReadAllBytes(target));
            CollectionAssert.AreEqual(File.ReadAllBytes(Path.ChangeExtension(fresh, ".map")), File.ReadAllBytes(Path.ChangeExtension(target, ".map")));
            CollectionAssert.AreEqual(new byte[] { 0xEF, 0xBB, 0xBF }, File.ReadAllBytes(target)[..3], "UTF-8 BOM as before.");
            CollectionAssert.AreEqual(Array.Empty<string>(), Directory.GetFiles(this.dir, "*.tmp", SearchOption.AllDirectories));
        }
    }
}
