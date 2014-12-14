//-----------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TypeResolver.
    /// </summary>
    public class TypeResolver : IClrResolver, NScript.Converter.TypeSystemConverter.IResolver
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly ClrContext context;

        /// <summary>
        /// Manager for runtime.
        /// </summary>
        private readonly RuntimeScopeManager runtimeManager;

        /// <summary>
        /// The converter references.
        /// </summary>
        private readonly NScript.Converter.ConverterKnownReferences converterRefs;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"> The context. </param>
        public TypeResolver(
            RuntimeScopeManager runtimeManager,
            ClrContext context)
        {
            this.runtimeManager = runtimeManager;
            this.context = context;
            this.converterRefs
                        = runtimeManager.Context.KnownReferences;

        }

        /// <summary>
        /// Gets a type reference.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="fullName"> Name of the full. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        public TypeReference GetTypeReference(string fullName)
        {
            string[] split1 = fullName.Split('!');
            if (split1.Length != 2)
            {
                throw new ApplicationException(
                    string.Format("Invalid TypeName: {0}", fullName));
            }

            var rv = this.context.GetTypeDefinition(
                Tuple.Create(split1[0],split1[1]));

            if (rv == null
                || rv.Resolve() == null)
            {
                return null;
            }

            return rv;
        }

        /// <summary>
        /// Gets a type reference.
        /// </summary>
        /// <param name="typeName"> Name of the type. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        public TypeReference GetTypeReference(Tuple<string, string> typeName)
        {
            string fullName = typeName.Item1 == null
                ? typeName.Item2
                : typeName.Item1 + "." + typeName.Item2;

            return this.GetTypeReference(fullName);
        }

        /// <summary>
        /// Gets a property reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="propertyName">  Name of the property. </param>
        /// <returns>
        /// The property reference.
        /// </returns>
        public FieldReference GetFieldReference(TypeReference typeReference, string fieldName)
        {
            do
            {
                foreach (var field in typeReference.Resolve().Fields)
                {
                    if (field.Name == fieldName)
                    {
                        return new FieldReference(
                            field.Name,
                            field.FieldType.FixGenericTypeArguments(typeReference),
                            typeReference);
                    }
                }
            } while ((typeReference = typeReference.GetBaseType()) != null);

            return null;
        }

        /// <summary>
        /// Gets a property reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="propertyName">  Name of the property. </param>
        /// <returns>
        /// The property reference.
        /// </returns>
        public PropertyReference GetPropertyReference(TypeReference typeReference, string propertyName)
        {
            do
            {
                if (typeReference.IsArray)
                {
                    typeReference = converterRefs.FixArrayType(typeReference);
                }

                foreach (var property in typeReference.Resolve().Properties)
                {
                    if (property.Name == propertyName)
                    {
                        return new NScript.CLR.AST.InternalPropertyReference(
                            property.GetMethod != null
                                ? property.GetMethod.FixGenericTypeArguments(typeReference)
                                : null,
                            property.SetMethod != null
                                ? property.SetMethod.FixGenericTypeArguments(typeReference)
                                : null);
                    }
                }
            } while ((typeReference = typeReference.GetBaseType()) != null);

            return null;
        }

        /// <summary>
        /// Gets a method reference.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="methodName">    Name of the method. </param>
        /// <returns>
        /// The method reference.
        /// </returns>
        public List<MethodReference> GetMethodReference(TypeReference typeReference, string methodName)
        {
            List<MethodReference> rv = new List<MethodReference>();
            do
            {
                foreach (var methodDef in typeReference.Resolve().Methods)
                {
                    if (methodDef.Name != methodName)
                    {
                        continue;
                    }

                    var method = methodDef.FixGenericTypeArguments(typeReference);
                    bool matched = false;
                    for (int iRv = 0; iRv < rv.Count && !matched; iRv++)
                    {
                        matched = this.IsSameMethod(rv[iRv], method);
                    }

                    if (!matched)
                    {
                        rv.Add(method);
                    }
                }
            } while ((typeReference = typeReference.GetBaseType()) != null);

            return rv.Count == 0 ? null : rv;
        }

        /// <summary>
        /// Gets an event references.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="eventName">     Name of the event. </param>
        /// <returns>
        /// The event references.
        /// </returns>
        public EventReference GetEventReference(TypeReference typeReference, string eventName)
        {
            do
            {
                var typeDef = typeReference.Resolve();
                if (typeDef.HasEvents)
                {
                    foreach (var evt in typeDef.Events)
                    {
                        if (evt.Name == eventName)
                        {
                            return evt;
                        }
                    }
                }
            } while ((typeReference = typeReference.GetBaseType()) != null);

            return null;
        }

        /// <summary>
        /// Type implements.
        /// </summary>
        /// <param name="typeReference">      The type reference. </param>
        /// <param name="interfaceReference"> The interface reference. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TypeImplements(TypeReference typeReference, TypeReference interfaceReference)
        {
            TypeDefinition typeDefinition;
            if (!interfaceReference.Resolve().IsInterface)
            {
                return false;
            }

            if (typeReference.IsArray)
            {
                typeReference = this.converterRefs.FixArrayType(typeReference);
            }

            while (typeReference != null
                && (typeDefinition = typeReference.Resolve()) != null)
            {
                foreach (var iface in typeDefinition.Interfaces)
                {
                    if (iface.FixGenericTypeArguments(typeReference).IsSame(interfaceReference))
                    {
                        return true;
                    }
                }

                typeReference = typeReference.GetBaseType();
            }

            return false;
        }

        /// <summary>
        /// Type inherits.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="parentType">    Type of the parent. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TypeInherits(TypeReference typeReference, TypeReference parentType)
        {
            if (parentType.Resolve().IsInterface)
            {
                return false;
            }

            if (typeReference.IsArray)
            {
                typeReference = this.converterRefs.FixArrayType(typeReference);
            }

            typeReference = typeReference.GetBaseType();
            while (typeReference != null)
            {
                if (typeReference.IsSame(parentType))
                {
                    return true;
                }

                typeReference = typeReference.GetBaseType();
            }

            return false;
        }

        /// <summary>
        /// Resolves the virtual method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression to get slot for this virtual function.</returns>
        public Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope)
        {
            return this.ResolveVirtualMethod(
                methodReference,
                scope,
                this.Resolve);
        }

        /// <summary>
        /// Resolves the virtual method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression to get slot for this virtual function.</returns>
        public Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope,
            Func<TypeReference, IList<IIdentifier>> typeResolver)
        {
            return this.runtimeManager.ResolveVirtualMethod(
                methodReference,
                scope,
                typeResolver);
        }

        /// <summary>
        /// Resolves the specified type reference base.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <returns>Identifier for givenType.</returns>
        public IList<IIdentifier> Resolve(TypeReference typeReference)
        {
            return ResolverHelper.Resolve(
                this.runtimeManager,
                delegate(TypeReference typeRef)
                {
                    return null;
                },
                typeReference);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="fieldReference">The field reference.</param>
        /// <returns>
        /// Identifier identifying the member.
        /// </returns>
        public IIdentifier Resolve(FieldReference fieldReference)
        {
            return ResolverHelper.Resolve(
                this.runtimeManager,
                this,
                fieldReference);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="propertyReference">The member reference.</param>
        /// <returns>
        /// Identifier identifying the member.
        /// </returns>
        public IIdentifier Resolve(PropertyReference propertyReference)
        {
            return this.runtimeManager.Resolve(propertyReference);
        }

        /// <summary>
        /// Resolves the specified method reference.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(MethodReference methodReference)
        {
            return this.Resolve(methodReference, false);
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Identifier identifying the member.</returns>
        public IIdentifier Resolve(MethodReference memberReference, bool forceStatic)
        {
            return this.runtimeManager.Resolve(memberReference, forceStatic);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Idnentifiers for accessing static member.</returns>
        public IList<IIdentifier> ResolveStaticMember(
            FieldReference member,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.runtimeManager,
                this,
                member,
                resolver);
        }

        /// <summary>
        /// Resolve static member.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <param name="resolver">           The resolver. </param>
        /// <returns>
        /// Identifier for static member.
        /// </returns>
        public IIdentifier ResolveStaticMember(
            PropertyDefinition propertyDefinition,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.runtimeManager,
                propertyDefinition,
                resolver);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Idnentifiers for accessing static member.</returns>
        public IList<IIdentifier> ResolveStaticMember(
            MethodReference member,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.runtimeManager,
                member,
                resolver,
                false);
        }

        /// <summary>
        /// Resolve factory.
        /// </summary>
        /// <param name="constructor"> The constructor. </param>
        /// <param name="resolver">    The resolver. </param>
        /// <returns>
        /// A list of identifiers.
        /// </returns>
        public IList<IIdentifier> ResolveFactory(
            MethodReference constructor,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.runtimeManager,
                constructor,
                resolver,
                false);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Resolve static member</returns>
        public IList<IIdentifier> ResolveStaticMember(
            FieldReference member)
        {
            return this.ResolveStaticMember(
                member,
                this.Resolve);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>Resolve static member</returns>
        public IList<IIdentifier> ResolveStaticMember(
            MethodReference member)
        {
            return this.ResolveStaticMember(
                member,
                this.Resolve);
        }

        /// <summary>
        /// Resolve static member.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// Identifier for static member.
        /// </returns>
        public IIdentifier ResolveStaticMember(PropertyDefinition propertyDefinition)
        {
            return this.ResolveStaticMember(propertyDefinition, this.Resolve);
        }

        /// <summary>
        /// Resolve factory.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="methodReference"> The method reference. </param>
        /// <returns>
        /// A list of.
        /// </returns>
        public IList<IIdentifier> ResolveFactory(MethodReference constructor)
        {
            return ResolverHelper.ResolveStaticMember(
                this.runtimeManager,
                constructor,
                this.Resolve,
                true);
        }

        /// <summary>
        /// Resolve method slot name.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="methodReference"> The method reference. </param>
        /// <param name="isVirtualCall">   true if this object is virtual call. </param>
        /// <param name="identifierScope"> The identifier scope. </param>
        /// <returns>
        /// .
        /// </returns>
        public Expression ResolveMethodSlotName(MethodReference methodReference, bool isVirtualCall, IdentifierScope identifierScope)
        {
            throw new NotImplementedException();
        }

        private bool IsSameMethod(MethodReference left, MethodReference right)
        {
            if (!left.ReturnType.IsSame(right.ReturnType))
            {
                return false;
            }

            if (left.Parameters.Count != right.Parameters.Count)
            {
                return false;
            }

            for (int iParam = 0; iParam < left.Parameters.Count; iParam++)
            {
                if (!left.Parameters[iParam].ParameterType.IsSame(right.Parameters[iParam].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
