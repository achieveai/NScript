using System;
using System.Collections.Generic;
using System.Text;
using Cs2JsC.Lib.MetaData;

namespace Cs2JsC.Lib.ILParser
{

    [Flags]
    public enum MethodImplementationAttributes
    {
        None = 0,
        Native = 0x1,
        Cil = 0x2,
        OptIl = 0x4,
        Managed = 0x8,
        Unmanaged = 0x10,
        Forwardref = 0x20,
        PreserveSig = 0x40,
        Runtime = 0x80,
        InternalCall = 0x100,
        Synchronized = 0x200,
        Noninlining = 0x400,
    }

    public class ILMethod
    {
        #region member variables
        #endregion

        #region constructors/Factories
        private ILMethod()
        { }

        public static ILMethod ParseFromHeader(
            string header,
            bool isProperty = false,
            List<string> classGenericParameter = null)
        {
            var head = ParseUtils.GetNextWord(header, 0);

            if ((!isProperty && head != IlStrings.ScopeNames.MethodType) ||
                (isProperty && head != IlStrings.ScopeNames.PropertyType))
            {
                // Note a header for ILType.
                return null;
            }

            var word = head;
            List<string> methodGenericParameters = null;

            // Read all the attributes.
            //
            var attributes = ILMethod.GetMethodAttributes(ref word);
            var callingConvention = ILMethod.GetCallingConvertion(ref word);
            string returnType = ILMethod.GetReturnType(ref word);
            
            var methodSignature = ILMethod.GetMethodSignature(
                !isProperty,
                ref word,
                out methodGenericParameters,
                classGenericParameter);

            returnType = returnType == null ? null :
                ParseUtils.ResolveGenericTypeName(
                    (StringFragment)returnType,
                    classGenericParameter,
                    methodGenericParameters);

            return new ILMethod()
            {
                MethodName = methodSignature.Value.Key,
                Arguments = methodSignature.Value.Value,
                Attributes = attributes,
                CallingConverntion = callingConvention,
                ReturnType = returnType,
            };
        }
        #endregion

        // ****************** Public  Stuff *****************************
        #region properties
        public MethodAttributes Attributes
        { get; private set; }

        public MethodCallingConvention CallingConverntion
        { get; private set; }

        public string Id
        { get; private set; }

        public string ReturnType
        { get; private set; }

        public string MethodName
        { get; private set; }

        public List<ArgumentSignature> Arguments
        { get; private set; }
        #endregion

        #region public functions
        #endregion

        // ****************** Private Stuff *****************************
        #region private properties
        #endregion

        #region private functions
        public static KeyValuePair<string, List<ArgumentSignature>>? GetMethodSignature(
            bool isMethodDecl,
            ref StringFragment word,
            out List<string> methodGenericParameters,
            List<string> classGenericParameters = null)
        {
            int firstWordStartIndex = -1;
            bool foundStartBraces = false;
            int firstBraceIndex = -1;
            int lastBraceIndex = -1;

            methodGenericParameters = null;
            string str = word.ParentString;
            bool inGenericParameterList = false;
            int genericParameterListStart = -1;

            for (int iStr = word.StartIndex; iStr < str.Length; iStr++)
            {
                if (firstWordStartIndex == -1 && !char.IsWhiteSpace(str[iStr]))
                {
                    firstWordStartIndex = iStr;
                }
                else if (!foundStartBraces && str[iStr] == '<')
                {
                    inGenericParameterList = true;
                    genericParameterListStart = iStr;
                }
                else if (inGenericParameterList)
                {
                    if (str[iStr] == '>')
                    {
                        inGenericParameterList = false;
                    }
                }
                else if (!foundStartBraces && str[iStr] == '(')
                {
                    foundStartBraces = true;
                    firstBraceIndex = iStr;
                }
                else if (str[iStr] == ')' &&
                    foundStartBraces)
                {
                    lastBraceIndex = iStr;
                    break;
                }
            }

            var functionName = new StringFragment(
                str,
                firstWordStartIndex,
                firstBraceIndex - firstWordStartIndex);

            StringFragment tmp = functionName;

            methodGenericParameters = ParseUtils.GetGenericParams(tmp, out functionName);

            var argumentList = new StringFragment(
                str,
                firstBraceIndex + 1,
                lastBraceIndex - firstBraceIndex - 1);

            var arguments = ILMethod.GetArgumentList(
                isMethodDecl,
                argumentList,
                methodGenericParameters,
                classGenericParameters);

            word = ParseUtils.GetNextWord(str, lastBraceIndex + 1);

            return new KeyValuePair<string,List<ArgumentSignature>>(
                    (string)functionName,
                    arguments);
        }

