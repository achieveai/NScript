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

        /// <summary>The live patch client's boot literal up to <c>gen:seq</c>, or null when the bundle has no client.</summary>
        private readonly string hmrPrefix;

        /// <summary>The full emit's generation (<see cref="HmrClient.NewGen"/>).</summary>
        private readonly string gen;

        /// <summary>Methods that run only while the bundle loads (entry point, module initializers): a change reloads.</summary>
        private readonly ISet<MethodDefinition> loadTime;
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
            OwaSourceMapper.SourceMap map,
            string hmrPrefix,
            string gen,
            ISet<MethodDefinition> loadTime)
        {
            this.hmrPrefix = hmrPrefix;
            this.gen = gen;
            this.loadTime = loadTime;
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
            // the written bundle works with them. A patch counts only what is new to it.
            runtimeManager.AcceptDemand();
        }

        /// <summary>Whether dev builds keep a patch session: <c>NSCRIPT_PATCH_EMIT=1</c>.</summary>
        public static bool IsEnabled
            => Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT") == "1";

        /// <summary>
        /// How much the patches may grow the bundle, in percent of the full emit, before the
        /// next build emits in full again (compaction): <c>NSCRIPT_PATCH_COMPACT_PERCENT</c>,
        /// 50 by default.
        /// </summary>
        public static int CompactPercent
            => int.TryParse(Environment.GetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT"), out var percent) && percent >= 0
                ? percent
                : 50;

        /// <summary>Patches written since the full emit.</summary>
        public int Patches { get; private set; }

        /// <summary>Script length now, in chars.</summary>
        public int ScriptLength => this.script.Length;

        /// <summary>The string literals the plugins write again in place, by marker (this patch).</summary>
        private readonly Dictionary<string, string> literals = new Dictionary<string, string>();

        /// <summary>What an open page runs to take this patch's literals (<see cref="IRuntimeConverterPlugin.LiveLiteral"/>).</summary>
        private readonly List<Statement> liveLiterals = new List<Statement>();

        /// <summary>Last patch: time rendering its fragments (Patch.Emit FragmentMs).</summary>
        public long ProbeFragmentMs { get; private set; }

        /// <summary>Last patch: time joining and writing the script and map (Patch.Emit WriteMs).</summary>
        public long ProbeWriteMs { get; private set; }

        /// <summary>What the last patch means for open pages, or null when it wrote nothing or the bundle has no client.</summary>
        public LivePatch LastLive { get; private set; }

        /// <summary>
        /// Patches the written bundle for a body-only change of <paramref name="changedTypes"/>,
        /// and for a change to embedded resources that came with it when
        /// <paramref name="withResources"/>. False with a reason when it cannot; the session is
        /// then unusable.
        /// </summary>
        /// <param name="methods">Methods whose override was written.</param>
        public bool TryPatch(
            IReadOnlyList<TypeDefinition> changedTypes,
            bool withResources,
            ConverterContext context,
            out string miss,
            out int methods)
        {
            var candidates = new List<Candidate>();
            var sections = new List<(string Name, List<Statement> Statements)>();
            return this.Patch(
                context,
                () => this.runtimePlugins.Select(plugin => plugin.TryPatchBodies(changedTypes)).FirstOrDefault(pluginMiss => pluginMiss != null)
                    ?? this.ConvertAgain(changedTypes, context, candidates)
                    ?? (withResources ? this.PatchPluginSections(sections) : null),
                candidates,
                sections,
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
            this.LastLive = null;
            this.literals.Clear();
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
            var observer = new Observer(
                this.runtimeManager.Scope,
                this.runtimeManager.TypeConverters.Values.Where(type => type.HasFactoryHook).Select(type => type.Scope));
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
            var written = new List<(TypeConverter Type, MethodDefinition Method, MethodConverter Converted)>();
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
                written.Add((typeConverter, method, converted));
                methods++;
                if (!converted.IsGlobalStaticImplementation && !method.IsStatic)
                {
                    copied.Add(typeConverter.ResolvePrototypeSlot(method));
                }
            }

            foreach (var (typeConverter, method, statement) in added)
            {
                Add(overrides, typeConverter, statement);
                written.Add((typeConverter, method, typeConverter.ImplementedMethod(method)));
                methods++;
                if (!typeConverter.ImplementedMethod(method).IsGlobalStaticImplementation && !method.IsStatic)
                {
                    copied.Add(typeConverter.ResolvePrototypeSlot(method));
                }
            }

            var copies = this.SlotCopies(copied);
            var pieces = new List<(string Marker, List<Statement> Statements)>();
            var factories = new Dictionary<string, TypeConverter>();
            foreach (var typeConverter in overrides.Keys.Union(copies.Keys))
            {
                // A generic type's piece names its factory's closure locals: it goes inside.
                var marker = typeConverter.HasFactoryHook
                    ? MarkerStatement.ForFactory(typeConverter.TypeDefinition.FullName)
                    : MarkerStatement.ForType(typeConverter.TypeDefinition.FullName);
                if (typeConverter.HasFactoryHook)
                {
                    factories.Add(marker, typeConverter);
                }

                pieces.Add((marker, Get(overrides, typeConverter).Concat(Get(copies, typeConverter)).ToList()));
            }

            foreach (var (name, statements) in sections)
            {
                pieces.Add((MarkerStatement.ForSection(name), statements));
            }

            if (pieces.Count == 0 && this.literals.Count == 0)
            {
                miss = null;
                return true;
            }

            // Read before the client's stamp joins the literals.
            string reload = this.LiveReload(written, sections);
            if (this.hmrPrefix != null)
            {
                this.literals[HmrClient.Marker] = this.hmrPrefix + this.gen + ":" + (this.Patches + 1);
            }

            miss = this.Splice(pieces);
            if (miss == null)
            {
                this.Patches++;
                if (this.hmrPrefix != null)
                {
                    this.LastLive = reload != null
                        ? new LivePatch(this.gen, this.Patches, null, methods, reload, Array.Empty<string>())
                        : new LivePatch(this.gen, this.Patches, this.LiveScript(pieces, factories), methods, null, this.LiveNotes(written));
                }
            }

            return miss == null;
        }

        /// <summary>
        /// Why an open page must reload to take this patch, or null when it can take it live:
        /// a skin a plugin cannot swap live (XWML can, in a patch-emit bundle), a literal no
        /// plugin can take live (XWML CSS can), a
        /// method that ran only while the bundle loaded, or an instance method a capturing
        /// delegate bound when it was made (the delegate keeps the old function). A live patch
        /// bundle's plain Delegate.CreateLateBound reads the method at each call, so it needs no reload.
        /// Fills <see cref="liveLiterals"/>.
        /// </summary>
        private string LiveReload(List<(TypeConverter Type, MethodDefinition Method, MethodConverter Converted)> written, List<(string Name, List<Statement> Statements)> sections)
        {
            foreach (var (name, _) in sections)
            {
                if (!this.runtimePlugins.First(plugin => plugin.PatchSectionName == name).PatchSectionLive)
                {
                    return "skin";
                }
            }

            this.liveLiterals.Clear();
            foreach (var (marker, value) in this.literals)
            {
                var live = this.runtimePlugins
                    .Select(plugin => plugin.LiveLiteral(marker, value, this.runtimeManager.Scope))
                    .FirstOrDefault(statements => statements != null);
                if (live == null)
                {
                    // "/*@L name*/"
                    return "literal " + marker.Substring(5, marker.Length - 7);
                }

                this.liveLiterals.AddRange(live);
            }

            foreach (var (_, method, _) in written)
            {
                if (this.loadTime.Contains(method) || (method.IsConstructor && method.IsStatic))
                {
                    return "load-time " + ShortName(method);
                }
            }

            var creates = this.DelegateCreateNames();
            foreach (var (typeConverter, method, _) in written)
            {
                if (method.IsStatic || method.IsConstructor)
                {
                    continue;
                }

                // Delegate.StaticInstanceCreate("slot", instance, f) and its kin, as the delegate converter writes them.
                // A guard: today a patch never gets here with one, since they bind generic methods,
                // structs, Extended types and optimized instance-as-static methods, which all miss.
                var slot = "(\"" + typeConverter.ResolvePrototypeSlot(method).GetName() + "\"";
                if (creates.Any(create => this.script.Contains(create + slot, StringComparison.Ordinal)))
                {
                    return "delegate " + ShortName(method);
                }
            }

            return null;
        }

        /// <summary>The emitted names of the System.Delegate functions that bind a method by its slot name and keep the function.</summary>
        private List<string> DelegateCreateNames()
        {
            var names = new List<string>();
            var delegateType = this.runtimeManager.TypeConverters.Keys.FirstOrDefault(type => type.FullName == "System.Delegate");
            if (delegateType == null)
            {
                return names;
            }

            var typeConverter = this.runtimeManager.TypeConverters[delegateType];
            foreach (var method in typeConverter.ImplementedMethods)
            {
                if (method.Name is "StaticInstanceCreate" or "CreateGeneric" or "CreateInstanceGeneric"
                    && typeConverter.ImplementedMethod(method)?.MethodFunctionExpression?.Name is IIdentifier name)
                {
                    names.Add(name.GetName());
                }
            }

            return names;
        }

        /// <summary>
        /// What an open page keeps running old code for after taking the patch: instances
        /// already made by a changed constructor, functions a changed method made before (its
        /// lambdas), and a changed static the bundle uses as a value (a delegate to it, or a
        /// callback), which keeps the function it was given.
        /// </summary>
        private string[] LiveNotes(List<(TypeConverter Type, MethodDefinition Method, MethodConverter Converted)> written)
        {
            var notes = new List<string>();
            foreach (var (_, method, converted) in written)
            {
                if (method.IsConstructor)
                {
                    notes.Add("ctor " + ShortName(method));
                    continue;
                }

                if (method.IsStatic
                    && converted.MethodFunctionExpression?.Name is IIdentifier name
                    && StoredUse(name.GetName()).IsMatch(this.script))
                {
                    notes.Add("stored " + ShortName(method));
                }

                var writer = new JSWriter(true, false);
                writer.Write(converted.MethodFunctionExpression);
                using var text = new StringWriter();
                writer.Write(text);
                if (NestedFunction.Matches(text.ToString()).Count > 1)
                {
                    notes.Add("closure " + ShortName(method));
                }
            }

            return notes.ToArray();
        }

        /// <summary>
        /// The global's name not called, assigned or read through: <c>f(Name)</c>, <c>x = Name</c>.
        /// </summary>
        private static System.Text.RegularExpressions.Regex StoredUse(string name) =>
            new System.Text.RegularExpressions.Regex(
                @"(?<![\w$.])" + System.Text.RegularExpressions.Regex.Escape(name) + @"(?![\w$])(?!\s*[(.])(?!\s*=[^=])");

        private static readonly System.Text.RegularExpressions.Regex NestedFunction =
            new System.Text.RegularExpressions.Regex(@"\bfunction\b\s*[\w$]*\s*\(");

        /// <summary>
        /// The patch as an open page runs it through the bundle's <c>__nsPatch</c> hook, an
        /// eval inside the bundle's closure: the same statements, but a global function is
        /// assigned rather than declared, as a declaration in the eval would stay local to it.
        /// A generic type's piece runs in each instantiation's closure, and is kept for later
        /// ones (<see cref="TypeConverter.CreateLiveFactoryPatch"/>).
        /// </summary>
        private string LiveScript(List<(string Marker, List<Statement> Statements)> pieces, Dictionary<string, TypeConverter> factories)
        {
            var writer = new JSWriter(true, false);
            foreach (var (marker, statements) in pieces)
            {
                if (factories.TryGetValue(marker, out var factory))
                {
                    var closureWriter = new JSWriter(true, false);
                    statements.ForEach(statement => closureWriter.Write(statement));
                    using var closureText = new StringWriter();
                    closureWriter.Write(closureText);
                    factory.CreateLiveFactoryPatch(closureText.ToString(), this.runtimeManager.Scope).ForEach(statement => writer.Write(statement));
                    continue;
                }

                foreach (var statement in statements)
                {
                    writer.Write(
                        statement is ExpressionStatement { Expression: FunctionExpression { Name: not null } function }
                            ? ExpressionStatement.CreateAssignmentExpression(
                                new IdentifierExpression(function.Name, statement.Scope),
                                function)
                            : statement);
                }
            }

            this.liveLiterals.ForEach(statement => writer.Write(statement));
            using var text = new StringWriter();
            writer.Write(text);
            return text + Environment.NewLine + "//# sourceURL=nscript-hmr:///" + Path.GetFileName(this.jsFileName) + "/" + this.gen + "-" + this.Patches + ".js";
        }

        private static string ShortName(MethodDefinition method) => method.DeclaringType.Name + "." + method.Name;

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
                var miss = plugin.TryPatchResources(statements, this.literals);
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
            string script = this.script;
            var fileName = Path.GetFileName(this.jsFileName);
            foreach (var (marker, literal) in this.literals)
            {
                script = MarkerStatement.ReplaceLiteral(script, this.map, fileName, marker, literal);
                if (script == null)
                {
                    return "marker " + marker;
                }
            }

            var places = new List<(int Offset, List<Statement> Statements)>();
            foreach (var (marker, statements) in pieces)
            {
                // The writer ends lines with Environment.NewLine, and indents a factory's marker.
                int at = script.IndexOf(marker, StringComparison.Ordinal);
                int offset = at < 0 ? -1 : script.LastIndexOf(Environment.NewLine, at, StringComparison.Ordinal);
                int end = at + marker.Length;
                if (offset < 0
                    || !script.AsSpan(offset + Environment.NewLine.Length, at - offset - Environment.NewLine.Length).Trim(' ').IsEmpty
                    || script.IndexOf(marker, end, StringComparison.Ordinal) >= 0
                    || (end < script.Length && script[end] != Environment.NewLine[0]))
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
                line += script.AsSpan(counted, places[i].Offset + Environment.NewLine.Length - counted).Count('\n');
                counted = places[i].Offset + Environment.NewLine.Length;
                lines[i] = line;
            }

            // Last place first: a fragment moves the lines after it, not before.
            long probeStart = System.Diagnostics.Stopwatch.GetTimestamp();
            var fragments = new string[places.Count];
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
            var builder = new System.Text.StringBuilder(script.Length + fragments.Sum(f => f.Length));
            int copiedUpTo = 0;
            for (int i = 0; i < places.Count; i++)
            {
                builder.Append(script, copiedUpTo, places[i].Offset - copiedUpTo).Append(fragments[i]);
                copiedUpTo = places[i].Offset;
            }

            builder.Append(script, copiedUpTo, script.Length - copiedUpTo);
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
        /// root, or of a generic factory's closure) and the new identifiers outside them (new
        /// root and member names) for naming, and notes a miss when a conversion adds a scope
        /// outside its own functions (a new type's member scope), or a local to a factory's
        /// closure, which the factory never declared: that needs a full emit.
        /// </summary>
        private sealed class Observer : IIdentifierObserver
        {
            private readonly IdentifierScope root;
            private readonly HashSet<IdentifierScope> factories;
            private readonly HashSet<IdentifierScope> made = new HashSet<IdentifierScope>();

            public Observer(IdentifierScope root, IEnumerable<IdentifierScope> factories)
            {
                this.root = root;
                this.factories = new HashSet<IdentifierScope>(factories);
            }

            public List<IdentifierScope> Functions { get; } = new List<IdentifierScope>();

            /// <summary>Identifiers made in the root or a type's member scope, for new members.</summary>
            public List<SimpleIdentifier> Members { get; } = new List<SimpleIdentifier>();

            public string Miss { get; private set; }

            public void ScopeCreated(IdentifierScope scope)
            {
                if (scope.ParentScope == this.root || this.factories.Contains(scope.ParentScope))
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
                if (this.factories.Contains(identifier.OwnerScope))
                {
                    this.Miss ??= "new generic local " + suggestedName;
                }
                else if (!enforceSuggestion && !this.made.Contains(identifier.OwnerScope))
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
