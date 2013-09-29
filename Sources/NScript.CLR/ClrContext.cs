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
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Cecil.Mdb;
    using Mono.Cecil.Pdb;

    /// <summary>
    /// Definition for ClrContext
    /// </summary>
    public class ClrContext
    {
        /// <summary>
        /// The assembly resolver.
        /// </summary>
        private readonly DefaultAssemblyResolver assemblyResolver =
            new DefaultAssemblyResolver();

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
        /// backing store for KnownReferences.
        /// </summary>
        private readonly ClrKnownReferences knownReferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrContext"/> class.
        /// </summary>
        public ClrContext()
        {
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
        public void LoadAssembly(string assemblyPath, bool loadSymbols = true)
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
                    ReadSymbols = loadSymbols,
                    SymbolReaderProvider = loadSymbols ? symbolReader : null
                });

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
                    return new GenericParameter(typeParameterNumber, GenericParameterType.Method, moduleDefinition);
                }
                else
                {
                    return new GenericParameter(typeParameterNumber, GenericParameterType.Method, moduleDefinition);
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

                        if (method.IsOverridable(overridableMethod, typeDefinition.BaseType) != null)
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
                            overridableMethod.FixGenericArguments(typeDefinition.BaseType));
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