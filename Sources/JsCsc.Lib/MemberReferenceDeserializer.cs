//-----------------------------------------------------------------------
// <copyright file="MemberReferenceDeserializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR;
    using Mono.Cecil;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Definition for MemberReferenceDeserializer
    /// </summary>
    public class MemberReferenceDeserializer
    {
        private ClrContext _context;
        private Dictionary<string, TypeReference> _systemTypes;
        private JObjectToCsAst _astDeserializer;
        private MethodDefinition methodContext = null;
        private Dictionary<string, GenericParameter> _methodContextTypeParams;
        private Dictionary<string, GenericParameter> _activeContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberReferenceDeserializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MemberReferenceDeserializer(
            JObjectToCsAst astDeserializer,
            ClrContext context)
        {
            this._astDeserializer = astDeserializer;
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
        /// Deserializes the method.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
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

                rv = new MethodReference();
                rv.Name = name;
                rv.DeclaringType = declaringType;

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
        /// Deserializes the field.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
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
                throw new InvalidOperationException();
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