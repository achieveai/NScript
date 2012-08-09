using System;
using System.Collections.Generic;
using System.Text;

namespace Cs2JsC.Lib.ILParser
{
    public class StringFragment : IEquatable<String>, IEquatable<StringFragment>, IComparable<StringFragment>, IEnumerable<char>
    {
        #region member variables
        readonly string parentString;
        readonly int parentEffectiveLength;
        readonly int startIndex;
        readonly int length;
        readonly int hashCode;

        public static readonly StringFragment Empty = new StringFragment();
        #endregion

        #region constructors/Factories
        public StringFragment()
        {
            this.parentString = string.Empty;
            this.length = 0;
            this.startIndex = 0;
            this.parentEffectiveLength = 0;
            this.hashCode = 0;
        }

        public StringFragment(
            string parentString,
            int startIndex,
            int length) :
            this(parentString, parentString.Length, startIndex, length)
        { }

        public StringFragment(
            string parentString,
            int effectiveParentLength,
            int startIndex,
            int length)
        {
            if (length > parentString.Length)
            {
                throw new ArgumentOutOfRangeException("length can't be greater than parent string's length");
            }

            this.parentString = parentString;
            this.startIndex = startIndex;
            this.length = length;
            this.parentEffectiveLength = effectiveParentLength;
            this.hashCode = StringFragment.CalculateHash(
                parentString,
                startIndex,
                length);
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public char this[int index]
        {
            get { return this.parentString[this.startIndex + index]; }
        }

        public string ParentString
        {
            get { return this.parentString; }
        }

        public int StartIndex
        {
            get { return this.startIndex; }
        }

        public int Length
        {
            get { return this.length; }
        }

        public int EffectiveParentLength
        {
            get { return this.parentEffectiveLength; }
        }
        #endregion

        #region public functions
        public static bool IsNull(StringFragment sf)
        {
            return object.ReferenceEquals(sf, null);
        }

        public bool StartsWith(char ch)
        {
            if (this.length > 0)
            {
                return this.parentString[0] == ch;
            }

            return false;
        }

        public bool StartsWith(string str)
        {
            if (str.Length <= this.length)
            {
                for (int i = 0, j = this.startIndex; i < str.Length; i++, j++)
                {
                    if (str[i] != this.parentString[j])
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        public bool StartsWith(StringFragment strFragment)
        {
            if (this.length >= strFragment.length)
            {
                for (int i = 0, iIndex = this.startIndex, jIndex = strFragment.startIndex; i < strFragment.length; i++, iIndex++, jIndex++)
                {
                    if (this.parentString[iIndex] != strFragment.parentString[jIndex])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public StringFragment SubString(
            int startIndex,
            int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length should be > 0");
            }

            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex should be >= 0");
            }

            if (this.length < startIndex + length)
            {
                throw new ArgumentOutOfRangeException("startIndex and length combination is greater than the length");
            }

            return new StringFragment(
                this.parentString,
                this.startIndex + startIndex,
                length);
        }

        public StringFragment RemainingParentPart()
        {
            return new StringFragment(
                this.ParentString,
                this.StartIndex + this.Length,
                this.EffectiveParentLength - this.StartIndex - this.Length);
        }

        public bool AreOverlapping(StringFragment strFragment)
        {
            if (this.startIndex <= strFragment.startIndex &&
                this.startIndex + this.length > strFragment.startIndex &&
                strFragment.length > 0)
            {
                return true;
            }
            else if (strFragment.startIndex <= this.startIndex &&
                strFragment.startIndex + strFragment.length > this.startIndex &&
                this.length > 0)
            {
                return true;
            }

            return false;
        }

        #region overloads
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return this.parentString.Substring(
                this.startIndex,
                this.length);
        }

        public override bool Equals(object obj)
        {
            var strFrag = obj as StringFragment;
            if (!object.ReferenceEquals(strFrag, null))
            {
                return this == strFrag;
            }

            string str = obj as string;
            if (str != null)
            {
                return this == str;
            }

            return false;
        }
        #endregion

        #region operator overloads
        public static explicit operator string(StringFragment stringFragment)
        {
            return StringFragment.IsNull(stringFragment) ? null : stringFragment.ToString();
        }

        public static explicit operator StringFragment(string str)
        {
            return str == null ? null : new StringFragment(str, 0, str.Length);
        }

        public static bool operator ==(StringFragment left, StringFragment right)
        {
            if (object.ReferenceEquals(right, null) ||
                object.ReferenceEquals(left, null))
            {
                if (object.ReferenceEquals(right, left))
                {
                    return true;
                }

                return false;
            }

            return left.parentString == right.parentString &&
                left.startIndex == right.startIndex &&
                left.length == right.length;
        }

        public static bool operator !=(StringFragment left, StringFragment right)
        {
            return !(left == right);
        }

        public static bool operator ==(StringFragment left, String right)
        {
            if (right == null ||
                object.ReferenceEquals(left, null))
            {
                if (object.ReferenceEquals(right, left))
                {
                    return true;
                }

                return false;
            }

            if (left.length == right.Length)
            {
                for (int i = 0, j = left.startIndex; i < left.length; i++, j++)
                {
                    if (right[i] != left.parentString[j])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool operator ==(String left, StringFragment right)
        {
            return right == left;
        }

        public static bool operator !=(StringFragment left, String right)
        {
            return !(left == right);
        }

        public static bool operator !=(String left, StringFragment right)
        {
            return !(right == left);
        }
        #endregion

        #region IEquatable<string> Members

        public bool Equals(string other)
        {
            return this == other;
        }

        #endregion

        #region IEquatable<StringFragment> Members

        public bool Equals(StringFragment other)
        {
            return this == other;
        }

        #endregion

        #region IComparable<StringFragment> Members

        public int CompareTo(StringFragment other)
        {
            int i = 0, j = 0;
            int iIndex = this.startIndex, jIndex = other.startIndex;

            for (; j < other.length && i < this.length; i++, j++, iIndex++, jIndex++)
            {
                if (this.parentString[iIndex] != other.parentString[jIndex])
                {
                    return this.parentString[iIndex].CompareTo(other.parentString[jIndex]);
                }
            }

            return i.CompareTo(j);
        }

        #endregion

        #region IEnumerable<char> Members

        public IEnumerator<char> GetEnumerator()
        {
            for (int i = 0; i < this.length; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region event handlers
        #endregion

        #region private functions
        private static int CalculateHash(
            string parentString,
            int startIndex,
            int length)
        {
            int returnValue = 0;
            for (int i = 0, j = startIndex; i < length; i++, j++)
            {
                int shift = i % 4;
                if (shift < 3)
                {
                    returnValue += parentString[j] >> (shift * 8);
                }
                else
                {
                    returnValue += ((byte)parentString[j]) >> (shift * 8);
                    returnValue += (parentString[j]) << 8;
                }
            }

            return returnValue;
        }

        private bool MatchPart(
            int startIndex,
            string str)
        {
            if (str.Length > this.Length - startIndex)
            {
                return false;
            }

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != this[startIndex + 1])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        internal int IndexOf(char p, int startIndex = 0)
        {
            for (int i = this.StartIndex + startIndex, j = startIndex; j < this.Length && i < this.EffectiveParentLength; i++)
            {
                if (this.ParentString[i] == p)
                {
                    return i - this.StartIndex;
                }
            }

            return -1;
        }

        internal int LastIndexOf(char p)
        {
            for (int i = this.StartIndex + this.Length - 1; i >= this.StartIndex; i--)
            {
                if (this.ParentString[i] == p)
                {
                    return i - this.StartIndex;
                }
            }

            return -1;
        }

        internal static bool IsNullOrEmpty(StringFragment typeName)
        {
            return StringFragment.IsNull(typeName) || typeName.Length == 0;
        }

        internal StringFragment Substring(int startIndex)
        {
            if (startIndex >= this.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new StringFragment(
                this.ParentString,
                this.EffectiveParentLength,
                this.StartIndex + startIndex,
                this.Length - startIndex);
        }

        internal StringFragment Substring(int startIndex, int length)
        {
            if (startIndex >= this.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new StringFragment(
                this.ParentString,
                this.EffectiveParentLength,
                this.StartIndex + startIndex,
                Math.Min(this.Length - startIndex, length));
        }
    }
}
