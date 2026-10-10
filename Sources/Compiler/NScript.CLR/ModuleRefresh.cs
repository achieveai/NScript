//-----------------------------------------------------------------------
// <copyright file="ModuleRefresh.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    /// <summary>
    /// Build session: the new images of loaded modules, checked against the loaded modules
    /// and applied in place, so every Cecil object the session holds stays valid.
    /// <list type="bullet">
    /// <item>An image with the loaded MVID may change only resources (a resource patch).</item>
    /// <item>A recompiled image (a new MVID) may change only method bodies and compiler
    /// generated members, and only in the types its changed source files declare
    /// (<c>$$SrcInfo$$</c>). Their bodies move into the loaded methods, their generated
    /// members (lambdas, closures, state machines) are replaced, and generated module types
    /// such as <c>&lt;PrivateImplementationDetails&gt;</c> gain what the new image added.</item>
    /// </list>
    /// Anything else (a signature, a new type, a new assembly reference) is a miss. Nothing
    /// changes before <see cref="Commit"/>; a failed commit leaves the session unusable.
    /// </summary>
    public sealed class ModuleRefresh : IDisposable
    {
        private const string PrivateImplementationDetails = "<PrivateImplementationDetails>";

        private readonly ClrContext context;
        private readonly List<ModuleDefinition> freshModules = new List<ModuleDefinition>();
        private readonly List<(ModuleDefinition module, int index, EmbeddedResource resource)> resourceSwaps =
            new List<(ModuleDefinition, int, EmbeddedResource)>();

        private readonly List<BodyPlan> bodyPlans = new List<BodyPlan>();
        private readonly HashSet<string> changedTypeNames = new HashSet<string>(StringComparer.Ordinal);
        private readonly List<ModuleDefinition> recompiled = new List<ModuleDefinition>();
        private readonly List<ModuleDefinition> sourceChanged = new List<ModuleDefinition>();
        private readonly List<TypeDefinition> changedTypes = new List<TypeDefinition>();
        private bool committed;

        private ModuleRefresh(ClrContext context)
        {
            this.context = context;
        }

        /// <summary>Gets the loaded modules that took a recompiled image.</summary>
        public IReadOnlyList<ModuleDefinition> Recompiled => this.recompiled;

        /// <summary>Gets the recompiled modules whose source files changed (their AST changed).</summary>
        public IReadOnlyList<ModuleDefinition> SourceChanged => this.sourceChanged;

        /// <summary>Gets the loaded types whose methods take new bodies (no generated types).</summary>
        public IReadOnlyList<TypeDefinition> ChangedTypes => this.changedTypes;

        /// <summary>Gets the number of resources the resource-only images replace.</summary>
        public int ResourcesReplaced => this.resourceSwaps.Count;

        /// <summary>Gets the number of method bodies the commit replaced.</summary>
        public int BodiesReplaced { get; private set; }

        /// <summary>
        /// Whether a type is a changed type or nested in one, by name, so it also answers for
        /// generated types the commit removed.
        /// </summary>
        public bool IsAffected(TypeReference type)
        {
            for (; type != null; type = type.DeclaringType)
            {
                if (type.Module != null
                    && this.changedTypeNames.Contains(Key(type.Module, type.FullName)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks every image against its loaded module. Returns null with a reason when one
        /// does not qualify; nothing changes either way.
        /// </summary>
        internal static ModuleRefresh TryPlan(
            ClrContext context,
            IReadOnlyList<byte[]> images,
            IAssemblyResolver resolver,
            bool resourcesOnly,
            out string reason)
        {
            var refresh = new ModuleRefresh(context);
            try
            {
                foreach (var image in images)
                {
                    var fresh = ModuleDefinition.ReadModule(
                        new MemoryStream(image, writable: false),
                        new ReaderParameters(ReadingMode.Deferred) { AssemblyResolver = resolver });
                    refresh.freshModules.Add(fresh);
                    if (!context.TryGetModuleDefinition(fresh.Name, out var kept))
                    {
                        reason = "not-loaded " + fresh.Name;
                        refresh.Dispose();
                        return null;
                    }

                    reason = fresh.Mvid == kept.Mvid ? refresh.PlanResources(kept, fresh)
                        : resourcesOnly ? "mvid " + fresh.Name
                        : refresh.PlanBodies(kept, fresh);
                    if (reason != null)
                    {
                        refresh.Dispose();
                        return null;
                    }
                }
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException))
            {
                reason = "plan-error " + ex.GetType().Name + ": " + ex.Message;
                refresh.Dispose();
                return null;
            }

            reason = null;
            return refresh;
        }

        /// <summary>Applies every planned change. Call once; a throw leaves the modules unusable.</summary>
        public void Commit()
        {
            if (this.committed)
            {
                throw new InvalidOperationException("Already committed.");
            }

            this.committed = true;
            foreach (var (module, index, resource) in this.resourceSwaps)
            {
                module.Resources[index] = resource;
            }

            foreach (var plan in this.bodyPlans)
            {
                this.BodiesReplaced += plan.Commit();
            }

            foreach (var plan in this.bodyPlans)
            {
                this.context.ForgetTypes(plan.ForgottenTypes);
            }
        }

        public void Dispose()
        {
            foreach (var module in this.freshModules)
            {
                module.Dispose();
            }

            this.freshModules.Clear();
        }

        private static string Key(ModuleDefinition module, string fullName)
            => module.Name.ToLowerInvariant() + "|" + fullName;

        private string PlanResources(ModuleDefinition kept, ModuleDefinition fresh)
        {
            if (fresh.Resources.Count != kept.Resources.Count)
            {
                return "resource-set " + fresh.Name;
            }

            int changed = 0;
            for (int index = 0; index < kept.Resources.Count; index++)
            {
                if (!(kept.Resources[index] is EmbeddedResource keptResource)
                    || !(fresh.Resources[index] is EmbeddedResource freshResource)
                    || keptResource.Name != freshResource.Name
                    || keptResource.Attributes != freshResource.Attributes)
                {
                    return "resource-set " + fresh.Name;
                }

                var data = freshResource.GetResourceData();
                if (data.AsSpan().SequenceEqual(keptResource.GetResourceData()))
                {
                    continue;
                }

                if (keptResource.Name.StartsWith("$$", StringComparison.Ordinal))
                {
                    return keptResource.Name + " " + fresh.Name;
                }

                this.resourceSwaps.Add((kept, index, new EmbeddedResource(keptResource.Name, keptResource.Attributes, data)));
                changed++;
            }

            return changed == 0 ? "no-resource-change " + fresh.Name : null;
        }

        private string PlanBodies(ModuleDefinition kept, ModuleDefinition fresh)
        {
            if (kept.Assembly.Name.FullName != fresh.Assembly.Name.FullName)
            {
                return "assembly-name " + fresh.Name;
            }

            if (!Names(kept.AssemblyReferences.Select(r => r.FullName)).SequenceEqual(Names(fresh.AssemblyReferences.Select(r => r.FullName)))
                || !Names(kept.ModuleReferences.Select(r => r.Name)).SequenceEqual(Names(fresh.ModuleReferences.Select(r => r.Name))))
            {
                return "references " + fresh.Name;
            }

            if (Surface.Attributes(kept.Assembly.CustomAttributes, true) != Surface.Attributes(fresh.Assembly.CustomAttributes, true)
                || Surface.Attributes(kept.CustomAttributes, true) != Surface.Attributes(fresh.CustomAttributes, true))
            {
                return "assembly-attributes " + fresh.Name;
            }

            var keptSource = SourceInfo.Read(kept);
            var freshSource = SourceInfo.Read(fresh);
            if (keptSource == null || freshSource == null)
            {
                return "no-srcinfo " + fresh.Name;
            }

            if (keptSource.Options != freshSource.Options)
            {
                return "options " + fresh.Name;
            }

            if (keptSource.Files.Count != freshSource.Files.Count
                || keptSource.Files.Where((file, index) => file.path != freshSource.Files[index].path).Any())
            {
                return "source-files " + fresh.Name;
            }

            if (!keptSource.TypeFiles.Keys.OrderBy(n => n, StringComparer.Ordinal)
                .SequenceEqual(freshSource.TypeFiles.Keys.OrderBy(n => n, StringComparer.Ordinal)))
            {
                return "types " + fresh.Name;
            }

            var changedFiles = new HashSet<int>(
                Enumerable.Range(0, keptSource.Files.Count)
                    .Where(index => keptSource.Files[index].checksum != freshSource.Files[index].checksum));

            var keptTypes = AllTypes(kept).ToDictionary(t => t.FullName, StringComparer.Ordinal);
            var freshTypes = AllTypes(fresh).ToDictionary(t => t.FullName, StringComparer.Ordinal);

            // Types no source file declares (anonymous types, embedded attributes, <Module>):
            // the same set, each checked like a changed type. <PrivateImplementationDetails>
            // only grows.
            bool IsGenerated(TypeDefinition type, SourceInfo source)
                => type.DeclaringType == null
                    && !source.TypeFiles.ContainsKey(type.FullName)
                    && !type.Name.StartsWith(PrivateImplementationDetails, StringComparison.Ordinal);
            var keptGenerated = kept.Types.Where(t => IsGenerated(t, keptSource)).Select(t => t.FullName).OrderBy(n => n, StringComparer.Ordinal).ToList();
            var freshGenerated = fresh.Types.Where(t => IsGenerated(t, freshSource)).Select(t => t.FullName).OrderBy(n => n, StringComparer.Ordinal).ToList();
            if (!keptGenerated.SequenceEqual(freshGenerated))
            {
                return "generated-types " + fresh.Name;
            }

            var pairs = new List<(TypeDefinition kept, TypeDefinition fresh)>();
            var changedNames = changedFiles.Count == 0
                ? Enumerable.Empty<string>()
                : keptSource.TypeFiles
                    .Where(entry => entry.Value.Concat(freshSource.TypeFiles[entry.Key]).Any(changedFiles.Contains))
                    .Select(entry => entry.Key)
                    .Concat(keptGenerated);
            foreach (var name in changedNames)
            {
                if (!keptTypes.TryGetValue(name, out var keptType) || !freshTypes.TryGetValue(name, out var freshType))
                {
                    return "type-missing " + name;
                }

                if (Surface.Type(keptType) != Surface.Type(freshType))
                {
                    return "surface " + name;
                }

                pairs.Add((keptType, freshType));
            }

            this.recompiled.Add(kept);
            if (changedFiles.Count > 0)
            {
                this.sourceChanged.Add(kept);
            }

            foreach (var (keptType, _) in pairs)
            {
                this.changedTypeNames.Add(Key(kept, keptType.FullName));
                this.changedTypes.Add(keptType);
            }

            this.bodyPlans.Add(new BodyPlan(kept, fresh, keptTypes, pairs));
            return null;
        }

        private static IEnumerable<string> Names(IEnumerable<string> names)
            => names.OrderBy(n => n, StringComparer.Ordinal);

        internal static IEnumerable<TypeDefinition> AllTypes(ModuleDefinition module)
        {
            var stack = new Stack<TypeDefinition>(module.Types.Reverse());
            while (stack.Count > 0)
            {
                var type = stack.Pop();
                yield return type;
                foreach (var nested in type.NestedTypes.Reverse())
                {
                    stack.Push(nested);
                }
            }
        }

        /// <summary>
        /// A member the compiler generated for a body: a nested type or method named
        /// <c>&lt;...</c> (closures, lambdas, local functions, state machines), or a field
        /// named <c>&lt;&gt;...</c>. A virtual method keeps its slot, so it never counts.
        /// </summary>
        internal static bool IsGenerated(IMemberDefinition member)
        {
            switch (member)
            {
                case TypeDefinition type:
                    return type.Name.StartsWith("<", StringComparison.Ordinal);
                case MethodDefinition method:
                    return method.Name.StartsWith("<", StringComparison.Ordinal) && !method.IsVirtual && !method.HasOverrides;
                case FieldDefinition field:
                    return field.Name.StartsWith("<>", StringComparison.Ordinal);
                default:
                    return false;
            }
        }

        /// <summary><c>$$SrcInfo$$</c>: options, source files with checksums, files per type.</summary>
        private sealed class SourceInfo
        {
            public string Options;

            public List<(string path, string checksum)> Files = new List<(string, string)>();

            public Dictionary<string, int[]> TypeFiles = new Dictionary<string, int[]>(StringComparer.Ordinal);

            public static SourceInfo Read(ModuleDefinition module)
            {
                if (!(module.Resources.FirstOrDefault(r => r.Name == "$$SrcInfo$$") is EmbeddedResource resource))
                {
                    return null;
                }

                var info = new SourceInfo();
                foreach (var line in Encoding.UTF8.GetString(resource.GetResourceData()).Split('\n'))
                {
                    var parts = line.Split('\t');
                    switch (parts[0])
                    {
                        case "O":
                            info.Options = line;
                            break;
                        case "F" when parts.Length == 3:
                            info.Files.Add((parts[1], parts[2]));
                            break;
                        case "T" when parts.Length == 3:
                            info.TypeFiles[parts[1]] = parts[2].Length == 0
                                ? Array.Empty<int>()
                                : parts[2].Split(',').Select(p => int.Parse(p, CultureInfo.InvariantCulture)).ToArray();
                            break;
                    }
                }

                return info;
            }
        }

        /// <summary>
        /// The parts of a type other code depends on, as text: everything but method bodies
        /// and generated members. With <c>wildcard</c>, generated type names read
        /// <c>&lt;generated&gt;</c>, so an attribute naming a state machine still matches.
        /// Members are sorted, so a move inside a file still matches (the commit applies the
        /// new order).
        /// </summary>
        internal static class Surface
        {
            public static string Type(TypeDefinition type, bool wildcard = true)
            {
                var lines = new List<string>();
                var head = new StringBuilder()
                    .Append("type ").Append(type.Attributes).Append(' ').Append(type.FullName)
                    .Append(" : ").Append(Name(type.BaseType, wildcard))
                    .Append(GenericParameters(type, wildcard))
                    .Append(Attributes(type.CustomAttributes, wildcard));
                if (type.HasLayoutInfo)
                {
                    head.Append(" layout ").Append(type.PackingSize).Append(',').Append(type.ClassSize);
                }

                lines.Add(head.ToString());
                lines.AddRange(type.Interfaces.Select(i => "implements " + Name(i.InterfaceType, wildcard) + Attributes(i.CustomAttributes, wildcard)));
                lines.AddRange(type.Fields.Where(f => !IsGenerated(f)).Select(f => Field(f, wildcard)));
                lines.AddRange(type.Methods.Where(m => !IsGenerated(m)).Select(m => Method(m, wildcard)));
                lines.AddRange(type.Properties.Select(p => Property(p, wildcard)));
                lines.AddRange(type.Events.Select(e => Event(e, wildcard)));
                lines.AddRange(type.NestedTypes.Where(t => !IsGenerated(t)).Select(t => "nested " + t.Name));
                if (type.HasSecurityDeclarations)
                {
                    lines.Add("security");
                }

                return string.Join("\n", lines.Take(1).Concat(lines.Skip(1).OrderBy(l => l, StringComparer.Ordinal)));
            }

            public static string Field(FieldDefinition field, bool wildcard)
                => "field " + field.Attributes + " " + Name(field.FieldType, wildcard) + " " + field.Name
                    + (field.HasConstant ? " = " + Value(field.Constant, wildcard) : string.Empty)
                    + (field.HasLayoutInfo ? " @" + field.Offset : string.Empty)
                    + (field.HasMarshalInfo ? " marshal" : string.Empty)
                    + (field.InitialValue.Length > 0 ? " data " + Convert.ToBase64String(field.InitialValue) : string.Empty)
                    + Attributes(field.CustomAttributes, wildcard);

            public static string Method(MethodDefinition method, bool wildcard)
            {
                var text = new StringBuilder()
                    .Append("method ").Append(method.Attributes).Append(' ').Append(method.ImplAttributes)
                    .Append(' ').Append(method.SemanticsAttributes).Append(' ').Append(method.CallingConvention)
                    .Append(' ').Append(Name(method.ReturnType, wildcard))
                    .Append(Attributes(method.MethodReturnType.CustomAttributes, wildcard))
                    .Append(method.MethodReturnType.HasConstant ? " = " + Value(method.MethodReturnType.Constant, wildcard) : string.Empty)
                    .Append(' ').Append(method.Name)
                    .Append(GenericParameters(method, wildcard))
                    .Append('(');
                foreach (var parameter in method.Parameters)
                {
                    text.Append(parameter.Attributes).Append(' ').Append(Name(parameter.ParameterType, wildcard))
                        .Append(' ').Append(parameter.Name)
                        .Append(parameter.HasConstant ? " = " + Value(parameter.Constant, wildcard) : string.Empty)
                        .Append(Attributes(parameter.CustomAttributes, wildcard))
                        .Append(parameter.HasMarshalInfo ? " marshal" : string.Empty)
                        .Append(", ");
                }

                text.Append(')');
                foreach (var overridden in method.Overrides)
                {
                    text.Append(" overrides ").Append(Name(overridden.DeclaringType, wildcard)).Append("::").Append(overridden.Name);
                }

                if (method.HasPInvokeInfo)
                {
                    text.Append(" pinvoke ").Append(method.PInvokeInfo.EntryPoint).Append(' ').Append(method.PInvokeInfo.Module?.Name);
                }

                if (method.HasSecurityDeclarations)
                {
                    text.Append(" security");
                }

                return text.Append(Attributes(method.CustomAttributes, wildcard)).ToString();
            }

            private static string Property(PropertyDefinition property, bool wildcard)
                => "property " + property.Attributes + " " + Name(property.PropertyType, wildcard) + " " + property.Name
                    + " get " + property.GetMethod?.Name + " set " + property.SetMethod?.Name
                    + " other " + string.Join(",", property.OtherMethods.Select(m => m.Name))
                    + (property.HasConstant ? " = " + Value(property.Constant, wildcard) : string.Empty)
                    + Attributes(property.CustomAttributes, wildcard);

            private static string Event(EventDefinition @event, bool wildcard)
                => "event " + @event.Attributes + " " + Name(@event.EventType, wildcard) + " " + @event.Name
                    + " add " + @event.AddMethod?.Name + " remove " + @event.RemoveMethod?.Name
                    + " invoke " + @event.InvokeMethod?.Name
                    + " other " + string.Join(",", @event.OtherMethods.Select(m => m.Name))
                    + Attributes(@event.CustomAttributes, wildcard);

            private static string GenericParameters(IGenericParameterProvider provider, bool wildcard)
            {
                if (!provider.HasGenericParameters)
                {
                    return string.Empty;
                }

                return "<" + string.Join(
                    ", ",
                    provider.GenericParameters.Select(p =>
                        p.Attributes + " " + p.Name
                        + " : " + string.Join("+", p.Constraints.Select(c => Name(c, wildcard)))
                        + Attributes(p.CustomAttributes, wildcard))) + ">";
            }

            public static string Attributes(Collection<CustomAttribute> attributes, bool wildcard)
            {
                if (attributes.Count == 0)
                {
                    return string.Empty;
                }

                var text = new StringBuilder(" [");
                foreach (var attribute in attributes)
                {
                    text.Append(attribute.Constructor.FullName).Append('(');
                    foreach (var argument in attribute.ConstructorArguments)
                    {
                        text.Append(Argument(argument, wildcard)).Append(", ");
                    }

                    foreach (var named in attribute.Fields)
                    {
                        text.Append("field ").Append(named.Name).Append('=').Append(Argument(named.Argument, wildcard)).Append(", ");
                    }

                    foreach (var named in attribute.Properties)
                    {
                        text.Append("property ").Append(named.Name).Append('=').Append(Argument(named.Argument, wildcard)).Append(", ");
                    }

                    text.Append(") ");
                }

                return text.Append(']').ToString();
            }

            private static string Argument(CustomAttributeArgument argument, bool wildcard)
                => Name(argument.Type, wildcard) + ":" + Value(argument.Value, wildcard);

            private static string Value(object value, bool wildcard)
            {
                switch (value)
                {
                    case null:
                        return "null";
                    case TypeReference type:
                        return "typeof(" + Name(type, wildcard) + ")";
                    case CustomAttributeArgument argument:
                        return Argument(argument, wildcard);
                    case CustomAttributeArgument[] arguments:
                        return "{" + string.Join(", ", arguments.Select(a => Argument(a, wildcard))) + "}";
                    case string text:
                        return "\"" + text + "\"";
                    case IFormattable formattable:
                        return value.GetType().Name + " " + formattable.ToString(null, CultureInfo.InvariantCulture);
                    default:
                        return value.GetType().Name + " " + value;
                }
            }

            public static string Name(TypeReference type, bool wildcard)
            {
                if (type == null)
                {
                    return "-";
                }

                return wildcard && NamesGenerated(type) ? "<generated>" : type.FullName;
            }

            private static bool NamesGenerated(TypeReference type)
            {
                switch (type)
                {
                    case GenericInstanceType instance:
                        return NamesGenerated(instance.ElementType) || instance.GenericArguments.Any(NamesGenerated);
                    case IModifierType modifier:
                        return NamesGenerated(modifier.ModifierType) || NamesGenerated(((TypeSpecification)type).ElementType);
                    case TypeSpecification specification:
                        return NamesGenerated(specification.ElementType);
                    case GenericParameter _:
                        return false;
                }

                for (; type != null; type = type.DeclaringType)
                {
                    if (type.Name.StartsWith("<", StringComparison.Ordinal)
                        && !type.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal)
                        && !type.Name.StartsWith(PrivateImplementationDetails, StringComparison.Ordinal)
                        && type.Name != "<Module>")
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// One recompiled module: maps the new image's references onto the loaded module and
        /// moves the changed types' bodies and generated members across.
        /// </summary>
        private sealed class BodyPlan
        {
            private readonly ModuleDefinition kept;
            private readonly ModuleDefinition fresh;
            private readonly Dictionary<string, TypeDefinition> keptTypes;
            private readonly List<(TypeDefinition kept, TypeDefinition fresh)> pairs;
            private readonly Dictionary<object, object> map = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
            private readonly Dictionary<TypeDefinition, Dictionary<string, MethodDefinition>> keptMethods =
                new Dictionary<TypeDefinition, Dictionary<string, MethodDefinition>>();

            /// <summary>Fresh generated types and members, with their new loaded copies.</summary>
            private readonly List<(TypeDefinition fresh, TypeDefinition kept)> clonedTypes = new List<(TypeDefinition, TypeDefinition)>();
            private readonly List<(FieldDefinition fresh, FieldDefinition kept)> clonedFields = new List<(FieldDefinition, FieldDefinition)>();
            private readonly List<(MethodDefinition fresh, MethodDefinition kept)> clonedMethods = new List<(MethodDefinition, MethodDefinition)>();

            public BodyPlan(
                ModuleDefinition kept,
                ModuleDefinition fresh,
                Dictionary<string, TypeDefinition> keptTypes,
                List<(TypeDefinition kept, TypeDefinition fresh)> pairs)
            {
                this.kept = kept;
                this.fresh = fresh;
                this.keptTypes = keptTypes;
                this.pairs = pairs;
            }

            /// <summary>The loaded types the commit removed or rebuilt (caches drop them).</summary>
            public List<TypeDefinition> ForgottenTypes { get; } = new List<TypeDefinition>();

            public int Commit()
            {
                // 1. Drop the old generated members, make empty copies of the new ones (so any
                // reference to them maps), and bind the matching members.
                foreach (var (keptType, freshType) in this.pairs)
                {
                    this.ForgottenTypes.Add(keptType);
                    foreach (var old in keptType.NestedTypes.Where(IsGenerated).ToList())
                    {
                        keptType.NestedTypes.Remove(old);
                        foreach (var gone in AllNested(old))
                        {
                            this.keptTypes.Remove(gone.FullName);
                            this.ForgottenTypes.Add(gone);
                        }
                    }

                    foreach (var old in keptType.Methods.Where(IsGenerated).ToList())
                    {
                        keptType.Methods.Remove(old);
                    }

                    foreach (var old in keptType.Fields.Where(IsGenerated).ToList())
                    {
                        keptType.Fields.Remove(old);
                    }

                    this.keptMethods.Remove(keptType);
                    this.map[freshType] = keptType;
                    foreach (var nested in freshType.NestedTypes.Where(IsGenerated))
                    {
                        keptType.NestedTypes.Add(this.Skeleton(nested));
                    }

                    this.CloneMembers(freshType, keptType, IsGenerated);
                    foreach (var method in freshType.Methods.Where(m => !IsGenerated(m)))
                    {
                        this.map[method] = this.KeptMethod(method);
                    }
                }

                this.MergePrivateImplementationDetails();

                // 2. Signatures of the copies; attributes and field types that name a generated
                // type on the kept members.
                foreach (var (freshType, keptType) in this.clonedTypes)
                {
                    this.FillType(freshType, keptType);
                }

                foreach (var (freshField, keptField) in this.clonedFields)
                {
                    this.FillField(freshField, keptField);
                }

                foreach (var (freshMethod, keptMethod) in this.clonedMethods)
                {
                    this.FillMethod(freshMethod, keptMethod);
                }

                foreach (var (keptType, freshType) in this.pairs)
                {
                    this.RepointGeneratedNames(keptType, freshType);
                }

                // 3. Bodies.
                int bodies = 0;
                foreach (var (keptType, freshType) in this.pairs)
                {
                    foreach (var freshMethod in freshType.Methods.Where(m => m.HasBody && !IsGenerated(m)))
                    {
                        var keptMethod = (MethodDefinition)this.map[freshMethod];
                        keptMethod.Body = this.CopyBody(freshMethod.Body, keptMethod);
                        bodies++;
                    }
                }

                foreach (var (freshMethod, keptMethod) in this.clonedMethods)
                {
                    if (freshMethod.HasBody)
                    {
                        keptMethod.Body = this.CopyBody(freshMethod.Body, keptMethod);
                        bodies++;
                    }
                }

                // 4. The new image's member order.
                foreach (var (keptType, freshType) in this.pairs)
                {
                    Reorder(keptType.Methods, freshType.Methods.Select(m => (MethodDefinition)this.map[m]).ToList());
                    Reorder(keptType.Fields, freshType.Fields.Select(f => this.KeptField(f)).ToList());
                    Reorder(keptType.NestedTypes, freshType.NestedTypes.Select(t => (TypeDefinition)this.Type(t)).ToList());
                }

                this.kept.Resources.Clear();
                foreach (var resource in this.fresh.Resources)
                {
                    if (!(resource is EmbeddedResource embedded))
                    {
                        throw new NotSupportedException("linked resource " + resource.Name);
                    }

                    this.kept.Resources.Add(new EmbeddedResource(embedded.Name, embedded.Attributes, embedded.GetResourceData()));
                }

                this.kept.Mvid = this.fresh.Mvid;
                return bodies;
            }

            private static IEnumerable<TypeDefinition> AllNested(TypeDefinition type)
            {
                yield return type;
                foreach (var nested in type.NestedTypes)
                {
                    foreach (var inner in AllNested(nested))
                    {
                        yield return inner;
                    }
                }
            }

            private static void Reorder<T>(Collection<T> members, List<T> order)
            {
                if (members.SequenceEqual(order))
                {
                    return;
                }

                members.Clear();
                foreach (var member in order)
                {
                    members.Add(member);
                }
            }

            /// <summary>An empty loaded copy of a fresh type and everything in it.</summary>
            private TypeDefinition Skeleton(TypeDefinition freshType)
            {
                var keptType = new TypeDefinition(freshType.Namespace, freshType.Name, freshType.Attributes);
                foreach (var parameter in freshType.GenericParameters)
                {
                    keptType.GenericParameters.Add(new GenericParameter(parameter.Name, keptType) { Attributes = parameter.Attributes });
                }

                this.map[freshType] = keptType;
                this.keptTypes[freshType.FullName] = keptType;
                this.clonedTypes.Add((freshType, keptType));
                foreach (var nested in freshType.NestedTypes)
                {
                    keptType.NestedTypes.Add(this.Skeleton(nested));
                }

                this.CloneMembers(freshType, keptType, _ => true);
                return keptType;
            }

            private void CloneMembers(TypeDefinition freshType, TypeDefinition keptType, Func<IMemberDefinition, bool> select)
            {
                foreach (var field in freshType.Fields.Where(f => select(f)))
                {
                    var copy = new FieldDefinition(field.Name, field.Attributes, keptType);
                    this.map[field] = copy;
                    this.clonedFields.Add((field, copy));
                    keptType.Fields.Add(copy);
                }

                foreach (var method in freshType.Methods.Where(m => select(m)))
                {
                    this.AddMethodCopy(method, keptType);
                }
            }

            private MethodDefinition AddMethodCopy(MethodDefinition method, TypeDefinition keptType)
            {
                var copy = new MethodDefinition(method.Name, method.Attributes, keptType);
                foreach (var parameter in method.GenericParameters)
                {
                    copy.GenericParameters.Add(new GenericParameter(parameter.Name, copy) { Attributes = parameter.Attributes });
                }

                this.map[method] = copy;
                this.clonedMethods.Add((method, copy));
                keptType.Methods.Add(copy);
                return copy;
            }

            /// <summary>
            /// Adds what the new image's <c>&lt;PrivateImplementationDetails&gt;</c> has and the
            /// loaded one lacks (array data, hash helpers); its members are named by content.
            /// </summary>
            private void MergePrivateImplementationDetails()
            {
                var freshType = this.fresh.Types.FirstOrDefault(t => t.Name.StartsWith(PrivateImplementationDetails, StringComparison.Ordinal));
                if (freshType == null)
                {
                    return;
                }

                if (!this.keptTypes.TryGetValue(freshType.FullName, out var keptType))
                {
                    this.kept.Types.Add(this.Skeleton(freshType));
                    return;
                }

                this.map[freshType] = keptType;
                foreach (var nested in freshType.NestedTypes)
                {
                    if (!keptType.NestedTypes.Any(t => t.Name == nested.Name))
                    {
                        keptType.NestedTypes.Add(this.Skeleton(nested));
                    }
                }

                foreach (var field in freshType.Fields)
                {
                    if (!keptType.Fields.Any(f => f.Name == field.Name))
                    {
                        var copy = new FieldDefinition(field.Name, field.Attributes, keptType);
                        this.map[field] = copy;
                        this.clonedFields.Add((field, copy));
                        keptType.Fields.Add(copy);
                    }
                }

                this.keptMethods.Remove(keptType);
                var names = new HashSet<string>(keptType.Methods.Select(m => m.FullName), StringComparer.Ordinal);
                foreach (var method in freshType.Methods)
                {
                    if (!names.Contains(method.FullName))
                    {
                        this.AddMethodCopy(method, keptType);
                    }
                }
            }

            private void FillType(TypeDefinition freshType, TypeDefinition keptType)
            {
                keptType.BaseType = this.Type(freshType.BaseType);
                foreach (var implementation in freshType.Interfaces)
                {
                    var copy = new InterfaceImplementation(this.Type(implementation.InterfaceType));
                    this.CopyAttributes(implementation.CustomAttributes, copy.CustomAttributes);
                    keptType.Interfaces.Add(copy);
                }

                if (freshType.HasLayoutInfo)
                {
                    keptType.PackingSize = freshType.PackingSize;
                    keptType.ClassSize = freshType.ClassSize;
                }

                this.FillGenericParameters(freshType, keptType);
                this.CopyAttributes(freshType.CustomAttributes, keptType.CustomAttributes);
                foreach (var property in freshType.Properties)
                {
                    var copy = new PropertyDefinition(property.Name, property.Attributes, this.Type(property.PropertyType))
                    {
                        GetMethod = (MethodDefinition)this.Method(property.GetMethod),
                        SetMethod = (MethodDefinition)this.Method(property.SetMethod),
                    };
                    foreach (var other in property.OtherMethods)
                    {
                        copy.OtherMethods.Add((MethodDefinition)this.Method(other));
                    }

                    if (property.HasConstant)
                    {
                        copy.Constant = property.Constant;
                    }

                    this.CopyAttributes(property.CustomAttributes, copy.CustomAttributes);
                    keptType.Properties.Add(copy);
                }

                foreach (var @event in freshType.Events)
                {
                    var copy = new EventDefinition(@event.Name, @event.Attributes, this.Type(@event.EventType))
                    {
                        AddMethod = (MethodDefinition)this.Method(@event.AddMethod),
                        RemoveMethod = (MethodDefinition)this.Method(@event.RemoveMethod),
                        InvokeMethod = (MethodDefinition)this.Method(@event.InvokeMethod),
                    };
                    foreach (var other in @event.OtherMethods)
                    {
                        copy.OtherMethods.Add((MethodDefinition)this.Method(other));
                    }

                    this.CopyAttributes(@event.CustomAttributes, copy.CustomAttributes);
                    keptType.Events.Add(copy);
                }

                if (freshType.HasSecurityDeclarations)
                {
                    throw new NotSupportedException("security declarations on " + freshType.FullName);
                }
            }

            private void FillField(FieldDefinition freshField, FieldDefinition keptField)
            {
                keptField.FieldType = this.Type(freshField.FieldType);
                if (freshField.HasConstant)
                {
                    keptField.Constant = freshField.Constant;
                }

                if (freshField.InitialValue.Length > 0)
                {
                    keptField.InitialValue = (byte[])freshField.InitialValue.Clone();
                }

                if (freshField.HasLayoutInfo)
                {
                    keptField.Offset = freshField.Offset;
                }

                if (freshField.HasMarshalInfo)
                {
                    throw new NotSupportedException("marshal info on " + freshField.FullName);
                }

                this.CopyAttributes(freshField.CustomAttributes, keptField.CustomAttributes);
            }

            private void FillMethod(MethodDefinition freshMethod, MethodDefinition keptMethod)
            {
                if (freshMethod.HasPInvokeInfo || freshMethod.HasSecurityDeclarations)
                {
                    throw new NotSupportedException("pinvoke or security on " + freshMethod.FullName);
                }

                keptMethod.ImplAttributes = freshMethod.ImplAttributes;
                keptMethod.SemanticsAttributes = freshMethod.SemanticsAttributes;
                keptMethod.CallingConvention = freshMethod.CallingConvention;
                keptMethod.ReturnType = this.Type(freshMethod.ReturnType);
                if (freshMethod.MethodReturnType.HasConstant)
                {
                    keptMethod.MethodReturnType.Constant = freshMethod.MethodReturnType.Constant;
                }

                this.CopyAttributes(freshMethod.MethodReturnType.CustomAttributes, keptMethod.MethodReturnType.CustomAttributes);
                foreach (var parameter in freshMethod.Parameters)
                {
                    var copy = new ParameterDefinition(parameter.Name, parameter.Attributes, this.Type(parameter.ParameterType));
                    if (parameter.HasConstant)
                    {
                        copy.Constant = parameter.Constant;
                    }

                    this.CopyAttributes(parameter.CustomAttributes, copy.CustomAttributes);
                    keptMethod.Parameters.Add(copy);
                }

                foreach (var overridden in freshMethod.Overrides)
                {
                    keptMethod.Overrides.Add(this.Method(overridden));
                }

                this.FillGenericParameters(freshMethod, keptMethod);
                this.CopyAttributes(freshMethod.CustomAttributes, keptMethod.CustomAttributes);
            }

            private void FillGenericParameters(IGenericParameterProvider freshOwner, IGenericParameterProvider keptOwner)
            {
                for (int index = 0; index < freshOwner.GenericParameters.Count; index++)
                {
                    var freshParameter = freshOwner.GenericParameters[index];
                    var keptParameter = keptOwner.GenericParameters[index];
                    foreach (var constraint in freshParameter.Constraints)
                    {
                        keptParameter.Constraints.Add(this.Type(constraint));
                    }

                    this.CopyAttributes(freshParameter.CustomAttributes, keptParameter.CustomAttributes);
                }
            }

            /// <summary>
            /// The surface matched with generated names read as one; where the exact names
            /// differ (an attribute or a fixed buffer naming a renumbered generated type), the
            /// kept member takes the new ones.
            /// </summary>
            private void RepointGeneratedNames(TypeDefinition keptType, TypeDefinition freshType)
            {
                void Repoint(Collection<CustomAttribute> keptAttributes, Collection<CustomAttribute> freshAttributes)
                {
                    if (Surface.Attributes(keptAttributes, false) != Surface.Attributes(freshAttributes, false))
                    {
                        keptAttributes.Clear();
                        this.CopyAttributes(freshAttributes, keptAttributes);
                    }
                }

                Repoint(keptType.CustomAttributes, freshType.CustomAttributes);
                foreach (var freshMethod in freshType.Methods.Where(m => !IsGenerated(m)))
                {
                    var keptMethod = (MethodDefinition)this.map[freshMethod];
                    Repoint(keptMethod.CustomAttributes, freshMethod.CustomAttributes);
                    Repoint(keptMethod.MethodReturnType.CustomAttributes, freshMethod.MethodReturnType.CustomAttributes);
                    for (int index = 0; index < freshMethod.Parameters.Count; index++)
                    {
                        Repoint(keptMethod.Parameters[index].CustomAttributes, freshMethod.Parameters[index].CustomAttributes);
                    }
                }

                foreach (var freshField in freshType.Fields.Where(f => !IsGenerated(f)))
                {
                    var keptField = this.KeptField(freshField);
                    Repoint(keptField.CustomAttributes, freshField.CustomAttributes);
                    if (Surface.Name(keptField.FieldType, false) != Surface.Name(freshField.FieldType, false))
                    {
                        keptField.FieldType = this.Type(freshField.FieldType);
                    }
                }

                foreach (var freshProperty in freshType.Properties)
                {
                    Repoint(keptType.Properties.First(p => p.Name == freshProperty.Name).CustomAttributes, freshProperty.CustomAttributes);
                }

                foreach (var freshEvent in freshType.Events)
                {
                    Repoint(keptType.Events.First(e => e.Name == freshEvent.Name).CustomAttributes, freshEvent.CustomAttributes);
                }
            }

            private void CopyAttributes(Collection<CustomAttribute> from, Collection<CustomAttribute> to)
            {
                foreach (var attribute in from)
                {
                    var copy = new CustomAttribute(this.Method(attribute.Constructor));
                    foreach (var argument in attribute.ConstructorArguments)
                    {
                        copy.ConstructorArguments.Add(this.Argument(argument));
                    }

                    foreach (var named in attribute.Fields)
                    {
                        copy.Fields.Add(new CustomAttributeNamedArgument(named.Name, this.Argument(named.Argument)));
                    }

                    foreach (var named in attribute.Properties)
                    {
                        copy.Properties.Add(new CustomAttributeNamedArgument(named.Name, this.Argument(named.Argument)));
                    }

                    to.Add(copy);
                }
            }

            private CustomAttributeArgument Argument(CustomAttributeArgument argument)
                => new CustomAttributeArgument(this.Type(argument.Type), this.Value(argument.Value));

            private object Value(object value)
            {
                switch (value)
                {
                    case TypeReference type:
                        return this.Type(type);
                    case CustomAttributeArgument argument:
                        return this.Argument(argument);
                    case CustomAttributeArgument[] arguments:
                        return arguments.Select(this.Argument).ToArray();
                    default:
                        return value;
                }
            }

            private MethodBody CopyBody(MethodBody freshBody, MethodDefinition keptMethod)
            {
                var body = new MethodBody(keptMethod)
                {
                    MaxStackSize = freshBody.MaxStackSize,
                    InitLocals = freshBody.InitLocals,
                };
                foreach (var variable in freshBody.Variables)
                {
                    body.Variables.Add(new VariableDefinition(this.Type(variable.VariableType)));
                }

                var instructions = new Dictionary<Instruction, Instruction>(freshBody.Instructions.Count);
                foreach (var instruction in freshBody.Instructions)
                {
                    var copy = Instruction.Create(OpCodes.Nop);
                    copy.OpCode = instruction.OpCode;
                    copy.Offset = instruction.Offset;
                    instructions.Add(instruction, copy);
                }

                Instruction At(Instruction instruction) => instruction == null ? null : instructions[instruction];
                foreach (var instruction in freshBody.Instructions)
                {
                    var copy = instructions[instruction];
                    switch (instruction.Operand)
                    {
                        case Instruction target:
                            copy.Operand = At(target);
                            break;
                        case Instruction[] targets:
                            copy.Operand = targets.Select(At).ToArray();
                            break;
                        case VariableDefinition variable:
                            copy.Operand = body.Variables[variable.Index];
                            break;
                        case ParameterDefinition parameter:
                            copy.Operand = parameter == freshBody.ThisParameter
                                ? body.ThisParameter
                                : keptMethod.Parameters[parameter.Index];
                            break;
                        case TypeReference type:
                            copy.Operand = this.Type(type);
                            break;
                        case MethodReference method:
                            copy.Operand = this.Method(method);
                            break;
                        case FieldReference field:
                            copy.Operand = this.Field(field);
                            break;
                        case CallSite callSite:
                            copy.Operand = this.CallSite(callSite);
                            break;
                        default:
                            copy.Operand = instruction.Operand;
                            break;
                    }

                    body.Instructions.Add(copy);
                }

                foreach (var handler in freshBody.ExceptionHandlers)
                {
                    body.ExceptionHandlers.Add(new ExceptionHandler(handler.HandlerType)
                    {
                        TryStart = At(handler.TryStart),
                        TryEnd = At(handler.TryEnd),
                        FilterStart = At(handler.FilterStart),
                        HandlerStart = At(handler.HandlerStart),
                        HandlerEnd = At(handler.HandlerEnd),
                        CatchType = this.Type(handler.CatchType),
                    });
                }

                return body;
            }

            private CallSite CallSite(CallSite callSite)
            {
                var copy = new CallSite(this.Type(callSite.ReturnType))
                {
                    HasThis = callSite.HasThis,
                    ExplicitThis = callSite.ExplicitThis,
                    CallingConvention = callSite.CallingConvention,
                };
                foreach (var parameter in callSite.Parameters)
                {
                    copy.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, this.Type(parameter.ParameterType)));
                }

                return copy;
            }

            private bool IsOwn(TypeReference type)
            {
                switch (type.Scope)
                {
                    case ModuleDefinition module:
                        return module == this.fresh || module == this.kept;
                    case AssemblyNameReference name:
                        return name.Name == this.fresh.Assembly.Name.Name;
                    default:
                        return false;
                }
            }

            private TypeDefinition KeptType(string fullName)
                => this.keptTypes.TryGetValue(fullName, out var type)
                    ? type
                    : throw new InvalidOperationException("no loaded type " + fullName);

            /// <summary>A reference of the new image, as the loaded module sees it.</summary>
            private TypeReference Type(TypeReference type)
            {
                if (type == null)
                {
                    return null;
                }

                if (this.map.TryGetValue(type, out var known))
                {
                    return (TypeReference)known;
                }

                TypeReference result;
                switch (type)
                {
                    case GenericParameter parameter:
                        result = this.GenericParameter(parameter);
                        break;
                    case GenericInstanceType instance:
                        var instanceCopy = new GenericInstanceType(this.Type(instance.ElementType));
                        foreach (var argument in instance.GenericArguments)
                        {
                            instanceCopy.GenericArguments.Add(this.Type(argument));
                        }

                        result = instanceCopy;
                        break;
                    case ArrayType array:
                        var arrayCopy = new ArrayType(this.Type(array.ElementType));
                        if (!array.IsVector)
                        {
                            arrayCopy.Dimensions.Clear();
                            foreach (var dimension in array.Dimensions)
                            {
                                arrayCopy.Dimensions.Add(new ArrayDimension(dimension.LowerBound, dimension.UpperBound));
                            }
                        }

                        result = arrayCopy;
                        break;
                    case ByReferenceType byReference:
                        result = new ByReferenceType(this.Type(byReference.ElementType));
                        break;
                    case PointerType pointer:
                        result = new PointerType(this.Type(pointer.ElementType));
                        break;
                    case PinnedType pinned:
                        result = new PinnedType(this.Type(pinned.ElementType));
                        break;
                    case SentinelType sentinel:
                        result = new SentinelType(this.Type(sentinel.ElementType));
                        break;
                    case RequiredModifierType required:
                        result = new RequiredModifierType(this.Type(required.ModifierType), this.Type(required.ElementType));
                        break;
                    case OptionalModifierType optional:
                        result = new OptionalModifierType(this.Type(optional.ModifierType), this.Type(optional.ElementType));
                        break;
                    case TypeSpecification specification:
                        throw new NotSupportedException(specification.GetType().Name + " " + specification.FullName);
                    case TypeDefinition definition:
                        result = definition.Module == this.kept ? definition : this.KeptType(definition.FullName);
                        break;
                    default:
                        if (this.IsOwn(type))
                        {
                            result = this.KeptType(type.FullName);
                            break;
                        }

                        // A signature's primitives carry an element type that a plain
                        // TypeReference lacks, and Resolve() compares it.
                        var primitive = this.Primitive(type.MetadataType);
                        if (primitive != null)
                        {
                            result = primitive;
                            break;
                        }

                        var reference = new TypeReference(type.Namespace, type.Name, this.kept, null, type.IsValueType);
                        this.map[type] = reference;
                        if (type.DeclaringType != null)
                        {
                            reference.DeclaringType = this.Type(type.DeclaringType);
                        }
                        else
                        {
                            reference.Scope = this.Scope(type.Scope);
                        }

                        foreach (var parameter in type.GenericParameters)
                        {
                            reference.GenericParameters.Add(new GenericParameter(parameter.Name, reference));
                        }

                        result = reference;
                        break;
                }

                this.map[type] = result;
                return result;
            }

            private TypeReference Primitive(MetadataType metadataType)
            {
                var types = this.kept.TypeSystem;
                switch (metadataType)
                {
                    case MetadataType.Void: return types.Void;
                    case MetadataType.Boolean: return types.Boolean;
                    case MetadataType.Char: return types.Char;
                    case MetadataType.SByte: return types.SByte;
                    case MetadataType.Byte: return types.Byte;
                    case MetadataType.Int16: return types.Int16;
                    case MetadataType.UInt16: return types.UInt16;
                    case MetadataType.Int32: return types.Int32;
                    case MetadataType.UInt32: return types.UInt32;
                    case MetadataType.Int64: return types.Int64;
                    case MetadataType.UInt64: return types.UInt64;
                    case MetadataType.Single: return types.Single;
                    case MetadataType.Double: return types.Double;
                    case MetadataType.String: return types.String;
                    case MetadataType.IntPtr: return types.IntPtr;
                    case MetadataType.UIntPtr: return types.UIntPtr;
                    case MetadataType.Object: return types.Object;
                    case MetadataType.TypedByReference: return types.TypedReference;
                    default: return null;
                }
            }

            private IMetadataScope Scope(IMetadataScope scope)
            {
                switch (scope)
                {
                    case AssemblyNameReference name:
                        return this.kept.AssemblyReferences.FirstOrDefault(r => r.FullName == name.FullName)
                            ?? throw new InvalidOperationException("no assembly reference " + name.FullName);
                    case ModuleReference module:
                        return this.kept.ModuleReferences.FirstOrDefault(r => r.Name == module.Name)
                            ?? throw new InvalidOperationException("no module reference " + module.Name);
                    default:
                        throw new NotSupportedException("scope " + scope?.GetType().Name);
                }
            }

            private TypeReference GenericParameter(GenericParameter parameter)
            {
                IGenericParameterProvider owner = parameter.Owner switch
                {
                    null => null,
                    TypeReference type => this.Type(type),
                    MethodReference method => this.Method(method),
                    _ => throw new NotSupportedException("generic parameter owner"),
                };

                return owner != null && parameter.Position < owner.GenericParameters.Count
                    ? owner.GenericParameters[parameter.Position]
                    : new GenericParameter(parameter.Position, parameter.Type, this.kept);
            }

            private MethodDefinition KeptMethod(MethodDefinition method)
            {
                var keptType = (TypeDefinition)this.Type(method.DeclaringType);
                if (!this.keptMethods.TryGetValue(keptType, out var byName))
                {
                    byName = new Dictionary<string, MethodDefinition>(StringComparer.Ordinal);
                    foreach (var candidate in keptType.Methods)
                    {
                        byName[candidate.FullName] = candidate;
                    }

                    this.keptMethods[keptType] = byName;
                }

                return byName.TryGetValue(method.FullName, out var kept)
                    ? kept
                    : throw new InvalidOperationException("no loaded method " + method.FullName);
            }

            private FieldDefinition KeptField(FieldDefinition field)
            {
                if (this.map.TryGetValue(field, out var known))
                {
                    return (FieldDefinition)known;
                }

                var keptType = (TypeDefinition)this.Type(field.DeclaringType);
                var kept = keptType.Fields.FirstOrDefault(f => f.Name == field.Name)
                    ?? throw new InvalidOperationException("no loaded field " + field.FullName);
                this.map[field] = kept;
                return kept;
            }

            private MethodReference Method(MethodReference method)
            {
                if (method == null)
                {
                    return null;
                }

                if (this.map.TryGetValue(method, out var known))
                {
                    return (MethodReference)known;
                }

                MethodReference result;
                switch (method)
                {
                    case GenericInstanceMethod instance:
                        var instanceCopy = new GenericInstanceMethod(this.Method(instance.ElementMethod));
                        foreach (var argument in instance.GenericArguments)
                        {
                            instanceCopy.GenericArguments.Add(this.Type(argument));
                        }

                        result = instanceCopy;
                        break;
                    case MethodSpecification specification:
                        throw new NotSupportedException(specification.GetType().Name + " " + specification.FullName);
                    case MethodDefinition definition:
                        result = definition.Module == this.kept ? definition : this.KeptMethod(definition);
                        break;
                    default:
                        var declaringType = this.Type(method.DeclaringType);
                        var reference = new MethodReference(method.Name, declaringType, declaringType)
                        {
                            HasThis = method.HasThis,
                            ExplicitThis = method.ExplicitThis,
                            CallingConvention = method.CallingConvention,
                        };
                        this.map[method] = reference;
                        foreach (var parameter in method.GenericParameters)
                        {
                            reference.GenericParameters.Add(new GenericParameter(parameter.Name, reference));
                        }

                        reference.ReturnType = this.Type(method.ReturnType);
                        foreach (var parameter in method.Parameters)
                        {
                            reference.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, this.Type(parameter.ParameterType)));
                        }

                        result = reference;
                        break;
                }

                this.map[method] = result;
                return result;
            }

            private FieldReference Field(FieldReference field)
            {
                if (this.map.TryGetValue(field, out var known))
                {
                    return (FieldReference)known;
                }

                if (field is FieldDefinition definition)
                {
                    return definition.Module == this.kept ? definition : this.KeptField(definition);
                }

                var declaringType = this.Type(field.DeclaringType);
                var reference = new FieldReference(field.Name, declaringType, declaringType);
                this.map[field] = reference;
                reference.FieldType = this.Type(field.FieldType);
                return reference;
            }
        }
    }
}
