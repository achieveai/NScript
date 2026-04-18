using System;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Base64 VLQ decoder used by the test suite to verify generated source map mappings.
    /// Production code only needs the encoder (<see cref="Utils.Base64VLQ"/>); this decoder
    /// exists so tests can assert structured mapping content rather than encoded byte strings.
    /// </summary>
    internal static class Base64VLQDecoder
    {
        private const string Base64MapString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private const int VLQ_BASE_SHIFT = 5;
        private const int VLQ_BASE = 1 << VLQ_BASE_SHIFT;
        private const int VLQ_BASE_MASK = VLQ_BASE - 1;
        private const int VLQ_CONTINUATION_BIT = VLQ_BASE;

        /// <summary>
        /// Decodes a single signed integer starting at <paramref name="position"/>. Advances
        /// <paramref name="position"/> past the consumed characters.
        /// </summary>
        public static int Decode(string encoded, ref int position)
        {
            int result = 0;
            int shift = 0;
            bool hasContinuation;

            do
            {
                if (position >= encoded.Length)
                {
                    throw new FormatException("Unexpected end of VLQ input");
                }

                int digit = DecodeBase64(encoded[position++]);
                hasContinuation = (digit & VLQ_CONTINUATION_BIT) != 0;
                digit &= VLQ_BASE_MASK;
                result |= digit << shift;
                shift += VLQ_BASE_SHIFT;
            } while (hasContinuation);

            // Decode sign bit (LSB).
            bool isNegative = (result & 1) == 1;
            int unsigned = (int)((uint)result >> 1);
            return isNegative ? -unsigned : unsigned;
        }

        private static int DecodeBase64(char c)
        {
            int idx = Base64MapString.IndexOf(c);
            if (idx < 0)
            {
                throw new FormatException($"Invalid Base64 character '{c}'");
            }

            return idx;
        }
    }
}
