namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Utils
    {
        /// <summary>
        /// The character mapping.
        /// </summary>
        private const string charMapping = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Toes the JS string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>fixed string for JS.</returns>
        public static string ToJSString(string str)
        {
            if (str == null)
            { return "null"; }

            StringBuilder strBuilder = new StringBuilder(str.Length);

            for (int strIndex = 0; strIndex < str.Length; strIndex++)
            {
                switch (str[strIndex])
                {
                    case '"':
                    case '\\':
                        strBuilder.Append('\\');
                        strBuilder.Append(str[strIndex]);
                        break;
                    case '\r':
                        strBuilder.Append("\\r");
                        break;
                    case '\n':
                        strBuilder.Append("\\n");
                        break;
                    default:
                        strBuilder.Append(str[strIndex]);
                        break;
                }
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Toes the JS identifier.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>fixed string for identifier.</returns>
        public static string ToJSIdentifier(string str)
        {
            StringBuilder strBuilder = new StringBuilder(str.Length);
            char ch = str[0];

            if ((ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch == '_'))
            {
                strBuilder.Append(ch);
            }
            else if (ch == '.')
            {
                strBuilder.Append("__");
            }
            else if (ch == '<')
            {
                strBuilder.Append("_$");
            }
            else if (ch == '>')
            {
                strBuilder.Append("$_");
            }
            else
            {
                strBuilder.AppendFormat("${0:x}", (int)ch);
            }

            for (int strIndex = 1; strIndex < str.Length; strIndex++)
            {
                ch = str[strIndex];

                if ((ch >= 'A' && ch <= 'Z') ||
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9') ||
                    ch == '_')
                {
                    strBuilder.Append(ch);
                }
                else if (ch == '.')
                {
                    strBuilder.Append("$$");
                }
                else if (ch == '<')
                {
                    strBuilder.Append("_$");
                }
                else if (ch == '>')
                {
                    strBuilder.Append("$_");
                }
                else if (ch == ',')
                {
                    strBuilder.Append("_x_");
                }
                else if (ch == '`')
                {
                    strBuilder.Append('$');
                }
                else
                {
                    strBuilder.AppendFormat("${0:x}", (int)ch);
                }
            }

            return strBuilder.ToString();
        }

        public static bool IsValidIdentifier(string str)
        {
            for (int strIndex = 1; strIndex < str.Length; strIndex++)
            {
                char ch = str[strIndex];

                if ((ch >= 'A' && ch <= 'Z') ||
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9') ||
                    ch == '_' || ch == '$')
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the compressed int.
        /// </summary>
        /// <param name="i">The number to encode.</param>
        /// <returns>string encoded i (encoded using [a-z][a-zA-Z]*)</returns>
        public static string GetCompressedInt(int i)
        {
            StringBuilder retValue = new StringBuilder();

            do
            {
                var idx = 0;
                if (retValue.Length == 0)
                {
                    idx = i % 26;
                    i /= 26;
                }
                else
                {
                    idx = i % 62;
                    i /= 62;
                }

                retValue.Append(charMapping[idx]);
            } while (i > 0);


            return retValue.ToString();
        }
    }
}
