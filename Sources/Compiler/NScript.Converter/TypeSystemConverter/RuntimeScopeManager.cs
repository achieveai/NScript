//-----------------------------------------------------------------------
// <copyright file="RuntimeScopeManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Text.RegularExpressions;

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
        /// Global namespace manager.
        /// </summary>
        private readonly NamespaceManager globalNamespaceManager =
            new NamespaceManager();

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
            Context = context;
            JSBaseObjectScopeManager = new JSBaseObjectIdentifierManager(this);
            DependencyAnalyzer = new DependencyAnalyzer(Context);
            ReusablePrototypeIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                globalNamespaceManager.Scope,
                "ptyp_",
                false);
            InitializeKnownGlobalIdentifiers();

            ImplementInstanceAsStatic = true;
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope => globalNamespaceManager.Scope;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public ConverterContext Context { get; }

        /// <summary>
        /// Gets the reusable prototype identifier.
        /// </summary>
        /// <value>The reusable prototype identifier.</value>
        public IIdentifier ReusablePrototypeIdentifier { get; }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public ReferenceIdentifierManager ReferenceManager { get; } =
            new ReferenceIdentifierManager();

        /// <summary>
        /// Gets the dependency analyzer.
        /// </summary>
        public DependencyAnalyzer DependencyAnalyzer { get; }

        /// <summary>
        /// Gets the JS base object scope manager.
        /// </summary>
        public JSBaseObjectIdentifierManager JSBaseObjectScopeManager { get; }

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

        public Statement GetVariableDeclarations()
        {
            var varList = new List<Expression>();
            foreach (var identifier in Scope.UsedLocalIdentifiers)
            {
                if (identifier.ShouldEnforceSuggestion
                    || identifier.IsFunctionName)
                {
                    continue;
                }

                varList.Add(new IdentifierExpression(identifier, Scope));
            }

            if (varList.Count > 0)
            {
                return new VarInitializerStatement(null, Scope, varList);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>List of statements.</returns>
        public List<Statement> ConvertForTests(IList<TypeDefinition> types, IRuntimeConverterPlugin[] plugins = null)
        {
            var typeConverters = new List<TypeConverter>();
            var convertersAdded = new Dictionary<TypeDefinition, TypeConverter>();

            foreach (var typeDefinition in types)
            {
                DependencyAnalyzer.AddTypeForAnalysis(typeDefinition);
                if (convertersAdded.ContainsKey(typeDefinition))
                {
                    continue;
                }

                var typeConverter = TypeConverter.Create(
                    this,
                    typeDefinition);

                convertersAdded.Add(typeDefinition, typeConverter);
                typeConverters.Add(typeConverter);
            }

            var returnValue = new List<Statement>();
            var typeRegistered = new HashSet<TypeDefinition>();
            var typeReferencesRegistered = new HashSet<TypeReference>(new MemberReferenceComparer());
            var typesToRegister = new HashSet<TypeDefinition>(types);
            var typesToConvert = DependencyAnalyzer.GetTypeReferenceDependencies(types);

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
                                DependencyAnalyzer.TypeToTypeReferences[typeConverter.TypeDefinition])
                            {
                                var genericArguments = typeReference.GetGenericArguments();
                                var fixedDependent = dependentTypeReference.FixGenericTypeArguments(
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
                                        Scope,
                                        InitializeGenericType(
                                            fixedDependent,
                                            Scope)));
                            }
                        }));

                // Once emited, remove the type from the convertersAdded dictionary.
                // This is done so that we don't reinit the type again (case of generics).
                typeRegistered.Add(typeReference.Resolve());
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
            foreach (var typeReference in DependencyAnalyzer.GetOrderedGenericTypeDependencies(typeReferenceIdentifiers.Keys))
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
                        Scope,
                        InitializeGenericType(
                            typeReference,
                            Scope)));
            }

            // here we need to initialize all the static constructor for
            // all these classes.
            foreach (var typeReference in typesToConvert)
            {
                var staticConstructor =
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
        public List<Statement> Convert(IList<MethodDefinition> methods, IRuntimeConverterPlugin[] plugins = null)
        {
            // put all the members and types in to process queue.
            foreach (var methodDefinition in methods)
            {
                usedMembersToProcess.Enqueue(methodDefinition);
                usedTypeReferencesToProcess.Enqueue(methodDefinition.DeclaringType);
            }

            var typesGenerated = -1;
            var membersGenerated = -1;
            WalkUsedDependencies();
            var returnValue = new List<Statement>();

            do
            {
                returnValue.Clear();

                // Now we have all the typeDefinitions and typeReferences that we need to work with.
                // Now let's crate JST for the script.
                var typeRegistered = new HashSet<TypeDefinition>();
                var typeReferencesRegistered = new HashSet<TypeReference>();
                var typesToConvert = DependencyAnalyzer.GetTypeReferenceDependencies(
                    typesDefinitionsUsed.Keys);
                var typesInitializedInOrder = new List<TypeReference>();

                // Here we will register all the concrete types and their dependencies.
                foreach (var typeReference in typesToConvert)
                {
                    if (typeRegistered.Contains(typeReference.Resolve()))
                    {
                        continue;
                    }

                    // Let's emit the type.
                    returnValue.AddRange(
                        typesDefinitionsUsed[typeReference.Resolve()].Convert(
                            (typeConverter, statements) =>
                            {
                                foreach (var dependent in
                                    DependencyAnalyzer.TypeToTypeReferences[typeConverter.TypeDefinition])
                                {
                                    if (dependent == null
                                        || dependent.IsGenericParameter
                                        || !typesDefinitionsUsed.ContainsKey(dependent.Resolve()))
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

                                    if (!Context.HasIgnoreNamespaceAttribute(dependentTypeReference.Resolve())
                                        && !Context.IsExtended(dependentTypeReference.Resolve()))
                                    {
                                        statements.Add(
                                            new ExpressionStatement(
                                                null,
                                                Scope,
                                                InitializeGenericType(
                                                    dependentTypeReference,
                                                    Scope)));
                                    }
                                }
                            }));

                    // Once emited, remove the type from the convertersAdded dictionary.
                    // This is done so that we don't reinit the type again (case of generics).
                    typeRegistered.Add(typeReference.Resolve());
                    typesInitializedInOrder.Add(typeReference);

                    // Investigate: why do we have to specifically resolve this type and why can't
                    // this be added in typeReferenceIdentifiers if it gets inside typesToConvert
                    // Result: The reason why we get here is because this type is referenced through
                    // second level Generic (or higher level generic).
                    ResolveType(typeReference);
                }

                if (plugins != null)
                {
                    foreach (var plugin in plugins)
                    {
                        var newMethods = plugin.GetMethodsToEmitPassN();
                        if (newMethods != null)
                        {
                            foreach (var methodRef in newMethods)
                            {
                                Resolve(methodRef);
                            }
                        }
                    }
                }

                // There is chance that we may have missed some Generic types that are
                // only used within methods and so don't have direct dependency on types.
                // Let's add all those types as well.
                foreach (var typeConverter in typesDefinitionsUsed.Values)
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
                foreach (var typeReference in DependencyAnalyzer.GetOrderedGenericTypeDependencies(typeReferenceIdentifiers.Keys))
                {
                    if (typeReferencesRegistered.Contains(typeReference)
                        || !typesDefinitionsUsed.ContainsKey(typeReference.Resolve())
                        || typeReference.GetGenericTypeScope().HasValue
                        || Context.IsImportedType(typeReference.Resolve())
                        || Context.IsJsonType(typeReference.Resolve()))
                    {
                        continue;
                    }

                    typeReferencesRegistered.Add(typeReference);
                    typesInitializedInOrder.Add(typeReference);
                    returnValue.Add(
                        new ExpressionStatement(
                            null,
                            Scope,
                            InitializeGenericType(
                                typeReference,
                                Scope)));
                }

                // here we need to initialize all the static constructor for
                // all these classes.
                foreach (var typeReference in typesInitializedInOrder)
                {
                    if (!typesDefinitionsUsed.ContainsKey(typeReference.Resolve()))
                    { continue; }

                    var staticConstructor =
                        typesDefinitionsUsed[typeReference.Resolve()]
                            .InitializeTypeStatement(typeReference);

                    if (staticConstructor != null)
                    {
                        returnValue.Add(staticConstructor);
                    }
                }

                typesGenerated = typesDefinitionsUsed.Count;
                membersGenerated = membersProcessed.Count;
                WalkUsedDependencies();
                foreach (var typeDefConverter in typesDefinitionsUsed.Values)
                {
                    typeDefConverter.ClearVariableInitializedTracking();
                }
            } while (typesGenerated < typesDefinitionsUsed.Count
                || membersGenerated < membersProcessed.Count);

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

            var genericArguments = typeReference.GetGenericArguments();
            if (genericArguments != null
                && genericArguments.Count > 0
                && !typeReference.IsDefinition)
            {
                strBuilder.Append('<');

                for (var paramIndex = 0; paramIndex < genericArguments.Count; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        strBuilder.Append(',');
                    }

                    RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                        genericArguments[paramIndex],
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
                return Tuple.Create<string, string>(string.Empty, string.Empty);
            }

            var dotIndex = name.LastIndexOf('.');

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
            knownIdentifiers.TryGetValue(name, out var returnValue);
            return returnValue;
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public IList<IIdentifier> ResolveType(TypeReference typeReference)
        {
            var typeDefinition = typeReference.Resolve();

            if (Context.IsJsonType(typeDefinition))
            {
                typeReference = Context.ClrKnownReferences.Object;
            }

            typeReference = Context.KnownReferences.FixArrayType(typeReference) ?? typeReference;

            if (!typeReferenceIdentifiers.TryGetValue(
                typeReference,
                out var returnValue))
            {
                IIdentifier resolvedIdentifier;
                var typeName = GetTypeName(typeReference);

                if (typeName == null)
                {
                    // This could be true if typeName is type with GlobalMethods attribute.
                    return null;
                }

                var nameSpace = globalNamespaceManager;
                var isExtended = Context.IsExtended(typeReference)
                    || Context.IsPsudoType(typeDefinition);

                // Imported types do not have generic symbols.
                //
                if (typeDefinition.HasGenericParameters
                    && isExtended
                    && typeName.Item2.Contains("`"))
                {
                    typeName = Tuple.Create(
                        typeName.Item1,
                        typeName.Item2.Substring(0, typeName.Item2.IndexOf('`')));
                }
                else if (isExtended
                    && typeName.Item2.Contains("<"))
                {
                    typeName = Tuple.Create(
                        typeName.Item1,
                        typeName.Item2.Substring(0, typeName.Item2.IndexOf('<')));
                }

                // put in queue to process. In case of conversion using dependency
                // walk, this will be used to start converting these types as well.
                usedTypeReferencesToProcess.Enqueue(typeReference);

                // Create the name of identifier to be used.
                if (typeName != null)
                {
                    if (isExtended
                        && !string.IsNullOrEmpty(typeName.Item1)
                        && !Context.HasIgnoreNamespaceAttribute(typeDefinition))
                    {
                        nameSpace = nameSpace.GetNamespace(typeName.Item1, true);
                    }

                    resolvedIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                        nameSpace.Scope,
                        isExtended || string.IsNullOrEmpty(typeName.Item1)
                            ? typeName.Item2
                            : (typeName.Item2).Replace('.', '_'),
                        isExtended);

                    returnValue = new List<IIdentifier>
                    {
                        resolvedIdentifier
                    };
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

                typeReferenceIdentifiers.Add(
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
            var typeReference = typeReferenceBase;
            typeReference = Context.KnownReferences.FixArrayType(typeReference) ?? typeReference;

            var typeDef = typeReference.Resolve();
            if ((typeDef == null || !Context.IsPsudoType(typeDef))
                && typeReferenceBase.GetGenericTypeScope().HasValue)
            {
                if (initializeRefsAndStaticCtor == null)
                {
                    var rv = IdentifierExpression.Create(
                    null,
                    scope,
                    ResolveType(typeReference.Resolve()));

                    var genericArguments = typeReference.GetGenericArguments();
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
                                        JSBaseObjectScopeManager.TypeId, scope)));
                        }
                    }

                    return rv;
                }
                else
                {
                    var arguments = new List<Expression>();

                    var genericArguments = typeReference.GetGenericArguments();
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

                    return new MethodCallExpression(
                        null,
                        scope,
                        IdentifierExpression.Create(
                            null,
                            scope,
                            ResolveType(typeReference.Resolve())),
                        arguments);
                }
            }
            else
            {
                return IdentifierExpression.Create(
                    null,
                    scope,
                    ResolveType(typeReference));
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
            usedTypeReferencesToProcess.Enqueue(propertyReference.DeclaringType);
            usedMembersToProcess.Enqueue(propertyReference);

            return GetTypeScope(propertyReference.DeclaringType).ResolveProperty(propertyReference.Resolve());
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="memberReference">The method reference.</param>
        /// <returns>Method name identifier for given method.</returns>
        public IIdentifier Resolve(
            FieldReference memberReference)
        {
            usedTypeReferencesToProcess.Enqueue(memberReference.DeclaringType);
            usedMembersToProcess.Enqueue(memberReference);

            return GetTypeScope(memberReference.DeclaringType).ResolveField(memberReference);
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
            usedTypeReferencesToProcess.Enqueue(memberReference.DeclaringType);
            usedMembersToProcess.Enqueue(memberReference);

            if (memberReference.Name == ".ctor"
                && forceStatic
                && memberReference.Parameters.Count == 0)
            {
                // This means that we are looking at finding factory method for this constructor
                // This constructor has 0 arguments, so we can as well return GetGetDefaultConstructor
                // function on Type.
                return GetTypeScope(Context.ClrKnownReferences.TypeType)
                    .ResolveMethod(Context.KnownReferences.GetDefaultConstructorMethod, false);
            }

            return GetTypeScope(memberReference.DeclaringType).ResolveMethod(memberReference, forceStatic);
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
                            ResolveVirtualMethodHelper(methodReference), scope),
                        string.Empty,
                        "_"),
                    GetTypeId(
                        methodReference.DeclaringType,
                        scope,
                        typeResolver));
            }
            else
            {
                return new IdentifierExpression(
                    ResolveVirtualMethodHelper(methodReference),
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
            if (!methodNameIdentifier.TryGetValue(methodReference, out var returnValue))
            {
                var suggestedName =
                    methodReference.DeclaringType.Resolve().Name + "." + methodReference.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(Scope, suggestedName, false);

                methodNameIdentifier.Add(methodReference, returnValue);
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
            if (!scriptAliasIdentifiers.TryGetValue(
                alias,
                out var returnValue))
            {
                IIdentifier resolvedIdentifier;
                var nameSpace = globalNamespaceManager;
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

                returnValue = new List<IIdentifier>
                {
                    resolvedIdentifier
                };

                while (nameSpace.Parent != null)
                {
                    returnValue.Insert(0, nameSpace.Identifier);
                    nameSpace = nameSpace.Parent;
                }

                returnValue = new ReadOnlyCollection<IIdentifier>(returnValue);
                scriptAliasIdentifiers.Add(
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
            var typeDef = methodDefinition.DeclaringType.Resolve();
            if ((typeDef.IsGenericInstance || typeDef.HasGenericParameters)
                && !Context.IsPsudoType(typeDef))
            {
                throw new InvalidOperationException(
                    "Can't resolve (static) method to global identifier if method is on generic type.");
            }

            usedTypeReferencesToProcess.Enqueue(methodDefinition.DeclaringType);
            usedMembersToProcess.Enqueue(methodDefinition);
            return ResolveFunctionName(methodDefinition);
        }

        /// <summary>
        /// Resolves the static factory for given constructor.
        /// </summary>
        /// <param name="constructor">The method definition.</param>
        /// <returns>identifier for the method</returns>
        public IIdentifier ResolveFactory(MethodDefinition constructor)
        {
            var typeDef = constructor.DeclaringType.Resolve();
            if (typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve (static) method to global identifier if method is on generic type.");
            }

            if (!staticFactoryMap.TryGetValue(constructor, out var returnValue))
            {
                var suggestedName =
                    constructor.DeclaringType.Resolve().Name + "_factory";

                returnValue = SimpleIdentifier.CreateScopeIdentifier(Scope, suggestedName, false);

                staticFactoryMap.Add(constructor, returnValue);
            }

            usedTypeReferencesToProcess.Enqueue(constructor.DeclaringType);
            usedMembersToProcess.Enqueue(constructor);
            return returnValue;
        }

        /// <summary>
        /// Resolves the static field.
        /// </summary>
        /// <param name="fieldDefinition">The field definition.</param>
        /// <returns>identifier for the field.</returns>
        public IIdentifier ResolveStatic(FieldDefinition fieldDefinition)
        {
            var typeDef = fieldDefinition.DeclaringType.Resolve();
            if (!fieldDefinition.IsStatic
                || typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve static field to global identifier if type is generic or member is not static.");
            }

            if (!staticMemberMap.TryGetValue(fieldDefinition, out var returnValue))
            {
                var suggestedName =
                    fieldDefinition.DeclaringType.Resolve().Name + "." + fieldDefinition.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(Scope, suggestedName, false);

                staticMemberMap.Add(fieldDefinition, returnValue);
            }

            usedTypeReferencesToProcess.Enqueue(fieldDefinition.DeclaringType);
            usedMembersToProcess.Enqueue(fieldDefinition);
            return returnValue;
        }

        public IIdentifier ResolveStatic(PropertyDefinition propertyDefinition)
        {
            var typeDef = propertyDefinition.DeclaringType.Resolve();
            if (!propertyDefinition.IsStatic()
                || typeDef.IsGenericInstance
                || typeDef.HasGenericParameters)
            {
                throw new InvalidOperationException(
                    "Can't resolve static field to global identifier if type is generic or member is not static.");
            }

            if (!staticMemberMap.TryGetValue(propertyDefinition, out var returnValue))
            {
                var suggestedName =
                    propertyDefinition.DeclaringType.Resolve().Name + "." + propertyDefinition.Name;

                returnValue = SimpleIdentifier.CreateScopeIdentifier(Scope, suggestedName, false);

                staticMemberMap.Add(propertyDefinition, returnValue);
            }

            usedTypeReferencesToProcess.Enqueue(propertyDefinition.DeclaringType);
            usedMembersToProcess.Enqueue(propertyDefinition);
            return returnValue;
        }

        /// <summary>
        /// Gets the type scope.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public TypeScopeManager GetTypeScope(TypeReference typeReference) => GetTypeScope(typeReference.Resolve());

        /// <summary>
        /// Gets the type scope.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns></returns>
        public TypeScopeManager GetTypeScope(TypeDefinition typeDefinition)
        {
            if (!typeScopes.TryGetValue(typeDefinition, out var returnValue))
            {
                if (typeDefinition.IsSameDefinition(Context.ClrKnownReferences.Object))
                {
                    returnValue = JSBaseObjectScopeManager.ObjecTypeScopeManager;
                }
                else if (typeDefinition.IsSameDefinition(Context.ClrKnownReferences.TypeType))
                {
                    returnValue = JSBaseObjectScopeManager.TypeTypeScopeManager;
                }
                else
                {
                    TypeScopeManager parentTypeScopeManager = null;

                    if (typeDefinition.BaseType != null)
                    {
                        parentTypeScopeManager = GetTypeScope(typeDefinition.BaseType);

                        returnValue = new TypeScopeManager(
                            typeDefinition,
                            parentTypeScopeManager);
                    }
                    else
                    {
                        returnValue = new TypeScopeManager(
                            Context,
                            typeDefinition,
                            JSBaseObjectScopeManager.InstanceScope,
                            JSBaseObjectScopeManager.StaticScope);
                    }
                }

                typeScopes.Add(typeDefinition, returnValue);
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
            var typeId = new StringBuilder();
            var genericArguments = typeReference.GetGenericArguments();
            if (genericArguments.Count > 0)
            {
                typeId.AppendFormat("{0}$", GetTypeId(typeReference.Resolve()));
                for (var paramIndex = 0; paramIndex < genericArguments.Count; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        typeId.Append('_');
                    }

                    typeId.Append(GetTypeId(genericArguments[paramIndex]));
                }

                typeId.Append("$");
            }
            else
            {
                typeId.Append(GetTypeId(typeReference.Resolve()));
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
            if (!typeIdMap.TryGetValue(typeDefinition, out var typeId))
            {
                if (Context.IsJsonType(typeDefinition)
                    || Context.IsPsudoType(typeDefinition))
                {
                    typeId = GetTypeId(Context.ClrKnownReferences.Object);
                }
                else
                {
                    var newTypeId = System.Threading.Interlocked.Increment(
                        ref typeIdsUsed);
                    typeId = Utils.GetCompressedInt(newTypeId);
                }

                typeIdMap[typeDefinition] = typeId;
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
            var typeScope = typeReference.GetGenericTypeScope();

            if (typeScope.HasValue)
            {
                return new IndexExpression(
                    null,
                    scope,
                    IdentifierExpression.Create(
                        null,
                        scope,
                        typeResolver(typeReference)),
                    new IdentifierExpression(JSBaseObjectScopeManager.TypeId, scope));
            }
            else
            {
                return new StringLiteralExpression(
                    scope,
                    GetTypeId(typeReference));
            }
        }

        /// <summary>
        /// Gets the type converter.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns>Type converter for given typeDefinition</returns>
        public TypeConverter GetTypeConverter(
            TypeDefinition typeDefinition) => typesDefinitionsUsed[typeDefinition];

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
            for (var i = 0; i < left.Count - 1 && i < right.Count - 1; i++)
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
            var globalNames = new string[]
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

            foreach (var globalName in globalNames)
            {
                knownIdentifiers.Add(
                    globalName,
                    SimpleIdentifier.CreateScopeIdentifier(
                        Scope,
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
                var typeName = Context.GetTypeName(typeReference);

                return RuntimeScopeManager.GetSplitName(typeName);
            }
            else
            {
                var typeNameBuilder = new StringBuilder();
                var nameSpaceSet = false;
                string nameSpace = null;

                RuntimeScopeManager.CalculateFriendlyTypeReferenceName(
                    typeReference,
                    typeNameBuilder,
                    (typeDef, strBuilder) =>
                    {
                        var typeName = Context.GetTypeName(typeDef);

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
            var genericArguments = new List<Expression>();
            var genericTypeArgs = typeReference.GetGenericArguments();
            for (var genericArgIndex = 0; genericArgIndex < genericTypeArgs.Count; genericArgIndex++)
            {
                genericArguments.Add(
                    IdentifierExpression.Create(
                        null,
                        scope,
                        ResolveType(genericTypeArgs[genericArgIndex])));
            }

            return new BinaryExpression(
                null,
                scope,
                BinaryOperator.Assignment,
                IdentifierExpression.Create(
                    null,
                    scope,
                    ResolveType(typeReference)),
                new MethodCallExpression(
                    null,
                    scope,
                    IdentifierExpression.Create(
                        null,
                        scope,
                        ResolveType(typeReference.Resolve())),
                    genericArguments));
        }

        /// <summary>
        /// Resolves the virtual method internal.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Identifier for the method that points to virtual function.</returns>
        private IIdentifier ResolveVirtualMethodHelper(MethodReference methodReference)
        {
            var methodDefinition = methodReference.Resolve();

            usedTypeReferencesToProcess.Enqueue(methodReference.DeclaringType);
            usedMembersToProcess.Enqueue(methodReference);
            virtualMethodReferencesUsed.Add(methodReference);
            virtualMethodsUsed.Add(methodDefinition);

            return GetTypeScope(methodReference.DeclaringType).ResolveVirtualMethod(methodDefinition);
        }

        /// <summary>
        /// Processes the types.
        /// </summary>
        private void ProcessTypes()
        {
            while (usedTypeReferencesToProcess.Count > 0)
            {
                var typeReference = usedTypeReferencesToProcess.Dequeue();
                if (typeReferencesUsed.Contains(typeReference)
                    || typeReference.IsGenericParameter
                    || (typeReference.IsArray && typeReference.GetElementType().IsGenericParameter))
                {
                    continue;
                }

                var typeDefinition = typeReference.Resolve();
                typeReferencesUsed.Add(typeReference);
                if (typeReference.IsGenericInstance)
                {
                    var genericTypeReference = (GenericInstanceType)typeReference;
                    foreach (var param in genericTypeReference.GenericArguments)
                    {
                        if (!param.IsGenericParameter)
                        { usedTypeReferencesToProcess.Enqueue(param); }
                    }
                }

                DependencyAnalyzer.AddTypeForAnalysis(typeDefinition);

                if (typesDefinitionsUsed.TryGetValue(typeDefinition, out var typeConverter))
                {
                    continue;
                }

                if (typeDefinition == Context.ClrKnownReferences.Object)
                {
                    usedMembersToProcess.Enqueue(
                        Context.KnownReferences.ToStringMethod);
                }

                typeConverter = TypeConverter.Create(this, typeDefinition, true);
                typesDefinitionsUsed.Add(typeDefinition, typeConverter);

                // Now let's add all the dependencies of the class.
                DependencyAnalyzer.GetTypeReferenceDependencies(new TypeDefinition[] { typeDefinition })
                    .ForEach(tr => usedTypeReferencesToProcess.Enqueue(tr));
            }
        }

        /// <summary>
        /// Processes the members.
        /// </summary>
        private void ProcessMembers()
        {
            while (usedMembersToProcess.Count > 0)
            {
                var memberReference = usedMembersToProcess.Dequeue();
                var memberDefinition = memberReference.GetDefinition() as MemberReference;

                if (membersProcessed.Contains(memberDefinition))
                {
                    continue;
                }

                membersProcessed.Add(memberDefinition);

                // Before we proceed we need to make sure that typeQueue is all flushed.
                // We do this to gurantee that TypeConverter for this member is already
                // in our tracking list.
                ProcessTypes();

                // Currently we only process field and methods. We do this because
                // even properties will show up as methods anyways.
                if (memberDefinition is MethodDefinition method)
                {
                    typesDefinitionsUsed[method.DeclaringType].AddMethodToImplementation(method);
                    continue;
                }

                if (memberDefinition is FieldDefinition fieldDefinition)
                {
                    typesDefinitionsUsed[fieldDefinition.DeclaringType].AddFieldToImplementation(fieldDefinition);
                }
            }
        }

        /// <summary>
        /// Walks the used dependencies.
        /// </summary>
        private void WalkUsedDependencies()
        {
            while (usedMembersToProcess.Count > 0
                || usedTypeReferencesToProcess.Count > 0)
            {
                ProcessTypes();
                ProcessMembers();

                // Last step here is to make sure that if a virtual method is used, all it's implementations
                // are also used. Currently our approach is brute force, but later on this can be optimized.
                foreach (var typeDefinition in typesDefinitionsUsed.Keys)
                {
                    foreach (var kvPair in typeDefinition.GetInterfaceOverrides(Context.ClrContext))
                    {
                        var overridenMethod = kvPair.Key;
                        var method = kvPair.Value;

                        if (virtualMethodsUsed.Contains(overridenMethod.Resolve()))
                        {
                            if (!membersProcessed.Contains(method))
                            {
                                usedMembersToProcess.Enqueue(method);
                                if (overridenMethod.DeclaringType.Resolve().IsInterface)
                                {
                                    var methodDef = (MethodDefinition)method.GetDefinition();
                                    if (methodDef.IsVirtual)
                                    {
                                        // If this type is inheriting from interface and this method is
                                        // also virtual. In this case any call to method on interface will
                                        // also make virtual call on this method. So we need to add this method
                                        // to virtuals used set.
                                        virtualMethodReferencesUsed.Add(method);
                                        virtualMethodsUsed.Add(methodDef);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}