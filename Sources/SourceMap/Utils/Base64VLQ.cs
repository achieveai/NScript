using System;
using System.Text;

namespace OwaSourceMapper.Utils
{
    public static class Base64VLQ
    {
        private const string Base64MapString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        private const int VLQ_BASE_SHIFT = 5;

        // binary: 100000
        private const int VLQ_BASE = 1 << VLQ_BASE_SHIFT;

        // binary: 011111
        private const int VLQ_BASE_MASK = VLQ_BASE - 1;


        // binary: 100000
        private const int VLQ_CONTINUATION_BIT = VLQ_BASE;


        public static string ConvertToBase64VLQ(int x)
        {
            StringBuilder encodedValue = new StringBuilder();
            x = ToSignedBitInt(x);

            do 
            {
                int digit = x & VLQ_BASE_MASK;
                x = (int)((uint)x >> VLQ_BASE_SHIFT);

                if (x > 0) 
                {
                    digit |= VLQ_CONTINUATION_BIT;
                }

                encodedValue.Append(EncodeBase64(digit));
            } while (x > 0);

            return encodedValue.ToString();
        }

        public static int ToSignedBitInt(int x)
        {
            if (x < 0)
            {
                return (-x << 1) + 1;
            }
            else
            {
                return x << 1;
            }
        }

        private static char EncodeBase64(int number)
        {
            if (number >= 0 && number <= 63)
            {
                return Base64MapString[number];
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
