using Microsoft.CodeAnalysis;

namespace NScript.RazorSkin
{
    public static class ObservableAnalyzer
    {
        private const string ObservableObjectFullName = "Sunlight.Framework.Observables.ObservableObject";
        private const string INotifyPropertyChangedFullName = "Sunlight.Framework.Observables.INotifyPropertyChanged";
        private const string IObservableCollectionFullName = "Sunlight.Framework.Observables.IObservableCollection";

        public static bool IsObservableProperty(IPropertySymbol property)
        {
            if (property == null)
                return false;

            var containingType = property.ContainingType;
            return IsObservableType(containingType);
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
            if (type.ContainingNamespace == null || type.ContainingNamespace.IsGlobalNamespace)
                return type.Name;

            return $"{type.ContainingNamespace.ToDisplayString()}.{type.Name}";
        }
    }
}
