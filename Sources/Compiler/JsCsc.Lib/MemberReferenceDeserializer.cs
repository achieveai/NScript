//-----------------------------------------------------------------------
// <copyright file="MemberReferenceDeserializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using JsCsc.Lib.Serialization;
    using Mono.Cecil;
    using Newtonsoft.Json.Linq;
    using NScript.CLR;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MemberReferenceDeserializer
    /// </summary>
    public class MemberReferenceDeserializer
    {
        private ClrContext _context;
        private Dictionary<string, TypeReference> _systemTypes;
        private MethodDefinition methodContext = null;
        private Dictionary<string, GenericParameter> _methodContextTypeParams;
        private Dictionary<string, GenericParameter> _activeContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberReferenceDeserializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MemberReferenceDeserializer(
            ClrContext context)
        {
            this._context = context;
            this._systemTypes = this.CreateKnownDefinitionsMap();
        }

        /// <summary>
        /// Sets the method context.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        public void SetMethodContext(MethodDefinition methodDefinition)
        {
            this.methodContext = methodDefinition;
            this._methodContextTypeParams =
                this.methodContext != null
                    ? this.GetTypeNameMaps(methodDefinition)
                    : null;

            this._activeContext = this._methodContextTypeParams;
        }

        /// <summary>
        ///     Deserializes the method.
        /// </summary>
        /// <param name="jObject"> The j object. </param>
        public MethodReference DeserializeMethod(JObject jObject)
        {
            var declaringType = this.DeserializeType(
                jObject.Value<JObject>(NameTokens.DeclaringType));

            this._activeContext = this.GetTypeNameMaps(declaringType);

            string name = jObject.Value<string>(NameTokens.Name);
            int arity = jObject.Value<int>(NameTokens.Arity);

            var returningType = this.DeserializeType(
                jObject.Value<JObject>(NameTokens.ReturnType));

            if (returningType.HasGenericParameters && returningType is TypeDefinition)
            {
                // Returning type can't be TypeDefinition if typeDefinition is generic, it has to
                // be TypeReference.
                var genericInstanceType = new GenericInstanceType(returningType);
                for (int iArity = 0; iArity < returningType.GenericParameters.Count; iArity++)
                {
                    genericInstanceType.GenericArguments.Add(returningType.GenericParameters[iArity]);
                }

                returningType = genericInstanceType;
            }

            MethodReference rv = new MethodReference(
                name,
                returningType,
                declaringType);

            MethodReference rvDef = new MethodReference(
                name,
                returningType,
                declaringType.Resolve());

            JArray argsArray = jObject.Value<JArray>(NameTokens.Parameters);
            for (int iParam = 0; iParam < argsArray.Count; iParam++)
            {
                JObject paramObj = argsArray.Value<JObject>(iParam);

                ParameterAttributes attr = (ParameterAttributes)
                    Enum.Parse(
                        typeof(ParameterAttributes),
                        paramObj.Value<string>(NameTokens.ModFlags),
                        true);

                TypeReference argType =
                        this.DeserializeType(
                            paramObj.Value<JObject>(NameTokens.Type));

                if ((attr & ParameterAttributes.Out) != 0
                    || (attr & ParameterAttributes.Retval) != 0)
                {
                    argType = new ByReferenceType(argType);
                }

                rv.Parameters.Add(
                    new ParameterDefinition(
                        paramObj.Value<string>(NameTokens.Name),
                        attr,
                        argType));

                rvDef.Parameters.Add(
                    new ParameterDefinition(
                        paramObj.Value<string>(NameTokens.Name),
                        attr,
                        argType));
            }

            this._activeContext = this._methodContextTypeParams;

            JArray typeArgsArray = jObject.Value<JArray>(NameTokens.TypeParams);
            if (arity > 0)
            {
                for (int iArity = 0; iArity < arity; iArity++)
                {
                    rvDef.GenericParameters.Add(
                        new GenericParameter(
                            iArity,
                            GenericParameterType.Method,
                            rv.Module));
                }

                // Now let's fix both the generic parameters and argument types so that
                // generic parameters have property owners.
                MethodDefinition tmpMethodDefinition = rvDef.Resolve();
                this._activeContext = this.GetTypeNameMaps(tmpMethodDefinition);

                rv = new MethodReference(name, declaringType);

                for (int iArity = 0; iArity < arity; iArity++)
                {
                    var genericParam =
                        new GenericParameter(
                            tmpMethodDefinition.GenericParameters[iArity].Name,
                            rv);

                    rv.GenericParameters.Add(genericParam);

                    this._activeContext[genericParam.Name] = genericParam;
                }

                returningType = this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.ReturnType));

                rv.ReturnType = returningType;

                for (int iParam = 0; iParam < argsArray.Count; iParam++)
                {
                    JObject paramObj = argsArray.Value<JObject>(iParam);

                    ParameterAttributes attr = (ParameterAttributes)
                        Enum.Parse(
                            typeof(ParameterAttributes),
                            paramObj.Value<string>(NameTokens.ModFlags),
                            true);

                    TypeReference argType =
                            this.DeserializeType(
                                paramObj.Value<JObject>(NameTokens.Type));

                    if (attr == ParameterAttributes.Out
                        || attr == ParameterAttributes.Retval)
                    {
                        argType = new ByReferenceType(argType);
                    }

                    rv.Parameters.Add(
                        new ParameterDefinition(
                            paramObj.Value<string>(NameTokens.Name),
                            attr,
                            argType));
                }

                this._activeContext = this._methodContextTypeParams;
            }

            MethodDefinition methodDefinition = rv.Resolve();
            rv.HasThis = methodDefinition.HasThis;
            rv.ExplicitThis = methodDefinition.ExplicitThis;

            if (typeArgsArray != null)
            {
                GenericInstanceMethod genericMethod = new GenericInstanceMethod(rv);

                for (int iParam = 0; iParam < typeArgsArray.Count; iParam++)
                {
                    genericMethod.GenericArguments.Add(
                        this.DeserializeType(
                            typeArgsArray.Value<JObject>(iParam)));
                }

                rv = genericMethod;
            }

            return rv;
        }

        /// <summary>
        ///     Deserializes the method.
        /// </summary>
        /// <param name="methodSpec"> Information describing the method. </param>
        public MethodReference DeserializeMethod(Serialization.MethodSpecSer methodSpec)
        {
            var declaringType = this.DeserializeType(methodSpec.DeclaringType);

            this._activeContext = this.GetTypeNameMaps(declaringType);

            string name = methodSpec.Name;
            int arity = methodSpec.Arity;

            var returningType = this.DeserializeType(methodSpec.ReturnType);

            if (returningType.HasGenericParameters && returningType is TypeDefinition)
            {
                // Returning type can't be TypeDefinition if typeDefinition is generic, it has to
                // be TypeReference.
                var genericInstanceType = new GenericInstanceType(returningType);
                for (int iArity = 0; iArity < returningType.GenericParameters.Count; iArity++)
                {
                    genericInstanceType.GenericArguments.Add(returningType.GenericParameters[iArity]);
                }

                returningType = genericInstanceType;
            }

            returningType = this.ApplyInitOnlyModifier(returningType, methodSpec.IsInitOnly);

            MethodReference rv = new MethodReference(
                name,
                returningType,
                declaringType);

            MethodReference rvDef = new MethodReference(
                name,
                returningType,
                declaringType.Resolve());

            var argsArray = methodSpec.Parameters;
            for (int iParam = 0; argsArray != null && iParam < argsArray.Count; iParam++)
            {
                var paramObj = argsArray[iParam];
                ParameterAttributes attr = (ParameterAttributes)paramObj.ModFlags;
                TypeReference argType = this.DeserializeType(paramObj.ParamType);
                if ((attr & ParameterAttributes.Out) != 0
                    || (attr & ParameterAttributes.Retval) != 0)
                { argType = new ByReferenceType(argType); }

                rv.Parameters.Add(
                    new ParameterDefinition(
                        paramObj.Name,
                        attr,
                        argType));

                rvDef.Parameters.Add(
                    new ParameterDefinition(
                        paramObj.Name,
                        attr,
                        argType));
            }

            this._activeContext = this._methodContextTypeParams;

            var typeArgsArray = methodSpec.TypeArgs;
            if (arity > 0)
            {
                for (int iArity = 0; iArity < arity; iArity++)
                {
                    rvDef.GenericParameters.Add(
                        new GenericParameter(
                            iArity,
                            GenericParameterType.Method,
                            rv.Module));
                }

                // Now let's fix both the generic parameters and argument types so that
                // generic parameters have property owners.
                MethodDefinition tmpMethodDefinition = this.ResolveOrThrow(rvDef, name, declaringType);
                this._activeContext = this.GetTypeNameMaps(tmpMethodDefinition);

                returningType = this.DeserializeType(methodSpec.ReturnType);
                returningType = this.ApplyInitOnlyModifier(returningType, methodSpec.IsInitOnly);
                rv = new MethodReference(name, returningType, declaringType);

                for (int iArity = 0; iArity < arity; iArity++)
                {
                    var genericParam =
                        new GenericParameter(
                            tmpMethodDefinition.GenericParameters[iArity].Name,
                            rv);

                    rv.GenericParameters.Add(genericParam);
                    this._activeContext[genericParam.Name] = genericParam;
                }

                for (int iParam = 0; argsArray != null && iParam < argsArray.Count; iParam++)
                {
                    var paramObj = argsArray[iParam];
                    ParameterAttributes attr = (ParameterAttributes)paramObj.ModFlags;
                    TypeReference argType = this.DeserializeType(paramObj.ParamType);
                    if (attr == ParameterAttributes.Out
                        || attr == ParameterAttributes.Retval)
                    { argType = new ByReferenceType(argType); }

                    rv.Parameters.Add(
                        new ParameterDefinition(
                            paramObj.Name,
                            attr,
                            argType));
                }

                this._activeContext = this._methodContextTypeParams;
            }

            MethodDefinition methodDefinition = this.ResolveOrThrow(rv, name, declaringType);
            rv.HasThis = methodDefinition.HasThis;
            rv.ExplicitThis = methodDefinition.ExplicitThis;

            if (typeArgsArray != null)
            {
                GenericInstanceMethod genericMethod = new GenericInstanceMethod(rv);

                for (int iParam = 0; iParam < typeArgsArray.Count; iParam++)
                { genericMethod.GenericArguments.Add(this.DeserializeType(typeArgsArray[iParam])); }

                rv = genericMethod;
            }

            return rv;
        }

        /// <summary>
        /// Wraps <paramref name="returningType"/> in a
        /// <see cref="RequiredModifierType"/> against
        /// <c>System.Runtime.CompilerServices.IsExternalInit</c> when
        /// <paramref name="isInitOnly"/> is true and the marker type is loaded.
        ///
        /// Roslyn writes that modreq on the return type of every C# 9
        /// <c>init</c> accessor; without it Cecil's <c>MetadataResolver</c>
        /// fails to match a freshly constructed <see cref="MethodReference"/>
        /// to its <see cref="MethodDefinition"/>, which used to surface as an
        /// NRE on <c>rv.Resolve().HasThis</c> in
        /// <see cref="DeserializeMethod(Serialization.MethodSpecSer)"/>.
        ///
        /// When the marker type cannot be resolved the modreq is skipped and
        /// the relaxed-signature fallback in <see cref="ResolveOrThrow"/> picks
        /// up the slack — preferring "no modreq" over a hard failure keeps the
        /// path backward-compatible with environments where Roslyn synthesised
        /// IsExternalInit into a module the deserializer hasn't seen.
        /// </summary>
        private TypeReference ApplyInitOnlyModifier(TypeReference returningType, bool isInitOnly)
        {
            if (!isInitOnly)
            { return returningType; }

            var isExternalInit = this._context.KnownReferences.IsExternalInit;
            if (isExternalInit == null)
            { return returningType; }

            return new RequiredModifierType(isExternalInit, returningType);
        }

        /// <summary>
        /// Resolves <paramref name="methodReference"/> to a
        /// <see cref="MethodDefinition"/>, falling back to a relaxed signature
        /// match when Cecil's strict <c>MetadataResolver</c> returns null.
        ///
        /// Strict resolution compares return type and parameter types via
        /// <c>AreSame</c>, which is sensitive to <see cref="RequiredModifierType"/>
        /// and <see cref="OptionalModifierType"/> wrappers (and a few other
        /// signature-shape details). Roslyn-synthesised members for new C#
        /// features periodically introduce modreqs the serializer has not yet
        /// learned to round-trip — historically this produced a silent NRE
        /// at the next <c>methodDefinition.HasThis</c> read.
        ///
        /// The fallback walks the declaring type's methods and matches by
        /// name + arity + parameter count + element-type FullName (stripping
        /// modreq/modopt/byref/pointer wrappers from both sides). If exactly
        /// one candidate matches the relaxed signature, it wins. Zero matches
        /// or more than one match throws <see cref="InvalidOperationException"/>
        /// with the method and declaring-type names embedded so the failure is
        /// loud and traceable rather than silent.
        /// </summary>
        private MethodDefinition ResolveOrThrow(
            MethodReference methodReference,
            string name,
            TypeReference declaringType)
        {
            var methodDefinition = methodReference.Resolve();
            if (methodDefinition != null)
            { return methodDefinition; }

            return this.ResolveWithRelaxedSignature(methodReference, name, declaringType);
        }

        private MethodDefinition ResolveWithRelaxedSignature(
            MethodReference methodReference,
            string name,
            TypeReference declaringType)
        {
            TypeDefinition declaringTypeDef = declaringType != null ? declaringType.Resolve() : null;
            if (declaringTypeDef == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Failed to resolve method '{0}' — declaring type '{1}' could not be resolved.",
                        name,
                        declaringType != null ? declaringType.FullName : "<null>"));
            }

            int arity = methodReference.GenericParameters.Count;
            int paramCount = methodReference.Parameters.Count;

            MethodDefinition single = null;
            int matchCount = 0;

            foreach (var candidate in declaringTypeDef.Methods)
            {
                if (candidate.Name != name)
                { continue; }
                if (candidate.GenericParameters.Count != arity)
                { continue; }
                if (candidate.Parameters.Count != paramCount)
                { continue; }
                if (!RelaxedTypeMatches(candidate.ReturnType, methodReference.ReturnType))
                { continue; }

                bool paramsMatch = true;
                for (int i = 0; i < paramCount; i++)
                {
                    if (!RelaxedTypeMatches(candidate.Parameters[i].ParameterType, methodReference.Parameters[i].ParameterType))
                    {
                        paramsMatch = false;
                        break;
                    }
                }

                if (!paramsMatch)
                { continue; }

                matchCount++;
                single = candidate;
            }

            if (matchCount == 1)
            { return single; }

            // matchCount is always 0 or >1 here — the ==1 case returned above.
            throw new InvalidOperationException(
                string.Format(
                    "Failed to resolve method '{0}' on declaring type '{1}' ({2} relaxed matches). " +
                    "This usually indicates a Roslyn-synthesised method-reference shape the BondToAst " +
                    "deserializer does not yet round-trip (e.g. an unfamiliar modreq/modopt or a new " +
                    "synthesised member). Add the missing shape to MethodSpecSer / SymbolSerializer.",
                    name,
                    declaringTypeDef.FullName,
                    matchCount));
        }

        /// <summary>
        /// Element-type FullName equality after stripping
        /// modreq/modopt/byref/pointer wrappers from both sides. Used by the
        /// relaxed-signature fallback in
        /// <see cref="ResolveWithRelaxedSignature"/>. Exposed as
        /// <c>public static</c> so unit tests can pin the comparison shape
        /// independently of the resolver state machine.
        /// </summary>
        public static bool RelaxedTypeMatches(TypeReference a, TypeReference b)
        {
            return StripSignatureWrappers(a).FullName == StripSignatureWrappers(b).FullName;
        }

        /// <summary>
        /// Walks through every <see cref="RequiredModifierType"/>,
        /// <see cref="OptionalModifierType"/>, <see cref="ByReferenceType"/>,
        /// and <see cref="PointerType"/> wrapper around
        /// <paramref name="type"/> and returns the inner element type. Plain
        /// types and array/generic-instance shapes pass through unchanged
        /// (they carry semantic identity that should participate in matching).
        /// </summary>
        public static TypeReference StripSignatureWrappers(TypeReference type)
        {
            while (type is RequiredModifierType
                || type is OptionalModifierType
                || type is ByReferenceType
                || type is PointerType)
            {
                type = ((TypeSpecification)type).ElementType;
            }

            return type;
        }

        /// <summary>
        /// Deserializes the type.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
        public TypeReference DeserializeType(JObject jObject)
        {
            ModuleDefinition moduleDef = this.GetModuleDefinition(
                jObject.Value<JObject>(NameTokens.Module));

            if (moduleDef == null)
            {
                return this._context.KnownReferences.Void;
            }

            string name = jObject.Value<string>(NameTokens.Name);
            string typeType = jObject.Value<string>(NameTokens.Type);
            int arity = jObject.Value<int>(NameTokens.Arity);

            if (typeType == ValueTokens.GenericParam)
            {
                bool isMethodOwned = jObject.Value<bool>(NameTokens.IsMethodOwned);
                int position = jObject.Value<int>(NameTokens.Position);
                string genericParamName = jObject.Value<string>(NameTokens.Name);

                if (this._activeContext == null
                    || !this._activeContext.ContainsKey(genericParamName))
                {
                    return new GenericParameter(
                        position,
                        isMethodOwned
                            ? GenericParameterType.Method
                            : GenericParameterType.Type,
                        moduleDef);
                }
                else
                {
                    return this._activeContext[genericParamName];
                }
            }
            else if (typeType == ValueTokens.Array)
            {
                return new ArrayType(
                    this.DeserializeType(
                        jObject.Value<JObject>(NameTokens.ElementType)));
            }

            TypeReference rv = new TypeReference(
                jObject.Value<string>(NameTokens.NameSpace),
                name,
                moduleDef,
                moduleDef);

            JObject declaringTypeObj = jObject.Value<JObject>(NameTokens.DeclaringType);
            GenericInstanceType genericDeclaringType = null;
            if (declaringTypeObj != null)
            {
                TypeReference declaringType = this.DeserializeType(declaringTypeObj);
                genericDeclaringType = declaringType as GenericInstanceType;
                if (genericDeclaringType != null)
                {
                    declaringType = genericDeclaringType.ElementType;
                }

                rv.DeclaringType = declaringType;
            }

            if (arity > 0)
            {
                for (int i = 0; i < arity; i++)
                {
                    rv.GenericParameters.Add(new GenericParameter(rv));
                }

                // since this is a definition, we should resolve this type.
                rv = rv.Resolve();
            }

            if (typeType == ValueTokens.GenericInstance)
            {
                JArray typeParamArray = jObject.Value<JArray>(NameTokens.TypeParams);
                GenericInstanceType type = new GenericInstanceType(rv.Resolve());
                for (int iTypeParam = 0; iTypeParam < typeParamArray.Count; iTypeParam++)
                {
                    type.GenericArguments.Add(
                        this.DeserializeType(
                            typeParamArray.Value<JObject>(iTypeParam)));
                }

                rv = type;
            }

            if (genericDeclaringType != null)
            {
                GenericInstanceType type = rv as GenericInstanceType;
                if (type == null)
                {
                    type = new GenericInstanceType(rv.Resolve());
                }

                for (int iTypeParam = 0; iTypeParam < genericDeclaringType.GenericArguments.Count; iTypeParam++)
                {
                    type.GenericArguments.Add(
                        genericDeclaringType.GenericArguments[iTypeParam]);
                }

                rv = type;
            }

            this.FixSystemType(ref rv);

            rv.IsValueType = rv.Resolve().IsValueType;

            return rv;
        }

        /// <summary>
        ///     Deserializes the type.
        /// </summary>
        /// <param name="typeSpec"> Information describing the type. </param>
        public TypeReference DeserializeType(Serialization.TypeSpecSer typeSpec)
        {
            if (typeSpec == null) { return this._context.KnownReferences.Void; }

            ModuleDefinition moduleDef = this.GetModuleDefinition(typeSpec.Module);

            var arrayTypeSer = typeSpec as Serialization.ArrayTypeSer;
            if (arrayTypeSer != null)
            {
                return new ArrayType(
                    this.DeserializeType(
                        arrayTypeSer.ElementType));
            }

            var pointerTypeSer = typeSpec as Serialization.PointerTypeSer;
            if (pointerTypeSer != null)
            { return new PointerType(this.DeserializeType(pointerTypeSer.PointedAtType)); }

            if (moduleDef == null)
            {
                if (typeSpec.Name == "dynamic")
                {
                    return this._context.KnownReferences.Object;
                }
                else
                {
                    return this._context.KnownReferences.Void;
                }
            }

            string name = typeSpec.Name;
            int arity = typeSpec.Arity;

            var genericParamSpec = typeSpec as Serialization.GenericParamSer;
            if (genericParamSpec != null)
            {
                bool isMethodOwned = genericParamSpec.IsMethodOwned;
                int position = genericParamSpec.Position;
                string genericParamName = genericParamSpec.Name;

                if (this._activeContext == null
                    || !this._activeContext.ContainsKey(genericParamName))
                {
                    return new GenericParameter(
                        position,
                        isMethodOwned
                            ? GenericParameterType.Method
                            : GenericParameterType.Type,
                        moduleDef);
                }
                else
                {
                    return this._activeContext[genericParamName];
                }
            }
            else if (typeSpec is Serialization.ArrayTypeSer)
            {
                return new ArrayType(
                    this.DeserializeType(
                        ((Serialization.ArrayTypeSer)typeSpec).ElementType));
            }

            TypeReference rv = new TypeReference(
                typeSpec.Namespace,
                name,
                moduleDef,
                moduleDef);

            var declaringTypeObj = typeSpec.NestedParent;
            GenericInstanceType genericDeclaringType = null;
            if (declaringTypeObj != null)
            {
                TypeReference declaringType = this.DeserializeType(declaringTypeObj);
                genericDeclaringType = declaringType as GenericInstanceType;
                if (genericDeclaringType != null)
                {
                    declaringType = genericDeclaringType.ElementType;
                }

                rv.DeclaringType = declaringType;
            }

            if (arity > 0)
            {
                for (int i = 0; i < arity; i++)
                {
                    rv.GenericParameters.Add(new GenericParameter(rv));
                }

                // since this is a definition, we should resolve this type.
                rv = rv.Resolve();
            }

            var typeDef = rv.Resolve();
            if (typeSpec is Serialization.GenericInstanceTypeSer)
            {
                var genericInstanceSpec = typeSpec as Serialization.GenericInstanceTypeSer;
                var typeParamArray = genericInstanceSpec.TypeParams;
                GenericInstanceType type = new GenericInstanceType(typeDef);
                for (int iTypeParam = 0; iTypeParam < typeParamArray.Count; iTypeParam++)
                { type.GenericArguments.Add(this.DeserializeType(typeParamArray[iTypeParam])); }

                rv = type;
            }

            if (genericDeclaringType != null)
            {
                GenericInstanceType type = rv as GenericInstanceType;
                if (type == null)
                { type = new GenericInstanceType(rv.Resolve()); }

                for (int iTypeParam = 0; iTypeParam < genericDeclaringType.GenericArguments.Count; iTypeParam++)
                { type.GenericArguments.Add(genericDeclaringType.GenericArguments[iTypeParam]); }

                rv = type;
            }

            this.FixSystemType(ref rv);

            if (rv != typeDef)
            { rv.IsValueType = typeDef.IsValueType; }

            return rv;
        }

        /// <summary>
        ///     Deserializes the field.
        /// </summary>
        /// <param name="jObject"> The j object. </param>
        public FieldReference DeserializeField(JObject jObject)
        {
            var declaringType = this.DeserializeType(
                jObject.Value<JObject>(NameTokens.DeclaringType));

            this._activeContext = this.GetTypeNameMaps(declaringType);
            var memberType = this.DeserializeType(
                jObject.Value<JObject>(NameTokens.MemberType));
            this._activeContext = this._methodContextTypeParams;

            return new FieldReference(
                jObject.Value<string>(NameTokens.Name),
                memberType,
                declaringType);
        }

        /// <summary>
        ///     Deserializes the field.
        /// </summary>
        /// <param name="fieldSpecSer"> The field specifier ser. </param>
        public FieldReference DeserializeField(Serialization.FieldSpecSer fieldSpecSer)
        {
            var declaringType = this.DeserializeType(fieldSpecSer.DeclaringType);

            this._activeContext = this.GetTypeNameMaps(declaringType);
            var memberType = this.DeserializeType(fieldSpecSer.MemberType);
            this._activeContext = this._methodContextTypeParams;

            return new FieldReference(
                fieldSpecSer.Name,
                memberType,
                declaringType);
        }

        /// <summary>
        /// Gets the module definition.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
        private ModuleDefinition GetModuleDefinition(JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            ModuleDefinition rv;
            string moduleName = jObject.Value<string>(NameTokens.Name);
            if (!this._context.TryGetModuleDefinition(moduleName, out rv))
            {
                throw new InvalidOperationException(
                    string.Format(
                    "Unable to resolve assembly:{0}, are you missing assembly reference?",
                    moduleName));
            }

            return rv;
        }

        /// <summary>
        ///     Gets the module definition.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="moduleSpec"> Information describing the module. </param>
        /// <returns>
        ///     The module definition.
        /// </returns>
        private ModuleDefinition GetModuleDefinition(Serialization.ModuleSpecSer moduleSpec)
        {
            if (moduleSpec == null
                || moduleSpec.Name == null)
            { return null; }

            ModuleDefinition rv;
            string moduleName = moduleSpec.Name;
            if (!this._context.TryGetModuleDefinition(moduleName, out rv))
            {
                throw new InvalidOperationException(
                    string.Format(
                    "Unable to resolve assembly:{0}, are you missing assembly reference?",
                    moduleName));
            }

            return rv;
        }

        /// <summary>
        /// Creates the known definitions map.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, TypeReference> CreateKnownDefinitionsMap()
        {
            var knownReferences = this._context.KnownReferences;
            Dictionary<string, TypeReference> rv = new Dictionary<string, TypeReference>();

            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Void);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Char);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Byte);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.SByte);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Short);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.UShort);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Int32);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.UInt32);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Int64);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.UInt64);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Single);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Double);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Enum);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.IntPtr);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.UIntPtr);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Object);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.TypeType);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.Boolean);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.String);
            MemberReferenceDeserializer.AddToMap(rv, knownReferences.TypedReference);

            return rv;
        }

        /// <summary>
        /// Fixes the type of the system.
        /// </summary>
        /// <param name="typeRef">The type ref.</param>
        private void FixSystemType(ref TypeReference typeRef)
        {
            if (this._systemTypes.ContainsKey(typeRef.FullName))
            {
                typeRef = this._systemTypes[typeRef.FullName];
            }
        }

        /// <summary>
        /// Adds to map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="typeRef">The type ref.</param>
        private static void AddToMap(
            Dictionary<string, TypeReference> map,
            TypeReference typeRef)
        {
            map.Add(typeRef.FullName, typeRef);
        }

        private Dictionary<string, GenericParameter> GetTypeNameMaps(TypeReference typeReference)
        {
            TypeReference currentType = typeReference;
            if (currentType is GenericInstanceType)
            {
                currentType = ((GenericInstanceType)currentType).ElementType;
            }

            Dictionary<string, GenericParameter> genericParameters = null;

            if (currentType.GenericParameters.Count > 0)
            {
                genericParameters = new Dictionary<string, GenericParameter>();
                for (int iGenericParam = 0; iGenericParam < currentType.GenericParameters.Count; iGenericParam++)
                {
                    var genericParam = currentType.GenericParameters[iGenericParam];

                    genericParameters.Add(
                        genericParam.Name,
                        genericParam);
                }
            }

            return genericParameters;
        }

        public Dictionary<string, GenericParameter> GetTypeNameMaps(MethodReference methodReference)
        {
            var rv = this.GetTypeNameMaps(methodReference.DeclaringType);
            if (methodReference.GenericParameters.Count > 0)
            {
                if (rv == null) rv = new Dictionary<string, GenericParameter>();

                for (int iGenericParam = 0; iGenericParam < methodReference.GenericParameters.Count; iGenericParam++)
                {
                    var genericParam = methodReference.GenericParameters[iGenericParam];
                    rv.Add(genericParam.Name, genericParam);
                }
            }

            return rv;
        }
    }
}