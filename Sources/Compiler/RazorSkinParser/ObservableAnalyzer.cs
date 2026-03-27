using System.Linq;
using Microsoft.CodeAnalysis;

namespace NScript.RazorSkin
{
    public static class ObservableAnalyzer
    {
        private const string ObservableObjectFullName = "Sunlight.Framework.Observables.ObservableObject";
        private const string INotifyPropertyChangedFullName = "Sunlight.Framework.Observables.INotifyPropertyChanged";
        private const string IObservableCollectionFullName = "Sunlight.Framework.Observables.IObservableCollection";
        private const string AutoFireAttributeName = "AutoFireAttribute";
        private const string AutoFireAttributeShortName = "AutoFire";
        private const string DefaultDataBindingAttributeName = "DefaultDataBindingAttribute";

        /// <summary>
        /// A property is observable if its containing type is observable AND the property
        /// has [AutoFire] attribute, or [DefaultDataBinding] attribute.
        /// Setter-body analysis (FirePropertyChanged calls) is deferred for now.
        /// If the containing type is observable and no attribute check fails, we consider
        /// all properties observable (conservative default matching XWML behavior).
        /// </summary>
        public static bool IsObservableProperty(IPropertySymbol property)
        {
            if (property == null)
                return false;

            var containingType = property.ContainingType;
            if (!IsObservableType(containingType))
                return false;

            // Check for [AutoFire] or [DefaultDataBinding] attribute on the property
            var attributes = property.GetAttributes();
            bool hasAutoFire = attributes.Any(a =>
            {
                var name = a.AttributeClass?.Name;
                return name == AutoFireAttributeName || name == AutoFireAttributeShortName;
            });
            bool hasDefaultDataBinding = attributes.Any(a =>
                a.AttributeClass?.Name == DefaultDataBindingAttributeName);

            // If the property has explicit attributes, use them
            if (hasAutoFire || hasDefaultDataBinding)
                return true;

            // Conservative default: if the containing type is observable and the property
            // has a setter, treat it as observable. This matches the common pattern where
            // ObservableObject subclasses emit change notifications for all properties.
            // TODO: Add setter-body analysis to check for FirePropertyChanged calls
            return property.SetMethod != null;
        }

        public static bool IsObservableType(ITypeSymbol type)
        {
            if (type == null)
                return false;

            // Check 1: Type inherits from ObservableObject
            var current = type;
            while (current != null)
            {
                if (GetFullName(current) == ObservableObjectFullName)
                    return true;
                current = current.BaseType;
            }

            // Check 2: Type implements INotifyPropertyChanged
            foreach (var iface in type.AllInterfaces)
            {
                if (GetFullName(iface) == INotifyPropertyChangedFullName)
                    return true;
            }

            return false;
        }

        public static bool IsObservableCollection(ITypeSymbol type)
        {
            if (type == null)
                return false;

            // Check if type implements IObservableCollection
            foreach (var iface in type.AllInterfaces)
            {
                if (GetFullName(iface) == IObservableCollectionFullName)
                    return true;
            }

            // Also check the type itself
            if (GetFullName(type) == IObservableCollectionFullName)
                return true;

            return false;
        }

        private static string GetFullName(ITypeSymbol type)
        {
            // Use OriginalDefinition for generic types so that e.g.
            // ObservableCollection<string> compares as ObservableCollection`1
            var originalType = type.OriginalDefinition ?? type;

            // Strip generic arity suffix (e.g., "ObservableCollection`1" → "ObservableCollection")
            var name = originalType.Name;
            var arityIndex = name.IndexOf('`');
            if (arityIndex >= 0)
                name = name.Substring(0, arityIndex);

            if (originalType.ContainingNamespace == null || originalType.ContainingNamespace.IsGlobalNamespace)
                return name;

            return $"{originalType.ContainingNamespace.ToDisplayString()}.{name}";
        }
    }
}
