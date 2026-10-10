//-----------------------------------------------------------------------
// <copyright file="ClrContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;

    /// <summary>
    /// Definition for ClrContext
    /// </summary>
    public class ClrContext : IDisposable
    {
        /// <summary>
        /// The assembly resolver. It returns the assemblies this context loaded, and reads any
        /// other into memory, so a kept build session holds no file open.
        /// </summary>
        private readonly InMemoryAssemblyResolver assemblyResolver;

        /// <summary>
        /// Resolves to an assembly the context already loaded when there is one. Otherwise it
        /// uses the search order and cache of <see cref="DefaultAssemblyResolver"/>, reading
        /// the file into memory (<see cref="ReaderParameters.InMemory"/>). Without the first
        /// step a reference into a loaded assembly resolved to a second copy read from disk,
        /// so one type had two TypeDefinitions and the converter emitted it twice.
        /// </summary>
        private sealed class InMemoryAssemblyResolver : DefaultAssemblyResolver
        {
            private readonly ClrContext context;

            public InMemoryAssemblyResolver(ClrContext context)
            {
                this.context = context;
            }

            public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
            {
                if (this.context.TryGetModuleDefinition(name.Name, out var module))
                {
                    return module.Assembly;
                }

                parameters.InMemory = true;
                return base.Resolve(name, parameters);
            }
        }

        /// <summary>
        /// Remembers what each reference object resolved to. Cecil's resolver scans the
        /// declaring type's members and compares signatures on every call, and a build session
        /// resolves the same references build after build. A refresh drops the answers that
        /// point into the types it rebuilt (<see cref="ForgetTypes"/>).
        /// </summary>
        private sealed class CachingMetadataResolver : MetadataResolver
        {
            // Weak keys: the converter builds new reference objects every build, and an answer
            // must not outlive the reference that asked.
            private readonly ConditionalWeakTable<TypeReference, TypeDefinition> types =
                new ConditionalWeakTable<TypeReference, TypeDefinition>();

            private readonly ConditionalWeakTable<MethodReference, MethodDefinition> methods =
                new ConditionalWeakTable<MethodReference, MethodDefinition>();

            private readonly ConditionalWeakTable<FieldReference, FieldDefinition> fields =
                new ConditionalWeakTable<FieldReference, FieldDefinition>();

            public CachingMetadataResolver(IAssemblyResolver assemblyResolver)
                : base(assemblyResolver)
            {
            }

            public override TypeDefinition Resolve(TypeReference type)
                => Cached(this.types, type, base.Resolve);

            public override MethodDefinition Resolve(MethodReference method)
                => Cached(this.methods, method, base.Resolve);

            public override FieldDefinition Resolve(FieldReference field)
                => Cached(this.fields, field, base.Resolve);

            public void Forget(HashSet<TypeDefinition> forgotten)
            {
                Remove(this.types, definition => forgotten.Contains(definition));
                Remove(this.methods, definition => forgotten.Contains(definition.DeclaringType));
                Remove(this.fields, definition => forgotten.Contains(definition.DeclaringType));
            }

            private static TValue Cached<TKey, TValue>(ConditionalWeakTable<TKey, TValue> map, TKey key, Func<TKey, TValue> resolve)
                where TKey : class
                where TValue : class
            {
                if (!map.TryGetValue(key, out var definition))
                {
                    definition = resolve(key);
                    if (definition != null)
                    {
                        map.AddOrUpdate(key, definition);
                    }
                }

                return definition;
            }

            private static void Remove<TKey, TValue>(ConditionalWeakTable<TKey, TValue> map, Func<TValue, bool> stale)
                where TKey : class
                where TValue : class
            {
                foreach (var key in ((IEnumerable<KeyValuePair<TKey, TValue>>)map).Where(entry => stale(entry.Value)).Select(entry => entry.Key).ToList())
                {
                    map.Remove(key);
                }
            }
        }

        private readonly CachingMetadataResolver metadataResolver;

        /// <summary>
        /// The assemblies.
        /// </summary>
        private readonly Dictionary<string, ModuleDefinition> assemblies =
            new Dictionary<string, ModuleDefinition>();

        /// <summary>
        /// Name of the simple name to assembly.
        /// </summary>
        private readonly Dictionary<string, string> simpleNameToAssemblyName =
            new Dictionary<string, string>();

        /// <summary>
        /// The directories to look at.
        /// </summary>
        private readonly HashSet<string> directoriesToLookAt =
            new HashSet<string>();

        /// <summary>
        /// The type reference to definition map.
        /// </summary>
        private readonly Dictionary<TypeReference, TypeDefinition> typeReferenceToDefinitionMap =
            new Dictionary<TypeReference, TypeDefinition>();

        private readonly Dictionary<TypeDefinition, ReadOnlyCollection<MethodReference>> typeToVirtualMethods =
            new Dictionary<TypeDefinition, ReadOnlyCollection<MethodReference>>();

        /// <summary>
        /// Interface overrides per type. They depend only on the loaded modules, so they live as long as this
        /// context and are never shared with another one.
        /// </summary>
        private readonly Dictionary<TypeDefinition, Dictionary<MethodReference, MethodReference>> typeToInterfaceOverrides =
            new Dictionary<TypeDefinition, Dictionary<MethodReference, MethodReference>>();

        /// <summary>
        /// backing store for KnownReferences.
        /// </summary>
        private readonly ClrKnownReferences knownReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrContext"/> class.
        /// </summary>
        public ClrContext()
        {
            this.assemblyResolver = new InMemoryAssemblyResolver(this);
            this.metadataResolver = new CachingMetadataResolver(this.assemblyResolver);
            this.knownReferences = new ClrKnownReferences(this);
        }

        /// <summary>
        /// Gets the known references.
        /// </summary>
        public ClrKnownReferences KnownReferences
        {
            get { return this.knownReferences; }
        }

        /// <summary>
        /// Gets the modules.
        /// </summary>
        public IEnumerable<ModuleDefinition> Modules
        { get { return this.assemblies.Values; } }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeDefinition> GetTypeDefinitions()
        {
            foreach (var module in this.Modules)
            {
                foreach (var type in module.Types)
                {
                    yield return type;
                }
            }

            yield break;
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public void LoadAssembly(string assemblyPath, bool loadSymbols = false)
        {
            string directoryName = System.IO.Path.GetDirectoryName(assemblyPath);
            if (!directoriesToLookAt.Contains(directoryName))
            {
                assemblyResolver.AddSearchDirectory(directoryName);
            }

            ISymbolReaderProvider symbolReader = null;
            if (File.Exists(Path.Combine(directoryName, Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb")))
            {
                symbolReader = new PdbReaderProvider();
            }
            else if (File.Exists(Path.Combine(directoryName, Path.GetFileNameWithoutExtension(assemblyPath) + ".mdb")))
            {
                symbolReader = new MdbReaderProvider();
            }

            ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(
                assemblyPath,
                new ReaderParameters()
                {
                    AssemblyResolver = assemblyResolver,
                    MetadataResolver = metadataResolver,
                    // Read the image into memory so no file handle outlives the load: the
                    // build service keeps the process alive and must not lock obj/ DLLs.
                    InMemory = true,
                    ReadSymbols = loadSymbols,
                    SymbolReaderProvider = loadSymbols ? symbolReader : null
                });

            var hasBstInfo = moduleDefinition.Resources
                .Where(res => res.Name == "$$BstInfo$$")
                .Any();

            if (!hasBstInfo)
            {
                // Early return when BstInfo is not present.
                // This is done to skip loading assemblies which
                // are not compiled with custom Roslyn compiler.
                moduleDefinition.Dispose();
                return;
            }

            string assemblyName = moduleDefinition.Assembly.Name.Name;

            if (this.simpleNameToAssemblyName.ContainsKey(assemblyName.ToLowerInvariant()))
            {
                throw new ApplicationException("Assembly already loaded");
            }

            this.simpleNameToAssemblyName[assemblyName.ToLowerInvariant()] =
                moduleDefinition.Name;

            this.assemblies[moduleDefinition.Name.ToLowerInvariant()] = moduleDefinition;
        }

        /// <summary>
        /// Releases the loaded modules and every assembly the resolver opened (the resolver
        /// reads referenced assemblies from disk and keeps their files open until disposed).
        /// </summary>
        public void Dispose()
        {
            foreach (var module in this.assemblies.Values)
            {
                module.Dispose();
            }

            this.assemblyResolver.Dispose();
        }

        /// <summary>
        /// Checks all dependencies loaded.
        /// </summary>
        /// <returns></returns>
        public bool CheckAllDependenciesLoaded()
        {
            foreach (var module in this.assemblies.Values)
            {
                foreach (var assemblyReference in module.AssemblyReferences)
                {
                    if (!this.simpleNameToAssemblyName.ContainsKey(
                        assemblyReference.Name.ToLowerInvariant()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Tries the get module definition.
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        /// <param name="module">The module.</param>
        /// <returns></returns>
        public bool TryGetModuleDefinition(
            string moduleName,
            out ModuleDefinition module)
        {
            string moduleFullName;
            module = null;

            switch (System.IO.Path.GetExtension(moduleName))
            {
                case ".exe":
                case ".dll":
                    moduleFullName = moduleName.ToLowerInvariant(); ;
                    break;

                default:
                    if (!this.simpleNameToAssemblyName.TryGetValue(moduleName.ToLowerInvariant(), out moduleFullName))
                    {
                        return false;
                    }

                    moduleFullName = moduleFullName.ToLowerInvariant();
                    break;
            }

            if (!this.assemblies.TryGetValue(moduleFullName, out module))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Build session: takes the resources of new images of loaded modules when only
        /// resources changed. An image qualifies when its module is loaded, it has the loaded
        /// module's MVID, the same embedded resources in the same order (name and attributes),
        /// every "$$" resource ($$BstInfo$$, $$ResInfo$$) byte-equal, and at least one other
        /// resource that differs. ResourcePatcher writes such images; a recompile gets a new
        /// MVID and a new $$BstInfo$$. Every image is checked before any module changes.
        /// </summary>
        /// <param name="images">The new bytes of each changed input.</param>
        /// <param name="replaced">The number of resources replaced.</param>
        /// <param name="reason">Why an image does not qualify; null on success.</param>
        /// <returns>true if the modules took the new resources; false if nothing changed.</returns>
        public bool TryRefreshResources(IReadOnlyList<byte[]> images, out int replaced, out string reason)
        {
            replaced = 0;
            using var refresh = ModuleRefresh.TryPlan(this, images, this.assemblyResolver, resourcesOnly: true, out reason);
            if (refresh == null)
            {
                return false;
            }

            refresh.Commit();
            replaced = refresh.ResourcesReplaced;
            return true;
        }

        /// <summary>
        /// Build session: checks new images of loaded modules (see <see cref="ModuleRefresh"/>).
        /// Returns null with a reason when one does not qualify; nothing changes until the
        /// caller commits the plan.
        /// </summary>
        public ModuleRefresh TryPlanRefresh(IReadOnlyList<byte[]> images, out string reason)
            => ModuleRefresh.TryPlan(this, images, this.assemblyResolver, resourcesOnly: false, out reason);

        /// <summary>Drops what this context computed from types a refresh rebuilt or removed.</summary>
        internal void ForgetTypes(IEnumerable<TypeDefinition> types)
        {
            var forgotten = new HashSet<TypeDefinition>(types);
            foreach (var type in forgotten)
            {
                this.typeToVirtualMethods.Remove(type);
                this.typeToInterfaceOverrides.Remove(type);
            }

            foreach (var stale in this.typeReferenceToDefinitionMap.Where(entry => forgotten.Contains(entry.Value)).Select(entry => entry.Key).ToList())
            {
                this.typeReferenceToDefinitionMap.Remove(stale);
            }

            this.metadataResolver.Forget(forgotten);
        }

        /// <summary>
        /// Resolves the specified type reference.
        /// </summary>
        /// <param name="paramDef">The type reference.</param>
        /// <returns></returns>
        public TypeDefinition Resolve(TypeReference typeReference)
        {
            TypeDefinition rv;
            if (!this.typeReferenceToDefinitionMap.TryGetValue(
                typeReference,
                out rv))
            {
                rv = typeReference.Resolve();

                this.typeReferenceToDefinitionMap.Add(
                    typeReference,
                    rv);
            }

            return rv;
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeDefinition> GetTypes()
        {
            foreach (var assembly in this.assemblies.Values)
            {
                foreach (var type in assembly.Types)
                {
                    yield return type;
                }
            }

            yield break;
        }

        public GenericParameter GetTypeParameter(
            ModuleDefinition moduleDefinition,
            Tuple<string, string> typeName)
        {
            if (string.IsNullOrEmpty(typeName.Item1)
                && typeName.Item2.StartsWith("!"))
            {
                int typeParameterNumber;
                if (!int.TryParse(typeName.Item2.Substring(typeName.Item2.LastIndexOf('!') + 1), out typeParameterNumber))
                {
                    return null;
                }

                if (typeName.Item2.StartsWith("!!"))
                {
                    return new GenericParameter(
                        typeParameterNumber,
                        GenericParameterType.Method,
                        moduleDefinition);
                }
                else
                {
                    return new GenericParameter(
                        typeParameterNumber,
                        GenericParameterType.Method,
                        moduleDefinition);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public TypeDefinition GetTypeDefinition(Tuple<string, string> typeName)
        {
            ModuleDefinition moduleDefinition;

            if (!this.TryGetModuleDefinition(typeName.Item1, out moduleDefinition))
            { return null; }

            string nspace = typeName.Item2.Substring(0, typeName.Item2.LastIndexOf('.'));
            string tName = typeName.Item2.Substring(typeName.Item2.LastIndexOf('.') + 1);
            int arity = 0;

            if (tName.Contains('`'))
            {
                int.TryParse(typeName.Item2.Substring(tName.LastIndexOf('`') + 1), out arity);
            }

            var rv = new TypeReference(
                nspace,
                tName,
                moduleDefinition,
                moduleDefinition).Resolve();

            return rv;
        }

        /// <summary>
        /// Gets the type definition.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public TypeDefinition GetTypeDefinition(Tuple<string, string, string> typeName)
        {
            ModuleDefinition moduleDefinition;

            if (!this.TryGetModuleDefinition(typeName.Item1, out moduleDefinition))
            { return null; }

            string[] typeNames = typeName.Item3.Split('.');

            var declaringType = new TypeReference(
                typeName.Item2,
                typeNames[0],
                moduleDefinition,
                moduleDefinition).Resolve();

            if (typeNames.Length > 1)
            {
                var rv = new TypeReference(
                    null,
                    typeNames[1],
                    moduleDefinition,
                    moduleDefinition);

                rv.DeclaringType = declaringType;
                declaringType = rv.Resolve();
            }

            return declaringType;
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>MethodReference</returns>
        public MethodReference GetMethodReference(
            string methodName,
            TypeReference returnType,
            TypeReference declaringType,
            params TypeReference[] arguments)
        {
            MethodReference methodReference = new MethodReference(
                methodName,
                returnType,
                declaringType);

            foreach (var argument in arguments)
            {
                methodReference.Parameters.Add(
                    new ParameterDefinition(argument));
            }

            return methodReference;
        }

        public MethodReference GetBaseSlotForVirtual(MethodReference methodReference)
        {
            var methodDefinition = methodReference.Resolve();
            if (methodDefinition.IsNewSlot)
            { return methodReference; }

            var baseTypeRef = methodReference.DeclaringType.GetBaseType();
            while (baseTypeRef != null)
            {
                foreach (var overridable in this.GetVirtualOverridables(baseTypeRef.Resolve()))
                {
                    if (methodReference.IsOverriding(overridable, baseTypeRef))
                    { return overridable; }
                }

                baseTypeRef = baseTypeRef.GetBaseType();
            }

            throw new InvalidProgramException();
        }

        /// <summary>
        /// Gets the interface overrides of a type, computed once per type for this context.
        /// The returned map is shared; callers must not change it.
        /// </summary>
        public Dictionary<MethodReference, MethodReference> GetInterfaceOverrides(TypeDefinition typeDefinition)
        {
            if (!this.typeToInterfaceOverrides.TryGetValue(typeDefinition, out var overrides))
            {
                overrides = TypeHelpers.ComputeInterfaceOverrides(typeDefinition, this);
                this.typeToInterfaceOverrides.Add(typeDefinition, overrides);
            }

            return overrides;
        }

        /// <summary>
        /// Gets the virtual overridables. The method is the method that creates the slot
        /// and not the method in passed in typeDefinition.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        /// <returns></returns>
        public IList<MethodReference> GetVirtualOverridables(TypeDefinition typeDefinition)
        {
            ReadOnlyCollection<MethodReference> rv;
            if (this.typeToVirtualMethods.TryGetValue(typeDefinition, out rv))
            {
                return rv;
            }

            if (typeDefinition.IsInterface)
            {
                List<MethodReference> tmp = new List<MethodReference>();
                tmp.AddRange(typeDefinition.Methods);
                rv = new ReadOnlyCollection<MethodReference>(tmp);
                this.typeToVirtualMethods.Add(typeDefinition, rv);

                return rv;
            }

            List<MethodReference> overridables = new List<MethodReference>();
            List<MethodDefinition> virtualMethods = new List<MethodDefinition>();

            virtualMethods.AddRange(
                typeDefinition.Methods.Where(method => method.IsVirtual));

            if (typeDefinition.BaseType != null)
            {
                foreach (var overridableMethod in this.GetVirtualOverridables(typeDefinition.BaseType.Resolve()))
                {
                    bool finalized = false;
                    for (int iMethod = 0; iMethod < virtualMethods.Count; iMethod++)
                    {
                        var method = virtualMethods[iMethod];

                        if (method.IsOverriding(overridableMethod, typeDefinition.BaseType))
                        {
                            // if this method is final, then this type is not exposing
                            // overridableMethod anymore.
                            finalized = method.IsFinal;

                            // This method will not be overriding any more methods so let's skip.
                            virtualMethods.RemoveAt(iMethod);
                            break;
                        }
                    }

                    if (!finalized)
                    {
                        overridables.Add(
                            overridableMethod.FixGenericTypeArguments(typeDefinition.BaseType));
                    }
                }
            }

            foreach (var methodDefinition in virtualMethods)
            {
                if (!methodDefinition.IsFinal)
                { overridables.Add(methodDefinition); }
            }

            rv = new ReadOnlyCollection<MethodReference>(overridables);
            this.typeToVirtualMethods.Add(typeDefinition, rv);

            return rv;
        }
    }
}