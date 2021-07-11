//-----------------------------------------------------------------------
// <copyright file="ConverterContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using NScript.CLR;
    using NScript.CLR.AST;
    using JsCsc.Lib;
    using Mono.Cecil;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NScript.Utils;
    using FullAst = JsCsc.Lib.Serialization.FullAst;
    using System.Linq;
    using Serializer = JsCsc.Lib.Serialization.Serializer;

    /// <summary>
    /// Definition for ConverterContext.
    /// </summary>
    public class ConverterContext
    {
        /// <summary>
        /// The real name string.
        /// </summary>
        public const string RealNameStr = "realName";

        /// <summary>
        /// The function name string that owns this item.
        /// </summary>
        public const string OwnerFuncNameStr = "ownerFuncName";

        /// <summary>
        /// The generated local variable string.
        /// </summary>
        public const string GeneratedLocalVarStr = @"^CS\$[\w\d]+\$\d+$";

        /// <summary>
        /// The generated this variable string.
        /// </summary>
        public const string GeneratedThisVarStr = @"^<>[\w\d]+__this$";

        /// <summary>
        /// The generated field name regular expression string.
        /// </summary>
        public const string GeneratedFieldNameRegexStr = @"^CS\$<>[\d\w]+__(?<realName>.*)$";

        /// <summary>
        /// The generated method name regular expression string.
        /// </summary>
        public const string GeneratedMethodNameRegexStr = @"^<(?<ownerFuncName>[^>]*)>[\w\d]+__[\w\d]+$";

        /// <summary>
        /// The generated type name regular expression string.
        /// </summary>
        public const string GeneratedTypeNameRegexStr = @"^<>[\w\d]+__DisplayClass[\w\d]+$";

        /// <summary>
        /// .
        /// </summary>
        public readonly static Regex GeneratedFieldNameRegex = new Regex(
                ConverterContext.GeneratedFieldNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// .
        /// </summary>
        public readonly static Regex GeneratedMethodNameRegex = new Regex(
                ConverterContext.GeneratedMethodNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// .
        /// </summary>
        public readonly static Regex GeneratedTypeNameRegex = new Regex(
                ConverterContext.GeneratedTypeNameRegexStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// .
        /// </summary>
        public readonly static Regex GeneratedLocalVarRegex = new Regex(
                ConverterContext.GeneratedLocalVarStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// .
        /// </summary>
        public readonly static Regex GeneratedThisVarRegex = new Regex(
                ConverterContext.GeneratedThisVarStr,
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Context for the colour.
        /// </summary>
        private readonly ClrContext clrContext;

        /// <summary>
        /// The converter known references.
        /// </summary>
        private readonly ConverterKnownReferences converterKnownReferences;

        /// <summary>
        /// .
        /// </summary>
        private readonly Dictionary<MethodDefinition, Func<TopLevelBlock>> methodAstMapping
            = new Dictionary<MethodDefinition, Func<TopLevelBlock>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// .
        /// </summary>
        private readonly Dictionary<TypeDefinition, TypeKind> typeKindMapping
            = new Dictionary<TypeDefinition, TypeKind>(MemberReferenceComparer.Instance);

        /// <summary>
        /// The resource name mapping.
        /// </summary>
        private readonly Dictionary<ModuleDefinition, Dictionary<string, string>> resourceNameMapping
            = new Dictionary<ModuleDefinition, Dictionary<string, string>>();

        /// <summary>
        /// The errors.
        /// </summary>
        private readonly List<Tuple<Location, string>> errors = new List<Tuple<Location, string>>();

        /// <summary>
        /// The warnings.
        /// </summary>
        private readonly List<Tuple<Location, string>> warnings = new List<Tuple<Location, string>>();

        /// <summary>
        /// The method converter plugins.
        /// </summary>
        private readonly IList<IMethodConverterPlugin> methodConverterPlugins;

        /// <summary>
        /// The method converter plugins.
        /// </summary>
        private readonly IList<ITypeConverterPlugin> typeConverterPlugins;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clrContext"> Context for the colour. </param>
        public ConverterContext(
            ClrContext clrContext,
            IList<IMethodConverterPlugin> methodConverterPlugins = null,
            IList<ITypeConverterPlugin> typeConverterPlugins = null)
        {
            // double jsonCost = 0;
            // double bondCost = 0;
            // var stopWatch = new System.Diagnostics.Stopwatch();

            this.clrContext = clrContext;
            this.converterKnownReferences = new ConverterKnownReferences(this.clrContext);
            this.methodConverterPlugins = methodConverterPlugins ?? new List<IMethodConverterPlugin>();
            this.typeConverterPlugins = typeConverterPlugins ?? new List<ITypeConverterPlugin>();

            foreach (var module in clrContext.Modules)
            {
                JArray jsonAstArray = null;
                JObject resourceFileNameMap = null;
                FullAst fullAst = null;
                foreach (var resource in module.Resources)
                {
                    if (resource.Name == "$$JstInfo$$")
                    {
                        using (var stream = ((EmbeddedResource)resource).GetResourceStream())
                        {
                            fullAst = Serializer.Deserialize(
                                stream,
                                Serializer.SerializationKind.Json);
                        }
                    }
                    if (resource.Name == "$$BstInfo$$")
                    {
                        using (var stream = ((EmbeddedResource)resource).GetResourceStream())
                        {
                            fullAst = Serializer.Deserialize(
                                stream,
                                Serializer.SerializationKind.NetSerializer);
                        }
                    }
                    else if (resource.Name == "$$ResInfo$$")
                    {
                        EmbeddedResource embededResource = (EmbeddedResource)resource;

                        using (var stream = embededResource.GetResourceStream())
                        {
                            if (stream.Length > 0)
                            {
                                StreamReader streamReader = new StreamReader(stream);
                                string tmp = streamReader.ReadToEnd();
                                stream.Position = 0;

                                JsonTextReader reader = new JsonTextReader(streamReader);
                                resourceFileNameMap = (JObject)JObject.ReadFrom(reader);
                            }
                        }
                    }
                }

                if (jsonAstArray != null)
                {
                    // stopWatch.Restart();
                    // for (int iAst = 0; iAst < jsonAstArray.Count; iAst++)
                    // {
                    //     var tuple = toAst.ParseMethodBody(
                    //         jsonAstArray.Value<JObject>(iAst));

                    //     this.methodAstMapping.Add(tuple.Item1, tuple.Item2);
                    // }
                    // stopWatch.Stop();
                    // jsonCost += stopWatch.Elapsed.TotalSeconds;

                    // stopWatch.Restart();
                }

                if (fullAst != null)
                {
                    var bondToAst = new BondToAst(
                        fullAst.TypeInfo,
                        this.ClrContext);

                    foreach (var item in fullAst.Methods)
                    {
                        var tuple = bondToAst.ParseMethodBody(item);
                        // tupl.Item2();
                        this.methodAstMapping.Add(tuple.Item1, tuple.Item2);
                    }

                    // stopWatch.Stop();
                    // bondCost += stopWatch.Elapsed.TotalSeconds;
                }

                Dictionary<string, string> resourceNameMap = new Dictionary<string, string>();
                if (resourceFileNameMap != null)
                {
                    foreach (var item in resourceFileNameMap.Properties())
                    {
                        resourceNameMap.Add(
                            item.Name,
                            (string)item.Value);
                    }
                }

                this.resourceNameMapping.Add(
                    module,
                    resourceNameMap);
            }

            // Console.WriteLine("JsonCost: {0}, BondCost: {1}", jsonCost, bondCost);
        }

        /// <summary>
        /// Gets the CLR context.
        /// </summary>
        /// <value>
        /// The colour context.
        /// </value>
        public ClrContext ClrContext
        { get { return this.clrContext; } }

        /// <summary>
        /// Gets the CLR known references.
        /// </summary>
        /// <value>
        /// The colour known references.
        /// </value>
        public ClrKnownReferences ClrKnownReferences
        { get { return this.clrContext.KnownReferences; } }

        /// <summary>
        /// Gets the known references.
        /// </summary>
        /// <value>
        /// The known references.
        /// </value>
        public ConverterKnownReferences KnownReferences
        { get { return this.converterKnownReferences; } }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IList<Tuple<Location, string>> Errors
        { get { return this.errors; } }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public IList<Tuple<Location, string>> Warnings
        { get { return this.warnings; } }

        /// <summary>
        /// Gets the method converter plugins.
        /// </summary>
        /// <value>
        /// The method converter plugins.
        /// </value>
        public IList<IMethodConverterPlugin> MethodConverterPlugins
        {get { return this.methodConverterPlugins; } }

        /// <summary>
        /// Gets the type converter plugins.
        /// </summary>
        /// <value>
        /// The type converter plugins.
        /// </value>
        public IList<ITypeConverterPlugin> TypeConverterPlugins
        { get { return this.typeConverterPlugins; } }

        /// <summary>
        /// Gets resource file name.
        /// </summary>
        /// <param name="module">       The module. </param>
        /// <param name="resourceName"> Name of the resource. </param>
        /// <returns>
        /// The resource file name.
        /// </returns>
        public string GetResourceFileName(ModuleDefinition module, string resourceName)
        {
            Dictionary<string, string> dict = null;
            if (!this.resourceNameMapping.TryGetValue(module, out dict))
            {
                return null;
            }

            string rv;
            if (dict.TryGetValue(resourceName, out rv))
            {
                return rv;
            }

            return null;
        }

        /// <summary>
        /// Adds an error.
        /// </summary>
        /// <param name="location">  The location. </param>
        /// <param name="error">     The error. </param>
        /// <param name="isWarning"> true if this object is warning. </param>
        public void AddError(Location location, string error, bool isWarning)
        {
            List<Tuple<Location, string>> list = isWarning ? this.warnings : this.errors;
            list.Add(Tuple.Create(location, error));
        }

        /// <summary>
        /// Try get method ast.
        /// </summary>
        /// <param name="method">    The method. </param>
        /// <param name="rootBlock"> [out] The root block. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetMethodAst(
            MethodDefinition method,
            out ParameterBlock rootBlock)
        {
            Func<TopLevelBlock> func;
            rootBlock = null;
            if (!this.methodAstMapping.TryGetValue(method, out func))
            {
                return false;
            }

            var topLevelBlock = func != null ? func() : null;
            rootBlock = topLevelBlock != null
                ? topLevelBlock.RootBlock
                : null;
            return true;
        }

        /// <summary>
        /// Determines whether the specified type reference is Extended type.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
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
        /// <param name="typeDefinition"> The type definition base. </param>
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

            if (this.GetTypeKind(typeDefinition) == TypeKind.Extended)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
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
                    return string.Empty;
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
                nameSpace = null;
            }

            if (nameSpace != null)
            {
                returnValue = nameSpace + '.' + returnValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Ignore generic arguments.
        /// </summary>
        /// <param name="methodReference"> The method reference. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool HasGenericArguments(MethodReference methodReference)
        {
            GenericInstanceMethod genericInstanceType = methodReference as GenericInstanceMethod;
            if (genericInstanceType != null)
            {
                return this.HasGenericArguments(genericInstanceType.ElementMethod);
            }

            return this.HasGenericArguments(methodReference.Resolve());
        }

        /// <summary>
        /// Ignore generic arguments.
        /// </summary>
        /// <param name="methodDefinition"> The method definition. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool HasGenericArguments(
            MethodDefinition methodDefinition)
        {
            return methodDefinition.HasGenericParameters
                && methodDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.IgnoreGenericArgumentsAttribute) == null
                && this.IsImplemented(methodDefinition);
        }

        /// <summary>
        /// Ignore generic arguments.
        /// </summary>
        /// <param name="methodDefinition"> The method definition. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool HasGenericArguments(
            TypeDefinition typeDefinition)
        {
            return typeDefinition.HasGenericParameters
                && typeDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.IgnoreGenericArgumentsAttribute) == null;
        }

        /// <summary>
        /// Determines whether the specified type definition has ignore namespace attribute .
        /// </summary>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// <c>true</c> if [the specified type definition] [has ignore namespace attribute]; otherwise,
        /// <c>false</c>.
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
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <param name="memberDefinition"> The member definition. </param>
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
        /// <param name="fieldDef"> The field def. </param>
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
        /// <param name="methodDefinition"> The member definition. </param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(MethodDefinition methodDefinition)
        {
            if (!this.IsImplementedInternal(methodDefinition))
            {
                if (methodDefinition.IsAddOn || methodDefinition.IsRemoveOn)
                {
                    return true;
                }

                if (this.IsWrappedType(methodDefinition.ReturnType))
                {
                    return true;
                }

                for (int iArgument = 0; iArgument < methodDefinition.Parameters.Count; iArgument++)
                {
                    if (this.IsWrappedType(methodDefinition.Parameters[iArgument].ParameterType))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Query if 'memberDefinition' is wrapped.
        /// </summary>
        /// <param name="memberDefinition"> The member definition. </param>
        /// <returns>
        /// true if wrapped, false if not.
        /// </returns>
        public bool IsWrapped(MethodDefinition memberDefinition)
        {
            return !this.IsImplementedInternal(memberDefinition)
                && this.IsImplemented(memberDefinition);
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
            return methodDefinition != null
                && (!methodDefinition.HasBody || methodDefinition.Body.Instructions.Count == 0)
                && !methodDefinition.DeclaringType.IsInterface
                && !methodDefinition.IsAbstract
                && null == methodDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.ScriptAttribute);
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="propDef"> The prop def. </param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(PropertyDefinition propDef)
        {
            if (this.GetTypeKind(propDef.DeclaringType) == TypeKind.Normal)
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
        /// Query if 'propDef' is wrapped.
        /// </summary>
        /// <param name="propDef"> The property def. </param>
        /// <returns>
        /// true if wrapped, false if not.
        /// </returns>
        public bool IsWrapped(PropertyDefinition propDef)
        {
            if (this.GetTypeKind(propDef.DeclaringType) == TypeKind.Normal)
            {
                return true;
            }

            if (propDef.GetMethod != null)
            {
                return this.IsWrapped(propDef.GetMethod);
            }

            return this.IsWrapped(propDef.SetMethod);
        }

        /// <summary>
        /// Determines whether the specified type definition is anonymous delegate wrapper.
        /// </summary>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// <c>true</c> if the specified type definition is anonymous delegate wrapper; otherwise,
        /// <c>false</c>.
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
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <param name="memberDefinition">  The member definition. </param>
        /// <param name="resolveUnderlying"> true to resolve underlying. </param>
        /// <param name="isFixedName">       [out] if set to <c>true</c> [is fixed name]. </param>
        /// <param name="isAlias">           [out] The is alias. </param>
        /// <returns>
        /// Member's name.
        /// </returns>
        public string GetMemberName(
            IMemberDefinition memberDefinition,
            bool resolveUnderlying,
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
            else if ((attribute = memberDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.PreserveCaseAttribute)) != null)
            {
                isFixedName = true;
                return memberDefinition.Name;
            }
            else if (memberDefinition.CustomAttributes.SelectAttribute(
                this.KnownReferences.PreserveNameAttribute) != null)
            {
                isFixedName = true;
                return char.ToLowerInvariant(memberDefinition.Name[0])
                    + memberDefinition.Name.Substring(1);
            }
            else if (
                (!this.IsImplemented(memberDefinition)
                    || (resolveUnderlying
                        && ((methodDefinition != null && this.IsWrapped(methodDefinition))
                            || (propertyDefinition != null && this.IsWrapped(propertyDefinition)))))
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
                    if ((attribute = propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.ScriptAliasAttribute)) != null)
                    {
                        isAlias = true;
                        return (string)attribute.ConstructorArguments[0].Value;
                    }
                    else if ((attribute = propertyDefinition.CustomAttributes.SelectAttribute(
                            this.KnownReferences.ScriptNameAttribute)) != null)
                    {
                        isFixedName = true;
                        return (string)attribute.ConstructorArguments[0].Value;
                    }
                    else if ((attribute = propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.PreserveCaseAttribute)) != null)
                    {
                        isFixedName = true;
                        return propertyDefinition.Name;
                    }
                    else if (propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.PreserveNameAttribute) != null)
                    {
                        isFixedName = true;
                        return char.ToLowerInvariant(propertyDefinition.Name[0])
                            + propertyDefinition.Name.Substring(1);
                    }

                    if (propertyDefinition.DeclaringType.FullName == "System.Number")
                    {
                        // All properties of Number are AS IS properties and don't change the names.
                        name = propertyDefinition.Name;
                    }

                    // if (this.IsIntrinsicProperty(propertyDefinition))
                    // {
                        string rv = propertyDefinition.Name;
                        name = char.ToLowerInvariant(rv[0]) + rv.Substring(1);
                    // }
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
        /// Query if 'memberDefinition' is renamed.
        /// </summary>
        /// <param name="memberDefinition"> The member definition. </param>
        /// <returns>
        /// true if renamed, false if not.
        /// </returns>
        public bool IsRenamed(
            IMemberDefinition memberDefinition)
        {
            return memberDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.ScriptNameAttribute) != null;
        }

        /// <summary>
        /// Determines whether the specified property definition is intrinsic property.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// <c>true</c> if [is intrinsic property] [the specified property definition]; otherwise,
        /// <c>false</c>.
        /// </returns>
        public bool IsIntrinsicProperty(PropertyDefinition propertyDefinition)
        {
            return (propertyDefinition.GetMethod == null
                    || (this.IsExtern(propertyDefinition.GetMethod)
                        && !this.IsWrapped(propertyDefinition.GetMethod)
                        && !this.IsRenamed(propertyDefinition.GetMethod)))
                && (propertyDefinition.SetMethod == null
                    || (this.IsExtern(propertyDefinition.SetMethod)
                        && !this.IsWrapped(propertyDefinition.SetMethod)
                        && !this.IsRenamed(propertyDefinition.SetMethod)));

            /*
            return null != propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.IntrinsicPropertyAttribute)
                || null != propertyDefinition.CustomAttributes.SelectAttribute(
                        this.KnownReferences.ScriptAliasAttribute);
            */
        }

        /// <summary>
        /// Query if 'evt' is intrinsic event.
        /// </summary>
        /// <param name="evt"> The event. </param>
        /// <returns>
        /// true if intrinsic event, false if not.
        /// </returns>
        public bool IsIntrinsicEvent(EventDefinition evt)
        {
            return (evt.AddMethod != null && this.IsExtern(evt.AddMethod))
                || (evt.RemoveMethod != null && this.IsExtern(evt.RemoveMethod));
        }

        /// <summary>
        /// Query if 'typeDefinition' is psudo type.
        /// </summary>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// true if psudo type, false if not.
        /// </returns>
        public bool IsPsudoType(TypeDefinition typeDefinition)
        {
            TypeKind typeKind = this.GetTypeKind(typeDefinition);
            return typeKind == TypeKind.Imported || typeKind == TypeKind.JSONType;
        }

        /// <summary>
        /// Query if 'typeDefinition' is json type.
        /// </summary>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// true if json type, false if not.
        /// </returns>
        public bool IsJsonType(TypeDefinition typeDefinition)
        {
            return this.GetTypeKind(typeDefinition) == TypeKind.JSONType;
        }

        /// <summary>
        /// Query if 'typeDefinition' is imported type.
        /// </summary>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// true if imported type, false if not.
        /// </returns>
        public bool IsImportedType(TypeDefinition typeDefinition)
        {
            return this.GetTypeKind(typeDefinition) == TypeKind.Imported;
        }

        /// <summary>
        /// Query if 'propertyDefinition' is compiler generated property.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// true if compiler generated property, false if not.
        /// </returns>
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
        /// Gets a type kind.
        /// </summary>
        /// <exception cref="InvalidProgramException"> Thrown when an invalid program error condition
        ///     occurs. </exception>
        /// <exception cref="InvalidDataException">    Thrown when an invalid data error condition
        ///     occurs. </exception>
        /// <param name="typeDefinition"> The type definition. </param>
        /// <returns>
        /// The type kind.
        /// </returns>
        public TypeKind GetTypeKind(TypeDefinition typeDefinition)
        {
            TypeKind rv;
            if (this.typeKindMapping.TryGetValue(typeDefinition, out rv))
            {
                return rv;
            }

            TypeDefinition baseType = null;
            TypeKind baseKind = TypeKind.Normal;

            if (typeDefinition.BaseType != null)
            {
                baseType = typeDefinition.GetBaseType().Resolve();
                baseKind = this.GetTypeKind(baseType);
            }

            // Delegate is one of the few types which have extern method that is native to CLR.
            if (typeDefinition.IsDelegate())
            {
                rv = TypeKind.Normal;
            }
            else if (typeDefinition.IsInterface
                && (null != typeDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.PsudoTypeAttribute)
                || null != typeDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.JsonTypeAttribute)))
            {
                //  rv = TypeKind.Imported;
                throw new InvalidProgramException("Imported interfaces yet to be supported");
            }
            else
            {
                bool hasConstructor = false;
                bool hasStaticConstructor = false;
                bool implmentedConstructor = false;
                bool hasExternMethod = false;
                bool hasImplementedVirtual = false;
                bool hasFields = typeDefinition.HasFields;
                bool hasPublicFields = false;
                bool hasNonNullableStructField = false;
                bool hasNonPropertyMethods = false;
                bool hasVirtualMethods = false;
                bool hasNoMethods = true;

                foreach (var method in typeDefinition.Methods)
                {
                    if (method.IsConstructor)
                    {
                        if (method.IsStatic)
                        {
                            hasStaticConstructor = true;
                        }
                        // TODO: move below logic into it's own function so that it can be used
                        // to simplify constructor code.
                        // method.Body == null for static type.
                        // method.Body.CodeSize == 0 for extern constructor.
                        else if (
                            method.Body != null
                            && (method.Body.Instructions
                                    .Where(_ => _.OpCode.Code != Mono.Cecil.Cil.Code.Nop)
                                    .Count() != 3
                                || typeDefinition.IsValueOrEnum()))
                        {
                            hasConstructor = true;
                            bool isExtern = this.IsExtern(method);
                            implmentedConstructor = implmentedConstructor || !isExtern;
                            hasExternMethod = isExtern || hasExternMethod;
                        }
                    }
                    else
                    {
                        hasNoMethods = false;
                        bool isExtern = this.IsExtern(method);

                        // Let's skip extern methods that are just ScriptAliased, since they do not require TypeDefinition name.
                        if (isExtern && null != method.CustomAttributes.SelectAttribute(this.KnownReferences.ScriptAliasAttribute))
                        { continue; }

                        hasExternMethod = isExtern || hasExternMethod;
                        if (!isExtern && !method.IsAbstract && method.IsVirtual)
                        {
                            hasImplementedVirtual = true;
                        }

                        if (isExtern && !method.IsGetter && !method.IsSetter)
                        {
                            hasNonPropertyMethods = true;
                        }

                        if ((method.IsVirtual || method.IsAbstract) && !method.IsFinal)
                        {
                            hasVirtualMethods = true;
                        }
                    }
                }

                foreach (var field in typeDefinition.Fields)
                {
                    if (!field.IsPrivate
                        && field.Constant == null)
                    {
                        hasPublicFields = true;
                    }

                    if (field.FieldType.IsValueOrEnum()
                        && !field.FieldType.IsSameDefinition(this.ClrKnownReferences.NullableType))
                    {
                        hasNonNullableStructField = true;
                    }
                }

                if (null != typeDefinition.CustomAttributes.SelectAttribute(
                    this.KnownReferences.ExtendedAttribute))
                {
                    if (baseType != null
                        && baseKind != TypeKind.Extended
                        && baseKind != TypeKind.Imported
                        && !typeDefinition.IsValueOrEnum())
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "BaseType:'{0}' of ExtendedType: '{1}' should also be ExtendedType or ImportedType.",
                                baseType,
                                typeDefinition));
                    }

                    rv = TypeKind.Extended;
                }
                else if (hasExternMethod)
                {
                    // This is candidate for Imported or JSONType
                    // It's an error to have base class that is not either Imported, JSONType or Extended

                    if (baseType != null
                        && baseKind != TypeKind.Imported
                        && baseKind != TypeKind.Extended
                        && baseKind != TypeKind.JSONType
                        && !typeDefinition.IsValueOrEnum())
                    {
                        throw new ApplicationException(
                            string.Format(
                                "BaseType: '{0}' of Imported/JSONType Type: '{1}' should be Imported/JSONType/Extended",
                                baseType,
                                typeDefinition));
                    }

                    if (hasImplementedVirtual)
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Imported Type: '{0}' can not have virtual method implementation. Only instance Methods/Properties/Fields are allowed",
                                typeDefinition));
                    }

                    if (hasVirtualMethods)
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Imported Type: '{0}' can not have virtual methods, virtuals are not really needed.",
                                typeDefinition));
                    }

                    if (hasPublicFields)
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Imported Type: '{0}' can not have fields, fields not yet supported for imported types (this may also because of compiler generated Properties or Events, please use extern in those cases).",
                                typeDefinition));
                    }

                    if (hasNonNullableStructField)
                    {
                        throw new ApplicationException(
                            string.Format(
                                "Imported Type: '{0}' can not have member field of struct type that is not Nullable",
                                typeDefinition));
                    }

                    if (!hasNonPropertyMethods
                        && !hasConstructor
                        && baseType != null
                        && (baseKind == TypeKind.JSONType
                            || baseType.IsSameDefinition(this.ClrKnownReferences.Object)))
                    {
                        rv = TypeKind.JSONType;
                    }
                    else
                    {
                        rv = TypeKind.Imported;
                    }
                }

                if (null != typeDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.ImportedTypeAttribute)
                    && rv != TypeKind.Imported)
                {
                    if (!hasExternMethod)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Imported Type: '{0}' does not have any extern member",
                                typeDefinition));
                    }
                    else if (rv == TypeKind.Extended)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Type: '{0}' can't be both Extended Type and Imported Type as the same time. Only use Extended type as is supersedes ImportedType",
                                typeDefinition));
                    }
                    else if (rv == TypeKind.JSONType)
                    {
                        rv = TypeKind.Imported;
                    }
                }

                if (null != typeDefinition.CustomAttributes.SelectAttribute(this.KnownReferences.JsonTypeAttribute)
                    && rv != TypeKind.JSONType)
                {
                    if (!hasExternMethod && !hasNoMethods)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Imported Type: '{0}' does not have any extern member",
                                typeDefinition));
                    }
                    else if (rv == TypeKind.Extended)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Type: '{0}' can't be both Extended Type and JSON Type as the same time. Only use Extended type as is supersedes JSON type",
                                typeDefinition));
                    }
                    else if (rv == TypeKind.Imported)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Type: '{0}' can't be both Imported Type and JSON Type as the same time. Only use Imported type as is supersedes JSON type",
                                typeDefinition));
                    }
                    else if (hasNonPropertyMethods)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "JSON Type: '{0}' can't have extern Methods or Events.",
                                typeDefinition));
                    }
                    else if (hasConstructor)
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "JSON Type: '{0}' can't have a constructor",
                                typeDefinition));
                    }
                    else if (baseType == null
                            || (baseKind != TypeKind.JSONType && !baseType.IsSameDefinition(this.ClrKnownReferences.Object)))
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "JSON Type: '{0}' can only be derived from other JSON types.",
                                typeDefinition));
                    }
                    else
                    {
                        rv = TypeKind.JSONType;
                    }
                }

                if (rv != TypeKind.Normal)
                {
                    if (implmentedConstructor
                        || (rv != TypeKind.Extended && hasStaticConstructor))
                    {
                        throw new InvalidDataException(
                            string.Format(
                                "Imported or Extended Type: '{0}' can not have constructor implementation",
                                typeDefinition));
                    }
                }
            }

            this.typeKindMapping.Add(typeDefinition, rv);

            return rv;
        }

        /// <summary>
        /// Determines whether the specified STR is caps name.
        /// </summary>
        /// <param name="str"> The STR. </param>
        /// <returns>
        /// <c>true</c> if the specified STR is caps name ; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsCapsName(string str)
        {
            return str.ToUpperInvariant() == str;
        }

        /// <summary>
        /// Query if 'typeReference' is wrapped type.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <returns>
        /// true if wrapped type, false if not.
        /// </returns>
        public bool IsWrappedType(TypeReference typeReference)
        {
            if (typeReference is ArrayType)
            {
                return true;
            }

            GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
            if (genericInstanceType != null
                && this.KnownReferences.ListGeneric.IsSameDefinition(genericInstanceType.ElementType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified member definition is implemented.
        /// </summary>
        /// <param name="memberDefinition"> The member definition. </param>
        /// <returns>
        /// <c>true</c> if the specified member definition is implemented; otherwise, <c>false</c>.
        /// </returns>
        private bool IsImplementedInternal(MethodDefinition memberDefinition)
        {
            MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
            if (methodDefinition != null)
            {
                return (!this.IsExtern(methodDefinition) && !memberDefinition.IsAbstract)
                    || (methodDefinition.DeclaringType.IsInterface && !this.IsPsudoType(methodDefinition.DeclaringType));
            }

            return false;
        }

        /// <summary>
        /// Values that represent TypeKind.
        /// </summary>
        public enum TypeKind
        {
            /// <summary>
            /// .
            /// </summary>
            Normal,
            /// <summary>
            /// .
            /// </summary>
            Interface,
            /// <summary>
            /// .
            /// </summary>
            Extended,
            /// <summary>
            /// .
            /// </summary>
            Imported,
            /// <summary>
            /// .
            /// </summary>
            JSONType
        }
    }
}