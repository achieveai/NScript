//-----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NScript.CLR;
    using NScript.Converter.ExpressionsConverter;
    using NScript.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeConverter
    ///
    /// Converted types have multiple segments.
    /// 0. For generic types, some code to make sure we don't create generic type multiple times.
    /// 1. Type decleration block: This block contains typeConstructor and typeId.
    /// 2. GenericType dependencies initializer: This block contains initialization of
    ///     generic type variables which contains generic arguments.
    /// 3. For value types, we have default value creator.
    ///    For types with default constructor, we assign it to known name.
    /// 4. Static methods.
    /// 5. Prototype initialization.
    /// 6. Instance method initializations.
    /// 7. Virtual method redirection and initializations.
    /// 8. Type registration.
    /// 9. Generic type reference initializers. These are type references that are used inside
    ///     methods but type doesn't have hard dependency on these types.
    ///
    /// Out of all the above steps, we initialize step 3, 5 and 8 at the time of construction.
    /// This is necessary to make sure all the dependencies of this type are also used. This is
    /// used when creating script using EntryPoint mechanism.
    ///
    /// Note: We still need to figure out how to tackle new T() scenario.
    /// </summary>
    public abstract class TypeConverter : IResolver
    {
        /// <summary>
        /// Backing field for TypeDefinition.
        /// </summary>
        private readonly TypeDefinition typeDefinition;

        /// <summary>
        /// Backing field for LocalTypeReference.
        /// </summary>
        private readonly TypeReference localTypeReference;

        /// <summary>
        /// Backing field for typeScope.
        /// </summary>
        private readonly IdentifierScope typeScope;

        /// <summary>
        /// Backing field for runtime scope manager.
        /// </summary>
        private readonly RuntimeScopeManager runtimeScopeManager;

        /// <summary>
        /// Backing field for ConverterContext.
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Backing field for type Scope Manager.
        /// </summary>
        private readonly TypeScopeManager typeScopeManager;

        /// <summary>
        /// Backing store for ClrKnownReferences.
        /// </summary>
        private readonly ClrKnownReferences clrKnownRefs;

        /// <summary>
        /// Backing store for Converter Known References.
        /// </summary>
        private readonly ConverterKnownReferences cnvtKnownRefs;

        /// <summary>
        /// Mapping from localVariableIndex to Identifier.
        /// </summary>
        private readonly Dictionary<string, IIdentifier> localVariableToIdentifierMap =
            new Dictionary<string, IIdentifier>();

        /// <summary>
        /// Dictionary for keeping track of local TypeReferences (typeReferences with scope
        /// of MethodReference).
        /// </summary>
        private readonly Dictionary<TypeReference, IIdentifier> localTypeReferences =
            new Dictionary<TypeReference, IIdentifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracking field for what all typeReferences have been initialized.
        /// </summary>
        private readonly HashSet<TypeReference> localTypeReferencesInitialized =
            new HashSet<TypeReference>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracks if methods were added or not.
        /// </summary>
        private readonly HashSet<MethodDefinition> methodsAdded =
            new HashSet<MethodDefinition>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracking field for all the implemented methods.
        /// </summary>
        private readonly Dictionary<MethodDefinition, MethodConverter> implementedMethods =
            new Dictionary<MethodDefinition, MethodConverter>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracking field for all the implemented fields.
        /// </summary>
        private readonly HashSet<FieldDefinition> implementedFields =
            new HashSet<FieldDefinition>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Tracker for if this typeConverter is working off selective typeConverter or not.
        /// </summary>
        private readonly bool isSelectiveInit;

        /// <summary>
        /// Step 5 expression. See remarks of TypeConverter to know more.
        /// </summary>
        private readonly Expression basePrototypeExpression;

        /// <summary>
        /// Tracking field for tracking if we have typeReferenceInit function or not.
        /// </summary>
        private bool hasTypeRefInit = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConverter"/> class.
        /// </summary>
        /// <param name="scopeManager">The scope manager.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="selectiveInit">if set to <c>true</c> [selective init].</param>
        protected TypeConverter(
            RuntimeScopeManager scopeManager,
            TypeDefinition typeDefinition,
            bool selectiveInit)
        {
            this.isSelectiveInit = selectiveInit;
            this.typeDefinition = typeDefinition;
            this.runtimeScopeManager = scopeManager;
            this.context = scopeManager.Context;
            this.clrKnownRefs = scopeManager.Context.ClrKnownReferences;
            this.cnvtKnownRefs = scopeManager.Context.KnownReferences;
            this.typeScopeManager = this.runtimeScopeManager.GetTypeScope(
                this.typeDefinition);
            this.localTypeReference = this.typeDefinition.FixGenericParameters();

            if (this.typeDefinition.GenericParameters.Count > 0)
            {
                List<string> genericParamNames = this.typeDefinition.GenericParameters.Select(g => g.Name).ToList();
                genericParamNames.Add("_callStatiConstructor");

                this.typeScope = new IdentifierScope(
                    scopeManager.Scope,
                    genericParamNames,
                    false);
            }
            else
            {
                this.typeScope = scopeManager.Scope;
            }

            this.localTypeReferencesInitialized.Add(this.localTypeReference);
            this.AddStaticConstructorImplementation();
            this.basePrototypeExpression = this.GetBasePrototypeExpression();
            this.TouchRegisterReqs();

            foreach (var fieldDef in this.typeDefinition.Fields)
            {
                if (fieldDef.FieldType is TypeSpecification)
                {
                    // Just resolve GetDefault method so that it get's registered for
                    // emision in the script
                    this.Resolve(this.context.KnownReferences.GetDefaultMethod);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether all methods are implemented as static methods.
        /// </summary>
        /// <value>
        /// <c>true</c> if all methods need to be implemented as static methods; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllStaticMethods
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is selective init.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selective init; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectiveInit
        { get { return this.isSelectiveInit; } }

        /// <summary>
        /// Gets the type scope.
        /// </summary>
        /// <value>The type scope.</value>
        public IdentifierScope Scope
        {
            get
            {
                return this.typeScope;
            }
        }

        /// <summary>
        /// Gets the runtime manager.
        /// </summary>
        /// <value>The runtime manager.</value>
        public RuntimeScopeManager RuntimeManager
        { get { return this.runtimeScopeManager; } }

        /// <summary>
        /// Gets a value indicating whether this object is generic like.
        /// </summary>
        /// <value>
        /// true if this object is generic like, false if not.
        /// </value>
        public bool IsGenericLike
        {
            get
            {
                // TODO: We currently do not treat implemented classes with IgnoreGenericArgument like
                // normal type.
                return this.typeDefinition.HasGenericParameters
                    // this.context.HasGenericArguments(this.typeDefinition)
                    && !this.context.IsPsudoType(this.typeDefinition);
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ConverterContext Context
        { get { return this.context; } }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        public TypeDefinition TypeDefinition
        { get { return this.typeDefinition; } }

        /// <summary>
        /// Gets the type scope manager.
        /// </summary>
        protected TypeScopeManager TypeScopeManager
        { get { return this.typeScopeManager; } }

        /// <summary>
        /// Gets the methods added.
        /// </summary>
        /// <value>
        /// The methods added.
        /// </value>
        protected HashSet<MethodDefinition> MethodsAdded
        { get { return this.methodsAdded; } }

        /// <summary>
        /// Gets the type registration method.
        /// </summary>
        protected abstract MethodReference TypeRegistrationMethod
        { get; }

        /// <summary>
        /// Creates the specified runtime scope manager.
        /// </summary>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>Type converters according to typeDefinition</returns>
        public static TypeConverter Create(
            RuntimeScopeManager runtimeScopeManager,
            TypeDefinition typeDefinition,
            bool selectiveInit = false)
        {
            if (typeDefinition.IsEnum)
            {
                return new EnumTypeConverter(
                    runtimeScopeManager,
                    typeDefinition,
                    selectiveInit);
            }

            if (typeDefinition.Equals(runtimeScopeManager.Context.ClrKnownReferences.NullableType))
            {
                return new NullableTypeConverter(
                    runtimeScopeManager,
                    typeDefinition,
                    selectiveInit);
            }

            if (typeDefinition.IsValueType)
            {
                return new StructTypeConverter(
                    runtimeScopeManager,
                    typeDefinition,
                    selectiveInit);
            }

            if (typeDefinition.IsInterface)
            {
                return new InterfaceTypeConverter(
                    runtimeScopeManager,
                    typeDefinition,
                    selectiveInit);
            }

            return new ReferenceTypeConverter(
                runtimeScopeManager,
                typeDefinition,
                selectiveInit);
        }

        /// <summary>
        /// Determines whether given typeDefinition has static constructor.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>
        /// <c>true</c> if given typeDefinition has static constructor;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static MethodDefinition HasStaticConstructor(
            TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
            {
                return null;
            }

            foreach (var method in typeDefinition.Methods)
            {
                if (method.IsStatic
                    && method.IsConstructor
                    && method.IsSpecialName)
                {
                    return method;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the method to implementation.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        public void AddMethodToImplementation(MethodDefinition methodDefinition)
        {
            if (!methodDefinition.DeclaringType.IsSameDefinition(this.typeDefinition))
            {
                throw new ArgumentException("methodDefinition");
            }

            if (this is InterfaceTypeConverter)
            {
                return;
            }

            if (this.methodsAdded.Contains(methodDefinition))
            {
                return;
            }

            this.methodsAdded.Add(methodDefinition);

            // If we can't implement this method, let's skip.
            if (!this.TypeScopeManager.IsImplemented(methodDefinition)
                || !this.context.IsImplemented(methodDefinition)
                || (methodDefinition.IsConstructor && this.context.IsPsudoType(this.typeDefinition)))
            {
                return;
            }

            if (!this.implementedMethods.ContainsKey(methodDefinition))
            {
                this.implementedMethods.Add(
                    methodDefinition,
                    new MethodConverter(this, methodDefinition));
            }
        }

        /// <summary>
        /// Adds the field to implementation.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        public void AddFieldToImplementation(FieldDefinition fieldDefinition)
        {
            if (!this.implementedFields.Contains(fieldDefinition))
            {
                if (!this.Context.IsImplemented(fieldDefinition))
                {
                    return;
                }

                this.implementedFields.Add(fieldDefinition);
            }
        }

        /// <summary>
        /// Resolves the specified type reference base.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <returns>Identifier for givenType.</returns>
        public IList<IIdentifier> Resolve(TypeReference typeReference)
        {
            // If we are resolving paramDef which points to typeDefinition
            // replace it with localTypeReference. This is done so that if typeDefinition
            // is generic, we could use GenericTypeDefinition.
            if (typeReference.IsSame(this.typeDefinition))
            {
                typeReference = this.localTypeReference;
            }

            return ResolverHelper.Resolve(
                this.RuntimeManager,
                delegate(TypeReference typeRef)
                {
                    IIdentifier rv;
                    if (!this.localTypeReferences.TryGetValue(typeReference, out rv))
                    {
                        GenericParameter genericParam = typeReference as GenericParameter;
                        if (genericParam != null)
                        {
                            rv = this.typeScope.ParameterIdentifiers[genericParam.Position];
                            this.localTypeReferencesInitialized.Add(typeReference);
                        }
                        else
                        {
                            StringBuilder strBuilder = new StringBuilder();

                            RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                                typeReference,
                                strBuilder,
                                (typeDefinitionBase, typeNameBuilder) =>
                                {
                                    typeNameBuilder.Append(
                                        RuntimeScopeManager.GetSplitName(
                                            typeDefinitionBase.Name).Item2);
                                });

                            rv = SimpleIdentifier.CreateScopeIdentifier(
                                this.typeScope,
                                strBuilder.ToString(),
                                false);
                        }

                        this.localTypeReferences.Add(typeReference, rv);
                    }

                    return rv;
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
                this.RuntimeManager,
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
            return this.runtimeScopeManager.Resolve(propertyReference);
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
            return this.runtimeScopeManager.Resolve(memberReference, forceStatic);
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
                this.RuntimeManager,
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
        internal IIdentifier ResolveStaticMember(
            PropertyDefinition propertyDefinition,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.RuntimeManager,
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
                this.RuntimeManager,
                member,
                resolver,
                false);
        }

        public IList<IIdentifier> ResolveFactory(
            MethodReference constructor,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            return ResolverHelper.ResolveStaticMember(
                this.RuntimeManager,
                constructor,
                resolver,
                true);
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
            return this.RuntimeManager.ResolveVirtualMethod(
                methodReference,
                scope,
                typeResolver);
        }

        /// <summary>
        /// Initializes the type statement.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// Static constructor statement.
        /// </returns>
        public Statement InitializeTypeStatement(
            TypeReference typeReference)
        {
            if (typeReference == null
                || !typeReference.IsSameDefinition(this.typeDefinition))
            {
                throw new ArgumentException("paramDef");
            }

            Statement rv = this.GenerateLocalTypeRefInitCall(
                typeReference,
                this.Scope);

            if (rv == null)
            {
                rv = this.GenerateStaticConstructorCall(typeReference);
            }

            return rv;
        }

        /// <summary>
        /// Converts the const value.
        /// </summary>
        /// <param name="constValue">The const value.</param>
        /// <returns>JST for given const value object</returns>
        public static Expression ConvertConstValue(
            object constValue)
        {
            Type type = constValue.GetType();

            if (type == typeof(string))
            {
                return new StringLiteralExpression(
                    null,
                    (string)constValue);
            }

            if (type == typeof(bool))
            {
                return new BooleanLiteralExpression(
                    null,
                    (bool)constValue);
            }

            if (type == typeof(ulong))
            {
                return new NumberLiteralExpression(
                    null,
                    (long)(ulong)constValue);
            }

            return new NumberLiteralExpression(
                null,
                long.Parse(constValue.ToString()));
        }

        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns></returns>
        public virtual List<Statement> Convert(
            Action<TypeConverter, List<Statement>> addDependenciesCallback)
        {
            List<Statement> returnValue = new List<Statement>();

            // Make sure that we have all the methodConverters for all implementations created.
            this.CheckImplementation();

            if (this.IsGenericLike)
            {
                // This is generic type so we will have to create generic type init functions.
                this.ConvertGenericType(returnValue, addDependenciesCallback);
            }
            else
            {
                this.CreateConstructor(returnValue);
                this.ConvertBody(returnValue, addDependenciesCallback);
            }

            return returnValue;
        }

        /// <summary>
        /// Clears the variable initialized tracking.
        /// </summary>
        public void ClearVariableInitializedTracking()
        {
            this.localTypeReferencesInitialized.Clear();
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
        public IList<IIdentifier> ResolveFactory(MethodReference methodReference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolve implemented version.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <returns>
        /// Identifier for ImplementedVersion of imported property.
        /// </returns>
        public IIdentifier ResolveImplementedVersion(PropertyDefinition propertyDefinition)
        {
            return new CompoundIdentifier(
                this.Resolve(this.cnvtKnownRefs.ImportedExtensionField),
                this.typeScopeManager.ResolveImportedExtendedProperty(propertyDefinition));
        }

        /// <summary>
        /// Resolve wrapped method.
        /// </summary>
        /// <param name="methodDefinition"> The method definition. </param>
        /// <returns>
        /// .
        /// </returns>
        public IIdentifier ResolveWrappedMethod(MethodDefinition methodDefinition)
        {
            if (methodDefinition.IsStatic)
            {
                bool isFixed, isAlias;
                string name = this.context.GetMemberName(methodDefinition, true, out isFixed, out isAlias);
                if (isAlias)
                {
                    return new CompoundIdentifier(this.RuntimeManager.ResolveScriptAlias(name));
                }
                else
                {
                    return new CompoundIdentifier(
                        new CompoundIdentifier(this.Resolve(this.typeDefinition)),
                        this.typeScopeManager.ResolveUnderlyingMethod(methodDefinition));
                }
            }

            return this.typeScopeManager.ResolveUnderlyingMethod(methodDefinition);
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

        /// <summary>
        /// Registers the type internal.
        /// </summary>
        /// <param name="typeNameExpression">The type name expression.</param>
        /// <param name="parentTypeExpression">The parent type expression.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns>Type registration expression.</returns>
        protected abstract IList<Expression> RegisterTypeMethodArguments(
            Expression typeNameExpression,
            Expression parentTypeExpression,
            List<Expression> interfaces);

        /// <summary>
        /// Initializes the static functions.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected virtual void InitializeStaticFunctions(
            List<Statement> statements)
        {
            foreach (var function in this.typeDefinition.Methods)
            {
                MethodConverter converter = this.GetMethodConverter(function);
                if (converter == null
                    || !(converter.IsGlobalStaticImplementation || function.IsStatic))
                {
                    continue;
                }

                if (converter.IsGlobalStaticImplementation)
                {
                    statements.Add(new ExpressionStatement(null, this.Scope, converter.MethodFunctionExpression));
                }
                else
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.ResolveStaticMember(function)),
                            converter.MethodFunctionExpression));
                }
            }
        }

        /// <summary>
        /// Initializes the static variables.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected virtual void InitializeStaticVariables(
            List<Statement> statements)
        {
            bool isGenericType = this.typeDefinition.IsGeneric();
            foreach (var field in this.typeDefinition.Fields)
            {
                if (!this.implementedFields.Contains(field))
                {
                    continue;
                }

                Expression value = null;

                if (field.HasConstant)
                {
                    value = TypeConverter.ConvertConstValue(
                        field.Constant);
                }
                else if (field.IsStatic)
                {
                    value =
                        DefaultValueConverter.GetDefaultValue(
                            this,
                            this.RuntimeManager,
                            this.Scope,
                            field.FieldType,
                            null);
                }
                else
                {
                    continue;
                }

                statements.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.ResolveStaticMember(field)),
                        value));
            }
        }

        /// <summary>
        /// Gets the prototype initializers.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>List of prototype index and initializer expression pair.</returns>
        protected abstract List<Tuple<Expression, Expression>> GetPrototypeInitializers(Expression prototype);

        /// <summary>
        /// Initializes the virtuals.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>List of expressions each initializing one field in prototype.</returns>
        protected abstract List<ExpressionStatement> InitializeVirtuals(Expression prototype);

        /// <summary>
        /// Maps the class virtual.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="virtualBase">The virtual base.</param>
        /// <param name="method">The method.</param>
        /// <returns>Expression for assignment to class Virtual</returns>
        protected Expression MapClassVirtual(
            Expression prototype,
            MethodReference virtualBase,
            MethodDefinition method)
        {
            MethodConverter converter = this.GetMethodConverter(method);
            if ((converter == null || !converter.IsGlobalStaticImplementation
                && !this.typeDefinition.IsValueType))
            {
                return new IndexExpression(
                        null,
                        this.Scope,
                        prototype,
                        new IdentifierExpression(
                            this.TypeScopeManager.ResolveMethod(method), this.Scope));
            }

            List<string> args = new List<string>();
            foreach (var genericArg in method.GenericParameters)
            { args.Add(genericArg.Name); }

            foreach (var arg in method.Parameters)
            { args.Add(arg.Name); }

            IdentifierScope innerScope2 =
                new IdentifierScope(
                    this.Scope,
                    args,
                    false);

            FunctionExpression funcExpression = new FunctionExpression(
                null,
                this.Scope,
                innerScope2,
                innerScope2.ParameterIdentifiers,
                null);

            List<Expression> arguments = new List<Expression>();

            for (int iGeneric = 0; iGeneric < method.GenericParameters.Count; iGeneric++)
            {
                arguments.Add(
                    new IdentifierExpression(
                        innerScope2.ParameterIdentifiers[iGeneric], innerScope2));
            }

            if (this is StructTypeConverter)
            {
                arguments.Add(
                    new IndexExpression(
                        null,
                        innerScope2,
                        new ThisExpression(null, innerScope2),
                        new IdentifierExpression(
                            this.Resolve(
                                this.Context.KnownReferences.BoxedValueField), innerScope2)));
            }
            else
            {
                arguments.Add(new ThisExpression(null, innerScope2));
            }

            for (int iArg = 0; iArg < method.Parameters.Count; iArg++)
            {
                arguments.Add(
                    new IdentifierExpression(
                        innerScope2.ParameterIdentifiers[iArg + method.GenericParameters.Count],
                        innerScope2));
            }

            Expression methodReference = IdentifierExpression.Create(
                null,
                innerScope2,
                this.ResolveStaticMember(method));

            Expression methodCallExpression = new MethodCallExpression(
                null,
                innerScope2,
                methodReference,
                arguments);

            funcExpression.AddStatement(
                new ReturnStatement(
                    null,
                    innerScope2,
                    methodCallExpression));

            return funcExpression;
        }

        /// <summary>
        /// Gets the method converter.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns></returns>
        protected MethodConverter GetMethodConverter(MethodDefinition methodDefinition)
        {
            MethodConverter returnValue;
            this.implementedMethods.TryGetValue(methodDefinition, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified field definition is field implemented.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <returns>
        /// <c>true</c> if the specified field definition is field implemented otherwise, <c>false</c>.
        /// </returns>
        protected bool IsFieldImplemented(FieldDefinition fieldDefinition)
        {
            return this.implementedFields.Contains(fieldDefinition);
        }

        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <returns></returns>
        protected virtual void CreateConstructor(List<Statement> statements)
        {
            // If a class is extended and it has ignore namespace, the constructor is
            // already defined.
            if (!this.Context.IsExtended(this.typeDefinition)
                && !this.Context.IsPsudoType(this.typeDefinition))
            {
                var typeExpr = this.ResolveTypeToExpression(this.typeDefinition, this.Scope);
                var identifierTypeExpr = typeExpr as IdentifierExpression;
                IIdentifier typeIdentifier = identifierTypeExpr != null
                    ? identifierTypeExpr.Identifier
                    : null;

                var constructorExpr = this.CreateConstructorFunction(typeIdentifier);

                // Add the constructor.
                if (typeIdentifier != null)
                {
                    statements.Add(
                        new ExpressionStatement(
                            null,
                            this.Scope,
                            constructorExpr));
                }
                else
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            this.ResolveTypeToExpression(this.typeDefinition, this.Scope),
                            constructorExpr));
                }
            }

            if (this.IsGenericLike)
            {
                // Add shortcut for the current type's self reference, so that we can
                // keep using self reference of created type.
                statements.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.Resolve(this.typeDefinition)),
                        this.ResolveTypeToExpression(
                            this.typeDefinition,
                            this.Scope)));

                this.localTypeReferencesInitialized.Add(this.typeDefinition);

                if (!this.typeDefinition.IsStatic())
                {
                    List<JST.Expression> typeExpressions = new List<Expression>();
                    for (int iGenericParam = 0; iGenericParam < this.typeDefinition.GenericParameters.Count; iGenericParam++)
                    {
                        typeExpressions.Add(
                            IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.Resolve(
                                    this.TypeDefinition.GenericParameters[iGenericParam])));
                    }

                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(this.typeDefinition)),
                                new IdentifierExpression(
                                    this.Resolve(
                                        this.Context.KnownReferences.GenericParametersField),
                                    this.Scope)),
                            new InlineNewArrayInitialization(
                                null,
                                this.Scope,
                                typeExpressions)));

                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(this.typeDefinition)),
                                new IdentifierExpression(
                                    this.Resolve(
                                        this.Context.KnownReferences.GenericClosureField),
                                    this.Scope)),
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.RuntimeManager.ResolveType(this.TypeDefinition))));
                }
            }

            // Now let's initialize the id of this type as well.
            if (!this.typeDefinition.IsStatic()
                && !this.context.IsPsudoType(this.TypeDefinition))
            {
                statements.Add(this.InitializeTypeId());
            }
        }

        /// <summary>
        /// Creates the constructor function.
        /// </summary>
        /// <returns></returns>
        protected virtual Expression CreateConstructorFunction(
            IIdentifier typeName)
        {
            IdentifierScope innerScope = new IdentifierScope(this.Scope);
            Expression objExpression = new FunctionExpression(
                null,
                this.Scope,
                innerScope,
                new List<IIdentifier>(),
                typeName ?? SimpleIdentifier.CreateScopeIdentifier(
                    this.Scope,
                    this.typeDefinition.FullName, false));

            return objExpression;
        }

        /// <summary>
        /// Initializes the type id.
        /// </summary>
        /// <returns>Expression for typeId initialization.</returns>
        protected Statement InitializeTypeId()
        {
            // Id is
            // 1. Normal: xyz
            // 2. Generic: xyz$GP1_GP2$
            Expression idExpression =
                new StringLiteralExpression(
                    this.Scope,
                    this.RuntimeManager.GetTypeId(this.typeDefinition) +
                        (this.IsGenericLike
                            ? "$"
                            : string.Empty));

            for (int genericParamIndex = 0; genericParamIndex < this.typeDefinition.GenericParameters.Count; genericParamIndex++)
            {
                if (genericParamIndex > 0)
                {
                    idExpression = new BinaryExpression(
                        null,
                        this.Scope,
                        BinaryOperator.Plus,
                        idExpression,
                        new StringLiteralExpression(
                            this.Scope,
                            "_"));
                }
                idExpression = new BinaryExpression(
                    null,
                    this.Scope,
                    BinaryOperator.Plus,
                    idExpression,
                    new IndexExpression(
                        null,
                        this.Scope,
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.Resolve(this.typeDefinition.GenericParameters[genericParamIndex])),
                        new IdentifierExpression(
                            this.RuntimeManager.JSBaseObjectScopeManager.TypeId,
                            this.Scope)));
            }

            if (this.IsGenericLike)
            {
                idExpression = new BinaryExpression(
                    null,
                    this.Scope,
                    BinaryOperator.Plus,
                    idExpression,
                    new StringLiteralExpression(
                        this.Scope,
                        "$"));
            }

            // When supporting Generics, we may want to compute the id based on the
            // generic arguments.
            return ExpressionStatement.CreateAssignmentExpression(
                new IndexExpression(
                    null,
                    this.Scope,
                    IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.Resolve(this.typeDefinition)),
                    new IdentifierExpression(this.RuntimeManager.JSBaseObjectScopeManager.TypeId, this.Scope)),
                idExpression);
        }

        /// <summary>
        /// Gets the generic type reference initializer.
        /// </summary>
        protected virtual void GenerateLocalTypeRefInitializer(List<Statement> statements)
        {
            if (!this.IsGenericLike)
            {
                return;
            }

            List<Statement> functionBody = new List<Statement>();
            IdentifierScope innerScope = new IdentifierScope(this.Scope);

            // Initialize the typeReferences.
            foreach (var typeReference in this.localTypeReferences)
            {
                if (!this.localTypeReferencesInitialized.Contains(
                    typeReference.Key))
                {
                    functionBody.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                                    new IdentifierExpression(
                                        typeReference.Value,
                                        innerScope),
                                    this.ResolveTypeToExpression(
                                        typeReference.Key,
                                        innerScope,
                                        new BooleanLiteralExpression(this.Scope, true))));

                    this.localTypeReferencesInitialized.Add(typeReference.Key);
                }
            }

            // If we have typeReferences to initialize, GenerateStaticConstructor when called
            // from outside, will redirect call to TypeReference initializer.
            Statement staticConstructorCall =
                this.GenerateStaticConstructorCall(
                    this.typeDefinition,
                    innerScope);

            if (staticConstructorCall != null)
            {
                functionBody.Add(staticConstructorCall);
            }

            if (functionBody.Count > 0)
            {
                this.hasTypeRefInit = true;

                // Add condition so that we don't do initialization twice.
                IIdentifier initTracker = SimpleIdentifier.CreateScopeIdentifier(
                    this.Scope,
                    "__initTracker",
                    false);

                List<Statement> ifStatements = new List<Statement>();
                ifStatements.Add(new ReturnStatement(null, innerScope, null));
                IfBlockStatement ifBlock = new IfBlockStatement(
                    null,
                    innerScope,
                    new IdentifierExpression(initTracker, innerScope),
                    new ScopeBlock(
                        null,
                        innerScope,
                        ifStatements),
                    null);

                functionBody.Insert(0, ifBlock);
                functionBody.Insert(
                    1,
                    ExpressionStatement.CreateAssignmentExpression(
                        new IdentifierExpression(initTracker, innerScope),
                        new BooleanLiteralExpression(innerScope, true)));

                FunctionExpression funcExpression = new FunctionExpression(
                    null,
                    this.Scope,
                    innerScope,
                    innerScope.ParameterIdentifiers,
                    null);

                funcExpression.AddStatements(functionBody);

                IndexExpression initializerMethodExpression =
                    new IndexExpression(
                        null,
                        this.Scope,
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.Resolve(this.typeDefinition)),
                        new IdentifierExpression(
                            this.RuntimeManager.JSBaseObjectScopeManager.GenericTypeRefInitializer,
                            this.Scope));

                statements.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        initializerMethodExpression,
                        funcExpression));
            }
        }

        /// <summary>
        /// Touches the register reqs.
        /// </summary>
        private void TouchRegisterReqs()
        {
            if (this.typeDefinition.IsStatic())
            { return; }

            this.RuntimeManager.Resolve(this.TypeRegistrationMethod);
            if (this.typeDefinition.BaseType != null)
            {
                this.ResolveTypeToExpression(
                    this.typeDefinition.BaseType,
                    this.Scope);
            }
        }

        /// <summary>
        /// Generates the static constructor call.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// Call to static constructor (if static constructor exists), else null
        /// </returns>
        protected Statement GenerateStaticConstructorCall(
            TypeReference typeReference,
            IdentifierScope scope = null)
        {
            scope = scope ?? this.runtimeScopeManager.Scope;

            MethodDefinition staticConstructor =
                TypeConverter.HasStaticConstructor(this.typeDefinition);

            if (staticConstructor == null)
            {
                return null;
            }

            MethodReference staticConstructorReference = staticConstructor;

            Expression expression =
                IdentifierExpression.Create(
                    null,
                    scope,
                    this.ResolveStaticMember(staticConstructorReference));

            expression = new MethodCallExpression(
                null,
                scope,
                expression,
                new Expression[0]);

            return new ExpressionStatement(
                null,
                scope,
                expression);
        }

        /// <summary>
        /// Generates the local type ref init call.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>
        /// If there is typeInit function, the return typeInit function else null.
        /// </returns>
        protected Statement GenerateLocalTypeRefInitCall(
            TypeReference typeReference,
            IdentifierScope scope)
        {
            if (this.hasTypeRefInit)
            {
                return new ExpressionStatement(
                    null,
                    scope,
                    new MethodCallExpression(
                        null,
                        scope,
                        new IndexExpression(
                            null,
                            scope,
                            IdentifierExpression.Create(
                                null,
                                scope,
                                this.Resolve(typeReference)),
                            new IdentifierExpression(
                                this.RuntimeManager.JSBaseObjectScopeManager.GenericTypeRefInitializer,
                                scope)),
                        new Expression[] { }));
            }

            return null;
        }

        /// <summary>
        /// Resolves the type to expression.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression for the type.</returns>
        private Expression ResolveTypeToExpression(
            TypeReference typeReference,
            IdentifierScope scope,
            Expression initializeRefsAndStaticCtor = null)
        {
            // If we are resolving paramDef which points to typeDefinition
            // replace it with localTypeReference. This is done so that if typeDefinition
            // is generic, we could use GenericTypeDefinition.
            if (typeReference.IsSame(this.typeDefinition))
            {
                typeReference = this.localTypeReference;
            }

            GenericParameter genericArgument = typeReference as GenericParameter;

            if (genericArgument != null)
            {
                return IdentifierExpression.Create(
                    null,
                    scope,
                    this.Resolve(typeReference));
            }

            return this.RuntimeManager.ResolveTypeToExpression(
                typeReference,
                scope,
                this.ResolveTypeToExpression,
                initializeRefsAndStaticCtor);
        }

        /// <summary>
        /// Initializes the prototype.
        /// </summary>
        /// <param name="statements">The statements.</param>
        private void InitializePrototype(List<Statement> statements)
        {
            IdentifierExpression prototypeExpression = new IdentifierExpression(
                this.RuntimeManager.ReusablePrototypeIdentifier,
                this.Scope);

            var prototypeInitializers = this.GetPrototypeInitializers(
                prototypeExpression);

            List<ExpressionStatement> virtualInitializers = this.InitializeVirtuals(
                prototypeExpression);

            if (prototypeInitializers.Count > 0
                || virtualInitializers.Count > 0)
            {
                if (this.basePrototypeExpression != null)
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            prototypeExpression,
                            this.basePrototypeExpression));

                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(this.typeDefinition)),
                                new IdentifierExpression(
                                    this.Resolve(
                                        this.cnvtKnownRefs.PrototypeField), this.Scope)),
                            prototypeExpression));
                }
                else
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            prototypeExpression,
                            new IndexExpression(
                                null,
                                this.Scope,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(this.typeDefinition)),
                                new IdentifierExpression(
                                    this.Resolve(this.cnvtKnownRefs.PrototypeField),
                                    this.Scope))));
                }

                for (int iPrototypeInitializer = 0; iPrototypeInitializer < prototypeInitializers.Count; iPrototypeInitializer++)
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                prototypeExpression,
                                prototypeInitializers[iPrototypeInitializer].Item1),
                            prototypeInitializers[iPrototypeInitializer].Item2));
                }

                statements.AddRange(virtualInitializers);
            }
            else
            {
                if (this.TypeDefinition.BaseType != null
                    && this.basePrototypeExpression != null)
                {
                    statements.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                this.Scope,
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(this.typeDefinition)),
                                new IdentifierExpression(
                                    this.Resolve(this.cnvtKnownRefs.PrototypeField),
                                    this.Scope)),
                            this.basePrototypeExpression));
                }
            }
        }

        /// <summary>
        /// Gets the base prototype expression.
        /// </summary>
        /// <returns></returns>
        private Expression GetBasePrototypeExpression()
        {
            if (this.TypeDefinition.BaseType != null
                && !this.TypeDefinition.IsStatic()
                && !this.TypeDefinition.BaseType.IsSame(this.clrKnownRefs.Object)
                && !this.Context.IsExtended(this.typeDefinition)
                && !this.Context.IsPsudoType(this.typeDefinition))
            {
                return new NewObjectExpression(
                        null,
                        this.Scope,
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            this.Resolve(this.typeDefinition.BaseType)));
            }

            return null;
        }

        /// <summary>
        /// Converts the type of the generic.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        private void ConvertGenericType(
            List<Statement> returnValue,
            Action<TypeConverter, List<Statement>> addDependenciesCallback)
        {
            IList<IIdentifier> identifiers = this.RuntimeManager.ResolveType(this.typeDefinition);

            IIdentifier functionName = identifiers.Count == 1 ? identifiers[0] : null;
            FunctionExpression genericTypeInitFunction = new FunctionExpression(
                null,
                this.RuntimeManager.Scope,
                this.Scope,
                this.Scope.ParameterIdentifiers,
                functionName);

            if (functionName == null)
            {
                // Initialize generic type with function that will be used to create generic type.
                returnValue.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        IdentifierExpression.Create(
                            null,
                            this.RuntimeManager.Scope,
                            this.RuntimeManager.ResolveType(this.typeDefinition)),
                        genericTypeInitFunction));
            }
            else
            {
                returnValue.Add(
                    new ExpressionStatement(
                        null,
                        this.RuntimeManager.Scope,
                        genericTypeInitFunction));
            }

            // Let's reuse returnValue to maintain list of all the statements within function body.
            returnValue = new List<Statement>();

            this.CheckOrCreateGenericTypeSlot(returnValue);

            // Generate constructor for this Type.
            this.CreateConstructor(returnValue);

            // We know that all the references to base interfaces that we may have, we will need
            // to refer them multiple times (atleast once for registerClass, and then virtual
            // function mappings). So let's create initializers for all of them right here.
            if (!this.RuntimeManager.DependencyAnalyzer.TypeToTypeReferences.ContainsKey(this.typeDefinition))
            {
                this.RuntimeManager.DependencyAnalyzer.AddTypeForAnalysis(this.typeDefinition);
            }

            foreach (var dependentTypeBase in this.RuntimeManager.DependencyAnalyzer.TypeToTypeReferences[this.typeDefinition])
            {
                if (!(dependentTypeBase is GenericParameter)
                    && dependentTypeBase.GetGenericTypeScope().HasValue
                    && (!this.isSelectiveInit
                        || this.RuntimeManager.DependencyAnalyzer.TypeToTypeReferences.ContainsKey(
                            dependentTypeBase.Resolve())))
                {
                    returnValue.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                                IdentifierExpression.Create(
                                    null,
                                    this.Scope,
                                    this.Resolve(dependentTypeBase)),
                                this.ResolveTypeToExpression(
                                    dependentTypeBase,
                                    this.Scope,
                                    new IdentifierExpression(
                                        this.Scope.ParameterIdentifiers.Last(),
                                        this.Scope))));

                    this.localTypeReferencesInitialized.Add(dependentTypeBase);
                }
            }

            // Now let's convert the entire body.
            this.ConvertBody(returnValue, (a, b) => { return; });

            // Now let's create local paramDef initializer function that will be called
            // to make sure this generic's local type references are initialized before this
            // class is being used.
            this.GenerateLocalTypeRefInitializer(returnValue);

            if (this.hasTypeRefInit)
            {
                List<Statement> initializerStatements = new List<Statement>();
                initializerStatements.Add(
                    this.GenerateLocalTypeRefInitCall(
                        this.localTypeReference,
                        this.Scope));

                IfBlockStatement ifBlockStatement = new IfBlockStatement(
                    null,
                    this.Scope,
                    new IdentifierExpression(
                        this.Scope.ParameterIdentifiers.Last(),
                        this.Scope),
                    new ScopeBlock(
                        null,
                        this.Scope,
                        initializerStatements),
                    null);

                returnValue.Add(ifBlockStatement);
            }

            returnValue.Add(
                new ReturnStatement(
                    null,
                    this.Scope,
                    IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.Resolve(this.TypeDefinition))));

            // Add all the statements to genericTypes.
            genericTypeInitFunction.AddStatements(returnValue);
        }

        /// <summary>
        /// Checks the or create type slot.
        /// </summary>
        /// <param name="statements">The statements.</param>
        private void CheckOrCreateGenericTypeSlot(List<Statement> statements)
        {
            Expression condition = null;
            Expression typePath = null;
            List<Expression> typePathPart = new List<Expression>();

            for (int argumentIndex = 0; argumentIndex < this.typeDefinition.GenericParameters.Count; argumentIndex++)
            {
                if (typePath == null)
                {
                    typePath = IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.runtimeScopeManager.ResolveType(this.typeDefinition));
                }

                typePath = new IndexExpression(
                    null,
                    this.Scope,
                    typePath,
                    new IndexExpression(
                        null,
                        this.Scope,
                        new IdentifierExpression(
                            this.Scope.ParameterIdentifiers[argumentIndex],
                            this.Scope),
                        new IdentifierExpression(
                            this.RuntimeManager.JSBaseObjectScopeManager.TypeId,
                            this.Scope)));

                typePathPart.Add(typePath);

                if (condition == null)
                {
                    condition = typePath;
                }
                else
                {
                    condition =
                        new BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.LogicalAnd,
                            condition,
                            typePath);
                }
            }

            ScopeBlock ifBlock = new ScopeBlock(
                null,
                this.Scope,
                new List<Statement>());

            ifBlock.AddStatement(
                new ReturnStatement(
                    null,
                    this.Scope,
                    typePath));

            // Let's create if statement that will return type with provided generic arguments if the type exists.
            IfBlockStatement ifStatement = new IfBlockStatement(
                null,
                this.Scope,
                condition,
                ifBlock,
                null);

            statements.Add(ifStatement);

            // Now let's create slot where newly created generic type with provided typeArgs is going to be stored at.
            for (int argumentIndex = 0; argumentIndex < typePathPart.Count - 1; argumentIndex++)
            {
                ifBlock = new ScopeBlock(
                    null,
                    this.Scope,
                    new List<Statement>());

                ifStatement = new IfBlockStatement(
                    null,
                    this.Scope,
                    new UnaryExpression(
                        null,
                        this.Scope,
                        UnaryOperator.LogicalNot,
                        typePathPart[argumentIndex]),
                    ifBlock,
                    null);

                ifBlock.AddStatement(
                    ExpressionStatement.CreateAssignmentExpression(
                            typePathPart[argumentIndex],
                            new InlineObjectInitializer(
                                null,
                                this.Scope)));

                statements.Add(ifBlock);
            }
        }

        /// <summary>
        /// Converts the body.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        private void ConvertBody(
            List<Statement> returnValue,
            Action<TypeConverter, List<Statement>> addDependenciesCallback)
        {
            Action<FieldDefinition> dele = this.AddFieldToImplementation;
            MulticastDelegate dele2 = dele;
            Delegate dele3 = dele;
            dele = (Action<FieldDefinition>)dele2;
            if (!this.typeDefinition.IsInterface)
            {
                this.InitializeStaticVariables(returnValue);
                this.InitializeStaticFunctions(returnValue);

                if (addDependenciesCallback != null)
                {
                    addDependenciesCallback(this, returnValue);
                }

                if (!this.typeDefinition.IsStatic()
                    && !this.context.IsPsudoType(this.typeDefinition)
                    && !this.context.IsJsonType(this.typeDefinition))
                {
                    this.InitializePrototype(returnValue);
                }
            }
            else if (addDependenciesCallback != null)
            {
                addDependenciesCallback(this, returnValue);
            }

            // Generate register instruction.
            // static classes do not have registration info.
            var typeRegistrationStatement = this.RegisterType();
            if (typeRegistrationStatement != null)
            {
                returnValue.Add(typeRegistrationStatement);
            }
        }

        /// <summary>
        /// Checks the implementation.
        /// </summary>
        private void CheckImplementation()
        {
            if (!this.isSelectiveInit)
            {
                foreach (var method in this.typeDefinition.Methods)
                {
                    this.AddMethodToImplementation(method);
                }

                foreach (var field in this.typeDefinition.Fields)
                {
                    if (this.context.IsPsudoType(this.typeDefinition))
                    {
                        continue;
                    }

                    this.AddFieldToImplementation(field);
                }
            }
        }

        /// <summary>
        /// Adds the static constructor implementation.
        /// </summary>
        private void AddStaticConstructorImplementation()
        {
            foreach (var method in this.typeDefinition.Methods)
            {
                if (method.IsStatic
                    && method.IsConstructor)
                {
                    this.AddMethodToImplementation(method);
                }
            }
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <returns>
        /// Returns statement to register the type.
        /// </returns>
        private Statement RegisterType()
        {
            if (this.TypeDefinition.IsStatic()
                || this.Context.IsPsudoType(this.TypeDefinition))
            {
                return null;
            }

            Expression typeNameExpression = null;
            Expression parentTypeExpression = null;
            List<Expression> interfaceExpressions = new List<Expression>();

            List<Expression> createArguments = new List<Expression>();

            if (!this.IsGenericLike)
            {
                typeNameExpression =
                    new StringLiteralExpression(
                        this.Scope,
                        typeDefinition.FullName);
            }
            else
            {
                // Let's create argument for this type's name.
                typeNameExpression =
                    new StringLiteralExpression(
                        this.Scope,
                        typeDefinition.FullName + "<");

                for (int paramIndex = 0; paramIndex < this.typeDefinition.GenericParameters.Count; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        typeNameExpression = new BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.Plus,
                            typeNameExpression,
                            new StringLiteralExpression(
                                this.Scope,
                                ","));
                    }

                    typeNameExpression = new BinaryExpression(
                        null,
                        this.Scope,
                        BinaryOperator.Plus,
                        typeNameExpression,
                        IdentifierExpression.Create(
                            null,
                            this.Scope,
                            new IIdentifier[] { this.typeScope.ParameterIdentifiers[paramIndex], this.RuntimeManager.JSBaseObjectScopeManager.TypeName }));
                }

                typeNameExpression = new BinaryExpression(
                    null,
                    this.Scope,
                    BinaryOperator.Plus,
                    typeNameExpression,
                    new StringLiteralExpression(this.Scope, ">"));
            }

            if (typeDefinition.BaseType != null)
            {
                parentTypeExpression =
                    IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.Resolve(this.typeDefinition.BaseType));
            }
            else
            {
                parentTypeExpression = new NullLiteralExpression(this.Scope);
            }

            if (typeDefinition.HasInterfaces)
            {
                foreach (var implementation in typeDefinition.Interfaces)
                {
                    // Only add the types that we are tracking. Anything extra is not needed
                    // and may generate undefined reference exception.
                    if (!this.isSelectiveInit
                        || this.RuntimeManager
                            .DependencyAnalyzer
                            .TypeToTypeReferences
                            .ContainsKey(
                                implementation.InterfaceType.Resolve()))
                    {
                        interfaceExpressions.Add(
                            IdentifierExpression.Create(
                                null,
                                this.Scope,
                                this.Resolve(
                                    implementation.InterfaceType)));
                    }
                }
            }

            List<Expression> arguments = new List<Expression>(
                this.RegisterTypeMethodArguments(
                        typeNameExpression,
                        parentTypeExpression,
                        interfaceExpressions));

            Expression typeReferenceExpression =
                    IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.Resolve(typeDefinition));

            return new ExpressionStatement(
                null,
                this.Scope,
                MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        typeReferenceExpression,
                        this.TypeRegistrationMethod,
                        false),
                    arguments,
                    this,
                    this.RuntimeManager));
        }
    }
}