        private static List<ArgumentSignature> GetArgumentList(
            bool isMethodDecl,
            StringFragment argument,
            List<string> functionGenericParameters,
            List<string> classGenericParameters)
        {
            List<ArgumentSignature> returnValue = new List<ArgumentSignature>();

            var word = ParseUtils.GetNextWord(argument, 0);

            if (!StringFragment.IsNull(word))
            {
                do
                {
                    bool isOut = false;
                    bool isRef = false;

                    string argumentType = ILMethod.GetReturnType(ref word);

                    if (argumentType == "[out]")
                    {
                        isOut = true;
                        argumentType = ILMethod.GetReturnType(ref word);
                    }

                    string argumentName = null;

                    if (isMethodDecl)
                    {
                        argumentName = (string)word;

                        if (argumentName.EndsWith(","))
                        {
                            argumentName = argumentName.Substring(0, argumentName.Length - 1);
                        }

                        word = ParseUtils.GetNextWord(word);
                    }
                    else if (argumentType.EndsWith(","))
                    {
                        argumentType = argumentType.Substring(
                            0,
                            argumentType.Length - 1);
                    }

                    if (argumentType.EndsWith("&"))
                    {
                        argumentType = argumentType.Substring(0, argumentType.Length - 1);
                        isRef = isOut ? false : true;
                    }

                    argumentType = ParseUtils.ResolveGenericTypeName(
                        (StringFragment)argumentType,
                        classGenericParameters,
                        functionGenericParameters);

                    // Let's take care of value keyword in property setter.
                    //
                    if (argumentName != null)
                    {
                        argumentName = argumentName.Replace('\'', '$');
                    }

                    returnValue.Add(
                        new ArgumentSignature(
                            argumentType,
                            argumentName,
                            returnValue.Count,
                            isRef,
                            isOut));
                }
                while (!StringFragment.IsNull(word));
            }

            return returnValue;
        }

        public static string GetReturnType(ref StringFragment word)
        {
            string returnValue = null;
            switch ((string)word)
            {
                case IlStrings.MethodReturnClassTypes.Void:
                    break;
                case "native":
                case IlStrings.MethodReturnClassTypes.Class:
                case IlStrings.MethodReturnClassTypes.Struct:
                    returnValue = (string)(word = ParseUtils.GetNextWord(word));
                    break;
                default:
                    returnValue = (string)word;
                    break;
            }

            word = ParseUtils.GetNextWord(word);
            return returnValue;
        }

