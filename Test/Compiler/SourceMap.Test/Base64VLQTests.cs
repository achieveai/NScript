using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwaSourceMapper.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Tests for Base64 VLQ encoding — verifies both specific known encodings from the V3
    /// source map spec and round-trip correctness across signed/unsigned integer boundaries.
    /// Decoder lives in <see cref="Base64VLQDecoder"/> because the production code only
    /// provides an encoder (V3 consumers need decode to verify generated maps).
    /// </summary>
    [TestClass]
    public class Base64VLQTests
    {
        [TestMethod]
        public void Encode_KnownValues_MatchesSpec()
        {
            // Known VLQ encodings from the V3 source map spec / source-map library:
            //   0 -> "A", 1 -> "C", -1 -> "D", 15 -> "e", 16 -> "gB", 123 -> "2H"
            Assert.AreEqual("A", Base64VLQ.ConvertToBase64VLQ(0));
            Assert.AreEqual("C", Base64VLQ.ConvertToBase64VLQ(1));
            Assert.AreEqual("D", Base64VLQ.ConvertToBase64VLQ(-1));
            Assert.AreEqual("e", Base64VLQ.ConvertToBase64VLQ(15));
            Assert.AreEqual("gB", Base64VLQ.ConvertToBase64VLQ(16));
            Assert.AreEqual("2H", Base64VLQ.ConvertToBase64VLQ(123));
        }

        [TestMethod]
        public void Encode_Negative_UsesSignBitInLowestPosition()
        {
            // V3 VLQ encodes sign in bit 0: positives become 2n, negatives become 2n+1.
            Assert.AreEqual("D", Base64VLQ.ConvertToBase64VLQ(-1));
            Assert.AreEqual("F", Base64VLQ.ConvertToBase64VLQ(-2));
            Assert.AreEqual("H", Base64VLQ.ConvertToBase64VLQ(-3));
        }

        [TestMethod]
        public void ToSignedBitInt_Zero()
        {
            Assert.AreEqual(0, Base64VLQ.ToSignedBitInt(0));
        }

        [TestMethod]
        public void ToSignedBitInt_Positive()
        {
            Assert.AreEqual(2, Base64VLQ.ToSignedBitInt(1));
            Assert.AreEqual(4, Base64VLQ.ToSignedBitInt(2));
            Assert.AreEqual(20, Base64VLQ.ToSignedBitInt(10));
        }

        [TestMethod]
        public void ToSignedBitInt_Negative()
        {
            Assert.AreEqual(3, Base64VLQ.ToSignedBitInt(-1));
            Assert.AreEqual(5, Base64VLQ.ToSignedBitInt(-2));
            Assert.AreEqual(21, Base64VLQ.ToSignedBitInt(-10));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-1)]
        [DataRow(15)]
        [DataRow(16)]
        [DataRow(31)]
        [DataRow(32)]
        [DataRow(123)]
        [DataRow(-123)]
        [DataRow(1024)]
        [DataRow(-1024)]
        [DataRow(65536)]
        public void Encode_Decode_Roundtrip(int value)
        {
            string encoded = Base64VLQ.ConvertToBase64VLQ(value);
            int position = 0;
            int decoded = Base64VLQDecoder.Decode(encoded, ref position);

            Assert.AreEqual(value, decoded, $"Round-trip failed for {value} (encoded: {encoded})");
            Assert.AreEqual(encoded.Length, position, "Decoder should consume entire string");
        }
    }
}
