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
    }
}
