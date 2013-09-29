//-----------------------------------------------------------------------
// <copyright file="TypeResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using System;

    /// <summary>
    /// Definition for TypeResolver.
    /// </summary>
    public class TypeResolver : IResolver
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly ClrContext context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"> The context. </param>
        public TypeResolver(ClrContext context)
        {
            this.context = context;
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
        public PropertyReference GetPropertyReference(TypeReference typeReference, string propertyName)
        {
            do
            {
                foreach (var property in typeReference.Resolve().Properties)
                {
                    if (property.Name == propertyName)
                    {
                        return property;
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
        public System.Collections.Generic.List<MethodReference> GetMethodReference(TypeReference typeReference, string methodName)
        {
            throw new NotImplementedException();
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
        public EventReference GetEventReferences(TypeReference typeReference, string eventName)
        {
            throw new NotImplementedException();
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
    }
}
