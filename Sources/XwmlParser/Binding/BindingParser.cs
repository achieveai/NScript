//-----------------------------------------------------------------------
// <copyright file="BindingParser.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.CLR;
    using System;
    using System.Collections.Generic;
    using XwmlParser.NodeInfos;
    using System.Linq;

    public enum BindingMode
    {
        Default,
        OneTime,
        OneWay,
        TwoWay
    }

    /// <summary>
    /// Definition for BindingParser
    /// </summary>
    public class BindingParser
    {
        enum BraceScopeType
        {
            None,
            Property,
            Cast
        }

        enum BindingPart
        {
            Path,
            Converter,
            Mode,
            Source
        }

        /// <summary>
        /// The one time string.
        /// </summary>
        const string OneTimeStr = "OneTime";

        /// <summary>
        /// The one way string.
        /// </summary>
        const string OneWayStr = "OneWay";

        /// <summary>
        /// The two way string.
        /// </summary>
        const string TwoWayStr = "TwoWay";

        /// <summary>
        /// The data context string.
        /// </summary>
        const string DataContextStr = "DataContext";

        /// <summary>
        /// The parent string.
        /// </summary>
        const string ParentStr = "TemplateParent";

        /// <summary>
        /// The path string.
        /// </summary>
        const string PathStr = "Path";

        /// <summary>
        /// The mode string.
        /// </summary>
        const string ModeStr = "Mode";

        /// <summary>
        /// The converter string.
        /// </summary>
        const string ConverterStr = "Converter";

        /// <summary>
        /// Source string.
        /// </summary>
        const string SourceStr = "Source";

        /// <summary>
        /// The property format error because of brace.
        /// </summary>
        const string propertyFormatErrorBecauseOfBrace =
                        "Invalid Property Path: {0}, braces not matching";

        /// <summary>
        /// Parse binding string.
        /// 
        /// Path Types: 1. {p1.P2.P3}
        /// 2. {((vm:FooVM)P1.P2).P3}
        /// 
        /// Binding Type: 1. Type=OneTime (default)
        /// 2. Type=OneWay 3. Type=TwoWay
        /// 
        /// Converter: 1. Converter=vm:FooConverter (required for twoway, may take argument)
        /// 2. Converter=vm:Foo.ConverterFunc (delegate to return value of type target, may have argument)
        /// 
        /// Converter Arguments list 1. number, 2. quoted string (string), 3. Enum 4. TypeMember as enum,
        /// 5. Const variable, 6. Property binding to static field/property
        /// 
        /// Source 1. DataContext (default)
        /// 2. Parent 2. vm:Class (static binding)
        /// 3. ::Name (other named element not supported yet)
        /// </summary>
        /// <param name="targetBinding">   Target binding. </param>
        /// <param name="bindingValue">    The binding value. </param>
        /// <param name="documentContext"> Context for the document. </param>
        /// <param name="dataContextType"> Type of the data context. </param>
        /// <param name="controlType">     Type of the control. </param>
        /// <returns>
        /// .
        /// </returns>
        public static BinderInfo ParseBinding(
            TargetBindingInfo targetBinding,
            string bindingValue,
            IDocumentContext documentContext,
            TypeReference dataContextType,
            TypeReference controlType)
        {
            var dict = BindingParser.GetBreakups(bindingValue);
            var sourceInfo = 
                BindingParser.GetSourceInfo(
                    dict,
                    bindingValue,
                    documentContext,
                    dataContextType,
                    controlType);

            return new BinderInfo(
                targetBinding,
                BindingParser.GetSourceBindingInfo(
                    dict,
                    bindingValue,
                    sourceInfo.Item1,
                    documentContext,
                    targetBinding),
                BindingParser.GetConverterInfo(
                    dict,
                    bindingValue,
                    documentContext),
                targetBinding.GetBindingMode(
                    BindingParser.GetBindingMode(
                        dict,
                        bindingValue),
                    documentContext.ParserContext.KnownTypes),
                sourceInfo.Item2,
                null);
        }

        /// <summary>
        /// Query if 'str' is binding text.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// true if binding text, false if not.
        /// </returns>
        public static bool IsBindingText(string str)
        {
            if (str.Length > 3
                && str[0] == '{'
                && str[1] != '{'
                && str[str.Length - 1] == '}')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets binding mode.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="dict">       The dictionary. </param>
        /// <param name="bindingStr"> The binding string. </param>
        /// <returns>
        /// The binding mode.
        /// </returns>
        private static BindingMode GetBindingMode(
            Dictionary<BindingPart, Tuple<int, int>> dict,
            string bindingStr)
        {
            Tuple<int, int> stringPart;
            if (!dict.TryGetValue(BindingPart.Mode, out stringPart))
            {
                return BindingMode.Default;
            }

            string sourceStr = bindingStr.Substring(stringPart.Item1, stringPart.Item2 - stringPart.Item1).Trim();
            switch (sourceStr)
            {
                case BindingParser.OneTimeStr:
                    return BindingMode.OneTime;
                case BindingParser.OneWayStr:
                    return BindingMode.OneWay;
                case BindingParser.TwoWayStr:
                    return BindingMode.TwoWay;
            }

            throw new ApplicationException(
                string.Format(
                    "Invalid binding mode {0}",
                    sourceStr));
        }

        /// <summary>
        /// Gets converter information.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="dict">            The dictionary. </param>
        /// <param name="bindingStr">      The binding string. </param>
        /// <param name="documentContext"> Context for the document. </param>
        /// <returns>
        /// The converter information.
        /// </returns>
        private static ConverterInfo GetConverterInfo(
            Dictionary<BindingPart, Tuple<int, int>> dict,
            string bindingStr,
            IDocumentContext documentContext)
        {
            Tuple<int, int> stringPart;
            if (!dict.TryGetValue(BindingPart.Converter, out stringPart))
            {
                return null;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets source information.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="dict">           The dictionary. </param>
        /// <param name="bindingStr">     The binding string. </param>
        /// <returns>
        /// The source information.
        /// </returns>
        private static Tuple<TypeReference, SourceType> GetSourceInfo(
            Dictionary<BindingPart, Tuple<int, int>> dict,
            string bindingStr,
            IDocumentContext documentContext,
            TypeReference dataContextType,
            TypeReference controlType)
        {
            Tuple<int, int> stringPart;
            if (!dict.TryGetValue(BindingPart.Source, out stringPart))
            {
                return Tuple.Create(dataContextType, SourceType.DataContext);
            }

            string sourceStr = bindingStr.Substring(stringPart.Item1, stringPart.Item2 - stringPart.Item1).Trim();
            if (sourceStr == BindingParser.DataContextStr)
            {
                return Tuple.Create(dataContextType, SourceType.DataContext);
            }

            if (sourceStr == BindingParser.ParentStr)
            {
                return Tuple.Create(controlType, SourceType.Parent);
            }

            TypeReference staticType =
                documentContext.Resolver.GetTypeReference(
                    documentContext.GetFullName(sourceStr));

            if (staticType == null)
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't resolve Source:{0} for binding",
                        sourceStr));
            }
            else
            {
                return Tuple.Create(staticType, SourceType.Static);
            }
        }

        /// <summary>
        /// Gets source binding information.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="dict">            The dictionary. </param>
        /// <param name="bindingStr">      The binding string. </param>
        /// <param name="sourceType">      Type of the source. </param>
        /// <param name="documentContext"> Context for the document. </param>
        /// <returns>
        /// The source binding information.
        /// </returns>
        private static SourceBindingInfo GetSourceBindingInfo(
            Dictionary<BindingPart, Tuple<int, int>> dict,
            string bindingStr,
            TypeReference sourceType,
            IDocumentContext documentContext,
            TargetBindingInfo targetBindingInfo)
        {
            Tuple<int, int> stringPart;
            if (!dict.TryGetValue(BindingPart.Path, out stringPart))
            {
                return null;
            }

            var pathInfo = BindingParser.ParsePath(
                bindingStr,
                stringPart.Item1,
                stringPart.Item2);

            var resolver = documentContext.Resolver;
            TypeReference currentType = sourceType;
            bool staticBinding = false;
            if (pathInfo.Count > 1
                && pathInfo[0].Item2.IndexOf(':') > 0)
            {
                var typeRef = resolver.GetTypeReference(
                    documentContext.GetFullName(pathInfo[0].Item2));

                if (typeRef != null)
                {
                    currentType = typeRef;
                    staticBinding = true;
                }
            }

            List<MemberReference> propertyReferences = new List<MemberReference>();
            for (int iPath = staticBinding ? 1 : 0; iPath < pathInfo.Count; iPath++)
            {
                var memberName = pathInfo[iPath].Item2;
                var memberInfo = BindingParser.GetFieldOrPropertyReference(
                    currentType,
                    memberName,
                    resolver);

                if (memberInfo == null)
                {
                    if (iPath == pathInfo.Count - 1
                        && targetBindingInfo.BindingType.IsDelegate())
                    {
                        MethodReference method = BindingParser.GetMethodReferenceWithMatch(
                            currentType,
                            memberName,
                            resolver,
                            targetBindingInfo.BindingType,
                            targetBindingInfo is DomEventTargetBindingInfo);

                        if (method != null)
                        {
                            return new MethodSourceBindingInfo(
                                propertyReferences.Count > 0
                                    ? new PropertySourceBindingInfo(
                                        sourceType,
                                        propertyReferences)
                                    : null,
                                method);
                        }
                    }

                    throw new ApplicationException(
                        string.Format("Can't resolve propertyName: {0} on Type: {1}",
                            pathInfo[iPath].Item2,
                            currentType));
                }

                propertyReferences.Add(memberInfo.Item1);
                currentType = memberInfo.Item2;

                if (pathInfo[iPath].Item1 != null)
                {
                    var castReference = documentContext.Resolver.GetTypeReference(
                        documentContext.GetFullName(pathInfo[iPath].Item1));

                    if (castReference == null)
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Can't resolve type:{0}",
                                pathInfo[iPath]));
                    }

                    if (!documentContext.Resolver.TypeInherits(
                            castReference,
                            currentType)
                        && !documentContext.Resolver.TypeImplements(
                            castReference,
                            currentType)
                        && !documentContext.Resolver.TypeInherits(
                            currentType,
                            castReference)
                        && !documentContext.Resolver.TypeImplements(
                            currentType,
                            castReference))
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Can't cast type:{0} to type:{1}",
                                currentType,
                                castReference));
                    }

                    currentType = castReference;
                }
            }

            return new PropertySourceBindingInfo(
                sourceType,
                propertyReferences);
        }

        private static Tuple<MemberReference, TypeReference> GetFieldOrPropertyReference(
            TypeReference typeReference,
            string memberName,
            IClrResolver resolver)
        {
            FieldReference field = null;
            PropertyReference property;
            property = resolver.GetPropertyReference(
                typeReference,
                memberName);

            // TODO: we need to check for methodReferences as well for event binding.

            if (property != null)
            {
                return Tuple.Create<MemberReference, TypeReference>(
                    property,
                    property.PropertyType);
            }

            field = resolver.GetFieldReference(
                typeReference,
                memberName);

            if (field != null)
            {
                return Tuple.Create<MemberReference, TypeReference>(
                    field,
                    field.FieldType);
            }

            return null;
        }

        private static MethodReference GetMethodReferenceWithMatch(
            TypeReference typeReference,
            string methodName,
            IClrResolver resolver,
            TypeReference delegateType,
            bool skipParamsOk)
        {
            var methods = resolver.GetMethodReference(
                typeReference,
                methodName);

            if (methods == null)
            { return null; }

            SortedDictionary<int, List<MethodReference>> matchedMethods = new SortedDictionary<int, List<MethodReference>>();
            for (int iMethod = 0; iMethod < methods.Count; iMethod++)
            {
                var matchNum = delegateType.ImplementsDelegate(methods[iMethod], skipParamsOk);
                if (matchNum.HasValue)
                {
                    List<MethodReference> coll;
                    if (!matchedMethods.TryGetValue(matchNum.Value, out coll))
                    {
                        coll = new List<MethodReference>();
                        matchedMethods.Add(matchNum.Value, coll);
                    }

                    coll.Add(methods[iMethod]);
                }
            }

            if (matchedMethods.Count > 0)
            {
                var bestMatch = matchedMethods.First().Value;
                if (bestMatch.Count > 1)
                {
                    throw new ApplicationException(
                        string.Format(
                            "More than one match found for Method:{0}",
                            methodName));
                }

                return bestMatch[0];
            }

            return null;
        }

        /// <summary>
        /// Parse binding string. (default)
        /// Path Types:
        /// 1. {p1.P2.P3}
        /// 2. {((vm:FooVM)P1.P2).P3}
        ///
        /// Binding Type:
        /// 1. Type=OneTime (default)
        /// 2. Type=OneWay
        /// 3. Type=TwoWay
        /// 
        /// Converter:
        /// 1. Converter=vm:FooConverter (required for twoway, may take argument)
        /// 2. Converter=vm:Foo.ConverterFunc (delegate to return value of type target, may have argument)
        ///
        /// Converter Arguments list
        /// 1. number,
        /// 2. quoted string (string),
        /// 3. Enum
        /// 4. TypeMember as enum,
        /// 5. Const variable,
        /// 6. Property binding to static field/property
        ///
        /// Source
        /// 1. DataContext (default)
        /// 2. Parent
        /// 2. vm:Class (static binding)
        /// 3. ::Name (other named element not supported yet)
        /// </summary>
        private static Dictionary<BindingPart, Tuple<int, int>> GetBreakups(string binderString)
        {
            List<int> commaIndexes = GetCommaIndexes(binderString, true);
            Dictionary<BindingPart, Tuple<int, int>> bindingParts = new Dictionary<BindingPart, Tuple<int, int>>();

            for (int i = 1; i < commaIndexes.Count; i++)
            {
                Tuple<BindingPart, int> bindingPartInfo = GetBindingPart(binderString, commaIndexes[i - 1] + 1, commaIndexes[i]);
                if (bindingPartInfo != null)
                {
                    BindingParser.AddPartInfo(
                        bindingParts,
                        bindingPartInfo.Item1,
                        binderString,
                        bindingPartInfo.Item2 + 1,
                        commaIndexes[i]);
                }
                else if (i == 1)
                {
                    bindingParts[BindingPart.Path] = Tuple.Create(commaIndexes[i - 1] + 1, commaIndexes[i]);
                }
            }

            return bindingParts;
        }

        /// <summary>
        /// Adds a part information.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="dict">         The dictionary. </param>
        /// <param name="bindingPart">  The binding part. </param>
        /// <param name="binderString"> The binder string. </param>
        /// <param name="startIndex">   The start index. </param>
        /// <param name="endIndex">     The end index. </param>
        private static void AddPartInfo(
            Dictionary<BindingPart, Tuple<int, int>> dict,
            BindingPart bindingPart,
            string binderString,
            int startIndex,
            int endIndex)
        {
            Tuple<int, int> tupl;
            if (dict.TryGetValue(bindingPart, out tupl))
            {
                throw new ApplicationException(
                    string.Format(
                        "Multiple {0} parts found 1:'{1}' & 2:'{2}'",
                        bindingPart,
                        binderString.Substring(tupl.Item1, tupl.Item2 - tupl.Item1),
                        binderString.Substring(startIndex, endIndex - startIndex)));
            }

            dict[bindingPart] = Tuple.Create(startIndex, endIndex);
        }

        /// <summary>
        /// Gets comma indexes.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// The comma indexes.
        /// </returns>
        private static List<int> GetCommaIndexes(string str, bool skipCorners = false)
        {
            List<int> commaIndexes = new List<int>(2);
            int delta = skipCorners ? 1 : 0;
            commaIndexes.Add(delta - 1);
            for (int i = 0; i < str.Length - delta; i++)
            {
                char ch = str[i];
                if (ch == ',')
                {
                    commaIndexes.Add(i);
                }
                else if (ch == '(')
                {
                    i = GetMatchingBraceIndex(str, i, str.Length);
                }
            }

            commaIndexes.Add(str.Length - delta);
            return commaIndexes;
        }

        /// <summary>
        /// Gets binding part.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="str">        The. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <returns>
        /// The binding part.
        /// </returns>
        private static Tuple<BindingPart, int> GetBindingPart(
            string str,
            int startIndex,
            int endIndex)
        {
            int equalsIndex = str.IndexOf('=', startIndex, endIndex - startIndex);
            if (equalsIndex < 0)
            {
                return null;
            }

            while (startIndex < equalsIndex && char.IsWhiteSpace(str[startIndex]))
            { ++startIndex; }

            BindingPart part;
            if (BindingParser.ContainsKey(str, startIndex, equalsIndex, PathStr))
            {
                part = BindingPart.Path;
            }
            else if (BindingParser.ContainsKey(str, startIndex, equalsIndex, ConverterStr))
            {
                part = BindingPart.Converter;
            }
            else if (BindingParser.ContainsKey(str, startIndex, equalsIndex, ModeStr))
            {
                part = BindingPart.Mode;
            }
            else if (BindingParser.ContainsKey(str, startIndex, equalsIndex, SourceStr))
            {
                part = BindingPart.Source;
            }
            else
            {
                throw new ApplicationException(
                    string.Format(
                    "Unknown binding part name '{0}'",
                    str.Substring(startIndex, equalsIndex - startIndex)));
            }

            return Tuple.Create(part, equalsIndex);
        }

        /// <summary>
        /// Query if this object contains key.
        /// </summary>
        /// <param name="str">        The. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <param name="key">        The key. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        private static bool ContainsKey(string str, int startIndex, int endIndex, string key)
        {
            int matchIndex = BindingParser.Contains(str, startIndex, endIndex, key);
            if (matchIndex < 0)
            {
                return false;
            }

            if ((matchIndex == startIndex || !char.IsLetterOrDigit(str[matchIndex-1]))
                && (matchIndex + key.Length == endIndex || !char.IsLetterOrDigit(str[matchIndex + key.Length])))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if this collection contains a given object.
        /// </summary>
        /// <param name="str">        The. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <param name="subStr">     The sub string. </param>
        /// <returns>
        /// .
        /// </returns>
        private static int Contains(string str, int startIndex, int endIndex, string subStr)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                int j;
                for (j = 0; j < subStr.Length && j + i < endIndex; j++)
                {
                    if (str[i + j] != subStr[j])
                    { break; }
                }

                if (j == subStr.Length)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Parse path.
        /// </summary>
        /// <exception cref="ApplicationException">      Thrown when an Application error condition
        ///     occurs. </exception>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="path">       Full pathname of the file. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <returns>
        /// .
        /// </returns>
        private static List<Tuple<string, string>> ParsePath(string path, int startIndex = 0, int endIndex = -1)
        {
            List<Tuple<string, string>> rv = new List<Tuple<string, string>>();
            string castType = null;
            endIndex = endIndex == -1 ? path.Length : endIndex;
            int trackerStart = startIndex;
            int trackerEnd;
            for (trackerEnd = startIndex; trackerEnd < endIndex; trackerEnd++)
            {
                switch (path[trackerEnd])
                {
                    case '(':
                        if (rv.Count > 0)
                        {
                            throw new ApplicationException(
                                string.Format(
                                    BindingParser.propertyFormatErrorBecauseOfBrace,
                                    path));
                        }

                        switch (InvestigateBrace(path, trackerEnd, endIndex))
                        {
                            case BraceScopeType.None:
                                return ParsePath(path, startIndex + 1, endIndex - 1);
                            case BraceScopeType.Property:
                                trackerEnd = GetMatchingBraceIndex(path, trackerEnd, endIndex);
                                rv = ParsePath(path, trackerStart + 1, trackerEnd);

                                // Let's skip next '.' as well.
                                trackerStart = (++trackerEnd) + 1;
                                break;
                            case BraceScopeType.Cast:
                                trackerEnd = GetMatchingBraceIndex(path, trackerEnd, endIndex);

                                // Adjust for starting brace.
                                castType = path.Substring(trackerStart + 1, trackerEnd - trackerStart - 1);
                                trackerStart = trackerEnd + 1;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }

                        break;
                    case '.':
                        rv.Add(
                            Tuple.Create(
                            (string)null,
                            path.Substring(trackerStart, trackerEnd - trackerStart).Trim()));
                        trackerStart = trackerEnd + 1;
                        break;
                    default:
                        break;
                }
            }

            if (trackerStart < trackerEnd)
            {
                rv.Add(
                    Tuple.Create(
                    castType,
                    path.Substring(trackerStart, trackerEnd - trackerStart).Trim()));
            }
            else if (castType != null)
            {
                rv[rv.Count - 1] = Tuple.Create(
                    castType,
                    rv[rv.Count - 1].Item2);
            }

            return rv;
        }

        /// <summary>
        /// Parse converter.
        /// </summary>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
        ///     illegal values. </exception>
        /// <param name="converterString"> The converter string. </param>
        /// <param name="startIndex">      The start index. </param>
        /// <param name="endIndex">        The end index. </param>
        /// <returns>
        /// A list of.
        /// </returns>
        private static Tuple<string, string, List<string>> ParseConverter(
            string converterString,
            int startIndex,
            int endIndex)
        {
            int indexOfBrace = converterString.IndexOf('(', startIndex, endIndex - startIndex);
            List<string> arguments = null;

            if (indexOfBrace < 0 || indexOfBrace > endIndex)
            {
                indexOfBrace = endIndex;
            }
            else if (converterString[endIndex - 1] != ')')
            {
                throw new ArgumentException(
                    string.Format(
                        "Converter format not correct: {0}",
                        converterString.Substring(startIndex, endIndex - startIndex)));
            }
            else
            {
                arguments = ParseConverterArguments(
                    converterString,
                    indexOfBrace + 1,
                    endIndex - 1);
            }

            int lastIndexOfDot = converterString.LastIndexOf('.', indexOfBrace, indexOfBrace - startIndex);

            if (lastIndexOfDot > 0)
            {
                return Tuple.Create(
                    converterString.Substring(startIndex, lastIndexOfDot - startIndex),
                    converterString.Substring(lastIndexOfDot + 1, indexOfBrace - lastIndexOfDot - 1),
                    arguments);
            }

            return Tuple.Create(
                converterString.Substring(startIndex, indexOfBrace - startIndex),
                (string)null,
                arguments);
        }

        /// <summary>
        /// Parse converter arguments.
        /// </summary>
        /// <remarks>
        /// Arguments list
        /// 1. number,
        /// 2. quoted string (string),
        /// 3. Enum
        /// 4. TypeMember as enum,
        /// 5. Const variable,
        /// 6. Property binding to static field/property
        /// </remarks>
        /// <param name="converterArgs"> The converter arguments. </param>
        /// <param name="startIndex">    The start index. </param>
        /// <param name="endIndex">      The end index. </param>
        /// <returns>
        /// .
        /// </returns>
        private static List<string> ParseConverterArguments(
            string converterArgs,
            int startIndex,
            int endIndex)
        {
            string[] splits = converterArgs.Split(',');
            List<string> rv = new List<string>(splits.Length);
            for (int i = 0; i < splits.Length; i++)
            {
                rv.Add(splits[i].Trim());
            }

            return rv;
        }

        /// <summary>
        /// Gets matching brace index.
        /// </summary>
        /// <param name="path">       Full pathname of the file. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <returns>
        /// The matching brace index.
        /// </returns>
        private static int GetMatchingBraceIndex(string path, int startIndex, int endIndex)
        {
            int loopedBraces = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (path[i] == ')')
                {
                    if (--loopedBraces == 0)
                    {
                        return i;
                    }
                }
                else if (path[i] == '(')
                {
                    loopedBraces++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Investigate brace.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="path">       Full pathname of the file. </param>
        /// <param name="startIndex"> The start index. </param>
        /// <param name="endIndex">   The end index. </param>
        /// <returns>
        /// .
        /// </returns>
        private static BraceScopeType InvestigateBrace(string path, int startIndex, int endIndex)
        {
            int matchingBrace = GetMatchingBraceIndex(path, startIndex, endIndex);
            if (matchingBrace < 0)
            {
                throw new ApplicationException(
                    string.Format(
                        BindingParser.propertyFormatErrorBecauseOfBrace,
                        path));
            }

            if (matchingBrace == endIndex - 1)
            {
                return BraceScopeType.None;
            }

            if (path[matchingBrace + 1] == '.')
            {
                return BraceScopeType.Property;
            }

            return BraceScopeType.Cast;
        }
    }
}
