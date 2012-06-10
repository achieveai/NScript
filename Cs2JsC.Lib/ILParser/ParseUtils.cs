using System;
using Cs2JsC.Lib.MetaData;
using System.Collections.Generic;

namespace Cs2JsC.Lib.ILParser
{
    static class ParseUtils
    {
        static public StringFragment GetNextWord(
            string str,
            int startIndex)
        {
            return ParseUtils.GetNextWord(
                str,
                startIndex,
                str.Length);
        }

        static public StringFragment GetNextWord(
            string str,
            int startIndex,
            int lastIndex)
        {
            return ParseUtils.GetNextWord(
                str,
                startIndex,
                lastIndex,
                char.IsWhiteSpace);
        }

        static public StringFragment GetNextWord(
            string str,
            int startIndex,
            int lastIndex,
            Func<char, bool> BreakCharFunc)
        {
            if (startIndex >= str.Length)
            {
                return null;
            }
            if (lastIndex > str.Length)
            {
                lastIndex = str.Length;
            }
            if (startIndex == lastIndex)
            {
                return null;
            }

            int fragmentStart = -1;
            int fragmentLength = -1;
            int i = startIndex;
            for (; i < lastIndex; i++)
            {
                bool isWhiteSpace = BreakCharFunc(str[i]);
                if (fragmentStart == -1)
                {
                    if (!isWhiteSpace)
                    {
                        fragmentStart = i;
                    }
                }
                else if (isWhiteSpace)
                {
                    break;
                }
            }

            fragmentLength = i - fragmentStart;

            if (fragmentStart < 0)
            {
                return null;
            }
            else
            {
                return new StringFragment(
                    str,
                    lastIndex,
                    fragmentStart,
                    fragmentLength);
            }
        }

        static public StringFragment GetNextWord(
            StringFragment stringFragment,
            int startIndex)
        {
            return ParseUtils.GetNextWord(
                stringFragment.ParentString,
                stringFragment.StartIndex + startIndex,
                stringFragment.StartIndex + stringFragment.Length);
        }

        static public StringFragment GetNextWord(
            StringFragment stringFragment)
        {
            return ParseUtils.GetNextWord(
                stringFragment.ParentString,
                stringFragment.StartIndex + stringFragment.Length,
                stringFragment.EffectiveParentLength);
        }

        static public StringFragment GetNextWord(
            StringFragment stringFragment,
            Func<char, bool> WordSeperatorCheck)
        {
            return ParseUtils.GetNextWord(
                stringFragment.ParentString,
                stringFragment.StartIndex + stringFragment.Length,
                stringFragment.ParentString.Length,
                WordSeperatorCheck);
        }

        static public StringFragment GetNextWord(
            StringFragment stringFragment,
            int startIndex,
            Func<Char, Boolean> WordSeperatorCheck)
        {
            return ParseUtils.GetNextWord(
                stringFragment.ParentString,
                stringFragment.StartIndex + startIndex,
                stringFragment.StartIndex + stringFragment.Length,
                WordSeperatorCheck);
        }

        static public bool InstructionWordSeperator(char ch)
        {
            return ch == '.' || char.IsWhiteSpace(ch);
        }

        static public StringFragment GetFunction(
            string str,
            int startIndex)
        {
            int firstWordStartIndex = -1;
            bool foundStartBraces = false;
            int lastBraceIndex = -1;

            for (int iStr = startIndex; iStr < str.Length; iStr++)
            {
                if (firstWordStartIndex == -1 && !char.IsWhiteSpace(str[iStr]))
                {
                    firstWordStartIndex = iStr;
                }
                else if (!foundStartBraces && str[iStr] == '(')
                {
                    foundStartBraces = true;
                }
                else if (str[iStr] == ')' &&
                    foundStartBraces)
                {
                    lastBraceIndex = iStr;
                    break;
                }
            }

            if (lastBraceIndex > 0)
            {
                return new StringFragment(str, firstWordStartIndex, lastBraceIndex);
            }

            return null;
        }

