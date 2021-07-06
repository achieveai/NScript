
namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

            this.AddMethodSpecId(method);

            return methodTokenMap[method];
        }

        public int GetTypeSpecId(TypeSymbol type)
        {
            if (typeTokenMap.TryGetValue(type, out var rv))
            { return rv; }

            this.AddTypeSpecId(type);

            return typeTokenMap[type];
        }

        public int GetFieldSpecId(FieldSymbol field)
        {
            if (fieldTokenMap.TryGetValue(field, out var rv))
            { return rv; }

            this.AddFieldSpecId(field);

            return fieldTokenMap[field];
        }

        public int GetPropertySpecId(PropertySymbol property)
        {
            if (propertyTokenMap.TryGetValue(property, out var rv))
            { return rv; }

            this.AddPropertySpecId(property);

            return propertyTokenMap[property];
        }

        public int GetEventSpecId(EventSymbol evt)
        {
            if (eventTokenMap.TryGetValue(evt, out var rv))
            { return rv; }

            this.AddEventSpecId(evt);

            return eventTokenMap[evt];
        }

        private TypeSpecSer GetTypeSpecSer(TypeSymbol type)
        {
            if (typeSerMap.TryGetValue(type, out var rv))
            { return rv; }

            this.AddTypeSpecId(type);

            return typeSerMap[type];
        }

        private void AddTypeSpecId(TypeSymbol type)
        {
            var ser = this.Serialize(type);
            if (typeTokenMap.ContainsKey(type)) { return; }

            lock(typeTokenMap)
            {
                if (typeTokenMap.ContainsKey(type)) { return; }

                typeSerMap[type] = ser;
                typeTokenMap[type] = typeTokenMap.Count + 1;
            }
        }

        private void AddMethodSpecId(MethodSymbol method)
        {
            var ser = this.Serialize(method);
            if (methodTokenMap.ContainsKey(method)) { return; }
            lock(methodTokenMap)
            {
                if (methodTokenMap.ContainsKey(method)) { return; }

                methodSerMap[method] = ser;
                methodTokenMap[method] = methodTokenMap.Count + 1;
            }
        }

        private void AddFieldSpecId(FieldSymbol field)
        {
            var ser = this.Serialize(field);
            if (fieldTokenMap.ContainsKey(field)) { return; }

            lock(fieldTokenMap)
            {
                if (fieldTokenMap.ContainsKey(field)) { return; }

                fieldSerMap[field] = ser;
                fieldTokenMap[field] = fieldTokenMap.Count + 1;
            }
        }

        private void AddPropertySpecId(PropertySymbol property)
        {
            var ser = this.Serialize(property);
            if (propertyTokenMap.ContainsKey(property)) { return; }

            lock(propertyTokenMap)
            {
                if (propertyTokenMap.ContainsKey(property)) { return; }

                propertySerMap[property] = ser;
                propertyTokenMap[property] = propertyTokenMap.Count + 1;
            }
        }

        private void AddEventSpecId(EventSymbol evt)
        {
            var ser = this.Serialize(evt);
            if (eventTokenMap.ContainsKey(evt)) { return; }

            lock(eventTokenMap)
            {
                if (eventTokenMap.ContainsKey(evt)) { return; }

                eventSerMap[evt] = ser;
                eventTokenMap[evt] = eventTokenMap.Count + 1;
            }
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
            { throw new NotSupportedException(); }

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
                NestedParent = nestedParent
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
                TypeArgs = typeArgs
            };
        }

        private FieldSpecSer Serialize(FieldSymbol field)
            => new FieldSpecSer
            {
                Name = field.MetadataName,
                DeclaringType = this.GetTypeSpecSer(field.ContainingType),
                MemberType = this.GetTypeSpecSer(field.OriginalDefinition.Type)
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
