namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Utils
    {
        private const string charMapping = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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

        public static string ToJSIdentifier(string str)
        {
            StringBuilder strBuilder = new StringBuilder(str.Length);
            char ch = str[0];

            if (str.Contains("$$"))
            { }

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

        public static IEnumerable<Tuple<TLeft, TRight>> Merge<TLeft, TRight>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right)
            => left.Merge(
                right,
                (l, r) => Tuple.Create(l, r));

        public static IEnumerable<TResult> Merge<TLeft, TRight, TResult>(
            this IEnumerable<TLeft> left,
            IEnumerable<TRight> right,
            Func<TLeft, TRight, TResult> resultSelector)
        {
            var leftIter = left.GetEnumerator();
            var rightIter = right.GetEnumerator();
            var rightMore = true;
            var leftMore = true;

            while(true)
            {
                leftMore = leftMore && leftIter.MoveNext();
                rightMore = rightMore && rightIter.MoveNext();

                if (leftMore && rightMore)
                {
                    yield return resultSelector(leftIter.Current, rightIter.Current);
                }
                else if (rightMore)
                {
                    yield return resultSelector(default, rightIter.Current);
                }
                else if (leftMore)
                {
                    yield return resultSelector(leftIter.Current, default);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
