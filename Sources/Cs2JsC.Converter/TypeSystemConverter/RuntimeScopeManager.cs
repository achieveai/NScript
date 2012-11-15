//-----------------------------------------------------------------------
// <copyright file="RuntimeScopeManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using Cs2JsC.CLR;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for RuntimeInitializer
    /// </summary>
    public class RuntimeScopeManager
    {
        /// <summary>
        /// Regex pattern to find all the generic arg counts. This is used in replacing
        /// this annoying pattern to create friendly names for generics.
        /// </summary>
        private const string genericArgCountFinderPattern = @"`\d*";

        /// <summary>
        /// Regex to find all the generic arg counts. This is used in replacing
        /// this annoying pattern to create friendly names for generics.
        /// </summary>
        private static readonly Regex genericArgCountFinder =
            new Regex(
                RuntimeScopeManager.genericArgCountFinderPattern,
                RegexOptions.Compiled);

        /// <summary>
        /// Regex pattern to find all the generic arg counts. This is used in replacing
        /// this annoying pattern to create friendly names for generics.
        /// </summary>
        private const string nestedGenericArgCountFinderPattern = @"`\d*\$";

        /// <summary>
        /// Regex to find all the generic arg counts. This is used in replacing
        /// this annoying pattern to create friendly names for generics.
        /// </summary>
        private static readonly Regex nestedGenericArgCountFinder =
            new Regex(
                RuntimeScopeManager.nestedGenericArgCountFinderPattern,
                RegexOptions.Compiled);

        /// <summary>
        /// Backing field for Context.
        /// </summary>
        private readonly ConverterContext context;

        /// <summary>
        /// Backing field for ReusablePrototypeIdentifier;
        /// </summary>
        private readonly IIdentifier reusablePrototypeIdentifier;

        /// <summary>
        /// Global namespace manager.
        /// </summary>
        private readonly NamespaceManager globalNamespaceManager =
            new NamespaceManager();

        /// <summary>
        /// Backing field for Dependency Analyzer.
        /// </summary>
        private readonly DependencyAnalyzer dependencyAnalyzer;

        /// <summary>
        /// Backing field for ReferenceIdentifierManager
        /// </summary>
        private readonly ReferenceIdentifierManager referenceIdentifierManager =
            new ReferenceIdentifierManager();

        /// <summary>
        /// mapping from type reference to identifier used for the type.
        /// </summary>
        private readonly Dictionary<TypeReference, IList<IIdentifier>> typeReferenceIdentifiers =
            new Dictionary<TypeReference, IList<IIdentifier>>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing field to keep track all the script alias to identifier sequence.
        /// </summary>
        private readonly Dictionary<string, IList<IIdentifier>> scriptAliasIdentifiers =
            new Dictionary<string, IList<IIdentifier>>();

        /// <summary>
        /// Backing field for global methodName identifiers.
        /// </summary>
        private readonly Dictionary<MethodReference, IIdentifier> methodNameIdentifier =
            new Dictionary<MethodReference, IIdentifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Scopes for all the types.
        /// </summary>
        private readonly Dictionary<TypeDefinition, TypeScopeManager> typeScopes =
            new Dictionary<TypeDefinition, TypeScopeManager>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing field for JSBaseObjectScopeManager
        /// </summary>
        private readonly JSBaseObjectIdentifierManager jsBaseObjectScopeManager;

        /// <summary>
        /// Backing field for typeIdMap.
        /// </summary>
        private readonly Dictionary<TypeDefinition, string> typeIdMap =
            new Dictionary<TypeDefinition, string>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Map from static member to identifier at global level.
        /// </summary>
        private readonly Dictionary<MemberReference, IIdentifier> staticMemberMap =
            new Dictionary<MemberReference, IIdentifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Map from static factory to identifier at global level.
        /// </summary>
        private readonly Dictionary<MemberReference, IIdentifier> staticFactoryMap =
            new Dictionary<MemberReference, IIdentifier>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Backing field for knownIdentifiers.
        /// </summary>
        private readonly Dictionary<string, IIdentifier> knownIdentifiers =
            new Dictionary<string, IIdentifier>();

        /// <summary>
        /// All the typeDefinitions that have been used. This is used during conversion
        /// in entryPoint convert mode.
        /// </summary>
        private readonly Dictionary<TypeDefinition, TypeConverter> typesDefinitionsUsed =
            new Dictionary<TypeDefinition, TypeConverter>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Type references used.
        /// </summary>
        private readonly HashSet<TypeReference> typeReferencesUsed =
            new HashSet<TypeReference>();

        /// <summary>
        /// Used method definitions.
        /// </summary>
        private readonly HashSet<MethodReference> virtualMethodReferencesUsed =
            new HashSet<MethodReference>(MemberReferenceComparer.Instance);

        private readonly HashSet<MethodDefinition> virtualMethodsUsed =
            new HashSet<MethodDefinition>(MemberReferenceComparer.Instance);

        /// <summary>
        /// members that have been processed (used in entryPoint convert).
        /// </summary>
        private readonly HashSet<MemberReference> membersProcessed =
            new HashSet<MemberReference>(MemberReferenceComparer.Instance);

        /// <summary>
        /// Queue for typeReferences to process (used in entryPoint convert).
        /// </summary>
        private readonly Queue<TypeReference> usedTypeReferencesToProcess =
            new Queue<TypeReference>();

        /// <summary>
        /// Queue for fields to process (used in entryPoint convert).
        /// </summary>
        private readonly Queue<MemberReference> usedMembersToProcess =
            new Queue<MemberReference>();

        /// <summary>
        /// Used to keep track of all the typeIds used.
        /// </summary>
        private int typeIdsUsed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeScopeManager"/> class.
        /// </summary>
        public RuntimeScopeManager(ConverterContext context)
        {
            this.context = context;
            this.jsBaseObjectScopeManager = new JSBaseObjectIdentifierManager(this);
            this.dependencyAnalyzer = new DependencyAnalyzer(this.context);
            this.reusablePrototypeIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                this.globalNamespaceManager.Scope,
                "ptyp_",
                false);
            this.InitializeKnownGlobalIdentifiers();
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope
        {
            get
            {
                return this.globalNamespaceManager.Scope;
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ConverterContext Context
        { get { return this.context; } }

        /// <summary>
        /// Gets the reusable prototype identifier.
        /// </summary>
        /// <value>The reusable prototype identifier.</value>
        public IIdentifier ReusablePrototypeIdentifier
        {
            get
            {
                return this.reusablePrototypeIdentifier;
            }
        }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public ReferenceIdentifierManager ReferenceManager
        {
            get { return this.referenceIdentifierManager; }
        }

        /// <summary>
        /// Gets the dependency analyzer.
        /// </summary>
        public DependencyAnalyzer DependencyAnalyzer
        {
            get
            {
                return this.dependencyAnalyzer;
            }
        }

        /// <summary>
        /// Gets the JS base object scope manager.
        /// </summary>
        public JSBaseObjectIdentifierManager JSBaseObjectScopeManager
        {
            get { return this.jsBaseObjectScopeManager; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [implement instance as static].
        /// </summary>
        /// <value>
        /// <c>true</c> if [implement instance as static]; otherwise, <c>false</c>.
        /// </value>
        public bool ImplementInstanceAsStatic
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>List of statements.</returns>
        public List<Statement> Convert(IList<TypeDefinition> types)
        {
            List<TypeConverter> typeConverters = new List<TypeConverter>();
            Dictionary<TypeDefinition, TypeConverter> convertersAdded = new Dictionary<TypeDefinition, TypeConverter>();

            foreach (var typeDefinition in types)
            {
                this.DependencyAnalyzer.AddTypeForAnalysis(typeDefinition);
                if (convertersAdded.ContainsKey(typeDefinition))
                {
                    continue;
                }

                TypeConverter typeConverter = TypeConverter.Create(
                    this,
                    typeDefinition);

                convertersAdded.Add(typeDefinition, typeConverter);
                typeConverters.Add(typeConverter);
            }

            List<Statement> returnValue = new List<Statement>();
            HashSet<TypeDefinition> typeRegistered = new HashSet<TypeDefinition>();
            HashSet<TypeReference> typeReferencesRegistered = new HashSet<TypeReference>(new MemberReferenceComparer());
            HashSet<TypeDefinition> typesToRegister = new HashSet<TypeDefinition>(types);
            List<TypeReference> typesToConvert = this.DependencyAnalyzer.GetTypeReferenceDependencies(types);

            // Here we will register all the concrete types and their dependencies.
            foreach (var typeReference in typesToConvert)
            {
                if (typeRegistered.Contains(typeReference.Resolve()))
                {
                    continue;
                }

                // Let's emit the type.
                returnValue.AddRange(
                    convertersAdded[typeReference.Resolve()].Convert(
                        (typeConverter, statements) =>
                        {
                            foreach (var dependentTypeReference in
                                dependencyAnalyzer.TypeToTypeReferences[typeConverter.TypeDefinition])
                            {
                                IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
                                TypeReference fixedDependent = dependentTypeReference.FixGenericTypeArguments(
                                    genericArguments,
                                    null);

                                // We only need to initialize generic classes here. All the other types should
                                // be initialized anyways.
                                genericArguments = fixedDependent.GetGenericArguments();
                                if (genericArguments.Count == 0
                                    || typeReferencesRegistered.Contains(fixedDependent))
                                {
                                    continue;
                                }

                                typeReferencesRegistered.Add(fixedDependent);

                                statements.Add(
                                    new ExpressionStatement(
                                        null,
                                        this.Scope,
                                        this.InitializeGenericType(
                                            fixedDependent,
                                            this.Scope)));
                            }
                        }));

                // Once emited, remove the type from the convertersAdded dictionary.
                // This is done so that we don't reinit the type again (case of generics).
                typeRegistered.Add((TypeDefinition)typeReference.Resolve());
            }

            // There is chance that we may have missed some Generic types that are
            // only used within methods and so don't have direct dependency on types.
            // Let's add all those types as well.
            foreach (var typeConverter in typeConverters)
            {
                if (typeRegistered.Contains(typeConverter.TypeDefinition))
                {
                    continue;
                }

                returnValue.AddRange(typeConverter.Convert(null));
                typeRegistered.Add(typeConverter.TypeDefinition);
            }

            // Now let's initialize all the Generic global symbols that will be used somewhere inside the
            // methods.
            foreach (var typeReference in this.dependencyAnalyzer.GetOrderedGenericTypeDependencies(this.typeReferenceIdentifiers.Keys))
            {
                if (typeReferencesRegistered.Contains(typeReference)
                    || !typesToRegister.Contains(typeReference.Resolve())
                    || typeReference.GetGenericTypeScope().HasValue)
                {
                    continue;
                }

                typeReferencesRegistered.Add(typeReference);
                returnValue.Add(
                    new ExpressionStatement(
                        null,
                        this.Scope,
                        this.InitializeGenericType(
                            typeReference,
                            this.Scope)));
            }

            // here we need to initialize all the static constructor for
            // all these classes.
            foreach (var typeReference in typesToConvert)
            {
                Statement staticConstructor =
                    convertersAdded[typeReference.Resolve()]
                        .InitializeTypeStatement(
                            typeReference);

                if (staticConstructor != null)
                {
                    returnValue.Add(staticConstructor);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Convert to script based execution points from provided methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        public List<Statement> Convert(IList<MethodDefinition> methods)
        {
            // put all the members and types in to process queue.
            foreach (var methodDefinition in methods)
            {
                this.usedMembersToProcess.Enqueue(methodDefinition);
                this.usedTypeReferencesToProcess.Enqueue(methodDefinition.DeclaringType);
            }

            int typesGenerated = -1;
            this.WalkUsedDependencies();
            List<Statement> returnValue = new List<Statement>();

            do
            {
                returnValue.Clear();

                // Now we have all the typeDefinitions and typeReferences that we need to work with.
                // Now let's crate JST for the script.
                HashSet<TypeDefinition> typeRegistered = new HashSet<TypeDefinition>();
                HashSet<TypeReference> typeReferencesRegistered = new HashSet<TypeReference>();
                List<TypeReference> typesToConvert = this.DependencyAnalyzer.GetTypeReferenceDependencies(
                    this.typesDefinitionsUsed.Keys);
                List<TypeReference> typesInitializedInOrder = new List<TypeReference>();

                // Here we will register all the concrete types and their dependencies.
                foreach (var typeReference in typesToConvert)
                {
                    if (typeRegistered.Contains(typeReference.Resolve()))
                    {
                        continue;
                    }

                    // Let's emit the type.
                    returnValue.AddRange(
                        this.typesDefinitionsUsed[typeReference.Resolve()].Convert(
                            (typeConverter, statements) =>
                            {
                                foreach (TypeReference dependent in
                                    dependencyAnalyzer.TypeToTypeReferences[typeConverter.TypeDefinition])
                                {
                                    if (dependent == null
                                        || !this.typesDefinitionsUsed.ContainsKey(dependent.Resolve()))
                                    {
                                        continue;
                                    }

                                    var dependentTypeReference = dependent.FixGenericTypeArguments(
                                        typeReference.GetGenericArguments(),
                                        null);

                                    // We only need to initialize generic classes here. All the other types should
                                    // be initialized anyways.
                                    if (!(dependentTypeReference is TypeSpecification)
                                        || typeReferencesRegistered.Contains(dependentTypeReference))
                                    {
                                        continue;
                                    }

                                    typeReferencesRegistered.Add(dependentTypeReference);
                                    typesInitializedInOrder.Add(dependentTypeReference);

                                    statements.Add(
                                        new ExpressionStatement(
                                            null,
                                            this.Scope,
                                            this.InitializeGenericType(
                                                dependentTypeReference,
                                                this.Scope)));
                                }
                            }));

                    // Once emited, remove the type from the convertersAdded dictionary.
                    // This is done so that we don't reinit the type again (case of generics).
                    typeRegistered.Add(typeReference.Resolve());
                    typesInitializedInOrder.Add(typeReference);
                }

                // There is chance that we may have missed some Generic types that are
                // only used within methods and so don't have direct dependency on types.
                // Let's add all those types as well.
                foreach (var typeConverter in this.typesDefinitionsUsed.Values)
                {
                    if (typeRegistered.Contains(typeConverter.TypeDefinition))
                    {
                        continue;
                    }

                    returnValue.AddRange(typeConverter.Convert(null));
                    typeRegistered.Add(typeConverter.TypeDefinition);
                }

                // Now let's initialize all the Generic global symbols that will be used somewhere inside the
                // methods.
                foreach (var typeReference in this.dependencyAnalyzer.GetOrderedGenericTypeDependencies(this.typeReferenceIdentifiers.Keys))
                {
                    if (typeReferencesRegistered.Contains(typeReference)
                        || !this.typesDefinitionsUsed.ContainsKey(typeReference.Resolve())
                        || typeReference.GetGenericTypeScope().HasValue)
                    {
                        continue;
                    }

                    typeReferencesRegistered.Add(typeReference);
                    typesInitializedInOrder.Add(typeReference);
                    returnValue.Add(
                        new ExpressionStatement(
                            null,
                            this.Scope,
                            this.InitializeGenericType(
                                typeReference,
                                this.Scope)));
                }

                // here we need to initialize all the static constructor for
                // all these classes.
                foreach (var typeReference in typesInitializedInOrder)
                {
                    if (!this.typesDefinitionsUsed.ContainsKey(typeReference.Resolve()))
                    { continue; }

                    Statement staticConstructor =
                        this.typesDefinitionsUsed[typeReference.Resolve()]
                            .InitializeTypeStatement(typeReference);

                    if (staticConstructor != null)
                    {
                        returnValue.Add(staticConstructor);
                    }
                }

                typesGenerated = this.typesDefinitionsUsed.Count;
                this.WalkUsedDependencies();
                foreach (var typeDefConverter in this.typesDefinitionsUsed.Values)
                {
                    typeDefConverter.ClearVariableInitializedTracking();
                }
            } while (typesGenerated < this.typesDefinitionsUsed.Count);

            return returnValue;
        }

        /// <summary>
        /// Calculates the name of the friendly type reference.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <param name="strBuilder">The STR builder.</param>
        /// <param name="worker">The worker.</param>
        public static void CalculateFriendlyTypeReferenceName(
            TypeReference typeReference,
            StringBuilder strBuilder,
            Action<TypeReference, StringBuilder> worker)
        {
            worker(typeReference, strBuilder);

            IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
            if (genericArguments != null
                && genericArguments.Count > 0
                && !typeReference.IsDefinition)
            {
                strBuilder.Append('<');

                for (int paramIndex = 0; paramIndex < genericArguments.Count; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        strBuilder.Append(',');
                    }

                    RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                        genericArguments[0],
                        strBuilder,
                        worker);
                }

                strBuilder.Append('>');
            }
        }

        /// <summary>
        /// Gets the name of the split.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Tuple with namespace and name broken up.</returns>
        public static Tuple<string, string> GetSplitName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            int dotIndex = name.LastIndexOf('.');

            if (dotIndex > 0)
            {
                return Tuple.Create(
                    name.Substring(0, dotIndex),
                    name.Substring(dotIndex + 1));
            }

            return Tuple.Create(
                (string)null,
                name);
        }

        /// <summary>
        /// Gets the known identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Identifier associated with known name</returns>
        public IIdentifier GetKnownIdentifier(string name)
        {
            IIdentifier returnValue = null;
            this.knownIdentifiers.TryGetValue(name, out returnValue);
            return returnValue;
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public IList<IIdentifier> ResolveType(TypeReference typeReference)
        {
            IList<IIdentifier> returnValue;

            if (this.context.IsPsudoType((TypeDefinition)typeReference.GetDefinition()))
            {
                typeReference = this.context.ClrKnownReferences.Object;
            }

            if (!this.typeReferenceIdentifiers.TryGetValue(
                typeReference,
                out returnValue))
            {
                IIdentifier resolvedIdentifier;
                var typeName = this.GetTypeName(typeReference);
                NamespaceManager nameSpace = this.globalNamespaceManager;
                bool isExtended = this.context.IsExtended(typeReference);

                // put in queue to process. In case of conversion using dependency
                // walk, this will be used to start converting these types as well.
                this.usedTypeReferencesToProcess.Enqueue(typeReference);

                // Create the name of identifier to be used.
                if (typeName != null)
                {
                    resolvedIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                        nameSpace.Scope,
                        isExtended
                            ? typeName.Item2
                            : (typeName.Item1 + '.' + typeName.Item2).Replace('.', '_'),
                        isExtended);

                    returnValue = new List<IIdentifier>();

                    returnValue.Add(resolvedIdentifier);

                    while (nameSpace.Parent != null)
                    {
                        returnValue.Insert(0, nameSpace.Identifier);
                        nameSpace = nameSpace.Parent;
                    }

                    returnValue = new ReadOnlyCollection<IIdentifier>(returnValue);
                }
                else
                {
                    returnValue = new ReadOnlyCollection<IIdentifier>(new IIdentifier[] { });
                }

                this.typeReferenceIdentifiers.Add(
                    typeReference,
                    returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the type to expression.
        /// </summary>
        /// <param name="typeReferenceBase">The type reference base.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression for the type.</returns>
        public Expression ResolveTypeToExpression(
            TypeReference typeReferenceBase,
            IdentifierScope scope,
            Func<TypeReference, IdentifierScope, Expression, Expression> resolveTypeToExpressionHelper,
            Expression initializeRefsAndStaticCtor = null)
        {
            TypeReference typeReference = (TypeReference)typeReferenceBase;

            if (typeReferenceBase.GetGenericTypeScope().HasValue)
            {
                if (initializeRefsAndStaticCtor == null)
                {
                    Expression rv = IdentifierExpression.Create(
                    null,
                    scope,
                    this.ResolveType(typeReference.Resolve()));

                    IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
                    if (genericArguments != null)
                    {
                        foreach (var genericParam in genericArguments)
                        {
                            rv = new IndexExpression(
                                null,
                                scope,
                                rv,
                                new IndexExpression(
                                    null,
                                    scope,
                                    resolveTypeToExpressionHelper(genericParam, scope, initializeRefsAndStaticCtor),
                                    new IdentifierExpression(
                                        this.JSBaseObjectScopeManager.TypeId, scope)));
                        }
                    }

                    return rv;
                }
                else
                {
                    List<Expression> arguments = new List<Expression>();

                    IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
                    if (genericArguments != null)
                    {
                        foreach (var genericParam in genericArguments)
                        {
                            arguments.Add(
                                resolveTypeToExpressionHelper(
                                    genericParam,
                                    scope,
                                    initializeRefsAndStaticCtor));
                        }
                    }

                    arguments.Add(initializeRefsAndStaticCtor);

                    if (typeReference.IsArray)
                    {
                        ArrayType arrayType = (ArrayType)typeReference;
                        GenericInstanceType genericArrayType = new GenericInstanceType(
                            this.context.KnownReferences.ArrayImplGeneric);

                        genericArrayType.GenericArguments.Add(arrayType.ElementType);

                        typeReference = genericArrayType;
                    }

                    return new MethodCallExpression(
                        null,
                        scope,
                        IdentifierExpression.Create(
                            null,
                            scope,
                            this.ResolveType(typeReference.Resolve())),
                        arguments);
                }
            }
            else
            {
                return IdentifierExpression.Create(
                    null,
                    scope,
                    this.ResolveType(typeReference));
            }
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="propertyReference">The property reference.</param>
        /// <returns>Method name identifier for given method.</returns>
        public IIdentifier Resolve(
            PropertyReference propertyReference)
        {
            this.usedTypeReferencesToProcess.Enqueue(propertyReference.DeclaringType);
            this.usedMembersToProcess.Enqueue(propertyReference);

            return this.GetTypeScope(propertyReference.DeclaringType).ResolveProperty(propertyReference.Resolve());
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="memberReference">The method reference.</param>
        /// <returns>Method name identifier for given method.</returns>
        public IIdentifier Resolve(
            FieldReference memberReference)
        {
            this.usedTypeReferencesToProcess.Enqueue(memberReference.DeclaringType);
            this.usedMembersToProcess.Enqueue(memberReference);

            return this.GetTypeScope(memberReference.DeclaringType).ResolveField(memberReference);
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="memberReference">The method reference.</param>
        /// <returns>Method name identifier for given method.</returns>
        public IIdentifier Resolve(
            MethodReference memberReference,
            bool forceStatic = false)
        {
            this.usedTypeReferencesToProcess.Enqueue(memberReference.DeclaringType);
            this.usedMembersToProcess.Enqueue(memberReference);

            if (memberReference.Name == ".ctor"
                && forceStatic
                && memberReference.Parameters.Count == 0)
            {
                // This means that we are looking at finding factory method for this constructor
                // This constructor has 0 arguments, so we can as well return GetDefaultConstructor
                // function on Type.
                return this.GetTypeScope(this.Context.ClrKnownReferences.TypeType)
                    .ResolveMethod(this.Context.KnownReferences.GetDefaultConstructorMethod, false);
            }

            return this.GetTypeScope(memberReference.DeclaringType).ResolveMethod(memberReference, forceStatic);
        }

        /// <summary>
        /// Resolves the virtual method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Expression for referencing the virtual method.</returns>
        public Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope,
            Func<TypeReference, IList<IIdentifier>> typeResolver)
        {
            if (methodReference.DeclaringType.Resolve().IsInterface)
            {
                return new BinaryExpression(
                    null,
                    scope,
                    BinaryOperator.Plus,
                    new IdentifierStringExpression(
                        null,
                        scope,
                        new IdentifierExpression(
                            this.ResolveVirtualMethodHelper(methodReference), scope),
                        string.Empty,
                        "_"),
                    this.GetTypeId(
                        methodReference.DeclaringType,
                        scope,
                        typeResolver));
            }
            else
            {
                return new IdentifierExpression(
                    this.ResolveVirtualMethodHelper(methodReference),
                    scope);
            }
        }

        /// <summary>
        /// Resolves the name of the function.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Method name identifier.</returns>
        public IIdentifier ResolveFunctionName(MethodReference methodReference)
        {
            IIdentifier returnValue;

            if (!this.methodNameIdentifier.TryGetValue(methodReference, out returnValue))
            {
                string suggestedName =
                    methodReference.DeclaringType.Resolve().FullName + "." + methodReference.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(this.Scope, suggestedName, false);

                this.methodNameIdentifier.Add(methodReference, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the script alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Identifier sequence for given alias.</returns>
        public IList<IIdentifier> ResolveScriptAlias(string alias)
        {
            IList<IIdentifier> returnValue;

            if (!this.scriptAliasIdentifiers.TryGetValue(
                alias,
                out returnValue))
            {
                IIdentifier resolvedIdentifier;
                NamespaceManager nameSpace = this.globalNamespaceManager;
                var typeName = RuntimeScopeManager.GetSplitName(alias);

                if (typeName.Item1 != null)
                {
                    nameSpace = nameSpace.GetNamespace(
                        typeName.Item1,
                        true);
                }

                resolvedIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                    nameSpace.Scope,
                    typeName.Item2,
                    true);

                returnValue = new List<IIdentifier>();

                returnValue.Add(resolvedIdentifier);

                while (nameSpace.Parent != null)
                {
                    returnValue.Insert(0, nameSpace.Identifier);
                    nameSpace = nameSpace.Parent;
                }

                returnValue = new ReadOnlyCollection<IIdentifier>(returnValue);
                this.scriptAliasIdentifiers.Add(
                    alias,
                    returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="methodDefinition">The method definition.</param>
        /// <returns>identifier for the method</returns>
        public IIdentifier ResolveStatic(MethodDefinition methodDefinition)
        {
            TypeDefinition typeDef = methodDefinition.DeclaringType.Resolve();
            if (typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve (static) method to global identifier if method is on generic type.");
            }

            this.usedTypeReferencesToProcess.Enqueue(methodDefinition.DeclaringType);
            this.usedMembersToProcess.Enqueue(methodDefinition);
            return this.ResolveFunctionName(methodDefinition);
        }

        /// <summary>
        /// Resolves the static factory for given constructor.
        /// </summary>
        /// <param name="constructor">The method definition.</param>
        /// <returns>identifier for the method</returns>
        public IIdentifier ResolveFactory(MethodDefinition constructor)
        {
            TypeDefinition typeDef = constructor.DeclaringType.Resolve();
            if (typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve (static) method to global identifier if method is on generic type.");
            }

            IIdentifier returnValue;

            if (!this.staticFactoryMap.TryGetValue(constructor, out returnValue))
            {
                string suggestedName =
                    constructor.DeclaringType.Resolve().FullName + "_factory";

                returnValue = SimpleIdentifier.CreateScopeIdentifier(this.Scope, suggestedName, false);

                this.staticFactoryMap.Add(constructor, returnValue);
            }

            this.usedTypeReferencesToProcess.Enqueue(constructor.DeclaringType);
            this.usedMembersToProcess.Enqueue(constructor);
            return returnValue;
        }

        /// <summary>
        /// Resolves the static field.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <returns>identifier for the field.</returns>
        public IIdentifier ResolveStatic(FieldDefinition fieldDefinition)
        {
            TypeDefinition typeDef = fieldDefinition.DeclaringType.Resolve();
            if (!fieldDefinition.IsStatic
                || typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve static field to global identifier if type is generic or member is not static.");
            }

            IIdentifier returnValue;

            if (!this.staticMemberMap.TryGetValue(fieldDefinition, out returnValue))
            {
                string suggestedName =
                    fieldDefinition.DeclaringType.Resolve().FullName + "." + fieldDefinition.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(this.Scope, suggestedName, false);

                this.staticMemberMap.Add(fieldDefinition, returnValue);
            }

            this.usedTypeReferencesToProcess.Enqueue(fieldDefinition.DeclaringType);
            this.usedMembersToProcess.Enqueue(fieldDefinition);
            return returnValue;
        }

        public IIdentifier ResolveStatic(PropertyDefinition propertyDefinition)
        {
            TypeDefinition typeDef = propertyDefinition.DeclaringType.Resolve();
            if (!propertyDefinition.IsStatic()
                || typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve static field to global identifier if type is generic or member is not static.");
            }

            IIdentifier returnValue;

            if (!this.staticMemberMap.TryGetValue(propertyDefinition, out returnValue))
            {
                string suggestedName =
                    propertyDefinition.DeclaringType.Resolve().FullName + "." + propertyDefinition.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(this.Scope, suggestedName, false);

                this.staticMemberMap.Add(propertyDefinition, returnValue);
            }

            this.usedTypeReferencesToProcess.Enqueue(propertyDefinition.DeclaringType);
            this.usedMembersToProcess.Enqueue(propertyDefinition);
            return returnValue;
        }

        /// <summary>
        /// Gets the type scope.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public TypeScopeManager GetTypeScope(TypeReference typeReference)
        {
            return this.GetTypeScope(typeReference.Resolve());
        }

        /// <summary>
        /// Gets the type scope.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns></returns>
        public TypeScopeManager GetTypeScope(TypeDefinition typeDefinition)
        {
            TypeScopeManager returnValue;

            if (!this.typeScopes.TryGetValue(typeDefinition, out returnValue))
            {
                if (typeDefinition.IsSameDefinition(this.context.ClrKnownReferences.Object))
                {
                    returnValue = this.jsBaseObjectScopeManager.ObjecTypeScopeManager;
                }
                else if (typeDefinition.IsSameDefinition(this.context.ClrKnownReferences.TypeType))
                {
                    returnValue = this.jsBaseObjectScopeManager.TypeTypeScopeManager;
                }
                else
                {
                    TypeScopeManager parentTypeScopeManager = null;

                    if (typeDefinition.BaseType != null)
                    {
                        parentTypeScopeManager = this.GetTypeScope(typeDefinition.BaseType);

                        returnValue = new TypeScopeManager(
                            typeDefinition,
                            parentTypeScopeManager);
                    }
                    else
                    {
                        returnValue = new TypeScopeManager(
                            this.context,
                            typeDefinition,
                            this.JSBaseObjectScopeManager.InstanceScope,
                            this.JSBaseObjectScopeManager.StaticScope);
                    }
                }

                this.typeScopes.Add(typeDefinition, returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the type id.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>Returns typeId for given type.</returns>
        public string GetTypeId(TypeReference typeReference)
        {
            // format for typeId is
            // curTypeId$[genericTypeId]_[genericTypeId]$
            StringBuilder typeId = new StringBuilder();
            IList<TypeReference> genericArguments = typeReference.GetGenericArguments();
            if (genericArguments.Count > 0)
            {
                typeId.AppendFormat("{0}$", this.GetTypeId(typeReference.Resolve()));
                for (int paramIndex = 0; paramIndex < genericArguments.Count; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        typeId.Append('_');
                    }

                    typeId.Append(this.GetTypeId(genericArguments[paramIndex]));
                }

                typeId.Append("$");
            }
            else
            {
                typeId.Append(this.GetTypeId(typeReference.Resolve()));
            }

            return typeId.ToString();
        }

        /// <summary>
        /// Gets the type id.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>Returns typeId for given type.</returns>
        public string GetTypeId(TypeDefinition typeDefinition)
        {
            string typeId;
            if (!this.typeIdMap.TryGetValue(typeDefinition, out typeId))
            {
                int newTypeId = System.Threading.Interlocked.Increment(
                    ref this.typeIdsUsed);
                typeId = Utils.GetCompressedInt(newTypeId);
                this.typeIdMap[typeDefinition] = typeId;
            }

            return typeId;
        }

        /// <summary>
        /// Gets the type id.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <returns>Expression that will provide access to typeId.</returns>
        public Expression GetTypeId(
            TypeReference typeReference,
            IdentifierScope scope,
            Func<TypeReference, IList<IIdentifier>> typeResolver)
        {
            GenericParameterType? typeScope = typeReference.GetGenericTypeScope();

            if (typeScope.HasValue)
            {
                return new IndexExpression(
                    null,
                    scope,
                    IdentifierExpression.Create(
                        null,
                        scope,
                        typeResolver(typeReference)),
                    new IdentifierExpression(this.JSBaseObjectScopeManager.TypeId, scope));
            }
            else
            {
                return new StringLiteralExpression(
                    scope,
                    this.GetTypeId((TypeReference)typeReference));
            }
        }

        /// <summary>
        /// Gets the type converter.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>Type converter for given typeDefinition</returns>
        public TypeConverter GetTypeConverter(
            TypeDefinition typeDefinition)
        {
            return this.typesDefinitionsUsed[typeDefinition];
        }

        /// <summary>
        /// Compares the identifiers.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>-1 if left is lessthan right, 0 if equals 1 if greater.</returns>
        private static int CompareNamespaceParts(
            IList<IIdentifier> left,
            IList<IIdentifier> right)
        {
            for (int i = 0; i < left.Count - 1 && i < right.Count - 1; i++)
            {
                if (left[i] != right[i])
                {
                    return left[i].SuggestedName.CompareTo(right[i].SuggestedName);
                }
            }

            return left.Count - right.Count;
        }

        /// <summary>
        /// Initializes the known global identifiers.
        /// </summary>
        private void InitializeKnownGlobalIdentifiers()
        {
            string[] globalNames = new String[]
            {
                "Function",
                "Array",
                "Number",
                "Boolean",
                "String",
                "Date",
                "parseInt",
                "parseFloat",
                "escape",
                "unescape",
                "eval",
                "decodeURI",
                "decodeURIComponent",
                "encodeURI",
                "encodeURIComponent",
                "isFinite",
                "isNaN",
                "Infinity",
                "NaN",
                "undefined",
                "window",
                "console",
                "arguments",
                "event",
            };

            foreach (string globalName in globalNames)
            {
                this.knownIdentifiers.Add(
                    globalName,
                    SimpleIdentifier.CreateScopeIdentifier(
                        this.Scope,
                        globalName,
                        true));
            }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns>Tuple of namespace and typeName</returns>
        private Tuple<string, string> GetTypeName(TypeReference typeReference)
        {
            // If we ever are asked to get typeName of a type that has genericTypeScope,
            // it means that we just need to return typeName of generic class and not
            // generic type with generic parameters.
            if (typeReference.GetGenericTypeScope() != null)
            {
                string typeName = this.context.GetTypeName(typeReference);

                return RuntimeScopeManager.GetSplitName(typeName);
            }
            else
            {
                StringBuilder typeNameBuilder = new StringBuilder();
                bool nameSpaceSet = false;
                string nameSpace = null;

                RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                    typeReference,
                    typeNameBuilder,
                    (typeDef, strBuilder) =>
                    {
                        string typeName = this.context.GetTypeName(typeDef);

                        if (typeName != null)
                        {
                            typeName = RuntimeScopeManager.nestedGenericArgCountFinder.Replace(
                                typeName,
                                "_");

                            typeName = RuntimeScopeManager.genericArgCountFinder.Replace(
                                typeName,
                                string.Empty);

                            var tuple = RuntimeScopeManager.GetSplitName(typeName);
                            if (!nameSpaceSet)
                            {
                                nameSpace = tuple.Item1;
                                nameSpaceSet = true;
                            }

                            strBuilder.Append(tuple.Item2);
                        }
                    });

                if (typeNameBuilder.Length > 0)
                {
                    return new Tuple<string, string>(
                        nameSpace,
                        typeNameBuilder.ToString());
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Initializes the type of the generic.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        private Expression InitializeGenericType(
            TypeReference typeReference,
            IdentifierScope scope)
        {
            // Here we will initialize the reference identifier for this generic type.
            // As part of this initialization, generic type will register itself.
            List<Expression> genericArguments = new List<Expression>();
            IList<TypeReference> genericTypeArgs = typeReference.GetGenericArguments();
            for (int genericArgIndex = 0; genericArgIndex < genericTypeArgs.Count; genericArgIndex++)
            {
                genericArguments.Add(
                    IdentifierExpression.Create(
                        null,
                        scope,
                        this.ResolveType(genericTypeArgs[genericArgIndex])));
            }

            return new BinaryExpression(
                null,
                scope,
                BinaryOperator.Assignment,
                IdentifierExpression.Create(
                    null,
                    scope,
                    this.ResolveType(typeReference)),
                new MethodCallExpression(
                    null,
                    scope,
                    IdentifierExpression.Create(
                        null,
                        scope,
                        this.ResolveType(typeReference.Resolve())),
                    genericArguments));
        }

        /// <summary>
        /// Resolves the virtual method internal.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Identifier for the method that points to virtual function.</returns>
        private IIdentifier ResolveVirtualMethodHelper(MethodReference methodReference)
        {
            this.usedTypeReferencesToProcess.Enqueue(methodReference.DeclaringType);
            this.usedMembersToProcess.Enqueue(methodReference);
            this.virtualMethodReferencesUsed.Add(methodReference);
            this.virtualMethodsUsed.Add(methodReference.Resolve());

            return this.GetTypeScope(methodReference.DeclaringType).ResolveVirtualMethod(methodReference);
        }

        /// <summary>
        /// Processes the types.
        /// </summary>
        private void ProcessTypes()
        {
            while (this.usedTypeReferencesToProcess.Count > 0)
            {
                var typeReference = this.usedTypeReferencesToProcess.Dequeue();
                if (this.typeReferencesUsed.Contains(typeReference)
                    || typeReference.IsGenericParameter)
                {
                    continue;
                }

                var typeDefinition = typeReference.Resolve();
                TypeConverter typeConverter;
                this.typeReferencesUsed.Add(typeReference);
                if (typeReference.IsGenericInstance)
                {
                    GenericInstanceType genericTypeReference = (GenericInstanceType)typeReference;
                    foreach (var param in genericTypeReference.GenericArguments)
                    {
                        this.usedTypeReferencesToProcess.Enqueue(param);
                    }
                }

                this.dependencyAnalyzer.AddTypeForAnalysis(typeDefinition);

                if (this.typesDefinitionsUsed.TryGetValue(typeDefinition, out typeConverter))
                {
                    continue;
                }

                if (typeDefinition == this.context.ClrKnownReferences.Object)
                {
                    this.usedMembersToProcess.Enqueue(
                        this.context.KnownReferences.ToStringMethod);
                }

                typeConverter = TypeConverter.Create(this, typeDefinition, true);
                this.typesDefinitionsUsed.Add(typeDefinition, typeConverter);

                // Now let's add all the dependencies of the class.
                this.dependencyAnalyzer.GetTypeReferenceDependencies(new TypeDefinition[] { typeDefinition })
                    .ForEach(tr => this.usedTypeReferencesToProcess.Enqueue(tr));
            }
        }

        /// <summary>
        /// Processes the members.
        /// </summary>
        private void ProcessMembers()
        {
            while (this.usedMembersToProcess.Count > 0)
            {
                var memberReference = this.usedMembersToProcess.Dequeue();
                var memberDefinition = memberReference.GetDefinition() as MemberReference;

                if (this.membersProcessed.Contains(memberDefinition))
                {
                    continue;
                }

                this.membersProcessed.Add(memberDefinition);

                // Before we proceed we need to make sure that typeQueue is all flushed.
                // We do this to gurantee that TypeConverter for this member is already
                // in our tracking list.
                this.ProcessTypes();

                // Currently we only process field and methods. We do this because
                // even properties will show up as methods anyways.
                MethodDefinition method = memberDefinition as MethodDefinition;
                if (method != null)
                {
                    this.typesDefinitionsUsed[method.DeclaringType].AddMethodToImplementation(method);
                    continue;
                }

                FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
                if (fieldDefinition != null)
                {
                    this.typesDefinitionsUsed[fieldDefinition.DeclaringType].AddFieldToImplementation(fieldDefinition);
                }
            }
        }

        /// <summary>
        /// Walks the used dependencies.
        /// </summary>
        private void WalkUsedDependencies()
        {
            while (this.usedMembersToProcess.Count > 0
                || this.usedTypeReferencesToProcess.Count > 0)
            {
                this.ProcessTypes();
                this.ProcessMembers();

                // Last step here is to make sure that if a virtual method is used, all it's implementations
                // are also used. Currently our approach is brute force, but later on this can be optimized.
                foreach (var typeDefinition in this.typesDefinitionsUsed.Keys)
                {
                    foreach (var kvPair in typeDefinition.GetInterfaceOverrides(this.Context.ClrContext))
                    {
                        var overridenMethod = kvPair.Key;
                        var method = kvPair.Value;

                        if (this.virtualMethodsUsed.Contains(overridenMethod.Resolve()))
                        {
                            if (!this.membersProcessed.Contains(method))
                            {
                                this.usedMembersToProcess.Enqueue(method);
                                if (overridenMethod.DeclaringType.Resolve().IsInterface
                                    && method.IsVirtual)
                                {
                                    // If this type is inheriting from interface and this method is
                                    // also virtual. In this case any call to method on interface will
                                    // also make virtual call on this method. So we need to add this method
                                    // to virtuals used set.
                                    this.virtualMethodReferencesUsed.Add(method);
                                    this.virtualMethodsUsed.Add(method);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}