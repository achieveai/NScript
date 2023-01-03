//-----------------------------------------------------------------------
// <copyright file="MemberReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR;
    using Mono.Cecil;
    using Mono.CSharp;
    using ParameterAttributes = Mono.Cecil.ParameterAttributes;

    /// <summary>
    /// Definition for MemberReferenceConverter
    /// </summary>
    public class MemberReferenceConverter
    {
        private ClrContext _context;

        private Dictionary<string, TypeReference> _systemTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberReferenceConverter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MemberReferenceConverter(ClrContext context)
        {
            this._context = context;
            this._systemTypes = this.CreateKnownDefinitionsMap();
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ClrContext Context
        { get { return this._context; } }

        /// <summary>
        /// Gets the module definition.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns></returns>
        public ModuleDefinition GetModuleDefinition(IKVM.Reflection.Module module)
        {
            ModuleDefinition rv;
            if (!this._context.TryGetModuleDefinition(module.Name, out rv))
            { throw new InvalidOperationException(); }

            return rv;
        }

        /// <summary>
        /// Gets the module definition.
        /// </summary>
        /// <param name="moduleContainer">The module container.</param>
        /// <returns></returns>
        public ModuleDefinition GetModuleDefinition(ModuleContainer moduleContainer)
        {
            ModuleDefinition rv;
            if (!this._context.TryGetModuleDefinition(moduleContainer.DeclaringAssembly.Name, out rv))
            { throw new InvalidOperationException(); }

            return rv;
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeReference GetTypeReference(TypeContainer type)
        {
            var rv = this.GetTypeReference(type.CurrentType);
            return rv;
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeReference GetTypeReference(TypeSpec type)
        {
            if (type == null)
            {
                return this.Context.KnownReferences.Void;
            }

            var metaInfo = type.GetMetaInfo();
            var moduleDefinition = this.GetModuleDefinition(metaInfo.Module);

            if (type.IsGenericParameter)
            {
                TypeParameterSpec paramSpec = (TypeParameterSpec)type;

                return new GenericParameter(
                    paramSpec.DeclaredPosition,
                    paramSpec.IsMethodOwned ? GenericParameterType.Method : GenericParameterType.Type,
                    moduleDefinition);
            }

            if (type.IsArray)
            {
                return new ArrayType(this.GetTypeReference(((ArrayContainer)type).Element));
            }

            TypeReference rv = null;

            rv = new TypeReference(
                metaInfo.Namespace,
                metaInfo.Name,
                moduleDefinition,
                moduleDefinition);

            if (metaInfo.IsNested)
            {
                rv.DeclaringType = this.GetTypeReference(
                    type.DeclaringType);
            }

            if (type is InflatedTypeSpec
                && type.IsGeneric)
            {
                GenericInstanceType genericType = new GenericInstanceType(rv);

                InflatedTypeSpec inflatedTypeSpec = (InflatedTypeSpec)type;

                for (int iTypeParam = 0; iTypeParam < inflatedTypeSpec.TypeArguments.Length; iTypeParam++)
                {
                    genericType.GenericArguments.Add(
                        this.GetTypeReference(inflatedTypeSpec.TypeArguments[iTypeParam]));
                }

                rv = genericType;
            }

            this.FixSystemType(ref rv);

            if (rv.Resolve() == null)
            {
                throw new InvalidOperationException();
            }

            return rv;
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="methodImplementation">The method implementation.</param>
        /// <returns></returns>
        public MethodReference GetMethodReference(Method methodImplementation)
        {
            return this.GetMethodReference((MethodSpec)methodImplementation.Spec);
            // var rv = new MethodReference(
            //     methodImplementation.Name,
            //     this.GetTypeReference(methodImplementation.ReturnType),
            //     this.GetTypeReference(methodImplementation.Parent));

            // for (int iParam = 0; iParam < methodImplementation.ParameterInfo.Count; iParam++)
            // {
            //     ParameterAttributes attribute = ParameterAttributes.None;
            //     var modFlags = methodImplementation.ParameterInfo.FixedParameters[iParam].ModFlags;
            //     TypeReference parameterType = this.GetTypeReference(methodImplementation.ParameterInfo.Types[iParam]);

            //     if ((modFlags & Parameter.Modifier.ISBYREF) == Parameter.Modifier.ISBYREF)
            //     {
            //         attribute |= ParameterAttributes.Retval;
            //         parameterType = new ByReferenceType(parameterType);
            //     }

            //     if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
            //     {
            //         attribute |= ParameterAttributes.Out;
            //         parameterType = new ByReferenceType(parameterType);
            //     }

            //     rv.Parameters.Add(
            //         new ParameterDefinition(
            //             methodImplementation.ParameterInfo.FixedParameters[iParam].Name,
            //             attribute,
            //             parameterType));
            // }

            // return rv;
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="methodSpec">The method spec.</param>
        /// <returns></returns>
        public MethodReference GetMethodReference(MethodSpec methodSpec)
        {
            IParametersMember parametersMember = methodSpec.MemberDefinition as IParametersMember;
            var parameters = parametersMember != null
                ? parametersMember.Parameters
                : methodSpec.Parameters;

            TypeSpec methodReturnType = parametersMember != null
                    ? parametersMember.MemberType
                    : methodSpec.ReturnType;

            var rv = new MethodReference(
                methodSpec.IsConstructor ? (methodSpec.IsStatic ? ".cctor" : ".ctor") : methodSpec.Name,
                this.GetTypeReference(methodReturnType),
                this.GetTypeReference(methodSpec.DeclaringType));

            for (int iParam = 0; iParam < parameters.Count; iParam++)
            {
                ParameterAttributes attribute = ParameterAttributes.None;
                var modFlags = parameters.FixedParameters[iParam].ModFlags;
                TypeReference parameterType = this.GetTypeReference(parameters.Types[iParam]);

                if ((modFlags & Parameter.Modifier.REF) == Parameter.Modifier.REF)
                {
                    attribute |= ParameterAttributes.Retval;
                    parameterType = new ByReferenceType(parameterType);
                }

                if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
                {
                    attribute |= ParameterAttributes.Out;
                    parameterType = new ByReferenceType(parameterType);
                }

                rv.Parameters.Add(
                    new ParameterDefinition(
                        parameters.FixedParameters[iParam].Name,
                        attribute,
                        parameterType));
            }

            if (methodSpec.IsGeneric)
            {
                for (int iArg = 0; iArg < methodSpec.Arity; iArg++)
                {
                    rv.GenericParameters.Add(
                        new GenericParameter(
                            iArg,
                            GenericParameterType.Method,
                            rv.Module));
                }

                if (methodSpec.TypeArguments != null)
                {
                    GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(rv);

                    for (int iGeneric = 0; iGeneric < methodSpec.TypeArguments.Length; iGeneric++)
                    {
                        genericInstanceMethod.GenericArguments.Add(
                            this.GetTypeReference(
                            methodSpec.TypeArguments[iGeneric]));
                    }

                    rv = genericInstanceMethod;
                }
            }

            if (rv.Resolve() == null)
            {
                throw new InvalidOperationException();
            }

            return rv;
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <param name="getterOrSetter">The getter or setter.</param>
        /// <returns></returns>
        public PropertyReference GetPropertyReference(MethodSpec getterOrSetter)
        {
            return new NScript.CLR.AST.InternalPropertyReference(
                getterOrSetter.Name.Contains("get_") ? this.GetMethodReference(getterOrSetter) : null,
                getterOrSetter.Name.Contains("set_") ? this.GetMethodReference(getterOrSetter) : null);
        }

        /// <summary>
        /// Gets the field reference.
        /// </summary>
        /// <param name="fieldSpec">The field spec.</param>
        /// <returns></returns>
        public FieldReference GetFieldReference(FieldSpec fieldSpec)
        {
            IInterfaceMemberSpec memberSpec = fieldSpec.MemberDefinition as IInterfaceMemberSpec;
            var rv = new FieldReference(
                fieldSpec.Name,
                this.GetTypeReference(
                    memberSpec != null
                        ? memberSpec.MemberType
                        : fieldSpec.MemberType),
                this.GetTypeReference(fieldSpec.DeclaringType));

            if (rv.Resolve() == null)
            {
                throw new InvalidOperationException();
            }

            return rv;
        }

        public bool IsConstructor(MethodSpec method)
        {
            return method.Name == method.DeclaringType.Name;
        }

        /// <summary>
        /// Adds to map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="typeRef">The type ref.</param>
        private static void AddToMap(Dictionary<string, TypeReference> map, TypeReference typeRef)
        {
            map.Add(typeRef.FullName, typeRef);
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
        /// Creates the known definitions map.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, TypeReference> CreateKnownDefinitionsMap()
        {
            var knownReferences = this.Context.KnownReferences;
            Dictionary<string, TypeReference> rv = new Dictionary<string, TypeReference>();

            MemberReferenceConverter.AddToMap(rv, knownReferences.Void);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Char);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Byte);
            MemberReferenceConverter.AddToMap(rv, knownReferences.SByte);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Short);
            MemberReferenceConverter.AddToMap(rv, knownReferences.UShort);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Int32);
            MemberReferenceConverter.AddToMap(rv, knownReferences.UInt32);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Int64);
            MemberReferenceConverter.AddToMap(rv, knownReferences.UInt64);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Single);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Double);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Enum);
            MemberReferenceConverter.AddToMap(rv, knownReferences.IntPtr);
            MemberReferenceConverter.AddToMap(rv, knownReferences.UIntPtr);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Object);
            MemberReferenceConverter.AddToMap(rv, knownReferences.TypeType);
            MemberReferenceConverter.AddToMap(rv, knownReferences.Boolean);
            MemberReferenceConverter.AddToMap(rv, knownReferences.String);

            return rv;
        }
    }
}