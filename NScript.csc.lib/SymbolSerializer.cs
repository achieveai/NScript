
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
    using Mono.Cecil;

    public class SymbolSerializer
    {
        private ConcurrentDictionary<IMethodSymbol, int> methodTokenMap
            = new ConcurrentDictionary<IMethodSymbol, int>();

        private ConcurrentDictionary<IFieldSymbol, int> fieldTokenMap
            = new ConcurrentDictionary<IFieldSymbol, int>();

        private ConcurrentDictionary<IPropertySymbol, int> propertyTokenMap
            = new ConcurrentDictionary<IPropertySymbol, int>();

        private ConcurrentDictionary<IEventSymbol, int> eventTokenMap
            = new ConcurrentDictionary<IEventSymbol, int>();

        private ConcurrentDictionary<ITypeSymbol, int> typeTokenMap
            = new ConcurrentDictionary<ITypeSymbol, int>();

        private ConcurrentDictionary<IMethodSymbol, MethodSpecSer> methodSerMap
            = new ConcurrentDictionary<IMethodSymbol, MethodSpecSer>();

        private ConcurrentDictionary<IFieldSymbol, FieldSpecSer> fieldSerMap
            = new ConcurrentDictionary<IFieldSymbol, FieldSpecSer>();

        private ConcurrentDictionary<IPropertySymbol, PropertySpecSer> propertySerMap
            = new ConcurrentDictionary<IPropertySymbol, PropertySpecSer>();

        private ConcurrentDictionary<IEventSymbol, EventSpecSer> eventSerMap
            = new ConcurrentDictionary<IEventSymbol, EventSpecSer>();

        private ConcurrentDictionary<ITypeSymbol, TypeSpecSer> typeSerMap
            = new ConcurrentDictionary<ITypeSymbol, TypeSpecSer>();

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

        public int GetMethodSpecId(IMethodSymbol method)
        {
            if (methodTokenMap.TryGetValue(method, out var rv))
            { return rv; }

            this.AddMethodSpecId(method);

            return methodTokenMap[method];
        }

        public int GetTypeSpecId(ITypeSymbol type)
        {
            if (typeTokenMap.TryGetValue(type, out var rv))
            { return rv; }

            this.AddTypeSpecId(type);

            return typeTokenMap[type];
        }

        public int GetFieldSpecId(IFieldSymbol field)
        {
            if (fieldTokenMap.TryGetValue(field, out var rv))
            { return rv; }

            this.AddFieldSpecId(field);

            return fieldTokenMap[field];
        }

        public int GetPropertySpecId(IPropertySymbol property)
        {
            if (propertyTokenMap.TryGetValue(property, out var rv))
            { return rv; }

            this.AddPropertySpecId(property);

            return propertyTokenMap[property];
        }

        public int GetEventSpecId(IEventSymbol evt)
        {
            if (eventTokenMap.TryGetValue(evt, out var rv))
            { return rv; }

            this.AddEventSpecId(evt);

            return eventTokenMap[evt];
        }

        private TypeSpecSer GetTypeSpecSer(ITypeSymbol type)
        {
            if (typeSerMap.TryGetValue(type, out var rv))
            { return rv; }

            this.AddTypeSpecId(type);

            return typeSerMap[type];
        }

        private void AddTypeSpecId(ITypeSymbol type)
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

        private void AddMethodSpecId(IMethodSymbol method)
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

        private void AddFieldSpecId(IFieldSymbol field)
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

        private void AddPropertySpecId(IPropertySymbol property)
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

        private void AddEventSpecId(IEventSymbol evt)
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

        private TypeSpecSer Serialize(ITypeSymbol type)
        {
            if (type.Kind == SymbolKind.ArrayType)
            {
                return new ArrayTypeSer
                { ElementType = GetTypeSpecSer(((IArrayTypeSymbol)type).ElementType) };
            }

            if (type.Kind == SymbolKind.PointerType)
            {
                return new PointerTypeSer
                { PointedAtType = GetTypeSpecSer(((IPointerTypeSymbol)type).PointedAtType) };
            }

            var moduleSpec = new ModuleSpecSer { Name = type.ContainingModule.Name };
            if (type.Kind == SymbolKind.TypeParameter)
            {
                var typeParameter = type as ITypeParameterSymbol;
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

            var name = type.MetadataName;
            var @namespace = namedTypeSymbol.IsNestedType()
                ? null
                : ((NamespaceSymbol)type.ContainingNamespace).QualifiedName;
            var nestedParent = type.ContainingType == null
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
                        namedTypeSymbol
                            .TypeSubstitution?
                            .SubstituteTypes(
                                namedTypeSymbol
                                    .TypeParameters
                                    .CastArray<TypeSymbol>())
                            .Select(_ => this.GetTypeSpecSer(_.Type))
                            .ToList()
                        ?? namedTypeSymbol
                            .TypeParameters
                            .Select(_ => this.GetTypeSpecSer(_))
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

        private MethodSpecSer Serialize(IMethodSymbol method)
        {
            if (method.Name == ".ctor" && ((NamedTypeSymbol)method.ContainingType).IsNestedType() )
            { }

            var returnType =
                method.ReturnsVoid
                ? null
                : this.GetTypeSpecSer(method.ReturnType);

            var declaringType = this.GetTypeSpecSer(method.ContainingType);

            var name = method.MetadataName;
            var parameters = method
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
                method.TypeArguments.Length == 0
                ? null
                : method
                .TypeArguments
                .Select(_ => GetTypeSpecSer(_))
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

        private FieldSpecSer Serialize(IFieldSymbol field)
            => new FieldSpecSer
            {
                Name = field.MetadataName,
                DeclaringType = this.GetTypeSpecSer(field.ContainingType),
                MemberType = this.GetTypeSpecSer(field.Type)
            };

        private PropertySpecSer Serialize(IPropertySymbol property)
            => new PropertySpecSer
            {
                Name = property.MetadataName,
                DeclaringType = this.GetTypeSpecSer(property.ContainingType),
                MemberType = this.GetTypeSpecSer(property.Type),
            };

        private EventSpecSer Serialize(IEventSymbol evt)
            => new EventSpecSer
            {
                Name = evt.MetadataName,
                DeclaringType = this.GetTypeSpecSer(evt.ContainingType),
                MemberType = this.GetTypeSpecSer(evt.Type)
            };
    }
}
