//-----------------------------------------------------------------------
// <copyright file="MemberReferenceSerializer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;
    using Mono.Cecil;
    using Mono.CSharp;
    using Newtonsoft.Json.Linq;
    using JsCsc.Lib.Serialization;

    /// <summary>
    /// Definition for MemberReferenceSerializer
    /// </summary>
    public static class MemberReferenceSerializer
    {
        public static void Serialize(ICustomSerializer serializer, IKVM.Reflection.Module module)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.ModuleDefinition);
            serializer.AddValue(NameTokens.Name, module.Name);
        }

        public static void Serialize(ICustomSerializer serializer, ModuleContainer moduleContainer)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.ModuleDefinition);
            serializer.AddValue(NameTokens.Name, moduleContainer.DeclaringAssembly.Name);
        }

        public static void Serialize(ICustomSerializer serializer, TypeContainer type)
        {
            MemberReferenceSerializer.Serialize(serializer, type.CurrentType);
        }

        public static void Serialize(ICustomSerializer serializer, TypeSpec type)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.TypeSpec);
            if (type == null)
            {
                serializer.AddValue(NameTokens.Name, (string)null);
            }

            var metaInfo = type.GetMetaInfo();
            serializer.AddValue(TypeTokens.ModuleDefinition, metaInfo.Module, MemberReferenceSerializer.Serialize);

            if (type.IsGenericParameter)
            {
                TypeParameterSpec paramSpec = (TypeParameterSpec)type;
                serializer.AddValue(NameTokens.Type, ValueTokens.GenericParam);
                serializer.AddValue(NameTokens.Position, paramSpec.DeclaredPosition);
                serializer.AddValue(NameTokens.IsMethodOwned, paramSpec.IsMethodOwned);

                return;
            }

            if (type.IsArray)
            {
                serializer.AddValue(NameTokens.Type, ValueTokens.Array);
                serializer.AddValue(
                    NameTokens.ElementType,
                    ((ArrayContainer)type).Element,
                    MemberReferenceSerializer.Serialize);

                return;
            }

            if (type is InflatedTypeSpec && type.IsGeneric)
            {
                serializer.AddValue(NameTokens.Type, ValueTokens.GenericInstance);
            }

            serializer.AddValue(NameTokens.NameSpace, metaInfo.Namespace);
            serializer.AddValue(NameTokens.Name, metaInfo.Name);
            if (metaInfo.IsNested)
            {
                serializer.AddValue(
                    NameTokens.DeclaringType,
                    type.DeclaringType,
                    MemberReferenceSerializer.Serialize);
            }

            if (type is InflatedTypeSpec && type.IsGeneric)
            {
                InflatedTypeSpec inflatedTypeSpec = (InflatedTypeSpec)type;
                serializer.AddValue(
                    NameTokens.TypeParams,
                    inflatedTypeSpec.TypeArguments,
                    MemberReferenceSerializer.Serialize);
            }
        }

        public static void Serialize(ICustomSerializer serializer, Method method)
        {
            MemberReferenceSerializer.Serialize(serializer, (MethodSpec)method.Spec);
        }

        public static void Serialize(ICustomSerializer serializer, MethodSpec method)
        {
            IParametersMember parametersMember = method.MemberDefinition as IParametersMember;
            var parameters = parametersMember != null
                ? parametersMember.Parameters
                : method.Parameters;

            TypeSpec methodReturnType = parametersMember != null
                    ? parametersMember.MemberType
                    : method.ReturnType;

            serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodSpec);
            serializer.AddValue(
                NameTokens.DeclaringType,
                method.DeclaringType,
                MemberReferenceSerializer.Serialize);

            serializer.AddValue(
                NameTokens.ReturnType,
                methodReturnType,
                MemberReferenceSerializer.Serialize);

            serializer.AddValue(
                NameTokens.Name,
                method.IsConstructor ? (method.IsStatic ? ".cctor" : ".ctor") : method.Name);

            serializer.AddValue(
                NameTokens.IsStatic,
                method.IsStatic);

            serializer.AddValue(
                NameTokens.Arity,
                method.Arity);

            List<Tuple<IParameterData, TypeSpec>> parameterList = new List<Tuple<IParameterData, TypeSpec>>(parameters.Count);
            for (int iParam = 0; iParam < parameters.Count; iParam++)
            {
                parameterList.Add(Tuple.Create(parameters.FixedParameters[iParam], parameters.Types[iParam]));
            }

            serializer.AddValue(
                NameTokens.Parameters,
                parameterList,
                MemberReferenceSerializer.Serialize);

            if (method.TypeArguments != null)
            {
                serializer.AddValue(
                    NameTokens.TypeParams,
                    method.TypeArguments,
                    MemberReferenceSerializer.Serialize);
            }
        }

        public static void Serialize(ICustomSerializer serializer, FieldSpec fieldSpec)
        {
            IInterfaceMemberSpec memberSpec = fieldSpec.MemberDefinition as IInterfaceMemberSpec;
            ImportedMemberDefinition importedMemberDefinition = fieldSpec.MemberDefinition as ImportedMemberDefinition;
            serializer.AddValue(NameTokens.TypeName, TypeTokens.FieldSpec);
            serializer.AddValue(
                NameTokens.Name,
                fieldSpec.Name);
            serializer.AddValue(
                NameTokens.MemberType,
                memberSpec != null
                    ? memberSpec.MemberType :
                        importedMemberDefinition != null
                            ? importedMemberDefinition.MemberType
                            : fieldSpec.MemberType,
                MemberReferenceSerializer.Serialize);
            serializer.AddValue(
                NameTokens.DeclaringType,
                fieldSpec.DeclaringType,
                MemberReferenceSerializer.Serialize);
        }

        private static void Serialize(ICustomSerializer serializer, Tuple<IParameterData, TypeSpec> paramData)
        {
            serializer.AddValue(NameTokens.TypeName, "paramData");
            ParameterAttributes attribute = ParameterAttributes.None;
            var modFlags = paramData.Item1.ModFlags;

            if ((modFlags & Parameter.Modifier.REF) == Parameter.Modifier.REF)
            {
                attribute |= ParameterAttributes.Retval;
            }

            if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
            {
                attribute |= ParameterAttributes.Out;
            }

            serializer.AddValue(
                NameTokens.ModFlags,
                attribute);
            serializer.AddValue(
                NameTokens.Name,
                paramData.Item1.Name);
            serializer.AddValue(
                NameTokens.Type,
                paramData.Item2,
                MemberReferenceSerializer.Serialize);
        }

        public static JObject Serialize(IKVM.Reflection.Module module)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ModuleDefinition;
            rv[NameTokens.Name] = module.Name;
            return rv;
        }

        public static JObject Serialize(ModuleContainer moduleContainer)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ModuleDefinition;
            rv[NameTokens.Name] = moduleContainer.DeclaringAssembly.Name;
            return rv;
        }

        public static JObject Serialize(TypeContainer type)
        {
            return MemberReferenceSerializer.Serialize(type.CurrentType);
        }

        public static JObject Serialize(TypeSpec type, TypeSpec typeContext = null, MethodSpec methodContext = null)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeSpec;
            if (type == null)
            {
                rv[NameTokens.Name] = null;
                return rv;
            }

            var metaInfo = type.GetMetaInfo();
            rv[NameTokens.Module] = MemberReferenceSerializer.Serialize(metaInfo.Module);

            if (type.IsGenericParameter)
            {
                TypeParameterSpec paramSpec = (TypeParameterSpec)type;
                rv[NameTokens.Type] = ValueTokens.GenericParam;
                rv[NameTokens.Name] = paramSpec.Name;
                rv[NameTokens.Position] = paramSpec.DeclaredPosition;
                rv[NameTokens.IsMethodOwned] = paramSpec.IsMethodOwned;

                return rv;
            }

            if (type.IsArray)
            {
                rv[NameTokens.Type] = ValueTokens.Array;
                rv[NameTokens.ElementType] = MemberReferenceSerializer.Serialize(((ArrayContainer)type).Element, typeContext, methodContext);

                return rv;
            }

            if (type.IsGeneric && (type is InflatedTypeSpec
                || (typeContext != null && typeContext.MemberDefinition == type.MemberDefinition)))
            {
                rv[NameTokens.Type] = ValueTokens.GenericInstance;
            }

            rv[NameTokens.Arity] = type.Arity;
            rv[NameTokens.NameSpace] = metaInfo.Namespace;
            rv[NameTokens.Name] = metaInfo.Name;
            if (metaInfo.IsNested)
            {
                rv[NameTokens.DeclaringType] = MemberReferenceSerializer.Serialize(type.DeclaringType, typeContext, methodContext);
            }

            if (type.IsGeneric && type is InflatedTypeSpec)
            {
                InflatedTypeSpec inflatedTypeSpec = (InflatedTypeSpec)type;
                JArray jarray = new JArray();
                foreach (var typeArg in inflatedTypeSpec.TypeArguments)
                {
                    jarray.Add(MemberReferenceSerializer.Serialize(typeArg, typeContext, methodContext));
                }

                rv[NameTokens.TypeParams] = jarray;
            }
            else if (type.IsGeneric && typeContext != null && type.MemberDefinition == typeContext.MemberDefinition)
            {
                // All this is a hack around bug in Mono where it doesn't really keep type as InflatedType instread
                // makes it a typeSpec.
                JArray jarray = new JArray();
                string typeNameStr = type.ToString();
                int indexOfGt = typeNameStr.IndexOf('<');
                int indexOfLt = typeNameStr.IndexOf('>');
                string[] typeArgNames = typeNameStr.Substring(indexOfGt + 1, indexOfLt - indexOfGt - 1).Split(',');
                for (int i = 0; i < type.Arity; i++)
                {
                    JObject paramObj = new JObject();
                    paramObj[NameTokens.Module] = MemberReferenceSerializer.Serialize(metaInfo.Module);
                    paramObj[NameTokens.Type] = ValueTokens.GenericParam;
                    paramObj[NameTokens.Name] = typeArgNames[i];
                    paramObj[NameTokens.Position] = i;
                    paramObj[NameTokens.IsMethodOwned] = false;

                    jarray.Add(paramObj);
                }

                rv[NameTokens.TypeParams] = jarray;
            }

            return rv;
        }

        public static JObject Serialize(Method method)
        {
            return MemberReferenceSerializer.Serialize((MethodSpec)method.Spec);
        }

        public static JObject Serialize(MethodSpec method)
        {
            if (method == null)
            {
                return null;
            }

            IParametersMember parametersMember = method.MemberDefinition as IParametersMember;
            var parameters = parametersMember != null // && method.Parameters == null
                ? parametersMember.Parameters
                : method.Parameters;

            TypeSpec methodReturnType = parametersMember != null
                    ? parametersMember.MemberType
                    : method.ReturnType;

            TypeSpec declaringType = parametersMember != null && parametersMember is MethodCore
                ? ((MethodCore)parametersMember).Spec.DeclaringType
                : method.DeclaringType;

            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodSpec;
            rv[NameTokens.DeclaringType] = MemberReferenceSerializer.Serialize(method.DeclaringType);
            rv[NameTokens.ReturnType] = MemberReferenceSerializer.Serialize(methodReturnType);
            rv[NameTokens.Name] = method.IsConstructor
                ? (method.IsStatic ? ".cctor" : ".ctor")
                : method.Name;
            rv[NameTokens.IsStatic] = method.IsStatic;
            rv[NameTokens.Arity] = method.Arity;

            JArray jarray = new JArray();
            for (int iParam = 0; iParam < parameters.Count; iParam++)
            {
                var paramData = Tuple.Create(parameters.FixedParameters[iParam], parameters.Types[iParam]);
                JObject paramObj = new JObject();
                paramObj[NameTokens.TypeName] = "paramData";
                ParameterAttributes attribute = ParameterAttributes.None;
                var modFlags = paramData.Item1.ModFlags;

                if ((modFlags & Parameter.Modifier.REF) == Parameter.Modifier.REF)
                {
                    attribute |= ParameterAttributes.Retval;
                }

                if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
                {
                    attribute |= ParameterAttributes.Out;
                }

                paramObj[NameTokens.ModFlags] = attribute.ToString();
                paramObj[NameTokens.Name] = paramData.Item1.Name;
                paramObj[NameTokens.Type] = MemberReferenceSerializer.Serialize(paramData.Item2, method.DeclaringType, method);

                jarray.Add(paramObj);
            }

            rv[NameTokens.Parameters] = jarray;

            if (method.TypeArguments != null)
            {
                jarray = new JArray();
                foreach (var typeArg in method.TypeArguments)
                {
                    jarray.Add(MemberReferenceSerializer.Serialize(typeArg));
                }

                rv[NameTokens.TypeParams] = jarray;
            }

            return rv;
        }

        public static JObject Serialize(FieldSpec fieldSpec)
        {
            IInterfaceMemberSpec memberSpec = fieldSpec.MemberDefinition as IInterfaceMemberSpec;
            ImportedMemberDefinition importedMemberDefinition = fieldSpec.MemberDefinition as ImportedMemberDefinition;
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.FieldSpec;
            rv[NameTokens.Name] = fieldSpec.Name;
            rv[NameTokens.MemberType] =
                MemberReferenceSerializer.Serialize(memberSpec != null
                    ? memberSpec.MemberType :
                        importedMemberDefinition != null
                            ? importedMemberDefinition.MemberType
                            : fieldSpec.MemberType,
                fieldSpec.DeclaringType);
            rv[NameTokens.DeclaringType] = MemberReferenceSerializer.Serialize(fieldSpec.DeclaringType);

            return rv;
        }

        private static JObject Serialize(Tuple<IParameterData, TypeSpec> paramData)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = "paramData";
            ParameterAttributes attribute = ParameterAttributes.None;
            var modFlags = paramData.Item1.ModFlags;

            if ((modFlags & Parameter.Modifier.REF) == Parameter.Modifier.REF)
            {
                attribute |= ParameterAttributes.Retval;
            }

            if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
            {
                attribute |= ParameterAttributes.Out;
            }

            rv[NameTokens.ModFlags] = attribute.ToString();
            rv[NameTokens.Name] = paramData.Item1.Name;
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(paramData.Item2);

            return rv;
        }

        public static ModuleSpecSer SerializeN(IKVM.Reflection.Module module)
        { return new ModuleSpecSer {Name = module.Name}; }

        public static ModuleSpecSer SerializeN(ModuleContainer moduleContainer)
        { return new ModuleSpecSer { Name = moduleContainer.DeclaringAssembly.Name }; }

        public static TypeSpecSer SerializeN(
            TypeSpec type,
            TypeSpec typeContext = null,
            MethodSpec methodContext = null)
        {
            var rvSer = new TypeSpecSer();

            ModuleSpecSer moduleSer = null;

            if (type == null)
            { return null; }

            var metaInfo = type.GetMetaInfo();
            moduleSer = MemberReferenceSerializer.SerializeN(metaInfo.Module);

            if (type.IsGenericParameter)
            {
                TypeParameterSpec paramSpec = (TypeParameterSpec)type;
                return new GenericParamSer
                {
                    Name = type.Name,
                    Position = paramSpec.DeclaredPosition,
                    IsMethodOwned = paramSpec.IsMethodOwned,
                    Module = moduleSer
                };
            }

            if (type.IsArray)
            {
                return new ArrayTypeSer
                {
                    Module = moduleSer,
                    ElementType = MemberReferenceSerializer.SerializeN(
                        ((ArrayContainer)type).Element,
                        typeContext,
                        methodContext)
                };
            }

            var genericArgs = new List<TypeSpecSer>();
            if (type.IsGeneric && type is InflatedTypeSpec)
            {
                InflatedTypeSpec inflatedTypeSpec = (InflatedTypeSpec)type;
                foreach (var typeArg in inflatedTypeSpec.TypeArguments)
                {
                    genericArgs.Add(
                        MemberReferenceSerializer.SerializeN(
                            typeArg,
                            typeContext,
                            methodContext));
                }
            }
            else if (type.IsGeneric && typeContext != null && type.MemberDefinition == typeContext.MemberDefinition)
            {
                // All this is a hack around bug in Mono where it doesn't really keep type as InflatedType instread
                // makes it a typeSpec.
                string typeNameStr = type.ToString();
                int indexOfGt = typeNameStr.IndexOf('<');
                int indexOfLt = typeNameStr.IndexOf('>');
                string[] typeArgNames = typeNameStr.Substring(indexOfGt + 1, indexOfLt - indexOfGt - 1).Split(',');
                for (int i = 0; i < type.Arity; i++)
                {
                    genericArgs.Add(
                        new GenericParamSer
                        {
                            Module = MemberReferenceSerializer.SerializeN(metaInfo.Module),
                            Name = typeArgNames[i],
                            Position = i,
                            IsMethodOwned = false
                        });
                }
            }

            TypeSpecSer rv;
            if (type.IsGeneric && (type is InflatedTypeSpec
                || (typeContext != null && typeContext.MemberDefinition == type.MemberDefinition)))
            {
                rv = new GenericInstanceTypeSer
                { TypeParams = genericArgs };
            }
            else
            { rv = new TypeSpecSer(); }

            rv.Name = metaInfo.Name;
            rv.Namespace = metaInfo.Namespace;
            rv.Module = moduleSer;
            rv.Arity = type.Arity;
            rv.NestedParent = metaInfo.IsNested
                ? MemberReferenceSerializer.SerializeN(
                    type.DeclaringType,
                    typeContext,
                    methodContext)
                : null;

            return rv;
        }

        public static MethodSpecSer SerializeN(Method method)
        {
            return MemberReferenceSerializer.SerializeN((MethodSpec)method.Spec);
        }

        public static MethodSpecSer SerializeN(MethodSpec method)
        {
            if (method == null)
            {
                return null;
            }

            IParametersMember parametersMember = method.MemberDefinition as IParametersMember;
            var parameters = parametersMember != null // && method.Parameters == null
                ? parametersMember.Parameters
                : method.Parameters;

            TypeSpec methodReturnType = parametersMember != null
                    ? parametersMember.MemberType
                    : method.ReturnType;

            TypeSpec declaringType = parametersMember != null && parametersMember is MethodCore
                ? ((MethodCore)parametersMember).Spec.DeclaringType
                : method.DeclaringType;

            var rv = new MethodSpecSer{
                    DeclaringType =
                        MemberReferenceSerializer.SerializeN(
                            method.DeclaringType),
                    ReturnType =
                        MemberReferenceSerializer.SerializeN(
                            methodReturnType),
                    Name = method.IsConstructor
                        ? (method.IsStatic ? ".cctor" : ".ctor")
                        : method.Name,
                    IsStatic = method.IsStatic,
                    Arity = method.Arity
            };

            rv.Parameters = new List<ParamSer>();
            for (int iParam = 0; iParam < parameters.Count; iParam++)
            {
                var paramData = Tuple.Create(parameters.FixedParameters[iParam], parameters.Types[iParam]);
                ParameterAttributes attribute = ParameterAttributes.None;
                var modFlags = paramData.Item1.ModFlags;

                if ((modFlags & Parameter.Modifier.REF) == Parameter.Modifier.REF)
                {
                    attribute |= ParameterAttributes.Retval;
                }

                if ((modFlags & Parameter.Modifier.OUT) == Parameter.Modifier.OUT)
                {
                    attribute |= ParameterAttributes.Out;
                }

                rv.Parameters.Add(
                    new ParamSer
                    {
                        Name = paramData.Item1.Name,
                        ModFlags = (int)attribute,
                        ParamType =
                            MemberReferenceSerializer.SerializeN(
                                paramData.Item2,
                                method.DeclaringType, method)
                    });
            }

            if (method.TypeArguments != null)
            {
                rv.TypeArgs = new List<TypeSpecSer>();

                foreach (var typeArg in method.TypeArguments)
                { rv.TypeArgs.Add(MemberReferenceSerializer.SerializeN(typeArg)); }
            }

            return rv;
        }

        public static FieldSpecSer SerializeN(FieldSpec fieldSpec)
        {
            IInterfaceMemberSpec memberSpec = fieldSpec.MemberDefinition as IInterfaceMemberSpec;
            ImportedMemberDefinition importedMemberDefinition = fieldSpec.MemberDefinition as ImportedMemberDefinition;
            return new FieldSpecSer
                {
                    Name = fieldSpec.Name,
                    MemberType = 
                        MemberReferenceSerializer.SerializeN(memberSpec != null
                            ? memberSpec.MemberType :
                                importedMemberDefinition != null
                                    ? importedMemberDefinition.MemberType
                                    : fieldSpec.MemberType,
                        fieldSpec.DeclaringType),
                    DeclaringType =
                        MemberReferenceSerializer.SerializeN(fieldSpec.DeclaringType)
                };
        }
    }
}