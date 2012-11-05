//-----------------------------------------------------------------------
// <copyright file="ConverterContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text.RegularExpressions;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using JsCsc.Lib;
    using Mono.Cecil;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Definition for ConverterContext
    /// </summary>
    public class ConverterContext
    {
        public const string RealNameStr = "realName";
        public const string OwnerFuncNameStr = "ownerFuncName";
        public const string GeneratedLocalVarStr = @"^CS\$[\w\d]+\$\d+$";
        public const string GeneratedThisVarStr = @"^<>[\w\d]+__this$";
        public const string GeneratedFieldNameRegexStr = @"^CS\$<>[\d\w]+__(?<realName>.*)$";
        public const string GeneratedMethodNameRegexStr = @"^<(?<ownerFuncName>[^>]*)>[\w\d]+__[\w\d]+$";
        public const string GeneratedTypeNameRegexStr = @"^<>[\w\d]+__DisplayClass[\w\d]+$";

        public readonly static Regex GeneratedFieldNameRegex = new Regex(
                ConverterContext.GeneratedFieldNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly static Regex GeneratedMethodNameRegex = new Regex(
                ConverterContext.GeneratedMethodNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly static Regex GeneratedTypeNameRegex = new Regex(
                ConverterContext.GeneratedTypeNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly static Regex GeneratedLocalVarRegex = new Regex(
                ConverterContext.GeneratedLocalVarStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public readonly static Regex GeneratedThisVarRegex = new Regex(
                ConverterContext.GeneratedThisVarStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ClrContext clrContext;
        private readonly ConverterKnownReferences converterKnownReferences;

        private readonly Dictionary<MethodDefinition, TopLevelBlock> methodAstMapping
            = new Dictionary<MethodDefinition, TopLevelBlock>(MemberReferenceComparer.Instance);

        public ConverterContext(ClrContext clrContext)
        {
            this.clrContext = clrContext;
            this.converterKnownReferences = new ConverterKnownReferences(this.clrContext);

            JObjectToCsAst toAst = new JObjectToCsAst(clrContext);
            foreach (var module in clrContext.Modules)
            {
                JArray jsonAstArray = null;
                foreach (var resource in module.Resources)
                {
                    if (resource.Name == "$$AstInfo$$")
                    {
                        EmbeddedResource embededResource = (EmbeddedResource)resource;

                        using (var stream = embededResource.GetResourceStream())
                        using (var unzipStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            JsonTextReader reader = new JsonTextReader(new StreamReader(unzipStream));
                            jsonAstArray = JArray.Load(reader);
                        }

                        break;
                    }
                }

                if (jsonAstArray != null)
                {
                    for (int iAst = 0; iAst < jsonAstArray.Count; iAst++)
                    {
                        var tuple = toAst.ParseMethodBody(
                            jsonAstArray.Value<JObject>(iAst));

                        this.methodAstMapping.Add(tuple.Item1, tuple.Item2);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the CLR context.
        /// </summary>
        public ClrContext ClrContext
        { get { return this.clrContext; } }

        /// <summary>
        /// Gets the CLR known references.
        /// </summary>
        public ClrKnownReferences ClrKnownReferences
        { get { return this.clrContext.KnownReferences; } }

        /// <summary>
        /// Gets the known references.
        /// </summary>
        public ConverterKnownReferences KnownReferences
        { get { return this.converterKnownReferences; } }

        public bool TryGetMethodAst(
            MethodDefinition method,
            out ParameterBlock rootBlock)
        {
            TopLevelBlock topLevelBlock;
            rootBlock = null;
            if (!this.methodAstMapping.TryGetValue(method, out topLevelBlock))
            {
                return false;
            }

            rootBlock = topLevelBlock != null
                ? topLevelBlock.RootBlock
                : null;
            return true;
        }

        /// <summary>
        /// Determines whether the specified type reference is Extended type.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// <c>true</c> if the specified type reference is Extended type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExtended(TypeReference typeReference)
        {
            return this.IsExtended(typeReference.Resolve());
        }

        /// <summary>
        /// Determines whether the specified type definition base is Extended type.
        /// </summary>
        /// <param name="typeDefinitionBase">The type definition base.</param>
        /// <returns>
        /// <c>true</c> if the specified type definition base is Extended type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExtended(TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
            {
                // Only proper typeDefinition can be extended.
                return false;
            }

            if (null != typeDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.ExtendedAttribute))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// Type name of the given type. If ScriptName attribute is set then use it.
        /// </returns>
        public string GetTypeName(TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();
            if (typeDefinition == null)
            {
                return typeReference.Name;
            }

            bool ignoreNamespace = false;
            string nameSpace = typeReference.Namespace;

            if (typeReference.DeclaringType != null)
            {
                // This is a nested type. So namespace is name of parent type.
                nameSpace = this.GetTypeName(typeReference.DeclaringType.Resolve());
            }

            foreach (var attribute in typeDefinition.CustomAttributes)
            {
                if (attribute.AttributeType.IsSame(
                    this.KnownReferences.ScriptNameAttribute))
                {
                    return (string)attribute.ConstructorArguments[0].Value;
                }

                if (attribute.AttributeType.IsSame(
                    this.KnownReferences.IgnoreNamespaceAttribute))
                {
                    ignoreNamespace = true;
                }

                if (attribute.AttributeType.IsSame(
                    this.KnownReferences.GlobalMethodsAttribute))
                {
                    return null;
                }

                if (attribute.AttributeType.IsSame(
                    this.KnownReferences.ScriptNamespaceAttribute))
                {
                    ignoreNamespace = true;
                    nameSpace = (string)attribute.ConstructorArguments[0].Value;
                }
            }

            string returnValue = typeDefinition.Name;

            if (ignoreNamespace)
            {
                int dotIndex = returnValue.LastIndexOf('.');

                returnValue = returnValue.Substring(dotIndex + 1);
            }

            if (nameSpace != null)
            {
                returnValue = nameSpace + '.' + returnValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified type definition has ignore namespace attribute .
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>
        /// <c>true</c> if [the specified type definition] [has ignore namespace attribute]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasIgnoreNamespaceAttribute(
            TypeDefinition typeDefinition)
        {
            return typeDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.IgnoreNamespaceAttribute) != null;
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(IMemberDefinition memberDefinition)
        {
            FieldDefinition fieldDef = memberDefinition as FieldDefinition;
            if (fieldDef != null)
            { return this.IsImplemented(fieldDef); }

            MethodDefinition methodDef = memberDefinition as MethodDefinition;
            if (methodDef != null)
            { return this.IsImplemented(methodDef); }

            PropertyDefinition propDef = memberDefinition as PropertyDefinition;
            if (propDef != null)
            { return this.IsImplemented(propDef); }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="fieldDef">The field def.</param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(FieldDefinition fieldDef)
        {
            if (!this.IsExtended(fieldDef.DeclaringType))
            {
                return true;
            }

            return fieldDef.CustomAttributes.SelectAttribute(
                this.KnownReferences.ImplementAttribute) != null;
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(MethodDefinition memberDefinition)
        {
            MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
            if (methodDefinition != null)
            {
                return (methodDefinition.HasBody && methodDefinition.Body.Instructions.Count > 0)
                    || null != methodDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.ScriptAttribute)
                    || (methodDefinition.DeclaringType.IsInterface && !this.IsPsudoType(methodDefinition.DeclaringType));
            }

            return false;
        }

        /// <summary>
        /// Query if 'methodDefinition' is extern.
        /// </summary>
        /// <param name="methodDefinition"> The method definition. </param>
        /// <returns>
        /// true if extern, false if not.
        /// </returns>
        public bool IsExtern(MethodDefinition methodDefinition)
        {
            return !this.IsImplemented(methodDefinition)
                && !methodDefinition.IsAbstract;
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="propDef">The prop def.</param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(PropertyDefinition propDef)
        {
            if (!this.IsExtended(propDef.DeclaringType))
            {
                return true;
            }

            if (propDef.GetMethod != null)
            {
                return this.IsImplemented(propDef.GetMethod);
            }

            return this.IsImplemented(propDef.SetMethod);
        }

        /// <summary>
        /// Determines whether the specified type definition is anonymous delegate wrapper.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>
        /// <c>true</c> if the specified type definition is anonymous delegate wrapper; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAnonymousDelegateWrapper(TypeDefinition typeDefinition)
        {
            if (null != typeDefinition.CustomAttributes.SelectAttribute(
                    this.ClrKnownReferences.CompilerGeneratedAttribute))
            {
                return ConverterContext.GeneratedTypeNameRegex.IsMatch(typeDefinition.Name);
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <param name="isFixedName">if set to <c>true</c> [is fixed name].</param>
        /// <returns>Member's name</returns>
        public string GetMemberName(
            IMemberDefinition memberDefinition,
            out bool isFixedName,
            out bool isAlias)
        {
            if (memberDefinition is TypeDefinition)
            {
                throw new InvalidProgramException();
            }

            bool isParentExtended = this.IsExtended(memberDefinition.DeclaringType);
            isFixedName = false;
            isAlias = false;

            MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
            PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;

            if (methodDefinition != null &&
                (methodDefinition.IsGetter
                    || methodDefinition.IsSetter))
            {
                propertyDefinition = methodDefinition.GetPropertyDefinition();
            }

            string name = null;
            CustomAttribute attribute;

            if ((attribute = memberDefinition.CustomAttributes.SelectAttribute(
                    this.KnownReferences.ScriptNameAttribute)) != null)
            {
                isFixedName = true;
                return (string)attribute.ConstructorArguments[0].Value;
            }
            else if (memberDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.PreserveNameAttribute) != null)
            {
                isFixedName = true;
                return char.ToLowerInvariant(memberDefinition.Name[0])
                    + memberDefinition.Name.Substring(1);
            }
            else if ((attribute = memberDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.PreserveCaseAttribute)) != null)
            {
                isFixedName = true;
                return memberDefinition.Name;
            }
            else if (!this.IsImplemented(memberDefinition)
                && (this.IsExtended(memberDefinition.DeclaringType) || this.IsPsudoType(memberDefinition.DeclaringType)))
            {
                isFixedName = true;
                attribute = memberDefinition.CustomAttributes.SelectAttribute(
                    this.KnownReferences.ScriptAliasAttribute);

                if (attribute != null)
                {
                    isAlias = true;
                    return (string)attribute.ConstructorArguments[0].Value;
                }

                attribute = memberDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.ScriptNameAttribute);
                if (attribute != null)
                {
                    return (string)attribute.ConstructorArguments[0].Value;
                }

                if (propertyDefinition != null)
                {
                    attribute = propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.ScriptAliasAttribute);

                    if (attribute != null)
                    {
                        isAlias = true;
                        return (string)attribute.ConstructorArguments[0].Value;
                    }

                    attribute = propertyDefinition.CustomAttributes.SelectAttribute(
                            this.KnownReferences.ScriptNameAttribute);

                    if (attribute != null)
                    {
                        return (string)attribute.ConstructorArguments[0].Value;
                    }

                    if (propertyDefinition.DeclaringType.FullName == "System.Number")
                    {
                        // All properties of Number are AS IS properties and don't change the names.
                        name = propertyDefinition.Name;
                    }

                    if (this.IsIntrinsicProperty(propertyDefinition))
                    {
                        string rv = propertyDefinition.Name;
                        name = char.ToLowerInvariant(rv[0]) + rv.Substring(1);
                    }
                }
                else if (ConverterContext.IsCapsName(memberDefinition.Name))
                {
                    name = memberDefinition.Name;
                }
            }

            if (name != null)
            {
                return name;
            }

            if (propertyDefinition != null)
            {
                if (memberDefinition == propertyDefinition)
                {
                    return propertyDefinition.Name;
                }

                if (this.IsIntrinsicProperty(propertyDefinition))
                {
                    return name = char.ToLowerInvariant(propertyDefinition.Name[0]) + propertyDefinition.Name.Substring(1);
                }

                return memberDefinition.Name.Substring(0, 5).ToLowerInvariant()
                       + memberDefinition.Name.Substring(5);
            }

            if (name == null)
            {
                if (memberDefinition is FieldDefinition)
                {
                    Match match = ConverterContext.GeneratedFieldNameRegex.Match(memberDefinition.Name);
                    if (match.Success)
                    {
                        name = match.Groups[ConverterContext.RealNameStr].Value;
                    }
                    else if (ConverterContext.GeneratedThisVarRegex.IsMatch(memberDefinition.Name))
                    {
                        name = "this_";
                    }
                }
                else if (memberDefinition is MethodDefinition)
                {
                    Match match = ConverterContext.GeneratedMethodNameRegex.Match(memberDefinition.Name);
                    if (match.Success)
                    {
                        name = match.Groups[ConverterContext.OwnerFuncNameStr].Value + "_Helper";
                    }
                }

                name = name ?? memberDefinition.Name;
            }

            return char.ToLowerInvariant(name[0])
                + name.Substring(1);
        }

        /// <summary>
        /// Determines whether the specified property definition is intrinsic property.
        /// </summary>
        /// <param name="propertyDefinition">The property definition.</param>
        /// <returns>
        /// <c>true</c> if [is intrinsic property] [the specified property definition]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsIntrinsicProperty(PropertyDefinition propertyDefinition)
        {
            return (propertyDefinition.GetMethod != null && this.IsExtern(propertyDefinition.GetMethod))
                || (propertyDefinition.SetMethod != null && this.IsExtern(propertyDefinition.SetMethod));

            /*
            return null != propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.IntrinsicPropertyAttribute)
                || null != propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.ScriptAliasAttribute);
            */
        }

        public bool IsIntrinsicEvent(EventDefinition evt)
        {
            return (evt.AddMethod != null && this.IsExtern(evt.AddMethod))
                || (evt.RemoveMethod != null && this.IsExtern(evt.RemoveMethod));
        }

        public bool IsPsudoType(TypeDefinition typeDefinition)
        {
            if (null != typeDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.PsudoTypeAttribute))
            {
                return true;
            }

            return false;
        }

        public bool IsCompilerGeneratedProperty(PropertyDefinition propertyDefinition)
        {
            if (propertyDefinition.GetMethod != null &&
                propertyDefinition.SetMethod != null &&
                ((MethodDefinition)propertyDefinition.GetMethod.GetDefinition()).CustomAttributes.SelectAttribute(
                        this.ClrKnownReferences.CompilerGeneratedAttribute) != null &&
                ((MethodDefinition)propertyDefinition.SetMethod.GetDefinition()).CustomAttributes.SelectAttribute(
                        this.ClrKnownReferences.CompilerGeneratedAttribute) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified STR is caps name.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// <c>true</c> if the specified STR is caps name ; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCapsName(string str)
        {
            return str.ToUpperInvariant() == str;
        }
    }
}