        static public StringFragment GetFunction(
            StringFragment word)
        {
            return ParseUtils.GetFunction(
                word.ParentString,
                word.StartIndex + word.Length);
        }

        static public StringFragment MakeParent(
            StringFragment word)
        {
            return new StringFragment(
                word.ParentString,
                word.StartIndex,
                word.Length,
                word.StartIndex + word.Length);
        }

        static public MetaData.ClassSignature ParseTypeSignature(StringFragment typeName)
        {
            if (StringFragment.IsNullOrEmpty(typeName))
            {
                return null;
            }

            StringFragment classNameFragment;

            var genericFragments = ParseUtils.GetGenericParams(typeName, out classNameFragment);

            int assemblyBracketIndex = classNameFragment.IndexOf(']');

            if (classNameFragment[0] == '[' && assemblyBracketIndex > 0)
            {
                return new MetaData.ClassSignature(
                    (string)classNameFragment.Substring(assemblyBracketIndex + 1),
                    new MetaData.AssemblySignature(
                        (string)classNameFragment.Substring(1, assemblyBracketIndex - 1)),
                    genericFragments != null ? genericFragments.Count : 0);
            }

            return new MetaData.ClassSignature(
                (string)classNameFragment,
                null,
                genericFragments != null ? genericFragments.Count : 0);
        }

        public static MetaData.FieldSignature Parse(string fieldInfo, bool isStatic)
        {
            var word = ParseUtils.GetNextWord(fieldInfo, 0);

            var fieldType = ILMethod.GetReturnType(ref word);

            var fieldFullName = (string)word;
            var nameParts = fieldFullName.Split(
                new string[] { "::"},
                StringSplitOptions.None);

            var className = nameParts[0];
            var fieldName = nameParts[1];

            return new MetaData.FieldSignature(
                fieldName,
                ParseUtils.ParseTypeSignature((StringFragment)fieldType),
                isStatic,
                ParseUtils.ParseTypeSignature((StringFragment)className));
        }

        public static FieldSignature ParseFieldSignature(string fieldInfo, bool isStatic)
        {
            var word = ParseUtils.GetNextWord(fieldInfo, 0);

            var fieldType = ILMethod.GetReturnType(ref word);

            var fieldFullName = (string)word;
            var nameParts = fieldFullName.Split(
                new string[] { "::"},
                StringSplitOptions.None);

            var className = nameParts[0];
            var fieldName = nameParts[1];

            return new FieldSignature(
                fieldName,
                ParseUtils.ParseTypeSignature((StringFragment)fieldType),
                isStatic,
                ParseUtils.ParseTypeSignature((StringFragment)className));
        }

        public static MethodSignature ParseMethodSignature(
            StringFragment signature)
        {
            StringFragment word = ParseUtils.GetNextWord(signature, 0);

            bool isStatic = true;
            if (word == IlStrings.MethodCallingConvention.Instance)
            {
                isStatic = false;
                word = ParseUtils.GetNextWord(word);
            }

            var returnType = ILMethod.GetReturnType(ref word);

            List<string> methodGenericParams;

            if (word == "class")
            {
                word = ParseUtils.GetNextWord(word);
            }

            var methodSignature = ILMethod.GetMethodSignature(
                false,
                ref word,
                out methodGenericParams,
                null);

            string[] methodNameParts = methodSignature.Value.Key.Split(
                new string[] { "::"},
                StringSplitOptions.None);

            string className = methodNameParts[methodNameParts.Length - 2];
            string functionName = methodNameParts[methodNameParts.Length - 1];
            var arguments = methodSignature.Value.Value;

            ClassSignature classSignature = ParseUtils.ParseTypeSignature((StringFragment)className);

            returnType = returnType == null ?
                null :
                ParseUtils.ResolveGenericTypeName(
                    (StringFragment)returnType,
                    null,
                    null);

            return new MetaData.MethodSignature(
                functionName,
                ParseUtils.ParseTypeSignature((StringFragment)returnType),
                isStatic,
                arguments,
                classSignature);
        }