        private static MethodAttributes GetMethodAttributes(ref StringFragment word)
        {
            MethodAttributes attributes = MethodAttributes.None;
            bool attributesDone = false;
            while (!attributesDone &&
                !Object.Equals(
                    null,
                    (word = ParseUtils.GetNextWord(word))))
            {
                switch ((string)word)
                {
                    case IlStrings.MethodAttributes.Public:
                        attributes |= MethodAttributes.Public;
                        break;
                    case IlStrings.MethodAttributes.Private:
                        attributes |= MethodAttributes.Private;
                        break;
                    case IlStrings.MethodAttributes.Family:
                        attributes |= MethodAttributes.Family;
                        break;
                    case IlStrings.MethodAttributes.Assembly:
                        attributes |= MethodAttributes.Assembly;
                        break;
                    case IlStrings.MethodAttributes.Famandassem:
                        attributes |= MethodAttributes.Famandassem;
                        break;
                    case IlStrings.MethodAttributes.Famorassem:
                        attributes |= MethodAttributes.Famorassem;
                        break;
                    case IlStrings.MethodAttributes.PrivateScope:
                        attributes |= MethodAttributes.PrivateScope;
                        break;
                    case IlStrings.MethodAttributes.Static:
                        attributes |= MethodAttributes.Static;
                        break;
                    case IlStrings.MethodAttributes.RtSpecialName:
                        attributes |= MethodAttributes.RtSpecialName;
                        break;
                    case IlStrings.MethodAttributes.SpecialName:
                        attributes |= MethodAttributes.SpecialName;
                        break;
                    case IlStrings.MethodAttributes.Final:
                        attributes |= MethodAttributes.Final;
                        break;
                    case IlStrings.MethodAttributes.Virtual:
                        attributes |= MethodAttributes.Virtual;
                        break;
                    case IlStrings.MethodAttributes.Abstract:
                        attributes |= MethodAttributes.Abstract;
                        break;
                    case IlStrings.MethodAttributes.HideBySig:
                        attributes |= MethodAttributes.HideBySig;
                        break;
                    case IlStrings.MethodAttributes.NewSlot:
                        attributes |= MethodAttributes.NewSlot;
                        break;
                    case IlStrings.MethodAttributes.ReqSecObj:
                        attributes |= MethodAttributes.ReqSecObj;
                        break;
                    default:
                        attributesDone = true;
                        break;
                }
            }

            return attributes;
        }

        private static MethodCallingConvention GetCallingConvertion(ref StringFragment word)
        {
            MethodCallingConvention returnValue = MethodCallingConvention.None;

            bool done = false;
            do
            {
                switch ((string)word)
                {
                    case IlStrings.MethodCallingConvention.Instance:
                        returnValue |= MethodCallingConvention.Instance;
                        word = ParseUtils.GetNextWord(word);
                        break;
                    case IlStrings.MethodCallingConvention.VarArg:
                        returnValue |= MethodCallingConvention.Varargs;
                        word = ParseUtils.GetNextWord(word);
                        break;
                    default:
                        done = true;
                        break;
                }
            } while (!done);

            return returnValue;
        }

        private static MethodImplementationAttributes GetMethodImplementationAttributes(ref StringFragment word)
        {
            MethodImplementationAttributes implementationAttributes = MethodImplementationAttributes.None;
            switch ((string)word)
            {
                case IlStrings.MethodImplementationAttributes.Native:
                    implementationAttributes |= MethodImplementationAttributes.Native;
                    break;
                case IlStrings.MethodImplementationAttributes.Cil:
                    implementationAttributes |= MethodImplementationAttributes.Cil;
                    break;
                case IlStrings.MethodImplementationAttributes.OptIl:
                    implementationAttributes |= MethodImplementationAttributes.OptIl;
                    break;
                case IlStrings.MethodImplementationAttributes.Managed:
                    implementationAttributes |= MethodImplementationAttributes.Managed;
                    break;
                case IlStrings.MethodImplementationAttributes.Unmanaged:
                    implementationAttributes |= MethodImplementationAttributes.Unmanaged;
                    break;
                case IlStrings.MethodImplementationAttributes.Forwardref:
                    implementationAttributes |= MethodImplementationAttributes.Forwardref;
                    break;
                case IlStrings.MethodImplementationAttributes.PreserveSig:
                    implementationAttributes |= MethodImplementationAttributes.PreserveSig;
                    break;
                case IlStrings.MethodImplementationAttributes.Runtime:
                    implementationAttributes |= MethodImplementationAttributes.Runtime;
                    break;
                case IlStrings.MethodImplementationAttributes.InternalCall:
                    implementationAttributes |= MethodImplementationAttributes.InternalCall;
                    break;
                case IlStrings.MethodImplementationAttributes.Synchronized:
                    implementationAttributes |= MethodImplementationAttributes.Synchronized;
                    break;
                case IlStrings.MethodImplementationAttributes.Noninlining:
                    implementationAttributes |= MethodImplementationAttributes.Noninlining;
                    break;
                default:
                    throw new InvalidOperationException("Cannot come here");
            }

            return implementationAttributes;
        }
        #endregion
    }
}
