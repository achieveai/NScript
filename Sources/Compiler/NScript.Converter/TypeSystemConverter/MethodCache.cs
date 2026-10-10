//-----------------------------------------------------------------------
// <copyright file="MethodCache.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;
    using NScript.JST.Visitors;

    /// <summary>
    /// Reuses converted methods across warm dev builds of one session (dev mode with chunks).
    /// A method's entry holds its rendered chunk and every resolver call its conversion made
    /// outside its own scopes. A hit replays those calls, so the build sees the same demand,
    /// identifiers and usages as a conversion would, and writes the chunk as it was.
    /// </summary>
    /// <remarks>
    /// A hit is checked after naming: every outside identifier it wrote must keep its name, and
    /// every name its locals tried must stay free or taken as before. A failed check drops the
    /// entry and asks for the build to run again (<see cref="RetryRequested"/>).
    /// Kill switch: <c>NSCRIPT_METHOD_CACHE=off</c>.
    /// </remarks>
    public sealed class MethodCache
    {
        private readonly Dictionary<MethodDefinition, MethodCacheEntry> entries =
            new Dictionary<MethodDefinition, MethodCacheEntry>();

        private readonly List<MethodRecorder> recorded = new List<MethodRecorder>();

        private readonly List<MethodReplay> replayed = new List<MethodReplay>();

        private readonly Dictionary<string, int> uncacheable = new Dictionary<string, int>(StringComparer.Ordinal);


        /// <summary>False when <c>NSCRIPT_METHOD_CACHE=off</c>.</summary>
        public static bool IsEnabled
            => Environment.GetEnvironmentVariable("NSCRIPT_METHOD_CACHE") != "off";

        public int Hits { get; private set; }

        public int Misses { get; private set; }

        public int Stored { get; private set; }

        public int Invalidated { get; private set; }

        public int Count => this.entries.Count;

        /// <summary>A hit failed a check; the build's output is wrong and must be built again.</summary>
        public bool RetryRequested { get; private set; }

        /// <summary>Why methods were not stored this build, with counts.</summary>
        public IReadOnlyDictionary<string, int> Uncacheable => this.uncacheable;

        public void BeginBuild()
        {
            this.recorded.Clear();
            this.replayed.Clear();
            this.uncacheable.Clear();
            this.Hits = this.Misses = this.Stored = this.Invalidated = 0;
            this.RetryRequested = false;
        }

        /// <summary>
        /// Replays a stored method; null on a miss. The caller converts on a miss.
        /// </summary>
        internal MethodConverter TryReplay(TypeConverter typeConverter, MethodDefinition method)
        {
            if (MethodRecorder.Active != null
                || !this.entries.TryGetValue(method, out var entry))
            {
                this.Misses++;
                return null;
            }

            if (MethodRecorder.HasPluginInterest(typeConverter.Context, method))
            {
                this.entries.Remove(method);
                this.Misses++;
                return null;
            }

            var replay = new MethodReplay(entry, new MethodReplayer(typeConverter, method));
            replay.Run();
            this.replayed.Add(replay);
            this.Hits++;
            return replay.Converter;
        }

        /// <summary>Drops the entries of the methods a session refresh changes; returns how many.</summary>
        public int Invalidate(Func<MethodDefinition, bool> changed)
        {
            var gone = this.entries.Keys.Where(changed).ToList();
            foreach (var method in gone)
            {
                this.entries.Remove(method);
            }

            return gone.Count;
        }

        internal void AddRecording(MethodRecorder recorder) => this.recorded.Add(recorder);

        internal void CountUncacheable(string reason)
        {
            this.uncacheable.TryGetValue(reason, out var count);
            this.uncacheable[reason] = count + 1;
        }

        /// <summary>
        /// After naming: checks each hit. False when one failed; its entry is gone and
        /// <see cref="RetryRequested"/> is set.
        /// </summary>
        public bool ValidateHits()
        {
            var outer = new IdentifierScope.DevStableNamer.OuterNames();
            foreach (var replay in this.replayed)
            {
                if (!replay.Validate(outer))
                {
                    this.entries.Remove(replay.Entry.Method);
                    this.Invalidated++;
                }
            }

            this.RetryRequested = this.Invalidated > 0;
            return !this.RetryRequested;
        }

        /// <summary>
        /// After the writer built its tokens (chunks rendered): stores this build's cacheable
        /// conversions.
        /// </summary>
        public void Harvest()
        {
            var outer = new IdentifierScope.DevStableNamer.OuterNames();
            foreach (var recorder in this.recorded)
            {
                var entry = recorder.ToEntry(outer, out var reason);
                if (entry == null)
                {
                    this.CountUncacheable(reason);
                    continue;
                }

                this.entries[entry.Method] = entry;
                this.Stored++;
            }

            this.recorded.Clear();
            this.replayed.Clear();
        }

        /// <summary>
        /// Collects the identifiers (leaves) and literal values a resolver result holds, in a
        /// fixed order, so a replayed result lines up with the recorded one.
        /// </summary>
        internal static void Flatten(object result, List<SimpleIdentifier> leaves, List<string> values)
        {
            switch (result)
            {
                case null:
                    break;
                case SimpleIdentifier simple:
                    leaves.Add(simple);
                    break;
                case CompoundIdentifier compound:
                    foreach (var part in compound.Identifiers)
                    {
                        Flatten(part, leaves, values);
                    }

                    break;
                case IList<IIdentifier> list:
                    foreach (var part in list)
                    {
                        Flatten(part, leaves, values);
                    }

                    break;
                case JST.Expression expression:
                    ((IJstVisitor)new LeafCollector(leaves, values)).DispatchExpression(expression);
                    break;
                case string text:
                    values.Add(text);
                    break;
                case int number:
                    values.Add(number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    break;
            }
        }

        private sealed class LeafCollector : IJstVisitor
        {
            private readonly List<SimpleIdentifier> leaves;
            private readonly List<string> values;

            public LeafCollector(List<SimpleIdentifier> leaves, List<string> values)
            {
                this.leaves = leaves;
                this.values = values;
            }

            void IJstVisitor.VisitIdentifierExpression(IdentifierExpression expr)
                => Flatten(expr.Identifier, this.leaves, this.values);

            void IJstVisitor.VisitStringLiteralExpression(StringLiteralExpression expr)
                => this.values.Add(expr.StringLiteral);

            void IJstVisitor.VisitNumberLiteralExpression(NumberLiteralExpression expr)
                => this.values.Add(expr.Number.ToString(System.Globalization.CultureInfo.InvariantCulture));

            void IJstVisitor.VisitScriptLiteralExpression(ScriptLiteralExpression expr)
            {
                this.values.Add(expr.Literal);
                foreach (var argument in expr.LiteralArguments)
                {
                    ((IJstVisitor)this).DispatchExpression(argument);
                }
            }
        }
    }

    /// <summary>One stored method.</summary>
    internal sealed class MethodCacheEntry
    {
        public MethodDefinition Method;

        public ReplayStep[] Steps;

        /// <summary>Each step's first index into the flattened leaves; one extra at the end.</summary>
        public int[] LeafStart;

        /// <summary>Each outside leaf's name after naming; null for a leaf in the method's own scopes.</summary>
        public string[] LeafNames;

        /// <summary>The literal values of all results, in order (type ids, string literals).</summary>
        public string[] Values;

        /// <summary>Outside leaves the method made function names.</summary>
        public int[] FunctionNames;

        public List<string> EnforcedLocals;

        public List<KeyValuePair<string, bool>> Probes;

        public RenderedChunk Chunk;

        public bool IsAsync;
    }

    /// <summary>A call to replay: a resolver call, or an identifier made outside the method.</summary>
    internal abstract class ReplayStep
    {
        public abstract object Replay(MethodReplayer replayer);
    }

    internal sealed class CallStep<TArg> : ReplayStep
    {
        private readonly Func<MethodReplayer, TArg, object> replay;
        private readonly TArg arg;

        public CallStep(Func<MethodReplayer, TArg, object> replay, TArg arg)
        {
            this.replay = replay;
            this.arg = arg;
        }

        public override object Replay(MethodReplayer replayer) => this.replay(replayer, this.arg);
    }

    internal sealed class CreateStep : ReplayStep
    {
        private readonly ScopeRef scope;
        private readonly string name;
        private readonly bool enforce;
        private readonly bool dontEscape;

        public CreateStep(ScopeRef scope, string name, bool enforce, bool dontEscape)
        {
            this.scope = scope;
            this.name = name;
            this.enforce = enforce;
            this.dontEscape = dontEscape;
        }

        /// <summary>The stable name dev naming gave it during the recorded conversion.</summary>
        public string StableName { get; set; }

        public override object Replay(MethodReplayer replayer)
        {
            var identifier = SimpleIdentifier.CreateScopeIdentifier(
                replayer.ResolveScope(this.scope),
                this.name,
                this.enforce,
                this.dontEscape);
            if (this.StableName != null
                && identifier.StableName == null
                && !identifier.ShouldEnforceSuggestion)
            {
                identifier.StableName = this.StableName;
            }

            return identifier;
        }
    }

    /// <summary>The method's first use of an outside identifier an earlier step returned.</summary>
    internal sealed class UseStep : ReplayStep
    {
        private readonly int leaf;

        public UseStep(int leaf)
        {
            this.leaf = leaf;
        }

        public override object Replay(MethodReplayer replayer)
        {
            replayer.Leaves[this.leaf].AddUsage(replayer.Scope);
            return null;
        }
    }

    /// <summary>A scope argument, by its role.</summary>
    internal enum ScopeRef
    {
        /// <summary>A scope of the method itself; the replay's stand-in scope.</summary>
        Method,

        /// <summary>The declaring type converter's scope.</summary>
        Type,

        /// <summary>The global scope.</summary>
        Root,
    }

    /// <summary>A resolver callback argument, by its role.</summary>
    internal enum ResolverRef
    {
        /// <summary>The method converter's own resolver.</summary>
        Method,

        /// <summary>The declaring type converter's <see cref="TypeConverter.Resolve(TypeReference)"/>.</summary>
        Type,
    }

    /// <summary>
    /// Records one method conversion: its outermost resolver calls, the identifiers it made
    /// outside its scopes and the outside identifiers it used.
    /// </summary>
    internal sealed class MethodRecorder : IIdentifierObserver
    {
        [ThreadStatic]
        private static MethodRecorder active;

        private readonly MethodCache cache;
        private readonly MethodConverter converter;
        private readonly IdentifierScope methodScope;
        private readonly ConverterContext context;
        private readonly List<ReplayStep> steps = new List<ReplayStep>();
        private readonly List<SimpleIdentifier> leaves = new List<SimpleIdentifier>();
        private readonly List<bool> leafWasFunctionName = new List<bool>();
        private readonly List<int> leafStart = new List<int>();
        private readonly List<string> values = new List<string>();
        private readonly Dictionary<SimpleIdentifier, int> leafIndex = new Dictionary<SimpleIdentifier, int>();
        private readonly HashSet<SimpleIdentifier> usedSet = new HashSet<SimpleIdentifier>();
        private readonly int errorCount;
        private readonly int warningCount;
        private int depth;
        private string poison;
        private FunctionExpression function;
        private int[] functionNames;

        private MethodRecorder(MethodCache cache, MethodConverter converter, IdentifierScope methodScope)
        {
            this.cache = cache;
            this.converter = converter;
            this.methodScope = methodScope;
            this.context = converter.RuntimeManager.Context;
            this.errorCount = this.context.Errors.Count;
            this.warningCount = this.context.Warnings.Count;
        }

        /// <summary>The recorder of the conversion running on this thread, or null.</summary>
        internal static MethodRecorder Active => active;

        /// <summary>
        /// Starts recording a conversion; null when it can't be cached. A conversion that starts
        /// inside a recorded one spoils the outer recording.
        /// </summary>
        internal static MethodRecorder Begin(MethodConverter converter, IdentifierScope methodScope)
        {
            var cache = converter.RuntimeManager.Context.MethodCache;
            if (cache == null)
            {
                return null;
            }

            if (active != null)
            {
                active.Spoil("nested-conversion");
                return null;
            }

            if (HasPluginInterest(converter.RuntimeManager.Context, converter.MethodDefinition))
            {
                cache.CountUncacheable("plugin");
                return null;
            }

            var recorder = new MethodRecorder(cache, converter, methodScope);
            active = recorder;
            IdentifierScope.Observer = recorder;
            return recorder;
        }

        internal static bool HasPluginInterest(ConverterContext context, MethodDefinition method)
        {
            foreach (var plugin in context.MethodConverterPlugins)
            {
                if (plugin.GetInterestLevel(method, context) != IntrestLevel.None)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Wraps a resolver entry point: runs it, and records it when it is the conversion's
        /// outermost call. The replay delegate must redo the same call on a later build.
        /// </summary>
        internal static TResult Call<TSelf, TArg, TResult>(
            TSelf self,
            TArg arg,
            Func<TSelf, TArg, TResult> core,
            Func<MethodReplayer, TArg, object> replay)
        {
            var recorder = active;
            if (recorder == null || recorder.depth != 0)
            {
                return core(self, arg);
            }

            return recorder.Record(self, arg, arg, core, replay);
        }

        /// <summary>
        /// <see cref="Call{TSelf, TArg, TResult}"/> for calls on a type converter, or whose
        /// arguments hold scopes, callbacks or expressions: <paramref name="capture"/> turns
        /// them into roles first.
        /// </summary>
        internal static TResult Call<TSelf, TArg, TRec, TResult>(
            TSelf self,
            TArg arg,
            Func<TSelf, TArg, TResult> core,
            Func<MethodRecorder, TSelf, TArg, TRec> capture,
            Func<MethodReplayer, TRec, object> replay)
        {
            var recorder = active;
            if (recorder == null || recorder.depth != 0)
            {
                return core(self, arg);
            }

            return recorder.Record(self, arg, capture(recorder, self, arg), core, replay);
        }

        /// <summary>Checks the converter a call is made on; only the method's own replays.</summary>
        internal void Target(TypeConverter typeConverter)
        {
            if (typeConverter != this.converter.TypeConverter)
            {
                this.Spoil("foreign-type-converter");
            }
        }

        /// <summary>The role of a scope argument; spoils the recording for a foreign scope.</summary>
        internal ScopeRef Scope(IdentifierScope scope)
        {
            if (this.InTree(scope))
            {
                return ScopeRef.Method;
            }

            if (scope == this.converter.TypeConverter.Scope)
            {
                return ScopeRef.Type;
            }

            if (scope == this.converter.RuntimeManager.Scope)
            {
                return ScopeRef.Root;
            }

            this.Spoil("foreign-scope-arg");
            return ScopeRef.Method;
        }

        /// <summary>The role of a resolver callback; spoils the recording for a foreign one.</summary>
        internal ResolverRef Resolver(Delegate resolver)
        {
            if (resolver?.Target == this.converter)
            {
                return ResolverRef.Method;
            }

            if (resolver?.Target == this.converter.TypeConverter
                && resolver.Method.Name == nameof(TypeConverter.Resolve))
            {
                return ResolverRef.Type;
            }

            this.Spoil("foreign-resolver");
            return ResolverRef.Method;
        }

        /// <summary>An expression argument that replay rebuilds: none, or a boolean literal.</summary>
        internal bool? Initializer(JST.Expression initializer)
        {
            switch (initializer)
            {
                case null:
                    return null;
                case BooleanLiteralExpression literal:
                    return literal.Value;
                default:
                    this.Spoil("expression-arg");
                    return null;
            }
        }

        /// <summary>Stops observing; then keeps the recording if the conversion is cacheable.</summary>
        internal void End(FunctionExpression function)
        {
            active = null;
            IdentifierScope.Observer = null;
            this.function = function;

            if (function == null)
            {
                this.Spoil("failed");
            }
            else if (this.context.Errors.Count != this.errorCount
                || this.context.Warnings.Count != this.warningCount)
            {
                this.Spoil("diagnostics");
            }

            if (this.poison == null)
            {
                this.FindFunctionNames();
            }

            if (this.poison != null)
            {
                this.cache.CountUncacheable(this.poison);
                return;
            }

            this.cache.AddRecording(this);
        }

        /// <summary>
        /// Builds the entry after the write; null (with a reason) when the result can't be reused.
        /// </summary>
        internal MethodCacheEntry ToEntry(IdentifierScope.DevStableNamer.OuterNames outer, out string reason)
        {
            reason = null;
            var chunk = this.function.RenderedChunk;
            if (chunk == null)
            {
                reason = "not-chunked";
                return null;
            }

            // The writer compares locations by reference; the stub keeps the function's (null).
            if (chunk.FirstLocation != this.function.Location
                || chunk.EndLocation != this.function.Location)
            {
                reason = "chunk-location";
                return null;
            }

            var leafNames = new string[this.leaves.Count];
            List<KeyValuePair<string, bool>> probes;
            try
            {
                for (int i = 0; i < this.leaves.Count; i++)
                {
                    leafNames[i] = this.InTree(this.leaves[i].OwnerScope) ? null : this.leaves[i].GetName();
                }

                probes = IdentifierScope.DevStableNamer.CaptureLocalProbes(this.methodScope, outer);
            }
            catch (Exception)
            {
                reason = "naming";
                return null;
            }

            for (int i = 0; i < this.steps.Count; i++)
            {
                if (this.steps[i] is CreateStep create)
                {
                    create.StableName = this.leaves[this.leafStart[i]].StableName;
                }
            }

            this.leafStart.Add(this.leaves.Count);
            return new MethodCacheEntry
            {
                Method = this.converter.MethodDefinition,
                Steps = this.steps.ToArray(),
                LeafStart = this.leafStart.ToArray(),
                LeafNames = leafNames,
                Values = this.values.ToArray(),
                FunctionNames = this.functionNames,
                EnforcedLocals = IdentifierScope.DevStableNamer.EnforcedNames(this.methodScope),
                Probes = probes,
                Chunk = chunk,
                IsAsync = this.function.IsAsync,
            };
        }

        void IIdentifierObserver.ScopeCreated(IdentifierScope scope)
        {
            if (this.depth == 0 && !this.InTree(scope.ParentScope))
            {
                this.Spoil("foreign-scope");
            }
        }

        void IIdentifierObserver.IdentifierCreated(SimpleIdentifier identifier, string suggestedName, bool enforceSuggestion, bool dontEscape)
        {
            if (this.depth != 0 || this.InTree(identifier.OwnerScope))
            {
                return;
            }

            ScopeRef scope;
            if (identifier.OwnerScope == this.converter.RuntimeManager.Scope)
            {
                scope = ScopeRef.Root;
            }
            else if (identifier.OwnerScope == this.converter.TypeConverter.Scope)
            {
                scope = ScopeRef.Type;
            }
            else
            {
                this.Spoil("foreign-identifier");
                return;
            }

            this.AddStep(new CreateStep(scope, suggestedName, enforceSuggestion, dontEscape), identifier);
        }

        void IIdentifierObserver.IdentifierUsed(SimpleIdentifier identifier)
        {
            if (this.depth != 0
                || this.InTree(identifier.OwnerScope)
                || !this.usedSet.Add(identifier))
            {
                return;
            }

            // A scope lists used identifiers in first-use order, and that order reaches the
            // output (variable declarations), so the use is a step in sequence.
            if (this.leafIndex.TryGetValue(identifier, out var leaf))
            {
                this.AddStep(new UseStep(leaf), null);
            }
            else
            {
                this.Spoil("unmapped-use");
            }
        }

        private TResult Record<TSelf, TArg, TRec, TResult>(
            TSelf self,
            TArg arg,
            TRec recorded,
            Func<TSelf, TArg, TResult> core,
            Func<MethodReplayer, TRec, object> replay)
        {
            TResult result;
            this.depth = 1;
            try
            {
                result = core(self, arg);
            }
            catch
            {
                // A caught failure would leave side effects no step replays.
                this.Spoil("resolver-threw");
                throw;
            }
            finally
            {
                this.depth = 0;
            }

            this.AddStep(new CallStep<TRec>(replay, recorded), result);
            return result;
        }

        private void AddStep(ReplayStep step, object result)
        {
            this.steps.Add(step);
            this.leafStart.Add(this.leaves.Count);
            var start = this.leaves.Count;
            MethodCache.Flatten(result, this.leaves, this.values);
            for (int i = start; i < this.leaves.Count; i++)
            {
                this.leafWasFunctionName.Add(this.leaves[i].IsFunctionName);
                this.leafIndex.TryAdd(this.leaves[i], i);
            }
        }

        private void FindFunctionNames()
        {
            var functionNames = new List<int>();
            for (int i = 0; i < this.leaves.Count; i++)
            {
                if (!this.leafWasFunctionName[i]
                    && this.leaves[i].IsFunctionName
                    && this.leafIndex[this.leaves[i]] == i)
                {
                    functionNames.Add(i);
                }
            }

            this.functionNames = functionNames.ToArray();
        }

        private bool InTree(IdentifierScope scope)
        {
            for (; scope != null; scope = scope.ParentScope)
            {
                if (scope == this.methodScope)
                {
                    return true;
                }
            }

            return false;
        }

        private void Spoil(string reason)
        {
            this.poison ??= reason;
        }
    }

    /// <summary>
    /// What a replay needs: the build's type converter and runtime, the stand-in scope, and
    /// resolver callbacks that answer as the method converter would.
    /// </summary>
    internal sealed class MethodReplayer
    {
        public MethodReplayer(TypeConverter typeConverter, MethodDefinition method)
        {
            this.TypeConverter = typeConverter;
            this.Runtime = typeConverter.RuntimeManager;
            this.Method = method;
            this.Scope = new IdentifierScope(typeConverter.Scope);
        }

        public TypeConverter TypeConverter { get; }

        public RuntimeScopeManager Runtime { get; }

        public MethodDefinition Method { get; }

        /// <summary>The stand-in for the method's scopes: usages and enforced locals land here.</summary>
        public IdentifierScope Scope { get; }

        /// <summary>The leaves the replayed steps returned so far.</summary>
        public List<SimpleIdentifier> Leaves { get; } = new List<SimpleIdentifier>();

        public IdentifierScope ResolveScope(ScopeRef scope)
            => scope == ScopeRef.Root ? this.Runtime.Scope
                : scope == ScopeRef.Type ? this.TypeConverter.Scope
                : this.Scope;

        public Func<TypeReference, IList<IIdentifier>> TypeResolver(ResolverRef resolver)
            => resolver == ResolverRef.Method
                ? this.ResolveType
                : this.TypeConverter.Resolve;

        public Func<TypeReference, IdentifierScope, JST.Expression, JST.Expression> ExpressionResolver()
            => this.ResolveTypeToExpression;

        public static JST.Expression Initializer(bool? value, IdentifierScope scope)
            => value.HasValue ? new BooleanLiteralExpression(scope, value.Value) : null;

        /// <summary><see cref="MethodConverter.Resolve(TypeReference)"/>, with a stand-in for the
        /// method's own generic arguments.</summary>
        private IList<IIdentifier> ResolveType(TypeReference typeReference)
        {
            var typeScope = typeReference.GetGenericTypeScope();
            if (typeReference is ByReferenceType byRefType)
            {
                typeReference = byRefType.ElementType;
            }

            if (typeScope.HasValue && typeScope.Value == GenericParameterType.Method)
            {
                return new IIdentifier[] { SimpleIdentifier.CreateScopeIdentifier(this.Scope, "T", false) };
            }

            return this.TypeConverter.Resolve(typeReference);
        }

        /// <summary><see cref="MethodConverter.ResolveTypeToExpression"/>.</summary>
        private JST.Expression ResolveTypeToExpression(
            TypeReference typeReference,
            IdentifierScope scope,
            JST.Expression initializeRefsAndStaticCtor)
        {
            if (typeReference is GenericParameter)
            {
                return IdentifierExpression.Create(null, scope, this.ResolveType(typeReference));
            }

            return this.Runtime.ResolveTypeToExpression(
                typeReference,
                scope,
                this.ResolveTypeToExpression,
                initializeRefsAndStaticCtor);
        }
    }

    /// <summary>One hit: the replayed results and the stub converter, checked after naming.</summary>
    internal sealed class MethodReplay
    {
        private readonly MethodReplayer replayer;
        private readonly List<string> values = new List<string>();
        private bool consistent = true;

        public MethodReplay(MethodCacheEntry entry, MethodReplayer replayer)
        {
            this.Entry = entry;
            this.replayer = replayer;
        }

        public MethodCacheEntry Entry { get; }

        public MethodConverter Converter { get; private set; }

        public void Run()
        {
            var entry = this.Entry;
            var scope = this.replayer.Scope;
            foreach (var name in entry.EnforcedLocals)
            {
                SimpleIdentifier.CreateScopeIdentifier(scope, name, true);
            }

            for (int i = 0; i < entry.Steps.Length; i++)
            {
                MethodCache.Flatten(entry.Steps[i].Replay(this.replayer), this.replayer.Leaves, this.values);
                if (this.replayer.Leaves.Count != entry.LeafStart[i + 1])
                {
                    // The calls answer differently now; don't index past this point.
                    this.consistent = false;
                    break;
                }
            }

            if (this.consistent)
            {
                this.consistent = this.values.SequenceEqual(entry.Values);
            }

            if (this.consistent)
            {
                foreach (var leaf in entry.FunctionNames)
                {
                    this.replayer.Leaves[leaf].MarkAsFunctionName();
                }
            }

            this.Converter = new MethodConverter(
                this.replayer.TypeConverter,
                entry.Method,
                scope,
                entry.Chunk,
                entry.IsAsync);
        }

        public bool Validate(IdentifierScope.DevStableNamer.OuterNames outer)
        {
            if (!this.consistent)
            {
                return false;
            }

            try
            {
                var names = this.Entry.LeafNames;
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i] != null && names[i] != this.replayer.Leaves[i].GetName())
                    {
                        return false;
                    }
                }

                return this.Converter.MethodFunctionExpression.Name?.GetName() == this.Entry.Chunk.Name
                    && IdentifierScope.DevStableNamer.ProbesMatch(this.replayer.Scope.ParentScope, this.Entry.Probes, outer);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