        /// <summary>
        /// There are two ways in which we can find generic Parameters,
        /// 1. Foo<T,U>
        /// 2. Foo<valuetype .ctor ([sscorlib]System.ValueType) T,class U>
        /// 
        /// The later case is where T is of type stuct and U is of type class.
        /// 
        /// To make sure we get T, and U (or for that matter Tee or Uoo), break this generic thing
        /// on commas, and pick up last word.
        /// </summary>
        /// <param name="nameWord"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<string> GetGenericParams(StringFragment nameWord, out StringFragment name)
        {
            name = nameWord;
            if (nameWord.IndexOf('<') >= 0)
            {
                int functionBeginIndex = 0;

                if (nameWord.IndexOf(':') >= 0)
                {
                    functionBeginIndex = nameWord.IndexOf(':');
                }

                int beginIndex = nameWord.IndexOf('<', functionBeginIndex) + nameWord.StartIndex;

                int endIndex = nameWord.ParentString.IndexOf('>', beginIndex);

                if (endIndex <= beginIndex)
                {
                    return null;
                }

                // We have a generic parameter list.
                //
                string genericStrDecl = nameWord.ParentString.Substring(
                    beginIndex + 1, endIndex - beginIndex - 1);

                var genericParts = genericStrDecl.Split(',');
                List<string> returnValue = new List<string>();

                foreach (var genericPart in genericParts)
                {
                    int spaceIndex = genericPart.LastIndexOf(' ');

                    if (spaceIndex < 0)
                    {
                        returnValue.Add(genericPart);
                    }
                    else
                    {
                        returnValue.Add(genericPart.Substring(spaceIndex + 1));
                    }
                }

                name = new StringFragment(
                    nameWord.ParentString,
                    nameWord.StartIndex,
                    beginIndex - nameWord.StartIndex);

                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public static string ResolveGenericTypeName(
            StringFragment typeName,
            List<string> classGenericParameters,
            List<string> methodGenericParameters)
        {
            if (!StringFragment.IsNull(typeName) &&
                typeName.Length > 0 &&
                typeName[0] == '!')
            {
                // In case we have !! at the begning of type, it means that the type is refereing back
                // to Method's generic parameter.
                //
                // In case we have ! (single !) at the begning of type, it means that the type is refering
                // back to Class's generic parameter.
                //
                // If the parameter name is a number, we don't have to resolve it, otherwise, we need to match
                // it to one of the generic parameter names in the list, and use it's index to get the number
                //
                bool typeIsMethodParameter = false;
                List<string> parameterListToUse;

                if (typeName[1] == '!')
                {
                    typeIsMethodParameter = true;
                    typeName = new StringFragment(
                        typeName.ParentString,
                        typeName.EffectiveParentLength,
                        typeName.StartIndex + 2,
                        typeName.Length - 2);

                    parameterListToUse = methodGenericParameters;
                }
                else
                {
                    typeName = new StringFragment(
                        typeName.ParentString,
                        typeName.EffectiveParentLength,
                        typeName.StartIndex + 1,
                        typeName.Length - 1);

                    parameterListToUse = classGenericParameters;
                }

                for (int i = 0;parameterListToUse !=  null && i < parameterListToUse.Count; i++)
                {
                    if (typeName == parameterListToUse[i])
                    {
                        if (typeIsMethodParameter)
                        {
                            return string.Format("!!{0}", i);
                        }
                        else
                        {
                            return string.Format("!{0}", i);
                        }
                    }
                }

                // We can only be here if typeName is already a number.
                //
                if (!char.IsDigit(typeName[0]))
                {
                    throw new InvalidOperationException();
                }

                return string.Format("{0}{1}",
                    typeIsMethodParameter ? "!!" : "!",
                    typeName);
            }

            return (string)typeName;
        }
    }
}

