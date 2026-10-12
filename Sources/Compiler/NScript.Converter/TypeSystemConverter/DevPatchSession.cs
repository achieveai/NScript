namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mono.Cecil;
    using NScript.JST;

    /// <summary>
    /// Dev patch emit, opt-in with <c>NSCRIPT_PATCH_EMIT=1</c>. A full dev emit leaves this
    /// behind: its <see cref="RuntimeScopeManager"/> with every scope named, the plugin
    /// instances it was built with, and the written script and map. A later build whose input
    /// change is only method bodies converts the changed types' methods again on it, and puts
    /// each method whose output changed just before its type's section marker, as an override
    /// that works wherever it runs (a bundle, or later a running page). A change to embedded
    /// resources only asks each plugin for what its section needs instead. The old code stays
    /// as dead code, so the bundle grows. Anything it cannot do is a miss; the caller then
    /// drops the session and emits in full.
    /// </summary>
    internal sealed class DevPatchSession
    {
        private readonly RuntimeScopeManager runtimeManager;
        private readonly IList<IMethodConverterPlugin> methodPlugins;
        private readonly IList<ITypeConverterPlugin> typePlugins;
        private readonly IList<IRuntimeConverterPlugin> runtimePlugins;
        private readonly bool devChunks;
        private readonly string jsFileName;
        private readonly OwaSourceMapper.SourceMap map;
        private readonly int fullLength;
        private string script;
        private (long Length, DateTime WriteTimeUtc) written;

        public DevPatchSession(
            RuntimeScopeManager runtimeManager,
            IList<IMethodConverterPlugin> methodPlugins,
            IList<ITypeConverterPlugin> typePlugins,
            IList<IRuntimeConverterPlugin> runtimePlugins,
            bool devChunks,
            string jsFileName,
            string script,
            OwaSourceMapper.SourceMap map)
        {
            this.runtimeManager = runtimeManager;
            this.methodPlugins = methodPlugins;
            this.typePlugins = typePlugins;
            this.runtimePlugins = runtimePlugins ?? Array.Empty<IRuntimeConverterPlugin>();
            this.devChunks = devChunks;
            this.jsFileName = Path.GetFullPath(jsFileName);
            this.script = script;
            this.fullLength = script.Length;
            this.map = map;
            this.written = Stamp(this.jsFileName);

            // Plugins resolve more members while writing their own output, after the last walk;
            // the written bundle is complete without them. A patch counts only its own demand.
            runtimeManager.TakeNewDemand(runtimeManager.VirtualMethodsUsedCount);
        }

        /// <summary>Whether dev builds keep a patch session: <c>NSCRIPT_PATCH_EMIT=1</c>.</summary>
        public static bool IsEnabled
            => Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT") == "1";

        /// <summary>
        /// How much the patches may grow the bundle, in percent of the full emit, before the
        /// next build emits in full again (compaction): <c>NSCRIPT_PATCH_COMPACT_PERCENT</c>,
        /// 25 by default.
        /// </summary>
        public static int CompactPercent
            => int.TryParse(Environment.GetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT"), out var percent) && percent >= 0
                ? percent
                : 25;

        /// <summary>Patches written since the full emit.</summary>
        public int Patches { get; private set; }

        /// <summary>Script length now, in chars.</summary>
        public int ScriptLength => this.script.Length;

        /// <summary>Last patch: time rendering its fragments (Patch.Emit FragmentMs).</summary>
        public long ProbeFragmentMs { get; private set; }

        /// <summary>Last patch: time joining and writing the script and map (Patch.Emit WriteMs).</summary>
        public long ProbeWriteMs { get; private set; }

        /// <summary>
        /// Patches the written bundle for a body-only change of <paramref name="changedTypes"/>.
        /// False with a reason when it cannot; the session is then unusable.
        /// </summary>
        /// <param name="methods">Methods whose override was written.</param>
        public bool TryPatch(
            IReadOnlyList<TypeDefinition> changedTypes,
            ConverterContext context,
            out string miss,
            out int methods)
        {
            var candidates = new List<Candidate>();
            return this.Patch(
                context,
                () => this.ConvertAgain(changedTypes, context, candidates),
                candidates,
                new List<(string, List<Statement>)>(),
                out miss,
                out methods);
        }

        /// <summary>
        /// Patches the written bundle for a change to embedded resources only: each runtime
        /// plugin adds what its section needs. False with a reason when it cannot; the session
        /// is then unusable.
        /// </summary>
        /// <param name="methods">Methods the plugins' new output called that were written.</param>
        public bool TryPatchResources(ConverterContext context, out string miss, out int methods)
        {
            var sections = new List<(string Name, List<Statement> Statements)>();
            return this.Patch(
                context,
                () => this.PatchPluginSections(sections),
                new List<Candidate>(),
                sections,
                out miss,
                out methods);
        }

        /// <summary>
        /// Runs <paramref name="convert"/> under the identifier observer, converts the methods
        /// it newly called, names what it made, and writes the changed methods and
        /// <paramref name="sections"/> before their markers.
        /// </summary>
        private bool Patch(
            ConverterContext context,
            Func<string> convert,
            List<Candidate> candidates,
            List<(string Name, List<Statement> Statements)> sections,
            out string miss,
            out int methods)
        {
            methods = 0;
            this.ProbeFragmentMs = this.ProbeWriteMs = 0;
            if ((long)this.script.Length * 100 > (long)this.fullLength * (100 + CompactPercent))
            {
                // The old code the patches left behind is dropped by one full emit.
                miss = "compaction";
                return false;
            }

            if (Stamp(this.jsFileName) != this.written)
            {
                miss = "bundle changed on disk";
                return false;
            }

            context.BeginBuild(this.methodPlugins, this.typePlugins);
            context.DevMode = true;
            context.DevChunks = this.devChunks;
            context.MethodCache = null;

            var added = new List<(TypeConverter Type, MethodDefinition Method, Statement Override)>();
            var observer = new Observer(this.runtimeManager.Scope);
            int virtualsBefore = this.runtimeManager.VirtualMethodsUsedCount;
            if (IdentifierScope.Observer != null)
            {
                miss = "identifier observer in use";
                return false;
            }

            IdentifierScope.Observer = observer;
            try
            {
                miss = convert();
                if (miss != null)
                {
                    return false;
                }

                // Methods the new code calls that the bundle never emitted, and the methods
                // those call, until nothing new is called.
                for (var demanded = new List<MethodDefinition>(); ; demanded.Clear())
                {
                    miss = this.runtimeManager.TakeNewMethods(virtualsBefore, demanded);
                    if (miss != null)
                    {
                        return false;
                    }

                    if (demanded.Count == 0)
                    {
                        break;
                    }

                    foreach (var method in demanded)
                    {
                        var typeConverter = this.runtimeManager.TypeConverters[method.DeclaringType];
                        miss = typeConverter.PatchMiss() ?? typeConverter.PatchMiss(method) ?? this.PluginMiss(method, context);
                        if (miss != null)
                        {
                            miss += " " + method.FullName;
                            return false;
                        }

                        typeConverter.AddMethodToImplementation(method);
                        var converted = typeConverter.ImplementedMethod(method);
                        if (converted == null)
                        {
                            // Implemented natively (extern, imported): nothing to write.
                            continue;
                        }

                        // Made here, so the identifiers the override makes are seen too.
                        var statement = typeConverter.CreateOverride(method, converted);
                        if (statement == null)
                        {
                            miss = "no override form " + method.FullName;
                            return false;
                        }

                        added.Add((typeConverter, method, statement));
                    }
                }
            }
            finally
            {
                IdentifierScope.Observer = null;
            }

            miss = observer.Miss
                ?? (context.Errors.Count > 0 ? "conversion errors" : null)
                ?? (context.RetryReason != null ? "retry " + context.RetryReason : null);
            if (miss != null)
            {
                return false;
            }

            // Members first: the new functions' free identifiers must already have names.
            var memberNamer = IdentifierScope.DevStableNamer.NameNewMembers(observer.Members);
            var namer = memberNamer.Errors.Count > 0
                ? memberNamer
                : IdentifierScope.DevStableNamer.NameNewFunctions(this.runtimeManager.Scope, observer.Functions);
            if (namer.Errors.Count > 0)
            {
                miss = "naming " + namer.Errors[0];
                return false;
            }

            // A new root variable (not a function) is not in the bundle's var header: the
            // first section declares it.
            var variables = observer.Members
                .Where(identifier => identifier.OwnerScope == this.runtimeManager.Scope && !identifier.IsFunctionName)
                .Select(identifier => (Expression)new IdentifierExpression(identifier, this.runtimeManager.Scope))
                .ToList();
            if (variables.Count > 0)
            {
                if (sections.Count == 0)
                {
                    miss = "new root variable " + ((IdentifierExpression)variables[0]).Identifier.GetName();
                    return false;
                }

                sections[0].Statements.Insert(0, new VarInitializerStatement(null, this.runtimeManager.Scope, variables));
            }

            // Keep the methods whose output changed; source positions count, so a method an
            // edit moved is written again and the map stays right.
            var overrides = new Dictionary<TypeConverter, List<Statement>>();
            var copied = new List<IIdentifier>();
            foreach (var (typeConverter, method, old, converted) in candidates)
            {
                new JSWriter(true, false).Write(converted.MethodFunctionExpression);
                var oldChunk = old.MethodFunctionExpression?.RenderedChunk;
                var newChunk = converted.MethodFunctionExpression.RenderedChunk;
                if (oldChunk != null && newChunk != null && newChunk.SameAs(oldChunk))
                {
                    continue;
                }

                // Only a changed method needs a form: an unchanged one stays as written.
                var statement = typeConverter.CreateOverride(method, converted);
                if (statement == null)
                {
                    miss = "no override form " + method.FullName;
                    return false;
                }

                typeConverter.ReplaceMethod(method, converted);
                Add(overrides, typeConverter, statement);
                methods++;
                if (!converted.IsGlobalStaticImplementation && !method.IsStatic)
                {
                    copied.Add(typeConverter.ResolvePrototypeSlot(method));
                }
            }

            foreach (var (typeConverter, method, statement) in added)
            {
                Add(overrides, typeConverter, statement);
                methods++;
                if (!typeConverter.ImplementedMethod(method).IsGlobalStaticImplementation && !method.IsStatic)
                {
                    copied.Add(typeConverter.ResolvePrototypeSlot(method));
                }
            }

            var copies = this.SlotCopies(copied);
            var pieces = new List<(string Marker, List<Statement> Statements)>();
            foreach (var typeConverter in overrides.Keys.Union(copies.Keys))
            {
                pieces.Add((
                    MarkerStatement.ForType(typeConverter.TypeDefinition.FullName),
                    Get(overrides, typeConverter).Concat(Get(copies, typeConverter)).ToList()));
            }

            foreach (var (name, statements) in sections)
            {
                pieces.Add((MarkerStatement.ForSection(name), statements));
            }

            if (pieces.Count == 0)
            {
                miss = null;
                return true;
            }

            miss = this.Splice(pieces);
            if (miss == null)
            {
                this.Patches++;
            }

            return miss == null;
        }

        /// <summary>
        /// Converts the emitted methods of <paramref name="changedTypes"/> again, into
        /// <paramref name="candidates"/>. Returns a miss reason, or null.
        /// </summary>
        private string ConvertAgain(
            IReadOnlyList<TypeDefinition> changedTypes,
            ConverterContext context,
            List<Candidate> candidates)
        {
            foreach (var type in changedTypes)
            {
                if (!this.runtimeManager.TypeConverters.TryGetValue(type, out var typeConverter))
                {
                    // Not emitted, and no emitted code can reach it without a change elsewhere.
                    continue;
                }

                var miss = typeConverter.PatchMiss();
                if (miss != null)
                {
                    if (typeConverter.ImplementedMethods.All(method => typeConverter.ImplementedMethod(method) == null))
                    {
                        // No body to replace, such as an interface in the edited file.
                        continue;
                    }

                    return miss + " " + type.FullName;
                }

                foreach (var method in typeConverter.ImplementedMethods)
                {
                    var old = typeConverter.ImplementedMethod(method);
                    if (old == null)
                    {
                        continue;
                    }

                    if (this.PluginMiss(method, context) != null)
                    {
                        // The plugin wrote this method in place of its body, from its
                        // attributes (part of the surface, so unchanged in a body-only build)
                        // and its state kept from the full emit: the written one stands.
                        continue;
                    }

                    miss = typeConverter.PatchMiss(method);
                    if (miss != null)
                    {
                        return miss + " " + method.FullName;
                    }

                    candidates.Add(new Candidate(typeConverter, method, old, typeConverter.ConvertAgain(method)));
                }
            }

            return null;
        }

        /// <summary>
        /// Asks each runtime plugin for its section's statements after a resource change.
        /// Returns a miss reason, or null.
        /// </summary>
        private string PatchPluginSections(List<(string Name, List<Statement> Statements)> sections)
        {
            foreach (var plugin in this.runtimePlugins)
            {
                var statements = new List<Statement>();
                var miss = plugin.TryPatchResources(statements);
                if (miss != null)
                {
                    return miss;
                }

                if (statements.Count > 0)
                {
                    if (plugin.PatchSectionName == null)
                    {
                        return "no section " + plugin.GetType().Name;
                    }

                    sections.Add((plugin.PatchSectionName, statements));
                }
            }

            return null;
        }

        /// <summary>
        /// Why a plugin's part in converting the method cannot be patched: it replaces the
        /// method with output that also needs the plugin's own section (a skin getter queues
        /// its template). Other parts depend only on the plugin's state from the full emit,
        /// which this session keeps with the plugin. A method the bundle already has keeps
        /// its written form instead (<see cref="ConvertAgain"/>).
        /// </summary>
        private string PluginMiss(MethodDefinition method, ConverterContext context)
        {
            foreach (var plugin in this.methodPlugins)
            {
                if (plugin.GetInterestLevel(method, context) == IntrestLevel.Overwrite
                    && !plugin.CanReplayConversion(method, context))
                {
                    return "plugin overwrite";
                }
            }

            return null;
        }

        /// <summary>
        /// The load-time slot copies, in any emitted type, that read a slot whose function
        /// changed (or a slot such a copy writes), so the copies take the new function.
        /// </summary>
        private Dictionary<TypeConverter, List<Statement>> SlotCopies(List<IIdentifier> changed)
        {
            var copies = new Dictionary<TypeConverter, List<Statement>>();
            if (changed.Count == 0)
            {
                return copies;
            }

            var bySource = new Dictionary<IIdentifier, List<(TypeConverter Type, Expression Slot)>>();
            foreach (var typeConverter in this.runtimeManager.TypeConverters.Values)
            {
                foreach (var (slot, source) in typeConverter.SlotCopies)
                {
                    if (!bySource.TryGetValue(source, out var list))
                    {
                        bySource.Add(source, list = new List<(TypeConverter, Expression)>());
                    }

                    list.Add((typeConverter, slot));
                }
            }

            var pending = new Queue<IIdentifier>(changed);
            var seen = new HashSet<(TypeConverter, Expression, IIdentifier)>();
            while (pending.Count > 0)
            {
                var source = pending.Dequeue();
                if (!bySource.TryGetValue(source, out var targets))
                {
                    continue;
                }

                foreach (var (typeConverter, slot) in targets)
                {
                    if (seen.Add((typeConverter, slot, source)))
                    {
                        Add(copies, typeConverter, typeConverter.CreateSlotCopy(slot, source));
                        if (slot is IdentifierExpression copiedTo)
                        {
                            // An interface slot is never a copy's source.
                            pending.Enqueue(copiedTo.Identifier);
                        }
                    }
                }
            }

            return copies;
        }

        /// <summary>
        /// Puts each piece's statements before its marker (a type's overrides, then its slot
        /// copies; a plugin section's statements), and writes the script and map. Returns a
        /// miss reason, or null.
        /// </summary>
        private string Splice(List<(string Marker, List<Statement> Statements)> pieces)
        {
            var places = new List<(int Offset, List<Statement> Statements)>();
            foreach (var (marker, statements) in pieces)
            {
                // The writer ends lines with Environment.NewLine.
                var needle = Environment.NewLine + marker;
                int offset = this.script.IndexOf(needle, StringComparison.Ordinal);
                int end = offset + needle.Length;
                if (offset < 0
                    || this.script.IndexOf(needle, end, StringComparison.Ordinal) >= 0
                    || (end < this.script.Length && this.script[end] != Environment.NewLine[0]))
                {
                    return "marker " + marker;
                }

                places.Add((offset, statements));
            }

            places.Sort((a, b) => a.Offset.CompareTo(b.Offset));

            // The marker's line: the line breaks up to and including the one before it.
            var lines = new int[places.Count];
            int line = 0, counted = 0;
            for (int i = 0; i < places.Count; i++)
            {
                line += this.script.AsSpan(counted, places[i].Offset + Environment.NewLine.Length - counted).Count('\n');
                counted = places[i].Offset + Environment.NewLine.Length;
                lines[i] = line;
            }

            // Last place first: a fragment moves the lines after it, not before.
            long probeStart = System.Diagnostics.Stopwatch.GetTimestamp();
            var fragments = new string[places.Count];
            var fileName = Path.GetFileName(this.jsFileName);
            for (int i = places.Count - 1; i >= 0; i--)
            {
                var writer = new JSWriter(true, false);
                foreach (var statement in places[i].Statements)
                {
                    writer.Write(statement);
                }

                fragments[i] = writer.WriteFragment(this.map, fileName, lines[i]);
            }

            long probeFragments = System.Diagnostics.Stopwatch.GetTimestamp();
            var builder = new System.Text.StringBuilder(this.script.Length + fragments.Sum(f => f.Length));
            int copiedUpTo = 0;
            for (int i = 0; i < places.Count; i++)
            {
                builder.Append(this.script, copiedUpTo, places[i].Offset - copiedUpTo).Append(fragments[i]);
                copiedUpTo = places[i].Offset;
            }

            builder.Append(this.script, copiedUpTo, this.script.Length - copiedUpTo);
            var patched = builder.ToString();
            OwaSourceMapper.AtomicFile.Write(this.jsFileName, System.Text.Encoding.UTF8, writer => writer.Write(patched));
            this.map.Write(Path.GetDirectoryName(this.jsFileName));
            this.script = patched;
            this.written = Stamp(this.jsFileName);
            this.ProbeFragmentMs = (probeFragments - probeStart) * 1000 / System.Diagnostics.Stopwatch.Frequency;
            this.ProbeWriteMs = (System.Diagnostics.Stopwatch.GetTimestamp() - probeFragments) * 1000 / System.Diagnostics.Stopwatch.Frequency;
            return null;
        }

        private static (long, DateTime) Stamp(string path)
        {
            var file = new FileInfo(path);
            return file.Exists ? (file.Length, file.LastWriteTimeUtc) : (-1, default);
        }

        private static void Add(Dictionary<TypeConverter, List<Statement>> lists, TypeConverter type, Statement statement)
        {
            if (!lists.TryGetValue(type, out var list))
            {
                lists.Add(type, list = new List<Statement>());
            }

            list.Add(statement);
        }

        private static IEnumerable<Statement> Get(Dictionary<TypeConverter, List<Statement>> lists, TypeConverter type)
            => lists.TryGetValue(type, out var list) ? list : Enumerable.Empty<Statement>();

        /// <summary>A method converted again: the emitted conversion and the new one.</summary>
        private readonly record struct Candidate(TypeConverter Type, MethodDefinition Method, MethodConverter Old, MethodConverter New);

        /// <summary>
        /// Watches a patch's conversions: collects the new function scopes (children of the
        /// root) and the new identifiers outside them (new root and member names) for naming,
        /// and notes a miss when a conversion adds a scope outside its own functions (a new
        /// type's member scope): that needs a full emit.
        /// </summary>
        private sealed class Observer : IIdentifierObserver
        {
            private readonly IdentifierScope root;
            private readonly HashSet<IdentifierScope> made = new HashSet<IdentifierScope>();

            public Observer(IdentifierScope root)
            {
                this.root = root;
            }

            public List<IdentifierScope> Functions { get; } = new List<IdentifierScope>();

            /// <summary>Identifiers made in the root or a type's member scope, for new members.</summary>
            public List<SimpleIdentifier> Members { get; } = new List<SimpleIdentifier>();

            public string Miss { get; private set; }

            public void ScopeCreated(IdentifierScope scope)
            {
                if (scope.ParentScope == this.root)
                {
                    this.Functions.Add(scope);
                }
                else if (!this.made.Contains(scope.ParentScope))
                {
                    this.Miss ??= "new scope outside a function";
                }

                this.made.Add(scope);
            }

            public void IdentifierCreated(SimpleIdentifier identifier, string suggestedName, bool enforceSuggestion, bool dontEscape)
            {
                if (!enforceSuggestion && !this.made.Contains(identifier.OwnerScope))
                {
                    // Named later by StableName, or a miss (NameNewMembers).
                    this.Members.Add(identifier);
                }
            }

            public void IdentifierUsed(SimpleIdentifier identifier)
            {
            }
        }
    }
}
