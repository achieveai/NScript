namespace NScript.Utils.Test
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Lib;

    /// <summary>
    /// Verify mode must name where an incremental emit and a full emit differ, or the
    /// stress loop's "zero mismatches" proves nothing.
    /// </summary>
    [TestClass]
    public class IncrementalVerifierTests
    {
        [TestMethod]
        public void Compare_EqualMissingAndDifferentFiles()
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nscript-verify-test-" + System.Guid.NewGuid().ToString("N"))).FullName;
            try
            {
                string a = Path.Combine(dir, "a.js"), b = Path.Combine(dir, "b.js");
                Assert.IsNull(IncrementalVerifier.Compare(a, b), "Both missing (no .map) is equal.");

                File.WriteAllText(a, "x\r\ny\r\nz\r\n");
                Assert.AreEqual("b.js is missing", IncrementalVerifier.Compare(b, a));
                Assert.AreEqual("a.js has no full counterpart", IncrementalVerifier.Compare(a, b));

                File.WriteAllText(b, "x\r\ny\r\nz\r\n");
                Assert.IsNull(IncrementalVerifier.Compare(a, b));

                File.WriteAllText(b, "x\r\ny\r\nZ\r\n");
                Assert.AreEqual("a.js line 3: incremental \"z\" vs full \"Z\"", IncrementalVerifier.Compare(a, b));

                File.WriteAllText(a, "x\ny");
                File.WriteAllText(b, "x\ny\nz");
                Assert.AreEqual("a.js: 2 lines vs 3 full", IncrementalVerifier.Compare(a, b));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
