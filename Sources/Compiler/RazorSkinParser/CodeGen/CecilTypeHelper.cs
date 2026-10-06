using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NScript.CLR;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Shared Cecil type lookup utilities with caching.
    /// Eliminates duplicated FindTypeDefinition/FindProperty across Razor compiler classes.
    /// </summary>
    public class CecilTypeHelper
    {
        private readonly ClrContext _clrContext;
        private Dictionary<string, TypeDefinition> _typeCache;

        public CecilTypeHelper(ClrContext clrContext)
        {
            _clrContext = clrContext;
        }

        public TypeDefinition FindTypeDefinition(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            if (_typeCache == null)
            {
                _typeCache = new Dictionary<string, TypeDefinition>();
                foreach (var t in _clrContext.GetTypes())
                {
                    if (!_typeCache.ContainsKey(t.FullName))
                        _typeCache[t.FullName] = t;
                }
            }

            _typeCache.TryGetValue(fullTypeName, out var result);
            return result;
        }

        public PropertyDefinition FindProperty(TypeDefinition type, string propertyName)
        {
            var current = type;
            while (current != null)
            {
                var prop = current.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (prop != null) return prop;
                try { current = current.BaseType?.Resolve(); }
                catch (Mono.Cecil.AssemblyResolutionException) { break; }
                catch (System.Exception) { break; }
            }
            return null;
        }

        /// <summary>
        /// Walks a property path hop by hop from an already-resolved root type — the
        /// <c>Child.Items</c> of <c>Model.Child.Items</c> — and returns the final property, or null
        /// when the root or any hop is unknown. Each hop is looked up on the previous property's
        /// declared type, so a chained <c>@foreach</c> source types its loop variable the same way
        /// a one-hop source does.
        /// </summary>
        public PropertyDefinition FindPropertyPath(TypeDefinition rootType, IEnumerable<string> path)
        {
            PropertyDefinition property = null;
            var type = rootType;
            foreach (var name in path)
            {
                property = type != null ? FindProperty(type, name) : null;
                if (property == null) return null;
                type = FindTypeDefinition(property.PropertyType.FullName) ?? SafeResolve(property.PropertyType);
            }
            return property;
        }

        /// <summary>
        /// Element type name of a generic collection property (ObservableCollection&lt;T&gt; → T's
        /// full name), or null when the property is missing or not a generic instance.
        /// </summary>
        public static string CollectionItemTypeName(PropertyDefinition collection)
        {
            var type = collection?.PropertyType as GenericInstanceType;
            return type != null && type.GenericArguments.Count > 0
                ? type.GenericArguments[0].FullName
                : null;
        }

        private static TypeDefinition SafeResolve(TypeReference reference)
        {
            try { return reference?.Resolve(); }
            catch (System.Exception) { return null; }
        }
    }
}
