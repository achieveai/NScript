
namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;
    using Microsoft.CodeAnalysis.Symbols;
    using Mono.Cecil;

    internal class SymbolSerializer
    {
        private ConcurrentDictionary<MethodSymbol, int> methodTokenMap
            = new ConcurrentDictionary<MethodSymbol, int>();

        private ConcurrentDictionary<FieldSymbol, int> fieldTokenMap
            = new ConcurrentDictionary<FieldSymbol, int>();

        private ConcurrentDictionary<PropertySymbol, int> propertyTokenMap
            = new ConcurrentDictionary<PropertySymbol, int>();

        private ConcurrentDictionary<EventSymbol, int> eventTokenMap
            = new ConcurrentDictionary<EventSymbol, int>();

        private ConcurrentDictionary<TypeSymbol, int> typeTokenMap
            = new ConcurrentDictionary<TypeSymbol, int>();

        private ConcurrentDictionary<MethodSymbol, MethodSpecSer> methodSerMap
            = new ConcurrentDictionary<MethodSymbol, MethodSpecSer>();

        private ConcurrentDictionary<FieldSymbol, FieldSpecSer> fieldSerMap
            = new ConcurrentDictionary<FieldSymbol, FieldSpecSer>();

        private ConcurrentDictionary<PropertySymbol, PropertySpecSer> propertySerMap
            = new ConcurrentDictionary<PropertySymbol, PropertySpecSer>();

        private ConcurrentDictionary<EventSymbol, EventSpecSer> eventSerMap
            = new ConcurrentDictionary<EventSymbol, EventSpecSer>();

        private ConcurrentDictionary<TypeSymbol, TypeSpecSer> typeSerMap
            = new ConcurrentDictionary<TypeSymbol, TypeSpecSer>();

        // Ids come from counters, not from a lock and the map's Count: Roslyn binds methods in
        // parallel, and the lock was the hottest point of a compile. A race can skip an id,
        // which is harmless because ids are only keys.
        private int lastMethodId, lastFieldId, lastPropertyId, lastEventId, lastTypeId;

        public TypeInfoSer GetTypesInfo()
        {
            var rv = new TypeInfoSer();
            rv.Types = typeTokenMap
                .Select(_ => (_.Value, typeSerMap[_.Key]))
                .ToDictionary(_ => _.Value, _ => _.Item2);
            rv.Fields = fieldTokenMap
                .Select(_ => (_.Value, fieldSerMap[_.Key]))
                .ToDictionary(_ => _.Value, _ => _.Item2);
            rv.Properties = propertyTokenMap
                .Select(_ => (_.Value, propertySerMap[_.Key]))
                .ToDictionary(_ => _.Value, _ => _.Item2);
            rv.Events = eventTokenMap
                .Select(_ => (_.Value, eventSerMap[_.Key]))
                .ToDictionary(_ => _.Value, _ => _.Item2);
            rv.Methods = methodTokenMap
                .Select(_ => (_.Value, methodSerMap[_.Key]))
                .ToDictionary(_ => _.Value, _ => _.Item2);

            return rv;
        }

        public int GetMethodSpecId(MethodSymbol method)
        {
            if (methodTokenMap.TryGetValue(method, out var rv))
            { return rv; }

            return this.AddMethodSpecId(method);
        }

        public int GetTypeSpecId(TypeSymbol type)
        {
            if (typeTokenMap.TryGetValue(type, out var rv))
            { return rv; }

            return this.AddTypeSpecId(type);
        }

        public int GetFieldSpecId(FieldSymbol field)
        {
            if (fieldTokenMap.TryGetValue(field, out var rv))
            { return rv; }

            return this.AddFieldSpecId(field);
        }

        public int GetPropertySpecId(PropertySymbol property)
        {
            if (propertyTokenMap.TryGetValue(property, out var rv))
            { return rv; }

            return this.AddPropertySpecId(property);
        }

        public int GetEventSpecId(EventSymbol evt)
        {
            if (eventTokenMap.TryGetValue(evt, out var rv))
            { return rv; }

            return this.AddEventSpecId(evt);
        }

        private TypeSpecSer GetTypeSpecSer(TypeSymbol type)
        {
            if (typeSerMap.TryGetValue(type, out var rv))
            { return rv; }

            this.AddTypeSpecId(type);

            return typeSerMap[type];
        }

        private int AddTypeSpecId(TypeSymbol type)
        {
            // The serialized form goes in first, so every published id has one.
            typeSerMap.TryAdd(type, this.Serialize(type));
            return typeTokenMap.GetOrAdd(
                type,
                static (_, self) => Interlocked.Increment(ref self.lastTypeId),
                this);
        }

        private int AddMethodSpecId(MethodSymbol method)
        {
            // The serialized form goes in first, so every published id has one.
            methodSerMap.TryAdd(method, this.Serialize(method));
            return methodTokenMap.GetOrAdd(
                method,
                static (_, self) => Interlocked.Increment(ref self.lastMethodId),
                this);
        }

        private int AddFieldSpecId(FieldSymbol field)
        {
            // The serialized form goes in first, so every published id has one.
            fieldSerMap.TryAdd(field, this.Serialize(field));
            return fieldTokenMap.GetOrAdd(
                field,
                static (_, self) => Interlocked.Increment(ref self.lastFieldId),
                this);
        }

        private int AddPropertySpecId(PropertySymbol property)
        {
            // The serialized form goes in first, so every published id has one.
            propertySerMap.TryAdd(property, this.Serialize(property));
            return propertyTokenMap.GetOrAdd(
                property,
                static (_, self) => Interlocked.Increment(ref self.lastPropertyId),
                this);
        }

        private int AddEventSpecId(EventSymbol evt)
        {
            // The serialized form goes in first, so every published id has one.
            eventSerMap.TryAdd(evt, this.Serialize(evt));
            return eventTokenMap.GetOrAdd(
                evt,
                static (_, self) => Interlocked.Increment(ref self.lastEventId),
                this);
        }

        private TypeSpecSer Serialize(TypeSymbol type)
        {
            if (type.Kind == SymbolKind.ArrayType)
            {
                return new ArrayTypeSer
                { ElementType = GetTypeSpecSer(((ArrayTypeSymbol)type).ElementType) };
            }

            if (type.Kind == SymbolKind.PointerType)
            {
                return new PointerTypeSer
                { PointedAtType = GetTypeSpecSer(((PointerTypeSymbol)type).PointedAtType) };
            }

            if (type.Kind == SymbolKind.ErrorType)
            {
                // WI-94: An ErrorTypeSymbol reaches here when a method signature
                // (param/return type) cannot be resolved (e.g. CS0246 unknown
                // type). The body-level HasErrors gate in
                // SerializationHelper.InjectIntoCompilation does not catch this
                // because the owning method's body may bind cleanly. Returning
                // a placeholder lets Roslyn finish emit; Emit() then reports
                // Success=false with the underlying diagnostics, instead of
                // crashing with NotSupportedException deep inside our hook.
                return new TypeSpecSer
                {
                    Name = "<error>",
                    Namespace = null,
                    Module = null
                };
            }

            if (type.ContainingModule == null)
            {
                return new TypeSpecSer
                {
                    Name = "dynamic",
                    Namespace = null,
                    Module = null
                };
            }

            var moduleSpec = new ModuleSpecSer { Name = type.ContainingModule.Name };
            if (type.Kind == SymbolKind.TypeParameter)
            {
                var typeParameter = type as TypeParameterSymbol;
                return new GenericParamSer
                {
                    Name = typeParameter.Name,
                    IsMethodOwned = typeParameter.TypeParameterKind == TypeParameterKind.Method,
                    Module = moduleSpec,
                    Position = typeParameter.Ordinal
                };
            }

            if (type.Kind != SymbolKind.NamedType)
            {
                // WI-93/WI-94: Surface enough context that the next regression
                // in this arm is identifiable without an instrumentation cycle.
                // ErrorTypeSymbol is handled by the dedicated arm above; the
                // body-level HasErrors path is filtered in
                // SerializationHelper.InjectIntoCompilation. This throw remains
                // a backstop for genuinely unsupported shapes such as
                // FunctionPointerType or future C# surface (e.g. new ref-struct
                // shapes Roslyn may introduce).
                throw new NotSupportedException(
                    string.Format(
                        "NScript.Csc.Lib.SymbolSerializer.Serialize: unsupported TypeSymbol Kind='{0}', RuntimeType='{1}', FQN='{2}', ContainingType='{3}'.",
                        type.Kind,
                        type.GetType().FullName,
                        type.ToDisplayString(),
                        type.ContainingType?.ToDisplayString() ?? "<null>"));
            }

            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)type;

            if (namedTypeSymbol.IsNestedType())
            { }

            var @namespace = namedTypeSymbol.IsNestedType()
                ? null
                : type.ContainingNamespace.QualifiedName;

            var nestedParent = TypeSymbol
                .Equals(
                    type.ContainingType,
                    null,
                    TypeCompareKind.ObliviousNullableModifierMatchesAny)
                ? null
                : GetTypeSpecSer(type.ContainingType);

            if (namedTypeSymbol.Arity > 0)
            {
                return new GenericInstanceTypeSer
                {
                    Name = type.MetadataName,
                    Namespace = @namespace,
                    Module = moduleSpec,
                    Arity = namedTypeSymbol.Arity,
                    NestedParent = nestedParent,
                    IsRecord = namedTypeSymbol.IsRecord || namedTypeSymbol.IsRecordStruct,
                    TypeParams =
                    namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics != null
                        ?  namedTypeSymbol
                            .TypeArgumentsWithAnnotationsNoUseSiteDiagnostics
                            .Select(annotation => this.GetTypeSpecSer(annotation.Type))
                            .ToList()
                        : namedTypeSymbol
                            .TypeParameters
                            .Select(type => this.GetTypeSpecSer(type))
                            .ToList()
                };
            }

            return new TypeSpecSer
            {
                Name = type.MetadataName,
                Namespace = @namespace,
                Module = moduleSpec,
                Arity = namedTypeSymbol.Arity,
                NestedParent = nestedParent,
                IsRecord = namedTypeSymbol.IsRecord || namedTypeSymbol.IsRecordStruct
            };
        }

        private MethodSpecSer Serialize(MethodSymbol method)
        {
            if (method.Name == "get_Item"
                && ((NamedTypeSymbol)method.ContainingType).MetadataName == "List`1")
            { }

            var methodDef = method.OriginalDefinition;
            var returnType =
                method.ReturnsVoid
                ? null
                : this.GetTypeSpecSer(methodDef.ReturnType);

            var declaringType = this.GetTypeSpecSer(method.ContainingType);

            var name = method.MetadataName;
            var parameters = methodDef
                .Parameters
                .Select(_ =>
                {
                    var modFlags = ParameterAttributes.None;
                    if ((_.RefKind & RefKind.Out) != 0)
                    { modFlags |= ParameterAttributes.Out; }
                    if ((_.RefKind & RefKind.Ref) != 0)
                    { modFlags |= ParameterAttributes.Retval; }

                    return new ParamSer
                    {
                        Name = _.MetadataName,
                        ModFlags = (int)modFlags,
                        ParamType = GetTypeSpecSer(_.Type)
                    };
                })
                .ToList();

            var typeArgs =
                method.TypeArgumentsWithAnnotations.Length == 0
                    ? null
                    : method
                        .TypeArgumentsWithAnnotations
                        .Select(annotation => GetTypeSpecSer(annotation.Type))
                        .ToList();

            return new MethodSpecSer
            {
                DeclaringType = declaringType,
                ReturnType = returnType,
                Name = name,
                IsStatic = method.IsStatic,
                Arity = method.Arity,
                Parameters = parameters,
                TypeArgs = typeArgs,
                IsInitOnly = method.IsInitOnly,
            };
        }

        private FieldSpecSer Serialize(FieldSymbol field)
            => new FieldSpecSer
            {
                Name = field.MetadataName,
                DeclaringType = this.GetTypeSpecSer(field.ContainingType),
                MemberType = this.GetTypeSpecSer(field.OriginalDefinition.Type),
                IsRequired = field.IsRequired,
            };

        private PropertySpecSer Serialize(PropertySymbol property)
            => new PropertySpecSer
            {
                Setter = property.SetMethod != null
                    ? (int?)this.GetMethodSpecId(property.SetMethod)
                    : null,
                Getter = property.GetMethod != null
                    ? (int?)this.GetMethodSpecId(property.GetMethod)
                    : null,
                IsInitOnly = property.SetMethod != null && property.SetMethod.IsInitOnly,
                IsRequired = property.IsRequired,
            };

        private EventSpecSer Serialize(EventSymbol evt)
            => new EventSpecSer
            {
                AddOn = evt.AddMethod != null
                    ? (int?)this.GetMethodSpecId(evt.AddMethod)
                    : null,
                RemoveOn = evt.AddMethod != null
                    ? (int?)this.GetMethodSpecId(evt.RemoveMethod)
                    : null,
            };
    }
}
