//-----------------------------------------------------------------------
// <copyright file="TypeScopeManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeScopeManager
    /// </summary>
    public class TypeScopeManager
    {
        /// <summary>
        /// Backing field for the scope.
        /// </summary>
        private readonly IdentifierScope scope;

        /// <summary>
        /// Backing field for static scope.
        /// </summary>
        private readonly IdentifierScope staticMemberScope;

        /// <summary>
        /// Backing field for the parentScopeManager.
        /// </summary>
        private readonly TypeScopeManager parentScopeManager;

        /// <summary>
        /// Backing field for typeDefinition.
        /// </summary>
        private readonly TypeDefinition typeDefinition;

        /// <summary>
        /// Backing field for HasMultipleConstructor Implementations.
        /// </summary>
        private bool hasMultipleConstructorImpls = false;

        /// <summary>
        /// Mapping for all the memberDefinitions to identifiers.
        /// </summary>
        private readonly Dictionary<MethodDefinition, Identifier> methodInstanceMap =
            new Dictionary<MethodDefinition, Identifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Mapping for all instance fieldDefinitions to identifiers.
        /// </summary>
        private readonly Dictionary<FieldDefinition, Identifier> fieldMap =
            new Dictionary<FieldDefinition, Identifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing store for intrinsic propertyDefinition to identifier map.
        /// </summary>
        private readonly Dictionary<PropertyDefinition, Identifier> propertyMap =
            new Dictionary<PropertyDefinition, Identifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracking field for identifiers for static members.
        /// </summary>
        private readonly Dictionary<MethodDefinition, Identifier> methodStaticMap =
            new Dictionary<MethodDefinition, Identifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing field for virtual methods.
        /// </summary>
        private readonly Dictionary<MethodDefinition, Identifier> virtualMethodIdentifiers =
            new Dictionary<MethodDefinition, Identifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing store for fixedName identifier.
        /// </summary>
        private readonly Dictionary<string, Identifier> fixedNameIdentifierMapping =
            new Dictionary<string, Identifier>();

        /// <summary>
        /// Implementation for instance methods.
        /// </summary>
        private readonly HashSet<MethodDefinition> implementationMethods =
            new HashSet<MethodDefinition>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing store for context,
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScopeManager"/> class.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="baseInstanceMemberScope">The base instance member scope.</param>
        /// <param name="baseStaticMemberScope">The base static member scope.</param>
        public TypeScopeManager(
            ConverterContext context,
            TypeDefinition typeDefinition,
            IdentifierScope baseInstanceMemberScope,
            IdentifierScope baseStaticMemberScope,
            bool inherit = true)
        {
            this.context = context;
            this.typeDefinition = typeDefinition;
            this.scope = inherit
                ? new IdentifierScope(baseInstanceMemberScope)
                : baseInstanceMemberScope;
            this.staticMemberScope = inherit
                ? new IdentifierScope(baseStaticMemberScope)
                : baseStaticMemberScope;

            this.MapMethods();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScopeManager"/> class.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="parentScopeManager">The parent scope manager.</param>
        public TypeScopeManager(
            TypeDefinition typeDefinition,
            TypeScopeManager parentScopeManager)
        {
            if (parentScopeManager == null)
            {
                throw new ArgumentException("Only Object doesn't have any base class so no parentScope");
            }

            this.context = parentScopeManager.context;
            this.parentScopeManager = parentScopeManager;
            this.typeDefinition = typeDefinition;

            this.scope = new IdentifierScope(this.parentScopeManager.scope);
            this.staticMemberScope = new IdentifierScope(this.parentScopeManager.staticMemberScope.ParentScope);

            this.MapMethods();
        }

        /// <summary>
        /// Gets a value indicating whether this instance has multiple constructors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has multiple constructors; otherwise, <c>false</c>.
        /// </value>
        public bool HasMultipleConstructors
        {
            get { return this.hasMultipleConstructorImpls; }
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="forceStatic">if set to <c>true</c> [force static].</param>
        /// <returns>Identifier for given method.</returns>
        public Identifier ResolveMethod(
            MethodReference methodReference,
            bool forceStatic)
        {
            return this.ResolveMethod(
                methodReference.Resolve(),
                forceStatic);
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="memberDefinition">The member definition.</param>
        /// <returns>Identifier for given member</returns>
        public Identifier ResolveMethod(
            MethodDefinition memberDefinition,
            bool forceStatic = false)
        {
            Identifier returnValue;
            forceStatic = forceStatic || memberDefinition.IsStatic;
            Dictionary<MethodDefinition, Identifier> identifierMap =
                forceStatic
                    ? this.methodStaticMap
                    : this.methodInstanceMap;

            if (!identifierMap.TryGetValue(memberDefinition, out returnValue))
            {
                MethodDefinition methodDefinition = memberDefinition as MethodDefinition;
                bool isFixedName;
                bool isAlias;
                bool isVirtualMethod = methodDefinition != null
                    && methodDefinition.IsVirtual
                    && !methodDefinition.IsFinal;

                string name = this.context.GetMemberName(
                    methodDefinition,
                    out isFixedName,
                    out isAlias);

                if (isFixedName && !isVirtualMethod)
                {
                    if (!this.fixedNameIdentifierMapping.TryGetValue(name, out returnValue))
                    {
                        returnValue = Identifier.CreateScopeIdentifier(
                            forceStatic ? this.staticMemberScope : this.scope,
                            name,
                            true);

                        this.fixedNameIdentifierMapping[name] = returnValue;
                    }
                }
                else if (forceStatic && name == ".ctor" && !methodDefinition.HasParameters)
                {
                    // This means that the name of this constructor is going to be defaultConstructor
                }
                else
                {
                    // if the memberName is fixed name and if it already exists, use the same identifer.
                    returnValue = Identifier.CreateScopeIdentifier(
                        forceStatic ? this.staticMemberScope : this.scope,
                        name,
                        false);
                }

                identifierMap.Add(memberDefinition, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="fieldReference">The member reference.</param>
        /// <returns>
        /// Identifier for given member
        /// </returns>
        public Identifier ResolveField(
            FieldReference fieldReference)
        {
            return this.ResolveField(
                fieldReference.Resolve());
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="fieldDefinition">The member definition.</param>
        /// <returns>
        /// Identifier for given member
        /// </returns>
        public Identifier ResolveField(
            FieldDefinition fieldDefinition)
        {
            Identifier returnValue;
            bool forceStatic = fieldDefinition.IsStatic;

            if (!this.fieldMap.TryGetValue(fieldDefinition, out returnValue))
            {
                bool isFixedName;
                bool isAlias;

                string name = this.context.GetMemberName(
                    fieldDefinition,
                    out isFixedName,
                    out isAlias);

                if (isFixedName)
                {
                    if (!this.fixedNameIdentifierMapping.TryGetValue(name, out returnValue))
                    {
                        returnValue = Identifier.CreateScopeIdentifier(
                            forceStatic ? this.staticMemberScope : this.scope,
                            name,
                            true);

                        this.fixedNameIdentifierMapping[name] = returnValue;
                    }
                }
                else
                {
                    // if the memberName is fixed name and if it already exists, use the same identifer.
                    returnValue = Identifier.CreateScopeIdentifier(
                        forceStatic ? this.staticMemberScope : this.scope,
                        name,
                        false);
                }

                this.fieldMap.Add(fieldDefinition, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        /// <param name="propertyDefinition">The member definition.</param>
        /// <returns>
        /// Identifier for given member
        /// </returns>
        public Identifier ResolveProperty(
            PropertyDefinition propertyDefinition)
        {
            Identifier returnValue;
            bool forceStatic = propertyDefinition.IsStatic();

            if (!this.propertyMap.TryGetValue(propertyDefinition, out returnValue))
            {
                bool isFixedName;
                bool isAlias;

                string name = this.context.GetMemberName(
                    propertyDefinition,
                    out isFixedName,
                    out isAlias);

                if (isFixedName)
                {
                    if (!this.fixedNameIdentifierMapping.TryGetValue(name, out returnValue))
                    {
                        returnValue = Identifier.CreateScopeIdentifier(
                            forceStatic ? this.staticMemberScope : this.scope,
                            name,
                            true);

                        this.fixedNameIdentifierMapping[name] = returnValue;
                    }
                }
                else
                {
                    // if the memberName is fixed name and if it already exists, use the same identifer.
                    returnValue = Identifier.CreateScopeIdentifier(
                        forceStatic ? this.staticMemberScope : this.scope,
                        name,
                        false);
                }

                propertyMap.Add(propertyDefinition, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the virtual method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Identifier for given virtual method</returns>
        public Identifier ResolveVirtualMethod(
            MethodReference methodReference)
        {
            MethodDefinition memberDefinition = methodReference.Resolve();

            return this.ResolveVirtualMethod(memberDefinition);
        }

        /// <summary>
        /// Resolves the virtual method.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>Identifier for given virtual method</returns>
        public Identifier ResolveVirtualMethod(
            MethodDefinition methodDefinition)
        {
            Identifier returnValue;

            if (methodDefinition.IsFinal
                || !methodDefinition.IsVirtual)
            {
                // If this method is not really virtual method, then why take
                // virtual slot.
                return this.ResolveMethod(methodDefinition);
            }

            if (!this.virtualMethodIdentifiers.TryGetValue(methodDefinition, out returnValue))
            {
                bool isFixedName;
                bool isAlias;

                string name = this.context.GetMemberName(
                    methodDefinition,
                    out isFixedName,
                    out isAlias);

                if (isFixedName)
                {
                    if (!this.fixedNameIdentifierMapping.TryGetValue(name, out returnValue))
                    {
                        returnValue = Identifier.CreateScopeIdentifier(
                            methodDefinition.IsStatic ? this.staticMemberScope : this.scope,
                            name,
                            isFixedName);

                        this.fixedNameIdentifierMapping[name] = returnValue;
                    }
                }
                else
                {
                    returnValue = Identifier.CreateScopeIdentifier(
                        this.scope,
                        "V_" + methodDefinition.Name,
                        false);
                }

                this.virtualMethodIdentifiers.Add(methodDefinition, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified method is implemented.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// <c>true</c> if the specified method is implemented; otherwise, <c>false</c>.
        /// </returns>
        public bool IsImplemented(MethodDefinition method)
        {
            return this.implementationMethods.Contains(method);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <param name="identifierString">The identifier string.</param>
        /// <param name="isInstance">if set to <c>true</c> [is instance].</param>
        /// <param name="enforce">if set to <c>true</c> [enforce].</param>
        /// <returns>identifier for given string</returns>
        internal Identifier GetIdentifier(
            string identifierString,
            bool isInstance,
            bool enforce)
        {
            var scope = isInstance
                ? this.scope
                : this.staticMemberScope;

            return Identifier.CreateScopeIdentifier(
                scope,
                identifierString,
                enforce);
        }

        /// <summary>
        /// Maps the method name to MethodDefinition.
        ///
        /// This is only done for imported/exported methods. Essentially methods
        /// who's name is defined in attribute.
        /// </summary>
        private void MapMethods()
        {
            Dictionary<string, List<MethodDefinition>> methodSlots = new Dictionary<string, List<MethodDefinition>>();
            Dictionary<string, List<MethodDefinition>> staticMethodSlots = new Dictionary<string, List<MethodDefinition>>();

            List<MethodDefinition> alterSignatureMethods = new List<MethodDefinition>();

            // Let's register all the imported names.
            foreach (var methodDefinition in this.typeDefinition.Methods)
            {
                bool isFinalName, isAlias;

                string name = this.context.GetMemberName(methodDefinition, out isFinalName, out isAlias);
                if (isFinalName)
                {
                    if (methodDefinition.IsVirtual)
                    {
                        this.ResolveVirtualMethod(methodDefinition);
                    }
                    else
                    {
                        this.ResolveMethod(methodDefinition);
                    }
                }
            }

            foreach (var propDef in this.typeDefinition.Properties)
            {
                bool isFinalName, isAlias;

                string name = this.context.GetMemberName(propDef, out isFinalName, out isAlias);
                if (isFinalName)
                {
                    this.ResolveProperty(propDef);
                }
            }

            foreach (var fieldDef in this.typeDefinition.Fields)
            {
                bool isFinalName, isAlias;

                string name = this.context.GetMemberName(fieldDef, out isFinalName, out isAlias);
                if (isFinalName)
                {
                    this.ResolveField(fieldDef);
                }
            }

            foreach (var methodDefinition in this.typeDefinition.Methods)
            {
                bool isFinalName, isAlias;

                string name = this.context.GetMemberName(methodDefinition, out isFinalName, out isAlias);
                if (isFinalName)
                {
                    if (methodDefinition.IsVirtual)
                    {
                        this.ResolveVirtualMethod(methodDefinition);
                    }
                    else
                    {
                        this.ResolveMethod(methodDefinition);
                    }
                }
            }

            foreach (var methodDefinition in this.typeDefinition.Methods)
            {
                Dictionary<string, List<MethodDefinition>> activeSlot =
                    methodDefinition.IsStatic
                        ? staticMethodSlots
                        : methodSlots;

                if (!this.context.IsImplemented(methodDefinition))
                {
                    continue;
                }

                List<MethodDefinition> methods;

                if (!activeSlot.TryGetValue(methodDefinition.Name, out methods))
                {
                    methods = new List<MethodDefinition>();
                    activeSlot.Add(methodDefinition.Name, methods);
                }

                methods.Add(methodDefinition);
                this.implementationMethods.Add(methodDefinition);
            }

            if (!this.typeDefinition.IsAbstract
                && !this.typeDefinition.IsSealed
                && !this.context.IsImported(typeDefinition))
            {
                this.hasMultipleConstructorImpls = methodSlots[".ctor"].Count > 1;
            }
        }
    }
}