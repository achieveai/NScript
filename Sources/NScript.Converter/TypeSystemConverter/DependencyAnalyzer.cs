//-----------------------------------------------------------------------
// <copyright file="DependencyAnalyzer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using NScript.CLR;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>
    /// Definition for DependencyAnalyzer
    /// </summary>
    public class DependencyAnalyzer
    {
        /// <summary>
        /// Backing field for TypeToTypeReferences.
        /// This mostly contains base type and base interfaces only.
        /// For generic it may be generic arguments as well.
        /// </summary>
        private readonly Dictionary<TypeDefinition, List<TypeReference>> typeToTypeReferences =
            new Dictionary<TypeDefinition,List<TypeReference>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing field for FieldToTypeReferences.
        /// This will contain type of the field only.
        /// </summary>
        private readonly Dictionary<FieldDefinition, TypeReference> fieldToTypeReference =
            new Dictionary<FieldDefinition,TypeReference>(MemberReferenceComparer.Instance);

        /// <summary>
        /// This will contain all the types who's 1.constructors 2.typeof(...), 3.type check, 4.Generic arg for MethodRefs/TypeRefs etc. are used.
        /// </summary>
        private readonly Dictionary<MethodDefinition, List<TypeReference>> methodToTypeReference =
            new Dictionary<MethodDefinition,List<TypeReference>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// This will contain all the fields that Method is accessing.
        /// </summary>
        private readonly Dictionary<MethodDefinition, List<FieldReference>> methodToFieldReference =
            new Dictionary<MethodDefinition,List<FieldReference>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// This will contain all the methods called from given method.
        /// </summary>
        private readonly Dictionary<MethodDefinition, List<Tuple<MethodReference, bool>>> methodToMethodReferences =
            new Dictionary<MethodDefinition,List<Tuple<MethodReference, bool>>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing store for context.
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Backing store for clrKnownReferences.
        /// </summary>
        private readonly ClrKnownReferences clrKnownReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyAnalyzer"/> class.
        /// </summary>
        public DependencyAnalyzer(ConverterContext context)
        {
            this.context = context;
            this.clrKnownReferences = this.context.ClrKnownReferences;
        }

        /// <summary>
        /// Gets the type to type references.
        /// </summary>
        public Dictionary<TypeDefinition, List<TypeReference>> TypeToTypeReferences
        {
            get { return this.typeToTypeReferences; }
        }

        /// <summary>
        /// Gets the field to type reference.
        /// </summary>
        public Dictionary<FieldDefinition, TypeReference> FieldToTypeReference
        {
            get { return this.fieldToTypeReference; }
        }

        /// <summary>
        /// Gets the method to type reference.
        /// </summary>
        public Dictionary<MethodDefinition, List<TypeReference>> MethodToTypeReference
        {
            get { return this.methodToTypeReference; }
        }

        /// <summary>
        /// Gets the method to field references.
        /// </summary>
        public Dictionary<MethodDefinition, List<FieldReference>> MethodToFieldReferences
        {
            get { return this.methodToFieldReference; }
        }

        /// <summary>
        /// Gets the method to method references.
        /// </summary>
        public Dictionary<MethodDefinition, List<Tuple<MethodReference, bool>>> MethodToMethodReferences
        {
            get { return this.methodToMethodReferences; }
        }

        /// <summary>
        /// Adds the type for analysis.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        public void AddTypeForAnalysis(TypeDefinition typeDefinition)
        {
            if (this.typeToTypeReferences.ContainsKey(typeDefinition))
            {
                return;
            }

            this.AddTypeBases(typeDefinition);

            foreach (var fieldDefinition in typeDefinition.Fields)
            {
                // Any field that is not static and is of value type, adds to
                // type dependency.
                if (fieldDefinition.DeclaringType.IsValueOrEnum()
                    && !fieldDefinition.HasConstant)
                {
                    this.AddTypeDependency(
                        this.typeToTypeReferences[typeDefinition],
                        fieldDefinition.DeclaringType);
                }

                this.AddFieldTypeReferences(fieldDefinition);
            }

            foreach (var methodDefinition in typeDefinition.Methods)
            {
                this.AddMethodReferences(methodDefinition);
            }
        }

        /// <summary>
        /// Gets the type reference dependencies.
        /// This is mostly used for ordering the typeRegistrations.
        /// </summary>
        /// <param name="typeReferences">The type references.</param>
        /// <returns>type references ordered in such a way that all the dependencies of a type are present before it in the list.</returns>
        public List<TypeReference> GetTypeReferenceDependencies(IEnumerable<TypeDefinition> typeDefinitions)
        {
            List<TypeReference> returnValue = new List<TypeReference>();
            HashSet<TypeReference> visited = new HashSet<TypeReference>(
                MemberReferenceComparer.Instance);
            HashSet<TypeDefinition> returnSet = new HashSet<TypeDefinition>(
                typeDefinitions,
                MemberReferenceComparer.Instance);

            // Let's seed the stack. We add last typeDefinition first in the stack.
            foreach (var typeDefinition in typeDefinitions)
            {
                if (!typeDefinition.HasGenericParameters)
                {
                    // We only seed with references of non genric types.
                    this.CollectDependencies(
                        typeDefinition,
                        visited,
                        returnSet,
                        returnValue);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the ordered generic type dependencies.
        /// </summary>
        /// <param name="typeReferences">The type references.</param>
        /// <returns></returns>
        public List<TypeReference> GetOrderedGenericTypeDependencies(IEnumerable<TypeReference> typeReferences)
        {
            List<TypeReference> rvPass1 = new List<TypeReference>();
            HashSet<TypeReference> visited = new HashSet<TypeReference>(
                MemberReferenceComparer.Instance);
            HashSet<TypeDefinition> returnSet = new HashSet<TypeDefinition>(
                MemberReferenceComparer.Instance);

            foreach (var typeReference in DependencyAnalyzer.EnumerateGenericTypes(typeReferences))
            {
                // We only care about generics.
                if (typeReference.GetGenericArguments().Count > 0)
                {
                    returnSet.Add(typeReference.Resolve());
                }
            }

            foreach (var typeReference in typeReferences)
            {
                IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
                if (genericArguments != null
                    && genericArguments.Count > 0
                    && genericArguments.FirstOrDefault(trb => trb.IsGenericParameter) == null)
                {
                    this.CollectDependencies(
                        typeReference,
                        visited,
                        returnSet,
                        rvPass1);
                }
            }

            HashSet<TypeReference> typesAlreadyAdded = new HashSet<TypeReference>();
            List<TypeReference> rv = new List<TypeReference>();
            foreach (var typeRef in rvPass1)
            {
                if (!(typeRef is TypeSpecification)
                    || typesAlreadyAdded.Contains(typeRef))
                {
                    continue;
                }

                typesAlreadyAdded.Add(typeRef);
                rv.Add(typeRef);
            }

            Dictionary<TypeReference, int> pointingTo = new Dictionary<TypeReference, int>();
            Dictionary<TypeReference, List<TypeReference>> dependencies
                = new Dictionary<TypeReference, List<TypeReference>>();

            foreach (var typeRef in rv)
            {
                List<TypeReference> list = new List<TypeReference>();
                dependencies[typeRef] = list;

                IList<TypeReference> genericArguments = typeRef.GetGenericArguments();
                if (genericArguments != null
                    && genericArguments.Count > 0
                    && genericArguments.FirstOrDefault(trb => trb.IsGenericParameter) == null)
                {
                    foreach (var genericArg in genericArguments)
                    {
                        if (typesAlreadyAdded.Contains(genericArg))
                        {
                            list.Add(genericArg);
                            if (pointingTo.ContainsKey(genericArg))
                            {
                                pointingTo[genericArg]++;
                            }
                            else
                            {
                                pointingTo[genericArg] = 1;
                            }
                        }
                    }
                }
            }

            List<TypeReference> reverseList = new List<TypeReference>();
            while(pointingTo.Count > 0)
            {
                for (int iRv = rv.Count - 1; iRv >= 0; iRv--)
                {
                    if (!pointingTo.ContainsKey(rv[iRv]))
                    {
                        reverseList.Add(rv[iRv]);
                        foreach (var dep in dependencies[rv[iRv]])
                        {
                            if (pointingTo[dep]-- == 1)
                            {
                                pointingTo.Remove(dep);
                            }
                        }

                        rv.RemoveAt(iRv);
                    }
                }
            }

            reverseList.AddRange(rv);

            reverseList.Reverse();
            return reverseList;
        }

        /// <summary>
        /// Collects the dependencies.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="visited">The visited.</param>
        /// <param name="returnSet">The return set.</param>
        /// <param name="returnList">The return list.</param>
        private void CollectDependencies(
            TypeReference typeReference,
            HashSet<TypeReference> visited,
            HashSet<TypeDefinition> returnSet,
            List<TypeReference> returnList)
        {
            // Make sure we haven't visited this type, if not visited add to
            // visited list.
            if (visited.Contains(typeReference))
            {
                return;
            }

            visited.Add(typeReference);

            // We skip dependencies of Type so that type is the first type that we initialize. This is
            // because this type has all the registration functions and all.
            if (!typeReference.Equals(this.clrKnownReferences.TypeType)
                && this.typeToTypeReferences.ContainsKey(typeReference.Resolve()))
            {
                foreach (TypeReference dependent in this.typeToTypeReferences[typeReference.Resolve()])
                {
                    TypeReference dependentReference =
                        dependent.FixGenericTypeArguments(
                            typeReference.GetGenericArguments(),
                            null);

                    if (returnSet.Contains(dependentReference.Resolve()))
                    {
                        this.CollectDependencies(
                            dependentReference,
                            visited,
                            returnSet,
                            returnList);
                    }
                }
            }

            returnList.Add(typeReference);
        }

        /// <summary>
        /// Adds the type bases.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        private void AddTypeBases(TypeDefinition typeDefinition)
        {
            List<TypeReference> typeReferences = new List<TypeReference>();

            if (!typeDefinition.IsStatic())
            {
                // All the types are dependent on System.Type
                if (typeDefinition != this.clrKnownReferences.TypeType)
                {
                    typeReferences.Add(this.clrKnownReferences.TypeType);

                    if (typeDefinition.BaseType != null)
                    {
                        this.AddTypeDependency(typeReferences, typeDefinition.BaseType);
                    }
                }

                if (typeDefinition.Interfaces != null)
                {
                    foreach (var ifaceImpl in typeDefinition.Interfaces)
                    {
                        this.AddTypeDependency(typeReferences, ifaceImpl.InterfaceType);
                    }
                }
            }

            this.typeToTypeReferences.Add(
                typeDefinition,
                typeReferences);
        }

        /// <summary>
        /// Adds the field type references.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        private void AddFieldTypeReferences(FieldDefinition fieldDefinition)
        {
            this.fieldToTypeReference.Add(
                fieldDefinition,
                fieldDefinition.FieldType);
        }

        /// <summary>
        /// Adds the method references.
        /// </summary>
        /// <param name="method">The method.</param>
        private void AddMethodReferences(MethodDefinition method)
        {
            List<Tuple<MethodReference, bool>> methodReferences = new List<Tuple<MethodReference, bool>>();
            List<FieldReference> fieldReferences = new List<FieldReference>();
            List<TypeReference> typeReferences = new List<TypeReference>();

            if (method.HasBody)
            {
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.Operand == null)
                    {
                        continue;
                    }

                    MethodReference methodReference;
                    TypeReference typeReference;

                    switch (instruction.OpCode.Code)
                    {
                        case Code.Call:
                        case Code.Calli:
                        case Code.Ldftn:
                            methodReference = (MethodReference)instruction.Operand;
                            methodReferences.Add(
                                Tuple.Create(
                                    methodReference,
                                    false));
                            break;
                        case Code.Callvirt:
                        case Code.Ldvirtftn:
                            methodReference = (MethodReference)instruction.Operand;
                            methodReferences.Add(
                                Tuple.Create(
                                    methodReference,
                                    methodReference.Resolve().IsVirtual));
                            break;
                        case Code.Newobj:
                            methodReference = (MethodReference)instruction.Operand;
                            typeReferences.Add(methodReference.DeclaringType);
                            methodReferences.Add(
                                Tuple.Create(
                                    methodReference,
                                    false));
                            break;
                        case Code.Newarr:
                            typeReference = (TypeReference)instruction.Operand;
                            typeReferences.Add(this.clrKnownReferences.SystemArray);
                            break;
                        case Code.Cpobj:
                            Debugger.Break();
                            break;
                        case Code.Ldobj:
                        case Code.Castclass:
                        case Code.Isinst:
                        case Code.Stobj:
                        case Code.Box:
                        case Code.Unbox_Any:
                        case Code.Initobj:
                        case Code.Constrained:
                            typeReferences.Add((TypeReference)instruction.Operand);
                            break;
                        case Code.Ldtoken:
                            if (instruction.Operand is TypeReference)
                            {
                                typeReferences.Add((TypeReference)instruction.Operand);
                            }
                            break;
                        case Code.Ldfld:
                        case Code.Ldflda:
                        case Code.Stfld:
                        case Code.Ldsfld:
                        case Code.Ldsflda:
                        case Code.Stsfld:
                            fieldReferences.Add((FieldReference)instruction.Operand);
                            break;
                        case Code.Refanyval:
                        case Code.Arglist:
                        case Code.Tail:
                        case Code.Cpblk:
                        case Code.Initblk:
                            Debugger.Break();
                            break;
                    }
                }
            }

            this.methodToFieldReference.Add(method, fieldReferences);
            this.methodToTypeReference.Add(method, typeReferences);
            this.methodToMethodReferences.Add(method, methodReferences);
        }

        /// <summary>
        /// Adds the type dependency.
        /// </summary>
        /// <param name="typeReferences">The type references.</param>
        /// <param name="paramDef">The type reference.</param>
        private void AddTypeDependency(
            List<TypeReference> typeReferences,
            TypeReference typeReference)
        {
            foreach (var genericParam in typeReference.GetGenericArguments())
            {
                this.AddTypeDependency(
                    typeReferences,
                    genericParam);
            }

            typeReferences.Add(typeReference);
        }

        private static IEnumerable<TypeReference> EnumerateGenericTypes(IEnumerable<TypeReference> typeReferences)
        {
            foreach (var typeReference in typeReferences)
            {
                var genericArgs = typeReference.GetGenericArguments();

                if (genericArgs.Count > 0)
                {
                    foreach (var genericArgDependence in DependencyAnalyzer.EnumerateGenericTypes(genericArgs))
                    {
                        yield return genericArgDependence;
                    }

                    yield return typeReference;
                }
            }

            yield break;
        }
    }
}
